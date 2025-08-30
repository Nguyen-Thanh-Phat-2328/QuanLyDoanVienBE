using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("PhuongXa")]
public partial class PhuongXa
{
    [Key]
    [Column("IDPhuongXa")]
    public int IdphuongXa { get; set; }

    [StringLength(50)]
    public string? TenPhuongXa { get; set; }

    [Column("IDQuanHuyen")]
    public int? IdquanHuyen { get; set; }

    [InverseProperty("IdphuongXaNavigation")]
    public virtual ICollection<DoanVien> DoanViens { get; set; } = new List<DoanVien>();

    [ForeignKey("IdquanHuyen")]
    [InverseProperty("PhuongXas")]
    public virtual QuanHuyen? IdquanHuyenNavigation { get; set; }
}
