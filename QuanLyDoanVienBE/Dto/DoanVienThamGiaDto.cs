namespace QuanLyDoanVienBE.Dto
{
    public class DoanVienThamGiaDto
    {
        public string IDDoanVien { get; set; }
        public string TenDoanVien { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public DateOnly? NgayDangKy { get; set; }
        public string TenChiDoan { get; set; }
        public bool? TrangThai { get; set; }
        public string IdhoatDong { get; set; }
    }
}
