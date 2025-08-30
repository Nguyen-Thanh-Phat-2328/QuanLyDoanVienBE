using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("HoatDong")]
public partial class HoatDong
{
    [Key]
    [Column("IDHoatDong")]
    [StringLength(20)]
    [Unicode(false)]
    public string IdhoatDong { get; set; } = null!;

    public string? TenHoatDong { get; set; }

    public string? NoiDung { get; set; }

    public int? DiemHoatDong { get; set; }

    public int? TongSoDoanVien { get; set; }

    [StringLength(255)]
    public string? DiaDiem { get; set; }

    public DateOnly? ThoiGianBatDau { get; set; }

    public DateOnly? ThoiGianKetThuc { get; set; }

    [Unicode(false)]
    public string? HinhAnh { get; set; }

    [InverseProperty("IdhoatDongNavigation")]
    public virtual ICollection<ThamGiaHoatDong> ThamGiaHoatDongs { get; set; } = new List<ThamGiaHoatDong>();
}
