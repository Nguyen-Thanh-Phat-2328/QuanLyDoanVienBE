using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("ThongBao")]
public partial class ThongBao
{
    [Key]
    [Column("IDThongBao")]
    [StringLength(20)]
    [Unicode(false)]
    public string IdthongBao { get; set; } = null!;

    [StringLength(255)]
    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    public DateOnly? NgayBanHanh { get; set; }

    [Unicode(false)]
    public string? FileDinhKem { get; set; }

    [Column("IDLoaiThongBao")]
    public int? IdloaiThongBao { get; set; }

    [Column("IDBanChapHanh")]
    public int? IdbanChapHanh { get; set; }

    [ForeignKey("IdbanChapHanh")]
    [InverseProperty("ThongBaos")]
    public virtual BanChapHanh? IdbanChapHanhNavigation { get; set; }

    [ForeignKey("IdloaiThongBao")]
    [InverseProperty("ThongBaos")]
    public virtual LoaiThongBao? IdloaiThongBaoNavigation { get; set; }
}
