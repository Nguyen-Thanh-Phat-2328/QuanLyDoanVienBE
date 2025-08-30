using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[PrimaryKey("IdhoatDong", "IddoanVien")]
[Table("ThamGiaHoatDong")]
public partial class ThamGiaHoatDong
{
    [Key]
    [Column("IDHoatDong")]
    [StringLength(20)]
    [Unicode(false)]
    public string IdhoatDong { get; set; } = null!;

    [Key]
    [Column("IDDoanVien")]
    [StringLength(20)]
    [Unicode(false)]
    public string IddoanVien { get; set; } = null!;

    public DateOnly? NgayDangKy { get; set; }

    public bool? TrangThai { get; set; }

    [ForeignKey("IddoanVien")]
    [InverseProperty("ThamGiaHoatDongs")]
    public virtual DoanVien IddoanVienNavigation { get; set; } = null!;

    [ForeignKey("IdhoatDong")]
    [InverseProperty("ThamGiaHoatDongs")]
    public virtual HoatDong IdhoatDongNavigation { get; set; } = null!;
}
