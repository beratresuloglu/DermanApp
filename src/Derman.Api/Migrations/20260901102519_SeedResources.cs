using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Derman.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "Address", "Latitude", "Longitude", "Name", "Phone", "Type" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Trabzon Mah. Atatürk Cad. No:12, Kahramanmaraş", 37.5753m, 36.9228m, "Merkez Eczanesi", "0344 123 45 67", "Eczane" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "İsmet Paşa Mah. Hastane Cad. No:1, Kahramanmaraş", 37.5822m, 36.9337m, "Kahramanmaraş Devlet Hastanesi", "0344 221 20 00", "Hastane" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Yörükselim Mah. Cumhuriyet Meydanı, Kahramanmaraş", 37.5700m, 36.9200m, "AFAD Koordinasyon Merkezi", "0344 225 10 10", "STK" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Yeşilyurt Mah. No:45, Kahramanmaraş", 37.5900m, 36.9400m, "Yeşilyurt Eczanesi", "0344 234 56 78", "Eczane" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Fevzi Çakmak Mah. No:8, Kahramanmaraş", 37.5650m, 36.9150m, "Kızılay Şube", "0344 245 67 89", "STK" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));
        }
    }
}
