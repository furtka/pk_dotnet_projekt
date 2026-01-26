using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Reservations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Reservations",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
