namespace QuanLyDoanVienBE.Dto
{
    public class HoatDongDto
    {
        public string TenHoatDong { get; set; }
        public string NoiDung { get; set; }
        public int DiemHoatDong { get; set; }
        public int TongSoDoanVien { get; set; }
        public string DiaDiem { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public IFormFile? HinhAnh { get; set; }
    }

}
