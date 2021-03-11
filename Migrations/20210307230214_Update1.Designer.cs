﻿// <auto-generated />
using System;
using Edbtvplays.UnturnedLog.Unturned.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Edbtvplays.UnturnedLog.Unturned.Migrations
{
    [DbContext(typeof(UnturnedLogDbContext))]
    [Migration("20210307230214_Update1")]
    partial class Update1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.PlayerData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("BIGINT UNSIGNED");

                    b.Property<string>("CharacterName")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Hwid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Ip")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("LastLoginGlobal")
                        .HasColumnType("datetime");

                    b.Property<long>("LastQuestGroupId")
                        .HasColumnName("LastQuestGroupId")
                        .HasColumnType("BIGINT UNSIGNED");

                    b.Property<string>("ProfilePictureHash")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<int>("ServerId")
                        .HasColumnType("int");

                    b.Property<long>("SteamGroup")
                        .HasColumnName("SteamGroup")
                        .HasColumnType("BIGINT UNSIGNED");

                    b.Property<string>("SteamGroupName")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("SteamName")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<double>("TotalPlaytime")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Edbtvplays_UnturnedLog_Players");
                });

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.PlayerEvents", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("EventData")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("EventTime")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("varbinary(4000)");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("PlayerId")
                        .HasColumnType("BIGINT UNSIGNED");

                    b.Property<int>("ServerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("ServerId");

                    b.ToTable("Edbtvplays_UnturnedLog_Events");
                });

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("IP")
                        .HasColumnType("bigint");

                    b.Property<string>("Instance")
                        .IsRequired()
                        .HasColumnType("varchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Edbtvplays_UnturnedLog_Servers");
                });

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.PlayerData", b =>
                {
                    b.HasOne("Edbtvplays.UnturnedLog.Unturned.API.Classes.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Edbtvplays.UnturnedLog.Unturned.API.Classes.PlayerEvents", b =>
                {
                    b.HasOne("Edbtvplays.UnturnedLog.Unturned.API.Classes.PlayerData", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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