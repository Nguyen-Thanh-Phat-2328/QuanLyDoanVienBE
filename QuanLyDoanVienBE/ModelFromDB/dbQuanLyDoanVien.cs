using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoanVienBE.ModelFromDB;

public partial class dbQuanLyDoanVien : DbContext
{
    public dbQuanLyDoanVien()
    {
    }

    public dbQuanLyDoanVien(DbContextOptions<dbQuanLyDoanVien> options)
        : base(options)
    {
    }

    public virtual DbSet<BanChapHanh> BanChapHanhs { get; set; }

    public virtual DbSet<ChiDoan> ChiDoans { get; set; }

    public virtual DbSet<ChucVu> ChucVus { get; set; }

    public virtual DbSet<DoanPhi> DoanPhis { get; set; }

    public virtual DbSet<DoanVien> DoanViens { get; set; }

    public virtual DbSet<DoanVienNopDoanPhi> DoanVienNopDoanPhis { get; set; }

    public virtual DbSet<HoatDong> HoatDongs { get; set; }

    public virtual DbSet<HocKy> HocKies { get; set; }

    public virtual DbSet<LoaiThongBao> LoaiThongBaos { get; set; }

    public virtual DbSet<PhieuChuyenSinhHoatDoan> PhieuChuyenSinhHoatDoans { get; set; }

    public virtual DbSet<PhuongXa> PhuongXas { get; set; }

    public virtual DbSet<QuanHuyen> QuanHuyens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThamGiaHoatDong> ThamGiaHoatDongs { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<TinhThanh> TinhThanhs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=PHAT;Initial Catalog=dbQuanLyDoanVien;Persist Security Info=True;User ID=sa;Password=123;Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanChapHanh>(entity =>
        {
            entity.HasKey(e => e.IdbanChapHanh).HasName("PK__BanChapH__C16255BD54233A3E");

            entity.Property(e => e.Sdt).IsFixedLength();

            entity.HasOne(d => d.IdtaiKhoanNavigation).WithMany(p => p.BanChapHanhs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BanChapHa__IDTai__5CD6CB2B");
        });

        modelBuilder.Entity<ChiDoan>(entity =>
        {
            entity.HasKey(e => e.IdchiDoan).HasName("PK__ChiDoan__5A8D6537A5688B46");
        });

        modelBuilder.Entity<ChucVu>(entity =>
        {
            entity.HasKey(e => e.IdchucVu).HasName("PK__ChucVu__70FCCF65E38EDACA");
        });

        modelBuilder.Entity<DoanPhi>(entity =>
        {
            entity.HasKey(e => e.IddoanPhi).HasName("PK__DoanPhi__0588F7D27A8D6AC5");

            entity.HasOne(d => d.IdhocKyNavigation).WithMany(p => p.DoanPhis)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__DoanPhi__IDHocKy__440B1D61");
        });

        modelBuilder.Entity<DoanVien>(entity =>
        {
            entity.HasKey(e => e.IddoanVien).HasName("PK__DoanVien__6DA1C45B4AE2C8CC");

            entity.Property(e => e.GioiTinh).IsFixedLength();
            entity.Property(e => e.Sdt).IsFixedLength();

            entity.HasOne(d => d.IdchiDoanNavigation).WithMany(p => p.DoanViens)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__DoanVien__IDChiD__4D94879B");

            entity.HasOne(d => d.IdchucVuNavigation).WithMany(p => p.DoanViens)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__DoanVien__IDChuc__4AB81AF0");

            entity.HasOne(d => d.IdphuongXaNavigation).WithMany(p => p.DoanViens)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__DoanVien__IDPhuo__4BAC3F29");

            entity.HasOne(d => d.IdtaiKhoanNavigation).WithMany(p => p.DoanViens)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__DoanVien__IDTaiK__4CA06362");
        });

        modelBuilder.Entity<DoanVienNopDoanPhi>(entity =>
        {
            entity.HasKey(e => new { e.IddoanPhi, e.IddoanVien }).HasName("PK__DoanVien__A352EB97D8323321");

            entity.HasOne(d => d.IddoanPhiNavigation).WithMany(p => p.DoanVienNopDoanPhis).HasConstraintName("FK__DoanVienN__IDDoa__534D60F1");

            entity.HasOne(d => d.IddoanVienNavigation).WithMany(p => p.DoanVienNopDoanPhis).HasConstraintName("FK__DoanVienN__IDDoa__5441852A");
        });

        modelBuilder.Entity<HoatDong>(entity =>
        {
            entity.HasKey(e => e.IdhoatDong).HasName("PK__HoatDong__F5465BC4A9FCA0B3");
        });

        modelBuilder.Entity<HocKy>(entity =>
        {
            entity.HasKey(e => e.IdhocKy).HasName("PK__HocKy__90FBC84B120CE83B");
        });

        modelBuilder.Entity<LoaiThongBao>(entity =>
        {
            entity.HasKey(e => e.IdloaiThongBao).HasName("PK__LoaiThon__7BF48C3729664064");
        });

        modelBuilder.Entity<PhieuChuyenSinhHoatDoan>(entity =>
        {
            entity.HasKey(e => e.Idphieu).HasName("PK__PhieuChu__8C53393B279BB3F6");

            entity.Property(e => e.TrangThai).HasDefaultValue(false);

            entity.HasOne(d => d.IddoanVienNavigation).WithMany(p => p.PhieuChuyenSinhHoatDoans)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PhieuChuy__IDDoa__5070F446");
        });

        modelBuilder.Entity<PhuongXa>(entity =>
        {
            entity.HasKey(e => e.IdphuongXa).HasName("PK__PhuongXa__6D002324E3CF7E54");

            entity.Property(e => e.IdphuongXa).ValueGeneratedNever();

            entity.HasOne(d => d.IdquanHuyenNavigation).WithMany(p => p.PhuongXas)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PhuongXa__IDQuan__3C69FB99");
        });

        modelBuilder.Entity<QuanHuyen>(entity =>
        {
            entity.HasKey(e => e.IdquanHuyen).HasName("PK__QuanHuye__29AC36EE1661514B");

            entity.HasOne(d => d.IdtinhThanhNavigation).WithMany(p => p.QuanHuyens)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__QuanHuyen__IDTin__398D8EEE");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.IdtaiKhoan).HasName("PK__TaiKhoan__BC5F907C77B58E14");
        });

        modelBuilder.Entity<ThamGiaHoatDong>(entity =>
        {
            entity.HasKey(e => new { e.IdhoatDong, e.IddoanVien }).HasName("PK__ThamGiaH__539C4781154E72EF");

            entity.Property(e => e.TrangThai).HasDefaultValue(false);

            entity.HasOne(d => d.IddoanVienNavigation).WithMany(p => p.ThamGiaHoatDongs).HasConstraintName("FK__ThamGiaHo__IDDoa__59FA5E80");

            entity.HasOne(d => d.IdhoatDongNavigation).WithMany(p => p.ThamGiaHoatDongs).HasConstraintName("FK__ThamGiaHo__IDHoa__59063A47");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.IdthongBao).HasName("PK__ThongBao__3EBBFAAEF6C192CF");

            entity.HasOne(d => d.IdbanChapHanhNavigation).WithMany(p => p.ThongBaos)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ThongBao__IDBanC__628FA481");

            entity.HasOne(d => d.IdloaiThongBaoNavigation).WithMany(p => p.ThongBaos)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ThongBao__IDLoai__619B8048");
        });

        modelBuilder.Entity<TinhThanh>(entity =>
        {
            entity.HasKey(e => e.IdtinhThanh).HasName("PK__TinhThan__3EF4861187A96E55");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
