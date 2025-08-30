using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("BanChapHanh")]
public partial class BanChapHanh
{
    [Key]
    [Column("IDBanChapHanh")]
    public int IdbanChapHanh { get; set; }

    [Column("SDT")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Sdt { get; set; }

    [Column("TenBCH")]
    [StringLength(50)]
    [Unicode(false)]
    public string? TenBch { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("IDTaiKhoan")]
    [StringLength(20)]
    [Unicode(false)]
    public string? IdtaiKhoan { get; set; }

    [ForeignKey("IdtaiKhoan")]
    [InverseProperty("BanChapHanhs")]
    public virtual TaiKhoan? IdtaiKhoanNavigation { get; set; }

    [InverseProperty("IdbanChapHanhNavigation")]
    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
