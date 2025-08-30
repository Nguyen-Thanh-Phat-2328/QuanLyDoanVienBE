using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("TinhThanh")]
public partial class TinhThanh
{
    [Key]
    [Column("IDTinhThanh")]
    public int IdtinhThanh { get; set; }

    [StringLength(50)]
    public string? TenTinhThanh { get; set; }

    [InverseProperty("IdtinhThanhNavigation")]
    public virtual ICollection<QuanHuyen> QuanHuyens { get; set; } = new List<QuanHuyen>();
}
