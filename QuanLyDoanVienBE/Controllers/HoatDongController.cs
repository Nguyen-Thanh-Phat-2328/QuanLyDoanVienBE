using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLyDoanVienBE.Dto;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HoatDongController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public HoatDongController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("/HoatDong/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.HoatDongs.ToList());
        }

        [HttpGet]
        [Route("/HoatDong/GetById/{id}")]
        public IActionResult GetById(string id)
        {
            var hoatDong = dbc.HoatDongs.FirstOrDefault(h => h.IdhoatDong == id);
            if (hoatDong == null)
                return NotFound(new { message = "Không tìm thấy hoạt động." });

            return Ok(hoatDong);
        }

        [HttpPost]
        [Route("/HoatDong/Insert")]
        public async Task<IActionResult> InsertHoatDong([FromForm] HoatDongDto dto)
        {
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            // Tạo ID mới dựa trên mã lớn nhất 
            var lastId = dbc.HoatDongs
    .Select(h => h.IdhoatDong)
    .Where(id => id.StartsWith("HD"))
    .AsEnumerable() // chuyển sang LINQ to Objects
    .OrderByDescending(id =>
        int.TryParse(id.Substring(2), out var n) ? n : 0
    )
    .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastId))
            {
                var numberPart = lastId.Substring(2);
                if (int.TryParse(numberPart, out var n))
                    nextNumber = n + 1;
            }

            var id = $"HD{nextNumber}";


            // Xử lý file ảnh (ví dụ: lưu vào wwwroot/uploads)
            string fileName = null;
            if (dto.HinhAnh != null && dto.HinhAnh.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                fileName = $"{Guid.NewGuid()}_{dto.HinhAnh.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.HinhAnh.CopyToAsync(stream);
                }
            }

            var hoatDong = new HoatDong
            {
                IdhoatDong = id,
                TenHoatDong = dto.TenHoatDong,
                NoiDung = dto.NoiDung,
                DiemHoatDong = dto.DiemHoatDong,
                TongSoDoanVien = dto.TongSoDoanVien,
                DiaDiem = dto.DiaDiem,
                ThoiGianBatDau = DateOnly.FromDateTime(dto.ThoiGianBatDau),
                ThoiGianKetThuc = DateOnly.FromDateTime(dto.ThoiGianKetThuc),
                HinhAnh = fileName // lưu tên file để dùng khi hiển thị ảnh
            };

            dbc.HoatDongs.Add(hoatDong);
            dbc.SaveChanges();

            return Ok(hoatDong);
        }

        [HttpPut]
        [Route("/HoatDong/Update/{id}")]
        public async Task<IActionResult> UpdateHoatDong(string id, [FromForm] HoatDongDto dto)
        {
            var hoatDong = dbc.HoatDongs.FirstOrDefault(h => h.IdhoatDong == id);
            if (hoatDong == null)
                return NotFound("Không tìm thấy hoạt động");

            // Cập nhật dữ liệu cơ bản
            hoatDong.TenHoatDong = dto.TenHoatDong;
            hoatDong.NoiDung = dto.NoiDung;
            hoatDong.DiemHoatDong = dto.DiemHoatDong;
            hoatDong.TongSoDoanVien = dto.TongSoDoanVien;
            hoatDong.DiaDiem = dto.DiaDiem;
            hoatDong.ThoiGianBatDau = DateOnly.FromDateTime(dto.ThoiGianBatDau);
            hoatDong.ThoiGianKetThuc = DateOnly.FromDateTime(dto.ThoiGianKetThuc);

            // Nếu có ảnh mới thì xử lý
            if (dto.HinhAnh != null && dto.HinhAnh.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Xoá ảnh cũ nếu cần (tuỳ chọn)
                if (!string.IsNullOrEmpty(hoatDong.HinhAnh))
                {
                    var oldPath = Path.Combine(uploadsFolder, hoatDong.HinhAnh);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Lưu ảnh mới
                var fileName = $"{Guid.NewGuid()}_{dto.HinhAnh.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.HinhAnh.CopyToAsync(stream);
                }

                hoatDong.HinhAnh = fileName;
            }

            dbc.HoatDongs.Update(hoatDong);
            await dbc.SaveChangesAsync();

            return Ok(hoatDong);
        }

        [HttpDelete]
        [Route("/HoatDong/Delete/{id}")]
        public async Task<IActionResult> DeleteHoatDong(string id)
        {
            var hoatDong = await dbc.HoatDongs.FindAsync(id);
            if (hoatDong == null)
                return NotFound("Không tìm thấy hoạt động");

            // Nếu có ảnh thì xoá khỏi thư mục uploads
            if (!string.IsNullOrEmpty(hoatDong.HinhAnh))
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var filePath = Path.Combine(uploadsFolder, hoatDong.HinhAnh);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            dbc.HoatDongs.Remove(hoatDong);
            await dbc.SaveChangesAsync();

            return Ok(new { message = "Đã xoá hoạt động thành công", id = id });
        }

        //danh sách và số lượng đki hđ
        [HttpGet("{id}/DoanVien")]
        public async Task<IActionResult> GetDoanVienThamGiaHoatDong(string id)
        {
            var danhSach = await dbc.ThamGiaHoatDongs
                .Where(t => t.IdhoatDong == id)
                .Include(t => t.IddoanVienNavigation)
                    .ThenInclude(dv => dv.IdchiDoanNavigation) // Include bảng ChiDoan
                .Select(t => new DoanVienThamGiaDto
                {
                    //IDDoanVien = t.IddoanVien,
                    //TenDoanVien = t.IddoanVienNavigation.TenDoanVien,
                    //NgaySinh = t.IddoanVienNavigation.NgaySinh,
                    //Email = t.IddoanVienNavigation.Email,
                    //SDT = t.IddoanVienNavigation.Sdt,
                    //NgayDangKy = t.NgayDangKy,
                    //TenChiDoan = t.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan
                    IDDoanVien = t.IddoanVien,
                    TenDoanVien = t.IddoanVienNavigation.TenDoanVien,
                    NgaySinh = t.IddoanVienNavigation.NgaySinh,
                    Email = t.IddoanVienNavigation.Email,
                    SDT = t.IddoanVienNavigation.Sdt,
                    NgayDangKy = t.NgayDangKy,
                    TenChiDoan = t.IddoanVienNavigation.IdchiDoanNavigation.TenChiDoan,
                    IdhoatDong = id,
                    TrangThai = t.TrangThai
                })
                .ToListAsync();

            return Ok(danhSach);
        }


        [HttpGet("{id}/SoLuongDangKy")]
        public async Task<IActionResult> GetSoLuongDangKy(string id)
        {
            var count = await dbc.ThamGiaHoatDongs
                .Where(t => t.IdhoatDong == id)
                .CountAsync();

            return Ok(count); // Trả về số nguyên
        }

        [HttpGet]
        [Route("/HoatDong/XuatDanhSachDangKy/{id}")]
        public async Task<IActionResult> XuatDanhSachDangKy(string id)
        {
            // Lấy dữ liệu gốc
            var rawData = await (from t in dbc.ThamGiaHoatDongs
                                 join d in dbc.DoanViens on t.IddoanVien equals d.IddoanVien
                                 join cd in dbc.ChiDoans on d.IdchiDoan equals cd.IdchiDoan
                                 where t.IdhoatDong == id
                                 select new
                                 {
                                     d.IddoanVien,
                                     d.TenDoanVien,
                                     TenChiDoan = cd.TenChiDoan,
                                     t.NgayDangKy
                                 }).ToListAsync();

            // Tạo danh sách có STT
            var data = rawData.Select((x, index) => new
            {
                STT = index + 1,
                x.IddoanVien,
                x.TenDoanVien,
                x.TenChiDoan,
                NgayDangKy = x.NgayDangKy?.ToString("dd/MM/yyyy")
            }).ToList();

            // Khởi tạo Excel
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("DaDangKy");

                // Ghi tiêu đề cột
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã Đoàn viên";
                worksheet.Cells[1, 3].Value = "Tên Đoàn viên";
                worksheet.Cells[1, 4].Value = "Chi đoàn";
                worksheet.Cells[1, 5].Value = "Ngày đăng ký";
                worksheet.Cells[1, 6].Value = "Ký tên";

                // Ghi dữ liệu từng dòng
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].STT;
                    worksheet.Cells[i + 2, 2].Value = data[i].IddoanVien;
                    worksheet.Cells[i + 2, 3].Value = data[i].TenDoanVien;
                    worksheet.Cells[i + 2, 4].Value = data[i].TenChiDoan;
                    worksheet.Cells[i + 2, 5].Value = data[i].NgayDangKy;
                }

                worksheet.Cells.AutoFitColumns(); // Tự động điều chỉnh độ rộng cột
                package.Save();
            }

            stream.Position = 0;
            string excelName = $"DoanVien_DaDangKy_{id}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        //update trạng thái tham gia hoạt động
        [HttpPut]
        [Route("/HoatDong/UpdateTrangThaiThamGia")]
        public IActionResult UpdateTrangThaiSoDoan(String idHoatDong, String idDoanVien, bool trangThai)
        {
            var dv = dbc.ThamGiaHoatDongs.FirstOrDefault(d => d.IdhoatDong == idHoatDong && d.IddoanVien == idDoanVien);
            if (dv == null)
            {
                return NotFound();
            }

            dv.TrangThai = trangThai;

            dbc.SaveChanges();
            return Ok(new
            {
                message = "Duyệt sổ thành công",
                hd = dv.IdhoatDong,
                iddv = dv.IddoanVien,
            });
        }
    }
}
