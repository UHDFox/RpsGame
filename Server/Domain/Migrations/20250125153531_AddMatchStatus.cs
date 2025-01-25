using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "MatchHistories");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:match_status", "finished,postponed,pending");

            migrationBuilder.AddColumn<string[]>(
                name: "PlayerMoves",
                table: "MatchHistories",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MatchHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Winner",
                table: "MatchHistories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerMoves",
                table: "MatchHistories");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MatchHistories");

            migrationBuilder.DropColumn(
                name: "Winner",
                table: "MatchHistories");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:match_status", "finished,postponed,pending");

            migrationBuilder.AddColumn<Guid>(
                name: "WinnerId",
                table: "MatchHistories",
                type: "uuid",
                nullable: true);
        }
    }
}
