using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public TaiKhoanController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        //lấy danh sách
        [HttpGet]
        [Route("/TaiKhoan/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.TaiKhoans.ToList());
        }

        //insert
        [HttpPost]
        [Route("/TaiKhoan/Insert")]
        public IActionResult InsertTaiKhoan(String idTaiKhoan, String matKhau, String idChucVu)
        {
            TaiKhoan tk = new TaiKhoan();
            tk.IdtaiKhoan = idTaiKhoan;
            tk.MatKhau = matKhau;
            

            dbc.TaiKhoans.Add(tk);
            dbc.SaveChanges();
            return Ok(new { tk });
        }
    }
}
