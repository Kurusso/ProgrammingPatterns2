﻿// <auto-generated />
using System;
using CreditApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CreditApplication.Migrations
{
    [DbContext(typeof(CreditDbContext))]
    partial class CreditDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Common.Models.BlockedUser", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("UserId");

                    b.ToTable("BlockedUsers");
                });

            modelBuilder.Entity("CreditApplication.Models.Credit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreditRateId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FullMoneyAmount")
                        .IsRequired()
                        .HasColumnType("varchar");

                    b.Property<DateTime>("ModifyDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MonthPayAmount")
                        .IsRequired()
                        .HasColumnType("varchar");

                    b.Property<Guid>("PayingAccountId")
                        .HasColumnType("uuid");

                    b.Property<string>("RemainingDebt")
                        .IsRequired()
                        .HasColumnType("varchar");

                    b.Property<string>("UnpaidDebt")
                        .IsRequired()
                        .HasColumnType("varchar");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreditRateId");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("CreditApplication.Models.CreditRate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifyDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("MonthPercent")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CreditRates");
                });

            modelBuilder.Entity("CreditApplication.Models.Credit", b =>
                {
                    b.HasOne("CreditApplication.Models.CreditRate", "CreditRate")
                        .WithMany()
                        .HasForeignKey("CreditRateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreditRate");
                });
#pragma warning restore 612, 618
        }
    }
}
