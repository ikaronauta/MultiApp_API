using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiApp_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_Users_DocumentType", "[DocumentType] IN ('CC', 'NIT', 'Passport')");
                });

            // Insertar datos iniciales
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "DocumentType", "DocumentNumber", "Email", "FirstName", "LastName", "BirthDate", "PasswordHash" },
                values: new object[] { "CC", "16076940", "julian.ortiz@example.com", "Julian", "Ortiz", new DateOnly(1983, 9, 20), "AQAAAAIAAYagAAAAEC5viSCTMhspWjqVoEAKKnYSWF2+nZCrhF1t+7yUc6HK+H5q/yenzrmG0Qtyq0pfgA==" }
            );

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "DocumentType", "DocumentNumber", "Email", "FirstName", "LastName", "BirthDate", "PasswordHash" },
                values: new object[] { "CC", "30287241", "maria.perez@example.com", "Maria", "Perez", new DateOnly(1992, 8, 15), "AQAAAAIAAYagAAAAEC5viSCTMhspWjqVoEAKKnYSWF2+nZCrhF1t+7yUc6HK+H5q/yenzrmG0Qtyq0pfgA==" }
            );

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "DocumentType", "DocumentNumber", "Email", "FirstName", "LastName", "BirthDate", "PasswordHash" },
                values: new object[] { "CC", "32156505", "luis.gomez@example.com", "Luis", "Gomez", new DateOnly(1985, 12, 1), "AQAAAAIAAYagAAAAEC5viSCTMhspWjqVoEAKKnYSWF2+nZCrhF1t+7yUc6HK+H5q/yenzrmG0Qtyq0pfgA==" }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
