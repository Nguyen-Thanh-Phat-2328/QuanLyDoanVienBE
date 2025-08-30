using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    public class LoaiThongBaoController : Controller
    {
        dbQuanLyDoanVien dbc;
        public LoaiThongBaoController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("/LoaiThongBao/GetAll")]
        public IActionResult GetLoaiThongBao()
        {
            var loaiThongBaos = dbc.LoaiThongBaos
                .Select(l => new { l.IdloaiThongBao, l.TenThongBao })
                .ToList();

            return Ok(loaiThongBaos);
        }

        [HttpGet]
        [Route("/LoaiThongBao/GetById/{id}")]
        public IActionResult GetLoaiThongBaoById(int id)
        {
            var loaiThongBao = dbc.LoaiThongBaos
                .Where(l => l.IdloaiThongBao == id)
                .Select(l => new { l.IdloaiThongBao, l.TenThongBao })
                .FirstOrDefault();

            if (loaiThongBao == null)
                return NotFound();

            return Ok(loaiThongBao);
        }

    }
}
