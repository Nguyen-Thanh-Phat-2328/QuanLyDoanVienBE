using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("ChucVu")]
public partial class ChucVu
{
    [Key]
    [Column("IDChucVu")]
    [StringLength(20)]
    [Unicode(false)]
    public string IdchucVu { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? TenChucVu { get; set; }

    public string? MoTa { get; set; }

    [InverseProperty("IdchucVuNavigation")]
    public virtual ICollection<DoanVien> DoanViens { get; set; } = new List<DoanVien>();
}
