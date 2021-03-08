using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Edbtvplays.UnturnedLog.Unturned.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deaths",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropColumn(
                name: "Headshots",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropColumn(
                name: "NodesMined",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropColumn(
                name: "PlayerKills",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropColumn(
                name: "Punishments",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropColumn(
                name: "TotalChatMessages",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropColumn(
                name: "TreesCutdown",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.DropColumn(
                name: "ZombieKills",
                table: "Edbtvplays_UnturnedLog_Players");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT UNSIGNED")
                .OldAnnotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "Edbtvplays_UnturnedLog_Events",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    EventType = table.Column<string>(nullable: false),
                    EventData = table.Column<string>(nullable: false),
                    ServerId = table.Column<int>(nullable: false),
                    EventTime = table.Column<byte[]>(rowVersion: true, nullable: true)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edbtvplays_UnturnedLog_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Edbtvplays_UnturnedLog_Events_Edbtvplays_UnturnedLog_Players~",
                        column: x => x.PlayerId,
                        principalTable: "Edbtvplays_UnturnedLog_Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Edbtvplays_UnturnedLog_Events_Edbtvplays_UnturnedLog_Servers~",
                        column: x => x.ServerId,
                        principalTable: "Edbtvplays_UnturnedLog_Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Edbtvplays_UnturnedLog_Events_PlayerId",
                table: "Edbtvplays_UnturnedLog_Events",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Edbtvplays_UnturnedLog_Events_ServerId",
                table: "Edbtvplays_UnturnedLog_Events",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Edbtvplays_UnturnedLog_Events");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT UNSIGNED")
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Deaths",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Headshots",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NodesMined",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerKills",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Punishments",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalChatMessages",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TreesCutdown",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ZombieKills",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
