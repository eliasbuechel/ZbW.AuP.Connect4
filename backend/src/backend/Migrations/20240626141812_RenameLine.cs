using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbFieldDbGameResult_GameResults_LineId1",
                table: "DbFieldDbGameResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbFieldDbGameResult",
                table: "DbFieldDbGameResult");

            migrationBuilder.DropIndex(
                name: "IX_DbFieldDbGameResult_LineId1",
                table: "DbFieldDbGameResult");

            migrationBuilder.RenameColumn(
                name: "LineId1",
                table: "DbFieldDbGameResult",
                newName: "GameResultsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbFieldDbGameResult",
                table: "DbFieldDbGameResult",
                columns: new[] { "GameResultsId", "LineId" });

            migrationBuilder.CreateIndex(
                name: "IX_DbFieldDbGameResult_LineId",
                table: "DbFieldDbGameResult",
                column: "LineId");

            migrationBuilder.AddForeignKey(
                name: "FK_DbFieldDbGameResult_GameResults_GameResultsId",
                table: "DbFieldDbGameResult",
                column: "GameResultsId",
                principalTable: "GameResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbFieldDbGameResult_GameResults_GameResultsId",
                table: "DbFieldDbGameResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbFieldDbGameResult",
                table: "DbFieldDbGameResult");

            migrationBuilder.DropIndex(
                name: "IX_DbFieldDbGameResult_LineId",
                table: "DbFieldDbGameResult");

            migrationBuilder.RenameColumn(
                name: "GameResultsId",
                table: "DbFieldDbGameResult",
                newName: "LineId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbFieldDbGameResult",
                table: "DbFieldDbGameResult",
                columns: new[] { "LineId", "LineId1" });

            migrationBuilder.CreateIndex(
                name: "IX_DbFieldDbGameResult_LineId1",
                table: "DbFieldDbGameResult",
                column: "LineId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DbFieldDbGameResult_GameResults_LineId1",
                table: "DbFieldDbGameResult",
                column: "LineId1",
                principalTable: "GameResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
