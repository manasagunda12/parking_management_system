using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingManagementModel.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberOfFloor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfFloors",
                table: "ParkingLots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfFloors",
                table: "ParkingLots");
        }
    }
}
