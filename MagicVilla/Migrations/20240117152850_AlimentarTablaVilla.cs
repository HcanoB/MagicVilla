using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImageUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "detalle de la villa", new DateTime(2024, 1, 17, 10, 28, 50, 754, DateTimeKind.Local).AddTicks(7105), new DateTime(2024, 1, 17, 10, 28, 50, 754, DateTimeKind.Local).AddTicks(7081), "", 50, "Villa real", 5, 200.0 },
                    { 2, "", "detalle de la villa...", new DateTime(2024, 1, 17, 10, 28, 50, 754, DateTimeKind.Local).AddTicks(7112), new DateTime(2024, 1, 17, 10, 28, 50, 754, DateTimeKind.Local).AddTicks(7111), "", 40, "Premium vista a la piscina", 4, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
