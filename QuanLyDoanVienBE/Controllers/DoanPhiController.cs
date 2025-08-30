using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLyDoanVienBE.ModelFromDB;
namespace QuanLyDoanVienBE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DoanPhiController : ControllerBase
    {
        dbQuanLyDoanVien dbc;
        public DoanPhiController(dbQuanLyDoanVien db)
        {
            dbc = db;
        }

        //lấy danh sách đoàn phí
        [HttpGet]
        [Route("/DoanPhi/List")]
        public async Task<IActionResult> ListDoanPhi()
        {
            var result = await (from p in dbc.DoanPhis
                                join d in dbc.HocKies on p.IdhocKy equals d.IdhocKy
                                select new
                                {
                                    p.IddoanPhi,
                                    p.TenDoanPhi,
                                    p.NoiDung,
                                    p.SoTien,
                                    p.NgayBatDau,
                                    p.NgayKetThuc,
                                    p.IdhocKy,
                                    d.KyHoc,
                                    d.NamHoc
                                }).ToListAsync();

            if (result == null || result.Count == 0)
            {
                return NotFound(new { message = "List rỗng" });
            }

            return Ok(result);
        }

        //insert
        [HttpPost]
        [Route("/DoanPhi/Insert")]
        public IActionResult InsertDoanVien(String tenDoanPhi, String noiDung, decimal soTien, DateOnly ngayBatDau, DateOnly ngayKetThuc, int idHocKy)
        {
            DoanPhi dv = new DoanPhi();
            dv.TenDoanPhi = tenDoanPhi;
            dv.NoiDung = noiDung;
            dv.SoTien = soTien;
            dv.NgayBatDau = ngayBatDau;
            dv.NgayKetThuc = ngayKetThuc;
            dv.IdhocKy = idHocKy;
            dbc.DoanPhis.Add(dv);

            dbc.SaveChanges();
            return Ok(new
            {
                message = "Thêm đoàn phí thành công",
                id = dv.IddoanPhi
            });
        }

        //update 
        [HttpPut]
        [Route("/DoanPhi/Update")]
        public IActionResult UpdateDoanVien(int idDoanPhi, String tenDoanPhi, String noiDung, decimal soTien, DateOnly ngayBatDau, DateOnly ngayKetThuc, int idHocKy)
        {
            var dv = dbc.DoanPhis.FirstOrDefault(d => d.IddoanPhi == idDoanPhi);
            if (dv == null)
            {
                return NotFound();
            }

            dv.IddoanPhi = idDoanPhi;
            dv.TenDoanPhi = tenDoanPhi;
            dv.NoiDung = noiDung;
            dv.SoTien = soTien;
            dv.NgayBatDau = ngayBatDau;
            dv.NgayKetThuc = ngayKetThuc;
            dv.IdhocKy = idHocKy;

            dbc.SaveChanges();
            return Ok(new
            {
                message = "Sửa đoàn phí thành công",
                id = dv.IddoanPhi
            });
        }

        //lấy đoàn phí theo mã đoàn phí
        [HttpGet]
        [Route("/DoanPhi/IdDoanPhi/{id}")]
        public async Task<IActionResult> GetDoanPhiByIdDoanPhi(int id)
        {
            var result = await (from p in dbc.DoanPhis
                                join d in dbc.HocKies on p.IdhocKy equals d.IdhocKy
                                where p.IddoanPhi == id
                                select new
                                {
                                    p.IddoanPhi,
                                    p.TenDoanPhi,
                                    p.NoiDung,
                                    p.SoTien,
                                    p.NgayBatDau,
                                    p.NgayKetThuc,
                                    p.IdhocKy,
                                    d.KyHoc,
                                    d.NamHoc
                                }).FirstOrDefaultAsync();
            return Ok(result);
        }

        //xóa đoàn phí
        [HttpDelete]
        [Route("/DoanPhi/Delete")]
        public IActionResult DeleteDoanPhi(int idDoanPhi)
        {
            var dv = dbc.DoanPhis.FirstOrDefault(d => d.IddoanPhi == idDoanPhi);
            if (dv != null)
            {
                dbc.DoanPhis.Remove(dv);
            }
            dbc.SaveChanges();
            return Ok(new { message = "Đã xóa đoàn phí thành công" });
        }

        [HttpGet]
        [Route("/DoanPhi/XuatDaNop/{id}")]
        public async Task<IActionResult> XuatDanhSachDaNop(int id)
        {
            var data = await (from n in dbc.DoanVienNopDoanPhis
                              join d in dbc.DoanViens on n.IddoanVien equals d.IddoanVien
                              join cd in dbc.ChiDoans on d.IdchiDoan equals cd.IdchiDoan
                              where n.IddoanPhi == id
                              select new
                              {
                                  d.TenDoanVien,
                                  d.IddoanVien,
                                  TenChiDoan = cd.TenChiDoan,
                                  TrangThai = "Hoàn thành"
                              }).ToListAsync();

            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("DaNop");
                worksheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"DoanVien_DaNop_{id}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpGet]
        [Route("/DoanPhi/XuatChuaNop/{id}")]
        public async Task<IActionResult> XuatDanhSachChuaNop(int id)
        {
            var daNopIds = await dbc.DoanVienNopDoanPhis
                                    .Where(x => x.IddoanPhi == id)
                                    .Select(x => x.IddoanVien)
                                    .ToListAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var data = await (from d in dbc.DoanViens
                              join cd in dbc.ChiDoans on d.IdchiDoan equals cd.IdchiDoan
                              where !daNopIds.Contains(d.IddoanVien)
                              select new
                              {
                                  d.TenDoanVien,
                                  d.IddoanVien,
                                  TenChiDoan = cd.TenChiDoan,
                                  TrangThai = "Chưa hoàn thành"
                              }).ToListAsync();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("ChuaNop");
                worksheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"DoanVien_ChuaNop_{id}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
