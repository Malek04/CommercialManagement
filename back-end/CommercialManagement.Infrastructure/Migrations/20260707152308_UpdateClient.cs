using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommercialManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdresseId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Adresse_CodePostal",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Pays",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Rue",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Ville",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdresseId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Adresse_CodePostal",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Adresse_Pays",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Adresse_Rue",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Adresse_Ville",
                table: "Clients");
        }
    }
}
