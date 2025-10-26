using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activity_types",
                columns: table => new
                {
                    activity_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__activity__D2470C87629CC28F", x => x.activity_type_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    role = table.Column<decimal>(type: "decimal(2,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__B9BE370F42B9A65E", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "labs",
                columns: table => new
                {
                    lab_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    manager_id = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__labs__66DE64DBFF9798EE", x => x.lab_id);
                    table.ForeignKey(
                        name: "FK__labs__manager_id__3B75D760",
                        column: x => x.manager_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "lab_zones",
                columns: table => new
                {
                    zone_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lab_zone__80B401DFEA5810CC", x => x.zone_id);
                    table.ForeignKey(
                        name: "FK__lab_zones__lab_i__3E52440B",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                });

            migrationBuilder.CreateTable(
                name: "lab_events",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    zone_id = table.Column<int>(type: "int", nullable: false),
                    activity_type_id = table.Column<int>(type: "int", nullable: false),
                    organizer_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lab_even__2370F7271B17C676", x => x.event_id);
                    table.ForeignKey(
                        name: "FK__lab_event__activ__45F365D3",
                        column: x => x.activity_type_id,
                        principalTable: "activity_types",
                        principalColumn: "activity_type_id");
                    table.ForeignKey(
                        name: "FK__lab_event__lab_i__440B1D61",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                    table.ForeignKey(
                        name: "FK__lab_event__organ__46E78A0C",
                        column: x => x.organizer_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__lab_event__zone___44FF419A",
                        column: x => x.zone_id,
                        principalTable: "lab_zones",
                        principalColumn: "zone_id");
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    zone_id = table.Column<int>(type: "int", nullable: true),
                    lab_id = table.Column<int>(type: "int", nullable: true),
                    generated_by = table.Column<int>(type: "int", nullable: false),
                    report_type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    generated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__reports__779B7C581AA75571", x => x.report_id);
                    table.ForeignKey(
                        name: "FK__reports__generat__59063A47",
                        column: x => x.generated_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__reports__lab_id__59FA5E80",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                    table.ForeignKey(
                        name: "FK__reports__zone_id__5AEE82B9",
                        column: x => x.zone_id,
                        principalTable: "lab_zones",
                        principalColumn: "zone_id");
                });

            migrationBuilder.CreateTable(
                name: "event_participants",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<decimal>(type: "decimal(2,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__event_pa__C8EB1457EABB0E83", x => new { x.event_id, x.user_id });
                    table.ForeignKey(
                        name: "FK__event_par__event__49C3F6B7",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__event_par__user___4AB81AF0",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipient_id = table.Column<int>(type: "int", nullable: false),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    is_read = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__E059842F693CAC11", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__notificat__event__5535A963",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__notificat__recip__5441852A",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "security_logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    security_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    photo_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__security__9E2397E03FC0E053", x => x.log_id);
                    table.ForeignKey(
                        name: "FK__security___event__4E88ABD4",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__security___secur__4F7CD00D",
                        column: x => x.security_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_participants_user_id",
                table: "event_participants",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_activity_type_id",
                table: "lab_events",
                column: "activity_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_lab_id",
                table: "lab_events",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_organizer_id",
                table: "lab_events",
                column: "organizer_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_zone_id",
                table: "lab_events",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_zones_lab_id",
                table: "lab_zones",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_labs_manager_id",
                table: "labs",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_event_id",
                table: "notifications",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_id",
                table: "notifications",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_generated_by",
                table: "reports",
                column: "generated_by");

            migrationBuilder.CreateIndex(
                name: "IX_reports_lab_id",
                table: "reports",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_zone_id",
                table: "reports",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_event_id",
                table: "security_logs",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_security_id",
                table: "security_logs",
                column: "security_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E616485912C3D",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_participants");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "security_logs");

            migrationBuilder.DropTable(
                name: "lab_events");

            migrationBuilder.DropTable(
                name: "activity_types");

            migrationBuilder.DropTable(
                name: "lab_zones");

            migrationBuilder.DropTable(
                name: "labs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
