using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceManagement.Dal.Migrations
{
    /// <inheritdoc />
    public partial class MigrateToLocalAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_keycloak_user_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "keycloak_user_id",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("UPDATE users SET username = split_part(email, '@', 1) || '_' || substring(user_id::text, 1, 8) WHERE username IS NULL OR username = '';");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_username",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password",
                table: "users");

            migrationBuilder.DropColumn(
                name: "username",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "keycloak_user_id",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("UPDATE users SET keycloak_user_id = user_id::text WHERE keycloak_user_id IS NULL OR keycloak_user_id = '';");

            migrationBuilder.AlterColumn<string>(
                name: "keycloak_user_id",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_keycloak_user_id",
                table: "users",
                column: "keycloak_user_id",
                unique: true);
        }
    }
}
