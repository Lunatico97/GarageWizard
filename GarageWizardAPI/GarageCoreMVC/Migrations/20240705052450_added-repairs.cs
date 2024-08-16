using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageCoreMVC.Migrations
{
    /// <inheritdoc />
    /// 
    [ExcludeFromCodeCoverage]
    public partial class addedrepairs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Hours",
                table: "Jobs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Repairs",
                columns: table => new
                {
                    ID = table.Column<string>(type: "text", nullable: false),
                    TransactionStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleID = table.Column<string>(type: "text", nullable: true),
                    JobID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Repairs_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Repairs_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "RegID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_JobID",
                table: "Repairs",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_VehicleID",
                table: "Repairs",
                column: "VehicleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repairs");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "Jobs");
        }
    }
}
