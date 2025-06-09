using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess.Migrations.Oracle
{
    /// <inheritdoc />
    public partial class SetUpOracle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OUTBOX_ITEMS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TYPE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    JSON_PAYLOAD = table.Column<string>(type: "JSON", maxLength: 4000, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PROCESSED_AT_UTC = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PROCESSED_BY = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OUTBOX_ITEMS", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OUTBOX_ITEMS_UNPROCESSED_ITEMS",
                table: "OUTBOX_ITEMS",
                columns: new[] { "ID", "PROCESSED_AT_UTC" },
                filter: "PROCESSED_AT_UTC IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OUTBOX_ITEMS");
        }
    }
}
