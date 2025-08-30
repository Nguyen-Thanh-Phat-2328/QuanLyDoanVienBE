using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanVienBE.Dto;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DoanVienController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public DoanVienController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        //lấy danh sách
        [HttpGet]
        [Route("/DoanVien/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.DoanViens.ToList());
        }

        //lấy danh sách đoàn viên theo chi đoàn
        [HttpGet]
        [Route("IdChiDoan/{id}")]
        public IActionResult GetListByIdChiDoan(String id)
        {
            var dv = dbc.DoanViens.Where(dv => dv.IdchiDoan == id).ToList();
            if (dv == null || dv.Count == 0)
            {
                return NotFound(new { mesage = $"Danh sách trống" });
            }
            return Ok(dv);
        }

        //lấy đoàn viên theo mã đoàn viên
        [HttpGet]
        [Route("/DoanVien/IdDoanVien/{id}")]
        public IActionResult GetDoanVienByIdDoanVien(String id)
        {
            var dv = dbc.DoanViens.FirstOrDefault(dv => dv.IddoanVien == id);
            if (dv == null)
            {
                return NotFound(new { mesage = $"Không có đoàn viên trên" });
            }
            return Ok(dv);
        }

        //lấy ds đoàn viên theo Tên đoàn viên
        [HttpGet]
        [Route("/DoanVien/TenDoanVien")]
        public async Task<IActionResult> GetDoanVienByTenDoanVien([FromQuery] String ten, [FromQuery] String lop)
        {
            string sql = "select * from DoanVien where contains(TenDoanVien,{0}) and IDChiDoan = {1}";
            var ds = await dbc.DoanViens.FromSqlRaw(sql, $"\"{ten}\"", lop).ToListAsync();
            return Ok(ds);
        }

        //lấy ds đoàn viên theo chi đoàn
        [HttpGet]
        [Route("/DoanVien/MaChiDoan")]
        public async Task<IActionResult> GetDoanVienByMaChiDoan([FromQuery] String chiDoan, [FromQuery] String? chucVu)
        {
            var dv = dbc.DoanViens.Where(dv => dv.IdchiDoan == chiDoan);
            if (!string.IsNullOrEmpty(chucVu))
            {
                dv = dv.Where(dv => dv.IdchucVu == chucVu);
            }
            return Ok(dv.ToList());
        }

        //insert
        [HttpPost]
        [Route("/DoanVien/Insert")]
        public IActionResult InsertDoanVien([FromBody] DoanVienRequest request)
        {
            var dv = new DoanVien
            {
                IddoanVien = request.IdDoanVien,
                TenDoanVien = request.TenDoanVien,
                NgaySinh = request.NgaySinh,
                IdphuongXa = request.IdPhuongXa,
                Sdt = request.SoDienThoai,
                Email = request.Email,
                GioiTinh = request.GioiTinh,
                NgayVaoDoan = request.NgayVaoDoan,
                TrangThaiSoDoan = request.TrangThaiSoDoan,
                IdtaiKhoan = request.IdDoanVien,
                IdchiDoan = request.IdChiDoan,
                IdchucVu = request.IdChucVu,
                HinhAnh = string.IsNullOrEmpty(request.HinhAnh) ? null : request.HinhAnh
            };
            dbc.DoanViens.Add(dv);
            var tk = new TaiKhoan
            {
                IdtaiKhoan = request.IdDoanVien,
                MatKhau = "123456"
            };
            dbc.TaiKhoans.Add(tk);

            dbc.SaveChanges();
            return Ok(new
            {
                message = "Thêm đoàn viên thành công",
                id = dv.IddoanVien
            });
        }

        //update 
        [HttpPut]
        [Route("/DoanVien/Update")]
        public IActionResult UpdateDoanVien([FromBody] DoanVienRequest request)
        {
            var dv = dbc.DoanViens.FirstOrDefault(d => d.IddoanVien == request.IdDoanVien);
            if (dv == null)
            {
                return NotFound();
            }

            dv.TenDoanVien = request.TenDoanVien;
            dv.NgaySinh = request.NgaySinh;
            dv.IdphuongXa = request.IdPhuongXa;
            dv.Sdt = request.SoDienThoai;
            dv.Email = request.Email;
            dv.GioiTinh = request.GioiTinh;
            dv.NgayVaoDoan = request.NgayVaoDoan;
            dv.IdchucVu = request.IdChucVu;

            // Cập nhật ảnh nếu có
            if (!string.IsNullOrEmpty(request.HinhAnh))
            {
                dv.HinhAnh = request.HinhAnh;
            }

            dbc.SaveChanges();
            return Ok(new
            {
                message = "Sửa đoàn viên thành công",
                id = dv.IddoanVien
            });
        }

        //update trạng thái sổ đoàn
        [HttpPut]
        [Route("/DoanVien/UpdateTrangThaiSoDoan")]
        public IActionResult UpdateTrangThaiSoDoan(String idDoanVien, bool trangThaiSoDoan)
        {
            var dv = dbc.DoanViens.FirstOrDefault(d => d.IddoanVien == idDoanVien);
            if (dv == null)
            {
                return NotFound();
            }

            dv.IddoanVien = idDoanVien;
            dv.TrangThaiSoDoan = trangThaiSoDoan;
            if (trangThaiSoDoan == false)
            {
                dv.NgayNopSoDoan = null;
            }
            else
            {
                dv.NgayNopSoDoan = DateOnly.FromDateTime(DateTime.Now);
            }


            dbc.SaveChanges();
            return Ok(new
            {
                message = "Duyệt sổ đoàn thành công",
                id = dv.IddoanVien,
                ngayNop = dv.NgayNopSoDoan
            });
        }

        [HttpDelete]
        [Route("/DoanVien/Delete")]
        public IActionResult DeleteListDoanVien([FromBody] List<string> idList)
        {
            if (idList == null || !idList.Any())
                return BadRequest("Danh sách xóa trống");
            foreach (var id in idList)
            {
                var dv = dbc.DoanViens.FirstOrDefault(d => d.IddoanVien == id);
                if (dv != null)
                {
                    var tk = dbc.TaiKhoans.FirstOrDefault(t => t.IdtaiKhoan == dv.IdtaiKhoan);
                    if (tk != null)
                    {
                        dbc.TaiKhoans.Remove(tk);
                    }
                    dbc.DoanViens.Remove(dv);
                }
            }
            dbc.SaveChanges();
            return Ok(new { message = "Đã xóa đoàn viên thành công" });
        }

        //lấy đoàn viên đã nộp
        [HttpGet]
        [Route("/DoanVien/DaNop/IdDoanPhi/{idDoanPhi}")]
        public async Task<IActionResult> GetDanhSachDoanVienDaNop(int idDoanPhi)
        {
            var result = await (from dvn in dbc.DoanVienNopDoanPhis
                                join dv in dbc.DoanViens on dvn.IddoanVien equals dv.IddoanVien
                                join cd in dbc.ChiDoans on dv.IdchiDoan equals cd.IdchiDoan
                                where dvn.IddoanPhi == idDoanPhi
                                select new
                                {
                                    dv.IddoanVien,
                                    dv.TenDoanVien,
                                    dv.IdchiDoan,
                                    cd.TenChiDoan
                                }).ToListAsync();

            return Ok(result);
        }

        //lấy đoàn viên chưa nôipj
        [HttpGet]
        [Route("/DoanVien/ChuaNop/IdDoanPhi/{idDoanPhi}")]
        public async Task<IActionResult> GetDanhSachDoanVienChuaNop(int idDoanPhi)
        {
            // Lấy danh sách ID đoàn viên đã nộp
            var daNopIds = await dbc.DoanVienNopDoanPhis
                                    .Where(x => x.IddoanPhi == idDoanPhi)
                                    .Select(x => x.IddoanVien)
                                    .ToListAsync();

            // Lấy danh sách đoàn viên chưa nộp (không nằm trong danh sách đã nộp)
            var result = await (from dv in dbc.DoanViens
                                join cd in dbc.ChiDoans on dv.IdchiDoan equals cd.IdchiDoan
                                where !daNopIds.Contains(dv.IddoanVien)
                                select new
                                {
                                    dv.IddoanVien,
                                    dv.TenDoanVien,
                                    dv.IdchiDoan,
                                    cd.TenChiDoan
                                }).ToListAsync();

            return Ok(result);
        }

        //lấy ds đoàn viên theo Tên đoàn viên theo toàn bộ đoàn viên
        [HttpGet]
        [Route("/DoanVien/TenDoanVienAll")]
        public async Task<IActionResult> GetDoanVienByTenDoanVienAll([FromQuery] String ten)
        {
            string sql = "select * from DoanVien where contains(TenDoanVien,{0})";
            var ds = await dbc.DoanViens.FromSqlRaw(sql, $"\"{ten}\"").ToListAsync();
            return Ok(ds);
        }

        ////lấy ds đoàn viên theo chức vụ theo lớp
        [HttpGet]
        [Route("/DoanVien/MaChucVuAll")]
        public async Task<IActionResult> GetDoanVienByMaChucVuAll([FromQuery] String? chucVu)
        {
            var dv = dbc.DoanViens.AsQueryable();
            if (!string.IsNullOrEmpty(chucVu))
            {
                dv = dv.Where(dv => dv.IdchucVu == chucVu);
            }
            var result = await dv.ToListAsync();
            return Ok(result);
        }
    }
}
