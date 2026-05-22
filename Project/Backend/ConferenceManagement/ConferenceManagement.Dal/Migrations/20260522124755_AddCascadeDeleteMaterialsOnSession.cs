using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceManagement.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteMaterialsOnSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_materials_sessions_session_id",
                table: "materials");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "materials",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "material_type",
                table: "materials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "materials",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "fk_materials_sessions_session_id",
                table: "materials",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_materials_sessions_session_id",
                table: "materials");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "materials",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "material_type",
                table: "materials",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "materials",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddForeignKey(
                name: "fk_materials_sessions_session_id",
                table: "materials",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id");
        }
    }
}
