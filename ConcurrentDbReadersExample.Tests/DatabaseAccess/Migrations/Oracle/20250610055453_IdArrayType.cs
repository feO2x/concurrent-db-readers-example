using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess.Migrations.Oracle;

/// <inheritdoc />
public partial class IdArrayType : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE OR REPLACE TYPE id_array_type AS TABLE OF RAW(16)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TYPE id_array_type");
    }
}
