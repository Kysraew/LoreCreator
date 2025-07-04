﻿// <auto-generated />
using LoreCreatorBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoreCreatorBackend.Migrations
{
    [DbContext(typeof(LoreDbContext))]
    [Migration("20250701165116_DatabaseInitialization")]
    partial class DatabaseInitialization
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LoreCreatorBackend.Models.Entity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("EntityId")
                        .HasColumnType("integer");

                    b.Property<int>("EntityTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EntityId");

                    b.HasIndex("EntityTypeId");

                    b.ToTable("Entities");
                });

            modelBuilder.Entity("LoreCreatorBackend.Models.EntityType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EntityTypes");
                });

            modelBuilder.Entity("LoreCreatorBackend.Models.Entity", b =>
                {
                    b.HasOne("LoreCreatorBackend.Models.Entity", null)
                        .WithMany("RelatedEntities")
                        .HasForeignKey("EntityId");

                    b.HasOne("LoreCreatorBackend.Models.EntityType", "EntityType")
                        .WithMany()
                        .HasForeignKey("EntityTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntityType");
                });

            modelBuilder.Entity("LoreCreatorBackend.Models.Entity", b =>
                {
                    b.Navigation("RelatedEntities");
                });
#pragma warning restore 612, 618
        }
    }
}
