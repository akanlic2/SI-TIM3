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
            migrationBuilder.Sql(@"
                ALTER TABLE agenda_items 
                ADD COLUMN IF NOT EXISTS created_at timestamp with time zone NOT NULL DEFAULT (NOW());
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE agenda_items 
                ADD COLUMN IF NOT EXISTS room_id uuid NULL;
            ");

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

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_agenda_items_room_id 
                ON agenda_items (room_id);
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE agenda_items 
                DROP CONSTRAINT IF EXISTS fk_agenda_items_sessions_session_id;
            ");

            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint 
                        WHERE conname = 'fk_agenda_items_rooms_room_id'
                    ) THEN
                        ALTER TABLE agenda_items
                        ADD CONSTRAINT fk_agenda_items_rooms_room_id
                        FOREIGN KEY (room_id) REFERENCES rooms(room_id)
                        ON DELETE SET NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint 
                        WHERE conname = 'fk_agenda_items_sessions_session_id'
                    ) THEN
                        ALTER TABLE agenda_items
                        ADD CONSTRAINT fk_agenda_items_sessions_session_id
                        FOREIGN KEY (session_id) REFERENCES sessions(session_id)
                        ON DELETE SET NULL;
                    END IF;
                END $$;
            ");
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