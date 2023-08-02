using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace File_Box_App.Repository.Models;

public partial class FileBoxDbContext : DbContext
{
    public FileBoxDbContext()
    {
    }

    public FileBoxDbContext(DbContextOptions<FileBoxDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FileboxFile> FileboxFiles { get; set; }

    public virtual DbSet<FileboxFolder> FileboxFolders { get; set; }

    public virtual DbSet<FileboxUser> FileboxUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-CAUALDU;Initial Catalog=FileBoxDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileboxFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__filebox___07D884C68D1AD97E");

            entity.ToTable("filebox_file");

            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("file_path");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.FileType)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasDefaultValueSql("('N/A')")
                .HasColumnName("file_type");
            entity.Property(e => e.FolderId).HasColumnName("folder_id");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Folder).WithMany(p => p.FileboxFiles)
                .HasForeignKey(d => d.FolderId)
                .HasConstraintName("FK__filebox_f__folde__76EBA2E9");
        });

        modelBuilder.Entity<FileboxFolder>(entity =>
        {
            entity.HasKey(e => e.FolderId).HasName("PK__filebox___0045071B995ACE68");

            entity.ToTable("filebox_folder");

            entity.Property(e => e.FolderId).HasColumnName("folder_id");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creation_date");
            entity.Property(e => e.FolderName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("folder_name");
            entity.Property(e => e.FolderPath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("folder_path");
            entity.Property(e => e.ParentFolderId).HasColumnName("parent_folder_id");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ParentFolder).WithMany(p => p.InverseParentFolder)
                .HasForeignKey(d => d.ParentFolderId)
                .HasConstraintName("FK__filebox_f__paren__703EA55A");

            entity.HasOne(d => d.User).WithMany(p => p.FileboxFolders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__filebox_f__user___7132C993");
        });

        modelBuilder.Entity<FileboxUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__filebox___B9BE370F0CAE5127");

            entity.ToTable("filebox_user");

            entity.HasIndex(e => e.Email, "UQ__filebox___AB6E6164495A24D6").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__filebox___F3DBC572804F23B3").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
