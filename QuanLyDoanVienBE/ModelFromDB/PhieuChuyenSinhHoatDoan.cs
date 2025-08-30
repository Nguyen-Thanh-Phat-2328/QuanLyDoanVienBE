using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("PhieuChuyenSinhHoatDoan")]
public partial class PhieuChuyenSinhHoatDoan
{
    [Key]
    [Column("IDPhieu")]
    public int Idphieu { get; set; }

    [Column("IDDoanVien")]
    [StringLength(20)]
    [Unicode(false)]
    public string? IddoanVien { get; set; }

    public DateOnly? NgayLapPhieu { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? ChiDoanMoi { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? ChiDoanHienTai { get; set; }

    public string? LyDo { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [StringLength(255)]
    public string? NguoiDuyet { get; set; }

    [ForeignKey("IddoanVien")]
    [InverseProperty("PhieuChuyenSinhHoatDoans")]
    public virtual DoanVien? IddoanVienNavigation { get; set; }
}
