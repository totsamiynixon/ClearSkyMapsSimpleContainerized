using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Infrastructure.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ApiKey = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    IsVisible = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticSensorReadings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CO2 = table.Column<float>(nullable: false),
                    LPG = table.Column<float>(nullable: false),
                    CO = table.Column<float>(nullable: false),
                    CH4 = table.Column<float>(nullable: false),
                    Dust = table.Column<float>(nullable: false),
                    Temp = table.Column<float>(nullable: false),
                    Hum = table.Column<float>(nullable: false),
                    Preassure = table.Column<float>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    StaticSensorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticSensorReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaticSensorReadings_Sensors_StaticSensorId",
                        column: x => x.StaticSensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaticSensorReadings_StaticSensorId",
                table: "StaticSensorReadings",
                column: "StaticSensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaticSensorReadings");

            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}
