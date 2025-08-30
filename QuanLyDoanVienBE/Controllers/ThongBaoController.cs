using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanVienBE.Dto;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    public class ThongBaoController : Controller
    {

        dbQuanLyDoanVien dbc;
        public ThongBaoController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("/ThongBao/List")]
        public IActionResult GetList()
        {
            var thongBaoList = dbc.ThongBaos
                .Include(t => t.IdloaiThongBaoNavigation)
            //.Include(t => t.IdbanChapHanhNavigation) // Để sau khi có dữ liệu
                .Select(t => new ThongBaoDto
                {
                    IdthongBao = t.IdthongBao,
                    TieuDe = t.TieuDe,
                    NoiDung = t.NoiDung,
                    NgayBanHanh = t.NgayBanHanh,
                    FileDinhKem = t.FileDinhKem,
                    IdloaiThongBao = t.IdloaiThongBao,
                    TenLoaiThongBao = t.IdloaiThongBaoNavigation != null ? t.IdloaiThongBaoNavigation.TenThongBao : null, //tên loại tb

                    IdbanChapHanh = t.IdbanChapHanh,
                    TenBanChapHanh = null // Tạm để null, sau này sẽ lấy từ bảng BanChapHanh
                })
                .ToList();

            return Ok(thongBaoList);
        }


        [HttpGet]
        [Route("/ThongBao/GetById/{id}")]
        public IActionResult GetById(string id)
        {
            var thongbao = dbc.ThongBaos.FirstOrDefault(h => h.IdthongBao == id);
            if (thongbao == null)
                return NotFound(new { message = "Không tìm thấy thông báo." });

            return Ok(thongbao);
        }

        [HttpPost]
        [Route("/ThongBao/Insert")]
        public async Task<IActionResult> InsertThongBao([FromForm] ThongBaoPostDto dto)
        {
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            // Tạo ID mới dựa trên mã lớn nhất 
            var lastId = dbc.ThongBaos
    .Select(h => h.IdthongBao)
    .Where(id => id.StartsWith("TB"))
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

            var id = $"TB{nextNumber}";

            // Xử lý file ảnh
            string fileName = null;
            if (dto.FileDinhKem != null && dto.FileDinhKem.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var originalFileName = Path.GetFileName(dto.FileDinhKem.FileName);

                // Gắn GUID để duy nhất, vẫn giữ tên gốc
                fileName = $"{Guid.NewGuid()}_{originalFileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.FileDinhKem.CopyToAsync(stream);
                }
            }

            var thongbao = new ThongBao
            {
                IdthongBao = id,
                TieuDe = dto.TieuDe,
                NoiDung = dto.NoiDung,
                IdloaiThongBao = dto.IdloaiThongBao,
                FileDinhKem = fileName,
                NgayBanHanh = DateOnly.FromDateTime(DateTime.Now),
                IdbanChapHanh = null
            };

            dbc.ThongBaos.Add(thongbao);
            await dbc.SaveChangesAsync();

            return Ok(thongbao);
        }

        [HttpPut]
        [Route("/ThongBao/Update/{id}")]
        public async Task<IActionResult> UpdateThongBao(string id, [FromForm] ThongBaoPostDto dto)
        {
            var thongBao = dbc.ThongBaos.FirstOrDefault(h => h.IdthongBao == id);
            if (thongBao == null)
                return NotFound("Không tìm thấy thông báo");

            // Cập nhật dữ liệu cơ bản
            thongBao.TieuDe = dto.TieuDe;
            thongBao.NoiDung = dto.NoiDung;
            thongBao.IdloaiThongBao= dto.IdloaiThongBao;
            thongBao.NgayBanHanh = DateOnly.FromDateTime(DateTime.Now);

            // Nếu có ảnh mới thì xử lý
            if (dto.FileDinhKem != null && dto.FileDinhKem.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Xoá ảnh cũ nếu cần (tuỳ chọn)
                if (!string.IsNullOrEmpty(thongBao.FileDinhKem))
                {
                    var oldPath = Path.Combine(uploadsFolder, thongBao.FileDinhKem);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Lưu ảnh mới
                var fileName = $"{Guid.NewGuid()}_{dto.FileDinhKem.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.FileDinhKem.CopyToAsync(stream);
                }

                thongBao.FileDinhKem = fileName;
            }

            dbc.ThongBaos.Update(thongBao);
            await dbc.SaveChangesAsync();

            return Ok(thongBao);
        }

        [HttpDelete]
        [Route("/ThongBao/Delete/{id}")]
        public async Task<IActionResult> DeleteThongBao(string id)
        {
            var thongBao = await dbc.ThongBaos.FindAsync(id);
            if (thongBao == null)
                return NotFound("Không tìm thấy thông báo");

            // Nếu có ảnh thì xoá khỏi thư mục
            if (!string.IsNullOrEmpty(thongBao.FileDinhKem))
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
                var filePath = Path.Combine(uploadsFolder, thongBao.FileDinhKem);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            dbc.ThongBaos.Remove(thongBao);
            await dbc.SaveChangesAsync();

            return Ok(new { message = "Đã xoá thành công", id = id });
        }

        [HttpDelete]
        [Route("/ThongBao/DeleteMultiple")]
        public async Task<IActionResult> DeleteMultipleThongBao([FromBody] List<string> ids)
        {
            var thongBaos = dbc.ThongBaos.Where(tb => ids.Contains(tb.IdthongBao)).ToList();

            if (!thongBaos.Any())
                return NotFound("Không tìm thấy thông báo nào để xóa.");

            dbc.ThongBaos.RemoveRange(thongBaos);
            await dbc.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }

    }
}
