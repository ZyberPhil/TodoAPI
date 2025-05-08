using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace TodoAPI.Models;

public partial class TodoDbContext : DbContext
{
    public TodoDbContext()
    {
    }

    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appuser> Appusers { get; set; }

    public virtual DbSet<Item> Items { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseMySql("server=localhost;user=root;database=TodoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Appuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("appuser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Klasse).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasMany(d => d.Items).WithMany(p => p.AppUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "Appuseritem",
                    r => r.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("appuseritems_ibfk_2"),
                    l => l.HasOne<Appuser>().WithMany()
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("appuseritems_ibfk_1"),
                    j =>
                    {
                        j.HasKey("AppUserId", "ItemId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("appuseritems");
                        j.HasIndex(new[] { "ItemId" }, "ItemID");
                        j.IndexerProperty<int>("AppUserId")
                            .HasColumnType("int(11)")
                            .HasColumnName("AppUserID");
                        j.IndexerProperty<int>("ItemId")
                            .HasColumnType("int(11)")
                            .HasColumnName("ItemID");
                    });
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("item");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.Beschreibung).HasMaxLength(1000);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasColumnType("int(11)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
