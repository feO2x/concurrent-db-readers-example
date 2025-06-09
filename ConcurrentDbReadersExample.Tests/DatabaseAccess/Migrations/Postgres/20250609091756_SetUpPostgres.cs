using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class SetUpPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    json_payload = table.Column<string>(type: "jsonb", maxLength: 4000, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    processed_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_items", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_items_unprocessed_items",
                table: "outbox_items",
                columns: new[] { "id", "processed_at_utc" },
                filter: "processed_at_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_items");
        }
    }
}
