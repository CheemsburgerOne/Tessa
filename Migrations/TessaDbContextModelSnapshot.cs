﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tessa.Persistance.PostgreSQL;

#nullable disable

namespace Tessa.Migrations
{
    [DbContext(typeof(TessaDbContext))]
    partial class TessaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tessa.Models.Filesystem.Base.Base", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<Guid?>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentId");

                    b.ToTable("Items");

                    b.HasDiscriminator<byte>("Type");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Tessa.Models.User.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Tessa.Models.Filesystem.Directory.Directory", b =>
                {
                    b.HasBaseType("Tessa.Models.Filesystem.Base.Base");

                    b.HasDiscriminator().HasValue((byte)0);
                });

            modelBuilder.Entity("Tessa.Models.Filesystem.File.File", b =>
                {
                    b.HasBaseType("Tessa.Models.Filesystem.Base.Base");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue((byte)1);
                });

            modelBuilder.Entity("Tessa.Models.Filesystem.Base.Base", b =>
                {
                    b.HasOne("Tessa.Models.User.User", "Owner")
                        .WithMany("Items")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Tessa.Models.Filesystem.Directory.Directory", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Owner");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Tessa.Models.User.User", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Tessa.Models.Filesystem.Directory.Directory", b =>
                {
                    b.Navigation("Children");
                });
#pragma warning restore 612, 618
        }
    }
}
