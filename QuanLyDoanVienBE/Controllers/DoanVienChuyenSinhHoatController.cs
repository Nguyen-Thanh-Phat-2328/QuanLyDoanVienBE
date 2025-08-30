using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyDoanVienBE.Dto;
using QuanLyDoanVienBE.ModelFromDB;

namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DoanVienChuyenSinhHoatController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public DoanVienChuyenSinhHoatController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        //lấy danh sách các đoàn viên đang chờ duyệt sinh hoạt đoàn
        [HttpGet]
        [Route("/PhieuChuyenSinhHoat/{trangthai}")]
        public IActionResult PhieuChuyenSinhHoatByTrangThai(bool trangthai)
        {
            var result = (from p in dbc.PhieuChuyenSinhHoatDoans
                          join d in dbc.DoanViens on p.IddoanVien equals d.IddoanVien
                          where p.TrangThai == trangthai
                          select new
                          {
                              p.Idphieu,
                              p.IddoanVien,
                              d.TenDoanVien,
                              d.NgaySinh,
                              p.ChiDoanMoi,
                              p.ChiDoanHienTai,
                              p.LyDo,
                              p.TrangThai,
                              p.NgayLapPhieu,
                              p.NgayCapNhat,
                              p.NguoiDuyet
                          }).ToList();

            if (result == null || result.Count == 0)
            {
                return NotFound(new { message = "Danh sách trống" });
            }

            return Ok(result);
        }

        //lấy đoàn viên đang chờ duyệt sinh hoạt đoàn theo mã đoàn viên
        [HttpGet]
        [Route("/PhieuChuyenSinhHoat/MaDoanVien/{id}")]
        public IActionResult PhieuChuyenSinhHoatByMaDoanVien(string id)
        {
            var result = (from p in dbc.PhieuChuyenSinhHoatDoans
                          join d in dbc.DoanViens on p.IddoanVien equals d.IddoanVien
                          where p.IddoanVien == id
                          select new
                          {
                              p.Idphieu,
                              p.IddoanVien,
                              d.TenDoanVien,
                              d.NgaySinh,
                              p.ChiDoanMoi,
                              p.ChiDoanHienTai,
                              p.LyDo,
                              p.TrangThai,
                              p.NgayLapPhieu,
                              p.NgayCapNhat,
                              d.HinhAnh,
                              p.NguoiDuyet
                          }).FirstOrDefault();

            if (result == null)
            {
                return NotFound(new { message = "Đoàn viên này không có đăng ký chuyển sinh hoạt" });
            }

            return Ok(result);
        }

        //update trạng thái (đã được duyệt hay chưa)
        [HttpPut]
        [Route("/PhieuChuyenSinhHoat/UpdateTrangThai")]
        public IActionResult UpdateTrangThaiSoDoan(int idPhieu, bool trangThaiSoDoan, string nguoiDuyet)
        {
            var dv = dbc.PhieuChuyenSinhHoatDoans.FirstOrDefault(d => d.Idphieu == idPhieu);
            if (dv == null)
            {
                return NotFound();
            }

            dv.Idphieu = idPhieu;
            dv.TrangThai = trangThaiSoDoan;
            dv.NguoiDuyet = nguoiDuyet;

            var thongBao = "";
            if (trangThaiSoDoan == true)
            {
                thongBao = "Duyệt chuyển công tác đoàn thành công";
            }
            else
            {
                thongBao = "Huỷ duyệt thành công";
            }

            dbc.SaveChanges();
            return Ok(new
            {
                message = thongBao,
                id = dv.IddoanVien
            });
        }

        [HttpPut]
        [Route("/PhieuChuyenSinhHoat/DuyetTrucTiep")]
        public IActionResult DuyetPhieuTrucTiep([FromBody] DuyetPhieuRequest request)
        {
            if (request.IdList == null || !request.IdList.Any())
                return BadRequest("Danh sách xóa trống");
            foreach (var id in request.IdList)
            {
                var dv = dbc.PhieuChuyenSinhHoatDoans.FirstOrDefault(d => d.Idphieu == id);
                if (dv == null)
                {
                    return NotFound($"Không tìm thấy phiếu với ID {id}");
                }

                dv.TrangThai = request.TrangThaiSoDoan;
                dv.NguoiDuyet = request.NguoiDuyet;
            }
            dbc.SaveChanges();
            return Ok(new { message = "Đã duyệt phiếu chuyển sinh hoạt thành công" });
        }
    }


}
