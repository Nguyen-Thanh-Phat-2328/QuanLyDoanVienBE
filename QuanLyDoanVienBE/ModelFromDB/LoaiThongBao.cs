using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("LoaiThongBao")]
public partial class LoaiThongBao
{
    [Key]
    [Column("IDLoaiThongBao")]
    public int IdloaiThongBao { get; set; }

    [StringLength(50)]
    public string? TenThongBao { get; set; }

    [InverseProperty("IdloaiThongBaoNavigation")]
    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
