using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[Table("HocKy")]
public partial class HocKy
{
    [Key]
    [Column("IDHocKy")]
    public int IdhocKy { get; set; }

    public byte? KyHoc { get; set; }

    public int? NamHoc { get; set; }

    [InverseProperty("IdhocKyNavigation")]
    public virtual ICollection<DoanPhi> DoanPhis { get; set; } = new List<DoanPhi>();
}
