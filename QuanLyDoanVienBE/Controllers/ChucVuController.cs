using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChucVuController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public ChucVuController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        //Lấy toàn bộ chức vụ
        [HttpGet]
        [Route("/ChucVu/List")]
        public IActionResult GetAll()
        {
            return Ok(dbc.ChucVus.ToList());
        }

        //Lấy chức vụ theo mã đoàn viên
        [HttpGet]
        [Route("/ChucVu/IdDoanVien/{id}")]
        public IActionResult GetChucVuByIdDoanVien(String id)
        {
            var doanVien = dbc.DoanViens.FirstOrDefault(dv => dv.IddoanVien == id);
            if (doanVien == null)
            {
                return NotFound(new { message = "Không tìm thấy đoàn viên này" });
            }

            var chucVu = dbc.ChucVus.Where(cv => cv.IdchucVu == doanVien.IdchucVu).Select(cv => new
            {
                cv.IdchucVu,
                cv.TenChucVu,
                cv.MoTa
            }).FirstOrDefault();

            return Ok(chucVu);
        }
    }
}
