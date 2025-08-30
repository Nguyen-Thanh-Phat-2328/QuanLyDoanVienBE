using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChiDoanController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public ChiDoanController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }
        //lấy danh sách
        [HttpGet]
        [Route("/ChiDoan/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.ChiDoans.ToList());
        }
        //Lấy chi đoàn theo mã đoàn viên
        [HttpGet]
        [Route("/ChiDoan/IdDoanVien/{id}")]
        public IActionResult GetChiDoanByIdDoanVien(String id)
        {
            var dv = dbc.DoanViens.FirstOrDefault(dv => dv.IddoanVien == id);

            if (dv == null)
            {
                return NotFound(new { mesage = $"Không có đoàn viên trên" });
            }

            var cd = dbc.ChiDoans.Where(cd => cd.IdchiDoan == dv.IdchiDoan).Select(cd => new
            {
                cd.IdchiDoan,
                cd.TenChiDoan
            }).FirstOrDefault();

            if (cd == null)
            {
                return NotFound(new { message = "Không tìm thấy chi đoàn tương ứng." });
            }

            return Ok(cd);
        }
    }
}
