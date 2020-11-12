﻿// <auto-generated />
using System;
using Edbtvplays.UnturnedLog.Unturned.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Edbtvplays.UnturnedLog.Migrations
{
    [DbContext(typeof(UnturnedLogStaticDbContext))]
    [Migration("20201111210122_inital")]
    partial class inital
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.PlayerData", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("BIGINT UNSIGNED");

                    b.Property<string>("CharacterName")
                        .IsRequired()
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Hwid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Ip")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("LastLoginGlobal")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("LastQuestGroupId")
                        .HasColumnName("LastQuestGroupId")
                        .HasColumnType("BIGINT UNSIGNED");

                    b.Property<string>("ProfilePictureHash")
                        .IsRequired()
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<decimal>("SteamGroup")
                        .HasColumnName("SteamGroup")
                        .HasColumnType("BIGINT UNSIGNED");

                    b.Property<string>("SteamGroupName")
                        .IsRequired()
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<string>("SteamName")
                        .IsRequired()
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<double>("TotalPlaytime")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Edbtvplays_UnturnedLog_Players");
                });

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Instance")
                        .IsRequired()
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Edbtvplays_UnturnedLog_Servers");
                });

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.TPS", b =>
                {
                    b.Property<long>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Timestamp");

                    b.ToTable("Edbtvplays_UnturnedLog_TPS");
                });

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.PlayerData", b =>
                {
                    b.HasOne("Edbtvplays.UnturnedLog.Unturned.API.Classes.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}