using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("DoanPhi")]
public partial class DoanPhi
{
    [Key]
    [Column("IDDoanPhi")]
    public int IddoanPhi { get; set; }

    [StringLength(255)]
    public string? TenDoanPhi { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? SoTien { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    [Column("IDHocKy")]
    public int? IdhocKy { get; set; }

    [InverseProperty("IddoanPhiNavigation")]
    public virtual ICollection<DoanVienNopDoanPhi> DoanVienNopDoanPhis { get; set; } = new List<DoanVienNopDoanPhi>();

    [ForeignKey("IdhocKy")]
    [InverseProperty("DoanPhis")]
    public virtual HocKy? IdhocKyNavigation { get; set; }
}
