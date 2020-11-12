﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Edbtvplays.UnturnedLog.Migrations
{
    public partial class inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Edbtvplays_UnturnedLog_Servers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Instance = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edbtvplays_UnturnedLog_Servers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Edbtvplays_UnturnedLog_TPS",
                columns: table => new
                {
                    Timestamp = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<int>(nullable: false),
                    ServerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edbtvplays_UnturnedLog_TPS", x => x.Timestamp);
                });

            migrationBuilder.CreateTable(
                name: "Edbtvplays_UnturnedLog_Players",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "BIGINT UNSIGNED", nullable: false),
                    SteamName = table.Column<string>(maxLength: 64, nullable: false),
                    CharacterName = table.Column<string>(maxLength: 64, nullable: false),
                    ProfilePictureHash = table.Column<string>(maxLength: 64, nullable: false),
                    LastQuestGroupId = table.Column<decimal>(type: "BIGINT UNSIGNED", nullable: false),
                    SteamGroup = table.Column<decimal>(type: "BIGINT UNSIGNED", nullable: false),
                    SteamGroupName = table.Column<string>(maxLength: 64, nullable: false),
                    Hwid = table.Column<string>(nullable: false),
                    Ip = table.Column<long>(nullable: false),
                    TotalPlaytime = table.Column<double>(nullable: false),
                    LastLoginGlobal = table.Column<DateTime>(nullable: false),
                    ServerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edbtvplays_UnturnedLog_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Edbtvplays_UnturnedLog_Players_Edbtvplays_UnturnedLog_Serve~",
                        column: x => x.ServerId,
                        principalTable: "Edbtvplays_UnturnedLog_Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Edbtvplays_UnturnedLog_Players_ServerId",
                table: "Edbtvplays_UnturnedLog_Players",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropTable(
                name: "Edbtvplays_UnturnedLog_TPS");

            migrationBuilder.DropTable(
                name: "Edbtvplays_UnturnedLog_Servers");
        }
    }
}
