using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("DoanVien")]
[Index("Email", Name = "uq_Email", IsUnique = true)]
public partial class DoanVien
{
    [Key]
    [Column("IDDoanVien")]
    [StringLength(20)]
    [Unicode(false)]
    public string IddoanVien { get; set; } = null!;

    [StringLength(255)]
    public string? TenDoanVien { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [Column("IDPhuongXa")]
    public int? IdphuongXa { get; set; }

    [Column("SDT")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Sdt { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string? GioiTinh { get; set; }

    public DateOnly? NgayVaoDoan { get; set; }

    public bool? TrangThaiSoDoan { get; set; }

    public DateOnly? NgayNopSoDoan { get; set; }

    [Column("IDTaiKhoan")]
    [StringLength(20)]
    [Unicode(false)]
    public string? IdtaiKhoan { get; set; }

    [Column("IDChiDoan")]
    [StringLength(20)]
    [Unicode(false)]
    public string? IdchiDoan { get; set; }

    [Column("IDChucVu")]
    [StringLength(20)]
    [Unicode(false)]
    public string? IdchucVu { get; set; }

    [Unicode(false)]
    public string? HinhAnh { get; set; }

    [InverseProperty("IddoanVienNavigation")]
    public virtual ICollection<DoanVienNopDoanPhi> DoanVienNopDoanPhis { get; set; } = new List<DoanVienNopDoanPhi>();

    [ForeignKey("IdchiDoan")]
    [InverseProperty("DoanViens")]
    public virtual ChiDoan? IdchiDoanNavigation { get; set; }

    [ForeignKey("IdchucVu")]
    [InverseProperty("DoanViens")]
    public virtual ChucVu? IdchucVuNavigation { get; set; }

    [ForeignKey("IdphuongXa")]
    [InverseProperty("DoanViens")]
    public virtual PhuongXa? IdphuongXaNavigation { get; set; }

    [ForeignKey("IdtaiKhoan")]
    [InverseProperty("DoanViens")]
    public virtual TaiKhoan? IdtaiKhoanNavigation { get; set; }

    [InverseProperty("IddoanVienNavigation")]
    public virtual ICollection<PhieuChuyenSinhHoatDoan> PhieuChuyenSinhHoatDoans { get; set; } = new List<PhieuChuyenSinhHoatDoan>();

    [InverseProperty("IddoanVienNavigation")]
    public virtual ICollection<ThamGiaHoatDong> ThamGiaHoatDongs { get; set; } = new List<ThamGiaHoatDong>();
}
