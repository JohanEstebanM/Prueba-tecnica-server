using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Technicians_Workshops_WorkshopId",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_WorkshopId",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "WorkshopId",
                table: "Technicians");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkshopId",
                table: "Technicians",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_WorkshopId",
                table: "Technicians",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Technicians_Workshops_WorkshopId",
                table: "Technicians",
                column: "WorkshopId",
                principalTable: "Workshops",
                principalColumn: "Id");
        }
    }
}
