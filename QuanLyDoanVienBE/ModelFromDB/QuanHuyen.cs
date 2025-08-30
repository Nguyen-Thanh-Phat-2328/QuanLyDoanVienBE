using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("QuanHuyen")]
public partial class QuanHuyen
{
    [Key]
    [Column("IDQuanHuyen")]
    public int IdquanHuyen { get; set; }

    [StringLength(50)]
    public string? TenQuanHuyen { get; set; }

    [Column("IDTinhThanh")]
    public int? IdtinhThanh { get; set; }

    [ForeignKey("IdtinhThanh")]
    [InverseProperty("QuanHuyens")]
    public virtual TinhThanh? IdtinhThanhNavigation { get; set; }

    [InverseProperty("IdquanHuyenNavigation")]
    public virtual ICollection<PhuongXa> PhuongXas { get; set; } = new List<PhuongXa>();
}
