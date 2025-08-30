using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HocKyController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public HocKyController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        //lấy danh sách
        [HttpGet]
        [Route("/HocKy/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.HocKies.ToList());
        }
    }
}
