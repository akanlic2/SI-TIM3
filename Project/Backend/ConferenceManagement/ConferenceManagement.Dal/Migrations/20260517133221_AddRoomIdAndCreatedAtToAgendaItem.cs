using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceManagement.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomIdAndCreatedAtToAgendaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_agenda_items_sessions_session_id",
                table: "agenda_items");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "agenda_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "agenda_items",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "agenda_items",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "agenda_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<Guid>(
                name: "room_id",
                table: "agenda_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_agenda_items_room_id",
                table: "agenda_items",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "fk_agenda_items_rooms_room_id",
                table: "agenda_items",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "room_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_agenda_items_sessions_session_id",
                table: "agenda_items",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_agenda_items_rooms_room_id",
                table: "agenda_items");

            migrationBuilder.DropForeignKey(
                name: "fk_agenda_items_sessions_session_id",
                table: "agenda_items");

            migrationBuilder.DropIndex(
                name: "ix_agenda_items_room_id",
                table: "agenda_items");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "agenda_items");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "agenda_items");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "agenda_items",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "agenda_items",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "agenda_items",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddForeignKey(
                name: "fk_agenda_items_sessions_session_id",
                table: "agenda_items",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id");
        }
    }
}
