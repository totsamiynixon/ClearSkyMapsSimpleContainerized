using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class AddApiKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("IPAddress", "Sensors");
            migrationBuilder.AddColumn<string>(name: "ApiKey", table: "Sensors");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("ApiKey", "Sensors");
            migrationBuilder.AddColumn<string>(name: "IPAddress", table: "Sensors");
        }
    }
}
