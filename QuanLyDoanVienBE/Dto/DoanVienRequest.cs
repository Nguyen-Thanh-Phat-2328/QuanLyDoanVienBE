namespace QuanLyDoanVienBE.Dto
{
    public class DoanVienRequest
    {
        public string IdDoanVien { get; set; } = null!;
        public string? TenDoanVien { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public int? IdPhuongXa { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? GioiTinh { get; set; }
        public DateOnly? NgayVaoDoan { get; set; }
        public bool TrangThaiSoDoan { get; set; }
        public string? IdChiDoan { get; set; }
        public string? IdChucVu { get; set; }
        public string? HinhAnh { get; set; }
    }
}
