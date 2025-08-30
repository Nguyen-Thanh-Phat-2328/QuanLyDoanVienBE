using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanVienBE.ModelFromDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuanLyPhanQuyenController : ControllerBase
    {
        private readonly dbQuanLyDoanVien _context;

        public QuanLyPhanQuyenController(dbQuanLyDoanVien context)
        {
            _context = context;
        }
        // GET: api/QuanLyPhanQuyen/roles - Lấy danh sách vai trò
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoles()
        {
            var roles = await _context.ChucVus
                .Select(c => new {
                    c.IdchucVu,
                    c.TenChucVu
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/QuanLyPhanQuyen?search=keyword
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTaiKhoans(string search = "")
        {
            var query = _context.TaiKhoans
                .Include(t => t.DoanViens)
                    .ThenInclude(dv => dv.IdchucVuNavigation)
                .Include(t => t.BanChapHanhs)
                .AsQueryable();

            // Thêm điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    t.IdtaiKhoan.Contains(search) ||
                    (t.DoanViens.FirstOrDefault() != null &&
                     t.DoanViens.FirstOrDefault().TenDoanVien.Contains(search)));
            }

            var result = await query
                .Select(t => new
                {
                    t.IdtaiKhoan,
                    TenDoanVien = t.DoanViens.FirstOrDefault() != null
                        ? t.DoanViens.FirstOrDefault().TenDoanVien
                        : null,
                    Username = t.IdtaiKhoan, // Giả sử username là IdtaiKhoan
                    Password = "********", // Ẩn mật khẩu thật
                    VaiTro = t.DoanViens.FirstOrDefault() != null
                        ? t.DoanViens.FirstOrDefault().IdchucVuNavigation != null
                            ? t.DoanViens.FirstOrDefault().IdchucVuNavigation.TenChucVu
                            : "Đoàn viên" // Mặc định nếu không có chức vụ
                        : "Đoàn viên",
                    TrangThai = DetermineAccountStatus(t), // Xác định trạng thái
                    TenBanChapHanh = t.BanChapHanhs.FirstOrDefault() != null
                        ? t.BanChapHanhs.FirstOrDefault().TenBch
                        : null
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(result);
        }

        // Helper method để xác định trạng thái tài khoản
        private static string DetermineAccountStatus(TaiKhoan taiKhoan)
        {
            // Kiểm tra nếu tài khoản đang được sử dụng bởi đoàn viên hoặc ban chấp hành
            bool isActive = taiKhoan.DoanViens.Any() || taiKhoan.BanChapHanhs.Any();
            return isActive ? "Hoạt động" : "Không hoạt động";
        }

        // GET: api/QuanLyPhanQuyen/TK001
        [HttpGet("{idTaiKhoan}")]
        public async Task<ActionResult<object>> GetTaiKhoanById(string idTaiKhoan)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.DoanViens)
                    .ThenInclude(dv => dv.IdchucVuNavigation)
                .Include(t => t.BanChapHanhs)
                .Where(t => t.IdtaiKhoan == idTaiKhoan)
                .Select(t => new
                {
                    t.IdtaiKhoan,
                    TenDoanVien = t.DoanViens.FirstOrDefault() != null
                        ? t.DoanViens.FirstOrDefault().TenDoanVien
                        : null,
                    Username = t.IdtaiKhoan,
                    Password = "********",
                    VaiTro = t.DoanViens.FirstOrDefault() != null
                        ? t.DoanViens.FirstOrDefault().IdchucVuNavigation != null
                            ? t.DoanViens.FirstOrDefault().IdchucVuNavigation.TenChucVu
                            : "Đoàn viên"
                        : "Đoàn viên",
                    TrangThai = DetermineAccountStatus(t),
                    TenBanChapHanh = t.BanChapHanhs.FirstOrDefault() != null
                        ? t.BanChapHanhs.FirstOrDefault().TenBch
                        : null
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (taiKhoan == null)
            {
                return NotFound();
            }

            return Ok(taiKhoan);
        }

        // POST: api/QuanLyPhanQuyen
        [HttpPost]
        public async Task<ActionResult<object>> CreateTaiKhoan([FromBody] CreateTaiKhoanModel model)
        {
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Password và ConfirmPassword không khớp");
            // Validate
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("Username là bắt buộc");
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Password là bắt buộc");
            if (string.IsNullOrEmpty(model.TenDoanVien))
                return BadRequest("Tên đoàn viên là bắt buộc");
            if (model.IdchucVu == null)
                return BadRequest("Vai trò là bắt buộc");

            // Kiểm tra username tồn tại
            if (await _context.TaiKhoans.AnyAsync(t => t.IdtaiKhoan == model.Username))
            {
                return Conflict("Username đã tồn tại");
            }

            // Tạo ID đoàn viên mới
            string newIdDoanVien = await GenerateNewDoanVienId();
            string email = $"{newIdDoanVien}@doanvien.vn";
            // Tạo tài khoản mới
            var taiKhoan = new TaiKhoan
            {
                IdtaiKhoan = model.Username,
                MatKhau = model.Password // Trong thực tế nên mã hóa
            };

            _context.TaiKhoans.Add(taiKhoan);

            // Tạo đoàn viên liên kết với tài khoản
            var doanVien = new DoanVien
            {
                IddoanVien = newIdDoanVien, // Gán ID mới tạo
                IdtaiKhoan = model.Username,
                TenDoanVien = model.TenDoanVien,
                IdchucVu = model.IdchucVu,
                Email = email
            };

            _context.DoanViens.Add(doanVien);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Lỗi khi tạo tài khoản: {ex.InnerException?.Message}");
            }

            // Lấy thông tin vai trò
            var chucVu = await _context.ChucVus.FindAsync(model.IdchucVu);

            return CreatedAtAction(nameof(GetTaiKhoanById), new { idTaiKhoan = taiKhoan.IdtaiKhoan }, new
            {
                taiKhoan.IdtaiKhoan,
                TenDoanVien = doanVien.TenDoanVien,
                Username = taiKhoan.IdtaiKhoan,
                Password = "********",
                VaiTro = chucVu?.TenChucVu ?? "Đoàn viên",
                TrangThai = "Hoạt động"
            });
        }
        private async Task<string> GenerateNewDoanVienId()
        {
            var idList = await _context.DoanViens
                .Where(d => d.IddoanVien.StartsWith("DV") && d.IddoanVien.Length >= 5)
                .Select(d => d.IddoanVien.Substring(2))
                .ToListAsync();

            var numberList = idList
                .Where(s => int.TryParse(s, out _))
                .Select(s => int.Parse(s))
                .ToList();

            int maxNumber = numberList.Any() ? numberList.Max() : 0;
            int nextNumber = maxNumber + 1;

            return $"DV{nextNumber:D3}";
        }



        // PUT: api/QuanLyPhanQuyen/TK001
        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateTaiKhoan(string username, [FromBody] UpdateTaiKhoanModel model)
        {
            // Validate
            if (string.IsNullOrEmpty(model.TenDoanVien))
                return BadRequest("Tên đoàn viên là bắt buộc");
            if (model.IdchucVu == null)
                return BadRequest("Vai trò là bắt buộc");

            // Tìm tài khoản và đoàn viên liên quan
            var taiKhoan = await _context.TaiKhoans.FindAsync(username);
            if (taiKhoan == null)
            {
                return NotFound("Tài khoản không tồn tại");
            }

            var doanVien = await _context.DoanViens
                .FirstOrDefaultAsync(d => d.IdtaiKhoan == username);

            if (doanVien == null)
            {
                return NotFound("Không tìm thấy thông tin đoàn viên");
            }

            // Cập nhật thông tin đoàn viên
            doanVien.TenDoanVien = model.TenDoanVien;
            doanVien.IdchucVu = model.IdchucVu;

            // Cập nhật mật khẩu nếu có
            if (!string.IsNullOrEmpty(model.Password))
            {
                taiKhoan.MatKhau = model.Password;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Lỗi khi cập nhật: {ex.InnerException?.Message}");
            }

            return NoContent();
        }

        // DELETE: api/QuanLyPhanQuyen/TK001
        [HttpDelete("{idTaiKhoan}")]
        public async Task<IActionResult> DeleteTaiKhoan(string idTaiKhoan)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(idTaiKhoan);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            // Xóa tài khoản mà không ảnh hưởng đến đoàn viên
            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/QuanLyPhanQuyen/TK001/cap-nhat
        [HttpPatch("{idTaiKhoan}/cap-nhat")]
        public async Task<IActionResult> UpdateThongTin(string idTaiKhoan, [FromBody] UpdateThongTinModel model)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.DoanViens)
                .FirstOrDefaultAsync(t => t.IdtaiKhoan == idTaiKhoan);

            if (taiKhoan == null)
            {
                return NotFound();
            }

            // Cập nhật mật khẩu nếu có
            if (!string.IsNullOrEmpty(model.Password))
            {
                taiKhoan.MatKhau = model.Password;
            }

            // Cập nhật thông tin đoàn viên liên quan nếu có
            var doanVien = taiKhoan.DoanViens.FirstOrDefault();
            if (doanVien != null)
            {
                if (!string.IsNullOrEmpty(model.TenDoanVien))
                {
                    doanVien.TenDoanVien = model.TenDoanVien;
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaiKhoanExists(string id)
        {
            return _context.TaiKhoans.Any(e => e.IdtaiKhoan == id);
        }
    }

    // Model cho tạo tài khoản
    public class CreateTaiKhoanModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string TenDoanVien { get; set; }
        public string? IdchucVu { get; set; } // Nullable nếu không bắt buộc
    }

    // Model cho cập nhật tài khoản
    public class UpdateTaiKhoanModel
    {
        public string? Password { get; set; }
        public string TenDoanVien { get; set; }
        public string IdchucVu { get; set; }
    }

    // Model cho cập nhật thông tin
    public class UpdateThongTinModel
    {
        public string Password { get; set; }
        public string TenDoanVien { get; set; }
        public string ChucVu { get; set; }
    }
}