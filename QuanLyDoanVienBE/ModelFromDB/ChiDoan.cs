using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("ChiDoan")]
public partial class ChiDoan
{
    [Key]
    [Column("IDChiDoan")]
    [StringLength(20)]
    [Unicode(false)]
    public string IdchiDoan { get; set; } = null!;

    [StringLength(255)]
    public string? TenChiDoan { get; set; }

    [InverseProperty("IdchiDoanNavigation")]
    public virtual ICollection<DoanVien> DoanViens { get; set; } = new List<DoanVien>();
}
