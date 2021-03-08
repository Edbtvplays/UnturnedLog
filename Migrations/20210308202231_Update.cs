using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Edbtvplays.UnturnedLog.Unturned.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT UNSIGNED")
                .OldAnnotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Edbtvplays_UnturnedLog_Players",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT UNSIGNED")
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);
        }
    }
}
