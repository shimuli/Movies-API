using Microsoft.EntityFrameworkCore.Migrations;

namespace moviesApi.Migrations
{
    public partial class AddWachedToReserve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Watched",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Watched",
                table: "Reservations");
        }
    }
}
