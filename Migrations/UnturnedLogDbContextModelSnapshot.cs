﻿// <auto-generated />
using System;
using Edbtvplays.UnturnedLog.Unturned.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Edbtvplays.UnturnedLog.Unturned.Migrations
{
    [DbContext(typeof(UnturnedLogDbContext))]
    partial class UnturnedLogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<int>("Deaths")
                        .HasColumnType("int");

                    b.Property<int>("Headshots")
                        .HasColumnType("int");

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

                    b.Property<int>("NodesMined")
                        .HasColumnType("int");

                    b.Property<int>("PlayerKills")
                        .HasColumnType("int");

                    b.Property<string>("ProfilePictureHash")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<int>("Punishments")
                        .HasColumnType("int");

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

                    b.Property<int>("TotalChatMessages")
                        .HasColumnType("int");

                    b.Property<double>("TotalPlaytime")
                        .HasColumnType("double");

                    b.Property<int>("TreesCutdown")
                        .HasColumnType("int");

                    b.Property<int>("ZombieKills")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Edbtvplays_UnturnedLog_Players");
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
#pragma warning restore 612, 618
        }
    }
}