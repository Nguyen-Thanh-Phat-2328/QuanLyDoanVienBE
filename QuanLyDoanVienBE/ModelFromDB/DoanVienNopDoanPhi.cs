using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

[PrimaryKey("IddoanPhi", "IddoanVien")]
[Table("DoanVienNopDoanPhi")]
public partial class DoanVienNopDoanPhi
{
    [Key]
    [Column("IDDoanPhi")]
    public int IddoanPhi { get; set; }

    [Key]
    [Column("IDDoanVien")]
    [StringLength(20)]
    [Unicode(false)]
    public string IddoanVien { get; set; } = null!;

    public DateOnly? NgayNop { get; set; }

    [ForeignKey("IddoanPhi")]
    [InverseProperty("DoanVienNopDoanPhis")]
    public virtual DoanPhi IddoanPhiNavigation { get; set; } = null!;

    [ForeignKey("IddoanVien")]
    [InverseProperty("DoanVienNopDoanPhis")]
    public virtual DoanVien IddoanVienNavigation { get; set; } = null!;
}
