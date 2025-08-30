namespace QuanLyDoanVienBE.Dto
{
    public class ThongBaoDto
    {
        public string IdthongBao { get; set; }
        public string? TieuDe { get; set; }
        public string? NoiDung { get; set; }
        public DateOnly? NgayBanHanh { get; set; }
        public string? FileDinhKem { get; set; }

        public int? IdloaiThongBao { get; set; }
        public string? TenLoaiThongBao { get; set; }

        public int? IdbanChapHanh { get; set; }
        public string? TenBanChapHanh { get; set; }
    }
}
