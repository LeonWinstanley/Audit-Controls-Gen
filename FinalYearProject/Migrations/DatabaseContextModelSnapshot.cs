﻿// <auto-generated />
using System;
using FinalYearProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinalYearProject.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FinalYearProject.Models.Control", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ControlExpected")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ControlSummary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ControlTest")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Controls");
                });

            modelBuilder.Entity("FinalYearProject.Models.ControlEvaluationControls", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ControlEvaluationsId")
                        .HasColumnType("int");

                    b.Property<int?>("ControlId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ControlEvaluationsId");

                    b.HasIndex("ControlId");

                    b.ToTable("ControlEvaluationControls");
                });

            modelBuilder.Entity("FinalYearProject.Models.ControlEvaluations", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ControlEvaluations");
                });

            modelBuilder.Entity("FinalYearProject.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserRole")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FinalYearProject.Models.ControlEvaluationControls", b =>
                {
                    b.HasOne("FinalYearProject.Models.ControlEvaluations", null)
                        .WithMany("ControlsList")
                        .HasForeignKey("ControlEvaluationsId");

                    b.HasOne("FinalYearProject.Models.Control", "Control")
                        .WithMany()
                        .HasForeignKey("ControlId");

                    b.Navigation("Control");
                });

            modelBuilder.Entity("FinalYearProject.Models.ControlEvaluations", b =>
                {
                    b.HasOne("FinalYearProject.Models.User", null)
                        .WithMany("ControlEvaluations")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("FinalYearProject.Models.ControlEvaluations", b =>
                {
                    b.Navigation("ControlsList");
                });

            modelBuilder.Entity("FinalYearProject.Models.User", b =>
                {
                    b.Navigation("ControlEvaluations");
                });
#pragma warning restore 612, 618
        }
    }
}
