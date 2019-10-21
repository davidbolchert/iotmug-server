using Microsoft.EntityFrameworkCore.Migrations;

namespace IoTMug.Data.Migrations
{
    public partial class rename_json_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultTwinData",
                table: "DeviceTypes");

            migrationBuilder.DropColumn(
                name: "TwinData",
                table: "Devices");

            migrationBuilder.AddColumn<string>(
                name: "DefaultTwin",
                table: "DeviceTypes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Twin",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultTwin",
                table: "DeviceTypes");

            migrationBuilder.DropColumn(
                name: "Twin",
                table: "Devices");

            migrationBuilder.AddColumn<string>(
                name: "DefaultTwinData",
                table: "DeviceTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TwinData",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
