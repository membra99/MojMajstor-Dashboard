using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations.MojMajstor
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
            name: "RegistrationDate",
            table: "Users",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
