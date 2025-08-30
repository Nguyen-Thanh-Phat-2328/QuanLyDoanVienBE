using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly dbQuanLyDoanVien _context;

        public ThongKeController(dbQuanLyDoanVien context)  
        {
            _context = context;
        }

        [HttpGet("DanhSachChiDoan")]
        public IActionResult GetDanhSachChiDoan()
        {
            var data = _context.ChiDoans
                .Select(c => new { c.IdchiDoan, c.TenChiDoan })
                .ToList();

            return Ok(new { success = true, data = data });
        }

        [HttpGet("ThongKeTongQuat")]
        public IActionResult ThongKeTongQuat(
  [FromQuery] int? kyHoc,
  [FromQuery] int? namHoc,
  [FromQuery] string? chiDoan)
        {
            try
            {
                var daNopQuery = _context.DoanVienNopDoanPhis
                    .Include(nop => nop.IddoanPhiNavigation)
                        .ThenInclude(dp => dp.IdhocKyNavigation)
                    .Include(nop => nop.IddoanVienNavigation)
                        .ThenInclude(dv => dv.IdchiDoanNavigation)
                    .Where(nop =>
                        (!kyHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.KyHoc == kyHoc.Value) &&
                        (!namHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.NamHoc == namHoc.Value) &&
                        (string.IsNullOrEmpty(chiDoan) || nop.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan == chiDoan)
                    )
                    .Select(nop => nop.IddoanVien)
                    .Distinct();

                var tongDoanVienQuery = _context.DoanViens.AsQueryable();

                if (!string.IsNullOrEmpty(chiDoan))
                {
                    tongDoanVienQuery = tongDoanVienQuery
                        .Include(dv => dv.IdchiDoanNavigation)
                        .Where(dv => dv.IdchiDoanNavigation.TenChiDoan == chiDoan);
                }

                int tongSo = tongDoanVienQuery.Count();
                int daNop = daNopQuery.Count();
                int chuaNop = tongSo - daNop;

                var result = new
                {
                    tongSo,
                    daNop,
                    chuaNop,
                    tiLe = tongSo > 0 ? (double)daNop / tongSo * 100 : 0
                };

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("SearchDoanVien")]
        public IActionResult SearchDoanVien(
        [FromQuery] string? chiDoan,
        [FromQuery] string? tenDoanVien,
        [FromQuery] string? chucVu,
        [FromQuery] string? trangThai,
        [FromQuery] int? kyHoc,
        [FromQuery] int? namHoc
    )
        {
            try
            {
                var query = _context.DoanViens
                    .Include(dv => dv.IdchiDoanNavigation)
                    .Include(dv => dv.IdchucVuNavigation)
                    .Include(dv => dv.DoanVienNopDoanPhis)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(chiDoan))
                {
                    query = query.Where(dv => dv.IdchiDoanNavigation != null &&
                                               dv.IdchiDoanNavigation.TenChiDoan.Contains(chiDoan));
                }

                if (!string.IsNullOrEmpty(tenDoanVien))
                {
                    query = query.Where(dv => dv.TenDoanVien != null &&
                                               dv.TenDoanVien.Contains(tenDoanVien));
                }

                if (!string.IsNullOrEmpty(chucVu))
                {
                    query = query.Where(dv => dv.IdchucVuNavigation != null &&
                                               dv.IdchucVuNavigation.TenChucVu.Contains(chucVu));
                }

                if (!string.IsNullOrEmpty(trangThai))
                {
                    var subQuery = _context.DoanVienNopDoanPhis
                        .Include(nop => nop.IddoanPhiNavigation)
                            .ThenInclude(dp => dp.IdhocKyNavigation)
                        .Where(nop =>
                            (!kyHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.KyHoc == kyHoc.Value) &&
                            (!namHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.NamHoc == namHoc.Value)
                        )
                        .Select(nop => nop.IddoanVien);

                    if (trangThai.ToLower() == "danop")
                    {
                        query = query.Where(dv => subQuery.Contains(dv.IddoanVien));
                    }
                    else if (trangThai.ToLower() == "chuanop")
                    {
                        query = query.Where(dv => !subQuery.Contains(dv.IddoanVien));
                    }
                }

                var rawResult = query
                    .AsNoTracking()
                    .Select(dv => new
                    {
                        idDoanVien = dv.IddoanVien,
                        hoTen = dv.TenDoanVien,
                        ChucVu = dv.IdchucVuNavigation != null ? dv.IdchucVuNavigation.TenChucVu : "Đoàn viên",
                        chiDoan = dv.IdchiDoanNavigation != null ? dv.IdchiDoanNavigation.TenChiDoan : "Không xác định",
                        NgayNopGanNhat = dv.DoanVienNopDoanPhis
                            .OrderByDescending(dn => dn.NgayNop)
                            .Select(dn => dn.NgayNop.HasValue
                                ? (DateTime?)dn.NgayNop.Value.ToDateTime(TimeOnly.MinValue)
                                : null)
                            .FirstOrDefault(),
                        TrangThai = _context.DoanVienNopDoanPhis.Any(dn => dn.IddoanVien == dv.IddoanVien &&
                            (!kyHoc.HasValue || dn.IddoanPhiNavigation.IdhocKyNavigation.KyHoc == kyHoc.Value) &&
                            (!namHoc.HasValue || dn.IddoanPhiNavigation.IdhocKyNavigation.NamHoc == namHoc.Value))
                    })
                    .ToList();

                var result = rawResult.Select((item, index) => new
                {
                    STT = index + 1,
                    item.idDoanVien,
                    item.hoTen,
                    item.ChucVu,
                    item.chiDoan,
                    ngayNop = item.NgayNopGanNhat?.ToString("dd/MM/yyyy"),
                    daNop = item.TrangThai
                }).ToList();

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [HttpGet("ThongKe")]
        public IActionResult ThongKe(
        [FromQuery] int? kyHoc,
        [FromQuery] int? namHoc,
        [FromQuery] string? loai,
        [FromQuery] string? chiDoan)
        {
            try
            {
                loai = loai?.ToLower();

                if (loai == "danop")
                {
                    var data = _context.DoanVienNopDoanPhis
                        .Include(nop => nop.IddoanVienNavigation)
                            .ThenInclude(dv => dv.IdchiDoanNavigation)
                        .Include(nop => nop.IddoanPhiNavigation)
                            .ThenInclude(dp => dp.IdhocKyNavigation)
                        .Where(nop =>
                            (!kyHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.KyHoc == kyHoc.Value) &&
                            (!namHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.NamHoc == namHoc.Value) &&
                            (string.IsNullOrEmpty(chiDoan) || nop.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan == chiDoan)
                        )
                        .Select(nop => new
                        {
                            idDoanVien = nop.IddoanVien,
                            hoTen = nop.IddoanVienNavigation.TenDoanVien,
                            chiDoan = nop.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan,
                            ngayNop = nop.NgayNop.HasValue ? nop.NgayNop.Value.ToString("dd/MM/yyyy") : "",
                            daNop = true
                        })
                        .Distinct()
                        .ToList();

                    return Ok(new { success = true, data });
                }
                else if (loai == "chuanop")
                {
                    var allDoanVien = _context.DoanViens
                        .Include(dv => dv.IdchiDoanNavigation)
                        .Where(dv =>
                            string.IsNullOrEmpty(chiDoan) || dv.IdchiDoanNavigation.TenChiDoan == chiDoan);

                    var daNopIds = _context.DoanVienNopDoanPhis
                        .Include(nop => nop.IddoanPhiNavigation)
                            .ThenInclude(dp => dp.IdhocKyNavigation)
                        .Where(nop =>
                            (!kyHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.KyHoc == kyHoc.Value) &&
                            (!namHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.NamHoc == namHoc.Value)
                        )
                        .Select(nop => nop.IddoanVien)
                        .Distinct();

                    var chuaNopList = allDoanVien
                        .Where(dv => !daNopIds.Contains(dv.IddoanVien))
                        .Select(dv => new
                        {
                            idDoanVien = dv.IddoanVien,
                            hoTen = dv.TenDoanVien,
                            chiDoan = dv.IdchiDoanNavigation.TenChiDoan,
                            ngayNop = "",
                            daNop = false
                        })
                        .ToList();

                    return Ok(new { success = true, data = chuaNopList });
                }
                else // loai == "all" hoặc null
                {
                    var daNop = _context.DoanVienNopDoanPhis
                        .Include(nop => nop.IddoanVienNavigation)
                            .ThenInclude(dv => dv.IdchiDoanNavigation)
                        .Include(nop => nop.IddoanPhiNavigation)
                            .ThenInclude(dp => dp.IdhocKyNavigation)
                        .Where(nop =>
                            (!kyHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.KyHoc == kyHoc.Value) &&
                            (!namHoc.HasValue || nop.IddoanPhiNavigation.IdhocKyNavigation.NamHoc == namHoc.Value) &&
                            (string.IsNullOrEmpty(chiDoan) || nop.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan == chiDoan)
                        )
                        .Select(nop => new
                        {
                            idDoanVien = nop.IddoanVien,
                            hoTen = nop.IddoanVienNavigation.TenDoanVien,
                            chiDoan = nop.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan,
                            ngayNop = nop.NgayNop.HasValue ? nop.NgayNop.Value.ToString("dd/MM/yyyy") : "",
                            daNop = true
                        });

                    var allDoanVien = _context.DoanViens
                        .Include(dv => dv.IdchiDoanNavigation)
                        .Where(dv =>
                            string.IsNullOrEmpty(chiDoan) || dv.IdchiDoanNavigation.TenChiDoan == chiDoan);

                    var daNopIds = daNop.Select(x => x.idDoanVien).Distinct();

                    var chuaNop = allDoanVien
                        .Where(dv => !daNopIds.Contains(dv.IddoanVien))
                        .Select(dv => new
                        {
                            idDoanVien = dv.IddoanVien,
                            hoTen = dv.TenDoanVien,
                            chiDoan = dv.IdchiDoanNavigation.TenChiDoan,
                            ngayNop = "",
                            daNop = false
                        });

                    var result = daNop.ToList().Concat(chuaNop.ToList()).ToList();

                    return Ok(new { success = true, data = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, inner = ex.InnerException?.Message });
            }
        }


        [HttpGet("DanhSachHocKy")]
        public async Task<ActionResult<IEnumerable<string>>> GetHocKyList()
        {
            var hocKyList = await _context.HocKies
                .OrderByDescending(hk => hk.NamHoc)
                .ThenBy(hk => hk.KyHoc)
                .Select(hk => $"HK{hk.KyHoc} - {hk.NamHoc}")
                .ToListAsync();

            return Ok(hocKyList);
        }

        [HttpGet("GetDoanVienThamGiaHoatDong")]
        public IActionResult GetDoanVienThamGiaHoatDong(
     [FromQuery] String idHoatDong = null,
     [FromQuery] string? tenHoatDong = null,
     [FromQuery] string? fromDate = null,
     [FromQuery] string? toDate = null,
     [FromQuery] string? chiDoan = null)
        {
            try
            {
                var query = _context.ThamGiaHoatDongs
                    .Include(tg => tg.IdhoatDongNavigation)
                    .Include(tg => tg.IddoanVienNavigation)
                    .ThenInclude(dv => dv.IdchiDoanNavigation)
                    .AsQueryable();

                // Lọc theo ID hoạt động
                if (!string.IsNullOrEmpty(idHoatDong))
                {
                    query = query.Where(tg => tg.IdhoatDong == idHoatDong);
                }

                // Lọc theo tên hoạt động
                if (!string.IsNullOrEmpty(tenHoatDong))
                {
                    query = query.Where(tg => tg.IdhoatDongNavigation.TenHoatDong.Contains(tenHoatDong));
                }

                // Lọc theo khoảng thời gian - ĐÃ SỬA LỖI SO SÁNH DateOnly? và DateTime
                if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fromDateParsed))
                {
                    var fromDateOnly = DateOnly.FromDateTime(fromDateParsed);
                    query = query.Where(tg => tg.NgayDangKy != null && tg.NgayDangKy.Value >= fromDateOnly);
                }

                if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var toDateParsed))
                {
                    var toDateOnly = DateOnly.FromDateTime(toDateParsed);
                    query = query.Where(tg => tg.NgayDangKy != null && tg.NgayDangKy.Value <= toDateOnly);
                }

                // Lọc theo chi đoàn - ĐÃ SỬA LỖI SO SÁNH string và int
                if (!string.IsNullOrEmpty(chiDoan))
                {
                    if (int.TryParse(chiDoan, out int chiDoanId))
                    {
                        query = query.Where(tg => tg.IddoanVienNavigation.IdchiDoan == chiDoanId.ToString());
                    }
                    else
                    {
                        query = query.Where(tg => tg.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan.Contains(chiDoan));
                    }
                }

                var result = query
                    .Select(tg => new
                    {
                        IdDoanVien = tg.IddoanVien,
                        hoTen = tg.IddoanVienNavigation.TenDoanVien,
                        // ĐÃ SỬA LỖI ToString()
                        NgayDangKy = tg.NgayDangKy.HasValue ? tg.NgayDangKy.Value.ToString("dd/MM/yyyy") : null,
                        ChiDoan = tg.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan,
                        HoatDong = tg.IdhoatDongNavigation.TenHoatDong,
                        IdHoatDong = tg.IdhoatDong
                    })
                    .ToList();

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("DemDoanVienThamGiaHoatDongTheoChiDoan")]
        public IActionResult DemDoanVienThamGiaHoatDongTheoChiDoan(
   [FromQuery] string? idHoatDong = null,
   [FromQuery] string? tenHoatDong = null)
        {
            try
            {
                // 1. Lấy danh sách TẤT CẢ chi đoàn
                var allChiDoan = _context.ChiDoans
                    .Select(cd => new { cd.IdchiDoan, cd.TenChiDoan })
                    .ToList();

                // 2. Truy vấn cơ sở cho hoạt động
                IQueryable<HoatDong> hoatDongQuery = _context.HoatDongs;

                if (!string.IsNullOrEmpty(idHoatDong))
                {
                    hoatDongQuery = hoatDongQuery.Where(hd => hd.IdhoatDong == idHoatDong);
                }

                if (!string.IsNullOrEmpty(tenHoatDong))
                {
                    hoatDongQuery = hoatDongQuery.Where(hd => hd.TenHoatDong.Contains(tenHoatDong));
                }

                // 3. Lấy danh sách ID hoạt động thỏa điều kiện
                var idHoatDongList = hoatDongQuery.Select(hd => hd.IdhoatDong).ToList();

                // 4. Đếm đoàn viên tham gia hoạt động theo chi đoàn
                var countedResults = _context.ThamGiaHoatDongs
                    .Where(tg => idHoatDongList.Contains(tg.IdhoatDong))
                    .Select(tg => tg.IddoanVienNavigation)
                    .GroupBy(dv => new { dv.IdchiDoan, dv.IdchiDoanNavigation.TenChiDoan })
                    .Select(g => new
                    {
                        IdChiDoan = g.Key.IdchiDoan,
                        TenChiDoan = g.Key.TenChiDoan,
                        SoDoanVienThamGia = g.Count()
                    })
                    .ToList();

                // 5. Gộp kết quả: Tất cả chi đoàn + số lượng (nếu có)
                var finalResult = allChiDoan
                    .Select(cd => new
                    {
                        cd.IdchiDoan,
                        cd.TenChiDoan,
                        SoDoanVienThamGia = countedResults
                            .FirstOrDefault(cr => cr.IdChiDoan == cd.IdchiDoan)?
                            .SoDoanVienThamGia ?? 0
                    })
                    .ToList();

                return Ok(new { success = true, data = finalResult });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetAllHoatDong")]
        public IActionResult GetAllHoatDong()
        {
            var hoatDongs = _context.HoatDongs
                .Select(hd => new { hd.IdhoatDong, hd.TenHoatDong })
                .ToList();
            return Ok(hoatDongs);
        }
    }
}
