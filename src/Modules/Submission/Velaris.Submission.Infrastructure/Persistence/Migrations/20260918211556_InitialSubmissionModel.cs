using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Velaris.Submission.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSubmissionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "submission");

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MaxMessageSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    MaxRecipientsPerSubmission = table.Column<int>(type: "integer", nullable: false),
                    MaxSmtpSessionDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                    table.CheckConstraint("CK_tenants_max_duration", "\"MaxSmtpSessionDurationSeconds\" > 0");
                    table.CheckConstraint("CK_tenants_max_message_size", "\"MaxMessageSizeBytes\" > 0");
                    table.CheckConstraint("CK_tenants_max_recipients", "\"MaxRecipientsPerSubmission\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "environments",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_environments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_environments_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "submission",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_quotas",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PeriodStartUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PeriodEndUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AllocatedLimit = table.Column<long>(type: "bigint", nullable: false),
                    UsedCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_quotas", x => x.Id);
                    table.CheckConstraint("CK_tenant_quotas_allocated_limit", "\"AllocatedLimit\" >= 0");
                    table.CheckConstraint("CK_tenant_quotas_used_count", "\"UsedCount\" >= 0");
                    table.ForeignKey(
                        name: "FK_tenant_quotas_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "submission",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "credentials",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_credentials_environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "submission",
                        principalTable: "environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_credentials_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "submission",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "environment_quotas",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PeriodStartUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PeriodEndUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AllocatedLimit = table.Column<long>(type: "bigint", nullable: false),
                    UsedCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_environment_quotas", x => x.Id);
                    table.CheckConstraint("CK_environment_quotas_allocated_limit", "\"AllocatedLimit\" >= 0");
                    table.CheckConstraint("CK_environment_quotas_used_count", "\"UsedCount\" >= 0");
                    table.ForeignKey(
                        name: "FK_environment_quotas_environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "submission",
                        principalTable: "environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suppressions",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Source = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SmtpCode = table.Column<int>(type: "integer", nullable: true),
                    EnhancedStatusCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppressions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_suppressions_environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "submission",
                        principalTable: "environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "credential_verifiers",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CredentialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Mechanism = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    IterationCount = table.Column<int>(type: "integer", nullable: false),
                    StoredKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    ServerKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credential_verifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_credential_verifiers_credentials_CredentialId",
                        column: x => x.CredentialId,
                        principalSchema: "submission",
                        principalTable: "credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CredentialId = table.Column<Guid>(type: "uuid", nullable: true),
                    EnvelopeSender = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Subject = table.Column<string>(type: "character varying(998)", maxLength: 998, nullable: true),
                    AcceptedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SubmissionIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    ClientEhlo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TlsCipher = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AuthenticatedUser = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_messages_credentials_CredentialId",
                        column: x => x.CredentialId,
                        principalSchema: "submission",
                        principalTable: "credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_messages_environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "submission",
                        principalTable: "environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "submission",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sender_grants",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CredentialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sender_grants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sender_grants_credentials_CredentialId",
                        column: x => x.CredentialId,
                        principalSchema: "submission",
                        principalTable: "credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attachment_references",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    OpaqueObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    HashAlgorithm = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    HashValue = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StorageVersion = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachment_references", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attachment_references_messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "submission",
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deliveries",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_deliveries_environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "submission",
                        principalTable: "environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_deliveries_messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "submission",
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_deliveries_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "submission",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "message_body_parts",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    MediaType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Charset = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ContentTransferEncoding = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ContentId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContentDisposition = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_body_parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_message_body_parts_messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "submission",
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message_headers",
                schema: "submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_headers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_message_headers_messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "submission",
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attachment_references_MessageId_OrderIndex",
                schema: "submission",
                table: "attachment_references",
                columns: new[] { "MessageId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_credential_verifiers_CredentialId_Version",
                schema: "submission",
                table: "credential_verifiers",
                columns: new[] { "CredentialId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_credentials_EnvironmentId_Name",
                schema: "submission",
                table: "credentials",
                columns: new[] { "EnvironmentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_credentials_TenantId",
                schema: "submission",
                table: "credentials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_deliveries_EnvironmentId",
                schema: "submission",
                table: "deliveries",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_deliveries_MessageId_RecipientEmail",
                schema: "submission",
                table: "deliveries",
                columns: new[] { "MessageId", "RecipientEmail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_deliveries_TenantId",
                schema: "submission",
                table: "deliveries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_environment_quotas_EnvironmentId_PeriodType_PeriodStartUtc",
                schema: "submission",
                table: "environment_quotas",
                columns: new[] { "EnvironmentId", "PeriodType", "PeriodStartUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_environments_TenantId_Name",
                schema: "submission",
                table: "environments",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_body_parts_MessageId_OrderIndex",
                schema: "submission",
                table: "message_body_parts",
                columns: new[] { "MessageId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_message_headers_MessageId_OrderIndex",
                schema: "submission",
                table: "message_headers",
                columns: new[] { "MessageId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_CredentialId",
                schema: "submission",
                table: "messages",
                column: "CredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_EnvironmentId",
                schema: "submission",
                table: "messages",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_TenantId_AcceptedAtUtc",
                schema: "submission",
                table: "messages",
                columns: new[] { "TenantId", "AcceptedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_sender_grants_CredentialId_Type_Value",
                schema: "submission",
                table: "sender_grants",
                columns: new[] { "CredentialId", "Type", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppressions_EnvironmentId_Email",
                schema: "submission",
                table: "suppressions",
                columns: new[] { "EnvironmentId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_quotas_TenantId_PeriodType_PeriodStartUtc",
                schema: "submission",
                table: "tenant_quotas",
                columns: new[] { "TenantId", "PeriodType", "PeriodStartUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Slug",
                schema: "submission",
                table: "tenants",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachment_references",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "credential_verifiers",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "deliveries",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "environment_quotas",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "message_body_parts",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "message_headers",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "sender_grants",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "suppressions",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "tenant_quotas",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "messages",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "credentials",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "environments",
                schema: "submission");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "submission");
        }
    }
}
