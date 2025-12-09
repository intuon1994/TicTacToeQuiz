using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableScoreSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "ScoreHistories");

            migrationBuilder.RenameColumn(
                name: "WinCount",
                table: "ScoreHistories",
                newName: "TotalScoreAfter");

            migrationBuilder.CreateTable(
                name: "ScoreSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerName = table.Column<string>(type: "TEXT", nullable: false),
                    TotalScore = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentWinStreak = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreSummaries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreSummaries");

            migrationBuilder.RenameColumn(
                name: "TotalScoreAfter",
                table: "ScoreHistories",
                newName: "WinCount");

            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                table: "ScoreHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
