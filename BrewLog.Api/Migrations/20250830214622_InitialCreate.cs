using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrewLog.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrewingEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Vendor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Specifications = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewingEquipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeBeans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RoastLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    Origin = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeBeans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrindSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GrindSize = table.Column<int>(type: "INTEGER", nullable: false),
                    GrindTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    GrindWeight = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    GrinderType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrindSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrewSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Method = table.Column<int>(type: "INTEGER", nullable: false),
                    WaterTemperature = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    BrewTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TastingNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: true),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CoffeeBeanId = table.Column<int>(type: "INTEGER", nullable: false),
                    GrindSettingId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrewingEquipmentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrewSessions_BrewingEquipment_BrewingEquipmentId",
                        column: x => x.BrewingEquipmentId,
                        principalTable: "BrewingEquipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BrewSessions_CoffeeBeans_CoffeeBeanId",
                        column: x => x.CoffeeBeanId,
                        principalTable: "CoffeeBeans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrewSessions_GrindSettings_GrindSettingId",
                        column: x => x.GrindSettingId,
                        principalTable: "GrindSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrewSessions_BrewingEquipmentId",
                table: "BrewSessions",
                column: "BrewingEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSessions_CoffeeBeanId",
                table: "BrewSessions",
                column: "CoffeeBeanId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSessions_GrindSettingId",
                table: "BrewSessions",
                column: "GrindSettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrewSessions");

            migrationBuilder.DropTable(
                name: "BrewingEquipment");

            migrationBuilder.DropTable(
                name: "CoffeeBeans");

            migrationBuilder.DropTable(
                name: "GrindSettings");
        }
    }
}
