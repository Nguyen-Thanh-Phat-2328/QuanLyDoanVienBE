namespace QuanLyDoanVienBE.Dto
{
    public class ThongBaoPostDto
    {
        public string? TieuDe { get; set; }
        public string? NoiDung { get; set; }
        public DateOnly? NgayBanHanh { get; set; }
        public IFormFile? FileDinhKem { get; set; }

        public int? IdloaiThongBao { get; set; }

        public int? IdbanChapHanh { get; set; }
    }
}
