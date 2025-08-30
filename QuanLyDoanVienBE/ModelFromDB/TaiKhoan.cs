using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("TaiKhoan")]
public partial class TaiKhoan
{
    [Key]
    [Column("IDTaiKhoan")]
    [StringLength(20)]
    [Unicode(false)]
    public string IdtaiKhoan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MatKhau { get; set; }

    [InverseProperty("IdtaiKhoanNavigation")]
    public virtual ICollection<BanChapHanh> BanChapHanhs { get; set; } = new List<BanChapHanh>();

    [InverseProperty("IdtaiKhoanNavigation")]
    public virtual ICollection<DoanVien> DoanViens { get; set; } = new List<DoanVien>();
}
