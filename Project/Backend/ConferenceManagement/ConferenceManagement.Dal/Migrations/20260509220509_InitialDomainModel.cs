using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceManagement.Dal.Migrations
{
    /// <inheritdoc />
    public partial class InitialDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conference_registrations",
                columns: table => new
                {
                    conference_registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    registration_status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conference_registrations", x => x.conference_registration_id);
                    table.ForeignKey(
                        name: "fk_conference_registrations_conferences_conference_id",
                        column: x => x.conference_id,
                        principalTable: "conferences",
                        principalColumn: "conference_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_conference_registrations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conference_user",
                columns: table => new
                {
                    organized_conferences_conference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organizers_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conference_user", x => new { x.organized_conferences_conference_id, x.organizers_user_id });
                    table.ForeignKey(
                        name: "fk_conference_user_conferences_organized_conferences_conferenc",
                        column: x => x.organized_conferences_conference_id,
                        principalTable: "conferences",
                        principalColumn: "conference_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_conference_user_users_organizers_user_id",
                        column: x => x.organizers_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logistics_tasks",
                columns: table => new
                {
                    logistics_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    task_type = table.Column<string>(type: "text", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logistics_tasks", x => x.logistics_task_id);
                    table.ForeignKey(
                        name: "fk_logistics_tasks_conferences_conference_id",
                        column: x => x.conference_id,
                        principalTable: "conferences",
                        principalColumn: "conference_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    notification_type = table.Column<string>(type: "text", nullable: false),
                    sent_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "fk_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.room_id);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conference_registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    payment_status = table.Column<string>(type: "text", nullable: false),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    payment_method = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.payment_id);
                    table.ForeignKey(
                        name: "fk_payments_conference_registrations_conference_registration_id",
                        column: x => x.conference_registration_id,
                        principalTable: "conference_registrations",
                        principalColumn: "conference_registration_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    session_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sessions", x => x.session_id);
                    table.ForeignKey(
                        name: "fk_sessions_conferences_conference_id",
                        column: x => x.conference_id,
                        principalTable: "conferences",
                        principalColumn: "conference_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sessions_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agenda_items",
                columns: table => new
                {
                    agenda_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agenda_items", x => x.agenda_item_id);
                    table.ForeignKey(
                        name: "fk_agenda_items_conferences_conference_id",
                        column: x => x.conference_id,
                        principalTable: "conferences",
                        principalColumn: "conference_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_agenda_items_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id");
                });

            migrationBuilder.CreateTable(
                name: "equipments",
                columns: table => new
                {
                    equipment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    availability_status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipments", x => x.equipment_id);
                    table.ForeignKey(
                        name: "fk_equipments_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                columns: table => new
                {
                    material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    material_type = table.Column<string>(type: "text", nullable: false),
                    upload_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_materials", x => x.material_id);
                    table.ForeignKey(
                        name: "fk_materials_conferences_conference_id",
                        column: x => x.conference_id,
                        principalTable: "conferences",
                        principalColumn: "conference_id");
                    table.ForeignKey(
                        name: "fk_materials_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id");
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    asked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    answer = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.question_id);
                    table.ForeignKey(
                        name: "fk_questions_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_registrations",
                columns: table => new
                {
                    session_registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    registration_status = table.Column<string>(type: "text", nullable: false),
                    is_speaker = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session_registrations", x => x.session_registration_id);
                    table.ForeignKey(
                        name: "fk_session_registrations_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_session_registrations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agenda_items_conference_id",
                table: "agenda_items",
                column: "conference_id");

            migrationBuilder.CreateIndex(
                name: "ix_agenda_items_session_id",
                table: "agenda_items",
                column: "session_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_conference_registrations_conference_id",
                table: "conference_registrations",
                column: "conference_id");

            migrationBuilder.CreateIndex(
                name: "ix_conference_registrations_user_id",
                table: "conference_registrations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_conference_user_organizers_user_id",
                table: "conference_user",
                column: "organizers_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipments_session_id",
                table: "equipments",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_logistics_tasks_conference_id",
                table: "logistics_tasks",
                column: "conference_id");

            migrationBuilder.CreateIndex(
                name: "ix_materials_conference_id",
                table: "materials",
                column: "conference_id");

            migrationBuilder.CreateIndex(
                name: "ix_materials_session_id",
                table: "materials",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_conference_registration_id",
                table: "payments",
                column: "conference_registration_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_questions_session_id",
                table: "questions",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_user_id",
                table: "questions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_registrations_session_id",
                table: "session_registrations",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_registrations_user_id",
                table: "session_registrations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_conference_id",
                table: "sessions",
                column: "conference_id");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_room_id",
                table: "sessions",
                column: "room_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agenda_items");

            migrationBuilder.DropTable(
                name: "conference_user");

            migrationBuilder.DropTable(
                name: "equipments");

            migrationBuilder.DropTable(
                name: "logistics_tasks");

            migrationBuilder.DropTable(
                name: "materials");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "session_registrations");

            migrationBuilder.DropTable(
                name: "conference_registrations");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}
