using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiaChiController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public DiaChiController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        //Lấy danh sách tỉnh thành
        [HttpGet]
        [Route("/DiaChi/TinhThanh/List")]
        public IActionResult GetListTinhThanh()
        {
            return Ok(dbc.TinhThanhs.ToList());
        }
        //Lấy ds huyện Quận
        [HttpGet]
        [Route("/DiaChi/QuanHuyen/List/IdTinhThanh/{id}")]
        public IActionResult GetListQuanHuyen(int id)
        {

            var quanHuyen = dbc.QuanHuyens.Where(qh => qh.IdtinhThanh == id);
            if (quanHuyen == null)
            {
                return NotFound("Không tồn tại quận huyện này");
            }
            return Ok(quanHuyen);
        }
        //lấy ds xã phường
        [HttpGet]
        [Route("/DiaChi/PhuongXa/List/IdQuanHuyen/{id}")]
        public IActionResult GetListPhuongXa(int id)
        {
            var xaPhuong = dbc.PhuongXas.Where(px => px.IdquanHuyen == id);
            if (xaPhuong == null)
            {
                return NotFound("Không tồn tại xã phường này");
            }
            return Ok(xaPhuong);
        }

        //lấy địa chỉ theo idDoanVien
        //Lấy xã
        [HttpGet]
        [Route("/DiaChi/PhuongXa/IdDoanVien/{id}")]
        public IActionResult GetPhuongXa(String id)
        {
            var doanVien = dbc.DoanViens.FirstOrDefault(dv => dv.IddoanVien == id);
            if (doanVien == null)
            {
                return NotFound(new { mesage = $"Không có đoàn viên trên" });
            }
            var phuongXa = dbc.PhuongXas.Where(px => px.IdphuongXa == doanVien.IdphuongXa)
                .Select(px => new
                {
                    px.IdphuongXa,
                    px.TenPhuongXa,
                    px.IdquanHuyen
                }).FirstOrDefault();

            if (phuongXa == null)
            {
                return NotFound("Không tìm thấy phường xã");
            }
            return Ok(phuongXa);
        }
        //lấy huyện
        [HttpGet]
        [Route("/DiaChi/QuanHuyen/IdDoanVien/{id}")]
        public IActionResult GetQuanHuyen(String id)
        {
            var doanVien = dbc.DoanViens.FirstOrDefault(dv => dv.IddoanVien == id);
            if (doanVien == null)
            {
                return NotFound(new { mesage = $"Không có đoàn viên trên" });
            }
            var phuongXa = dbc.PhuongXas.Where(px => px.IdphuongXa == doanVien.IdphuongXa)
                .Select(px => new
                {
                    px.IdphuongXa,
                    px.TenPhuongXa,
                    px.IdquanHuyen
                }).FirstOrDefault();

            if (phuongXa == null)
            {
                return NotFound("Không tìm thấy phường xã");
            }

            var quanHuyen = dbc.QuanHuyens.Where(px => px.IdquanHuyen == phuongXa.IdquanHuyen)
                .Select(px => new
                {
                    px.IdquanHuyen,
                    px.TenQuanHuyen,
                    px.IdtinhThanh
                }).FirstOrDefault();

            if (quanHuyen == null)
            {
                return NotFound("Không tìm thấy quận huyện");
            }
            return Ok(quanHuyen);
        }
        //lấy tỉnh
        [HttpGet]
        [Route("/DiaChi/TinhThanh/IdDoanVien/{id}")]
        public IActionResult GetTinhThanh(String id)
        {
            var doanVien = dbc.DoanViens.FirstOrDefault(dv => dv.IddoanVien == id);
            if (doanVien == null)
            {
                return NotFound(new { mesage = $"Không có đoàn viên trên" });
            }
            var phuongXa = dbc.PhuongXas.Where(px => px.IdphuongXa == doanVien.IdphuongXa)
                .Select(px => new
                {
                    px.IdphuongXa,
                    px.TenPhuongXa,
                    px.IdquanHuyen
                }).FirstOrDefault();

            if (phuongXa == null)
            {
                return NotFound("Không tìm thấy phường xã");
            }

            var quanHuyen = dbc.QuanHuyens.Where(px => px.IdquanHuyen == phuongXa.IdquanHuyen)
                .Select(px => new
                {
                    px.IdquanHuyen,
                    px.TenQuanHuyen,
                    px.IdtinhThanh
                }).FirstOrDefault();

            if (quanHuyen == null)
            {
                return NotFound("Không tìm thấy quận huyện");
            }

            var tinhThanh = dbc.TinhThanhs.Where(px => px.IdtinhThanh == quanHuyen.IdtinhThanh)
                .Select(px => new
                {
                    px.IdtinhThanh,
                    px.TenTinhThanh
                }).FirstOrDefault();

            if (tinhThanh == null)
            {
                return NotFound("Không tìm thấy Tỉnh Thành");
            }
            return Ok(tinhThanh);
        }

        [HttpGet]
        [Route("/DiaChi/PhuongXaDefault/")]
        public IActionResult GetDefaultPhuongXa()
        {
            var phuongXa = dbc.PhuongXas.Where(px => px.IdquanHuyen == 1).ToList();
            return Ok(phuongXa);
        }

        [HttpGet]
        [Route("/DiaChi/IdPhuongXa/{id}")]
        public IActionResult GetDiaChiDayDu(int id)
        {
            var phuongXa = dbc.PhuongXas.FirstOrDefault(px => px.IdphuongXa == id);
            if (phuongXa == null) return NotFound();

            var quanHuyen = dbc.QuanHuyens.FirstOrDefault(qh => qh.IdquanHuyen == phuongXa.IdquanHuyen);
            var tinhThanh = dbc.TinhThanhs.FirstOrDefault(tt => tt.IdtinhThanh == quanHuyen.IdtinhThanh);

            string diaChi = $"{phuongXa.TenPhuongXa} - {quanHuyen?.TenQuanHuyen} - \n{tinhThanh?.TenTinhThanh}";
            return Ok(diaChi);
        }
    }
}
