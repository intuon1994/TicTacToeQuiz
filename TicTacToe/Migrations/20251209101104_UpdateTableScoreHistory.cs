using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableScoreHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "player",
                table: "ScoreHistories");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "ScoreHistories",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ScoreHistories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "userlogin",
                table: "ScoreHistories",
                newName: "Result");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "ScoreHistories",
                newName: "PlayerName");

            migrationBuilder.RenameColumn(
                name: "score",
                table: "ScoreHistories",
                newName: "WinCount");

            migrationBuilder.RenameColumn(
                name: "log",
                table: "ScoreHistories",
                newName: "TotalScore");

            migrationBuilder.AddColumn<int>(
                name: "ScoreChange",
                table: "ScoreHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreChange",
                table: "ScoreHistories");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ScoreHistories",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ScoreHistories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WinCount",
                table: "ScoreHistories",
                newName: "score");

            migrationBuilder.RenameColumn(
                name: "TotalScore",
                table: "ScoreHistories",
                newName: "log");

            migrationBuilder.RenameColumn(
                name: "Result",
                table: "ScoreHistories",
                newName: "userlogin");

            migrationBuilder.RenameColumn(
                name: "PlayerName",
                table: "ScoreHistories",
                newName: "status");

            migrationBuilder.AddColumn<string>(
                name: "player",
                table: "ScoreHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
