CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'submission') THEN
        CREATE SCHEMA submission;
    END IF;
END $EF$;

CREATE TABLE submission.tenants (
    "Id" uuid NOT NULL,
    "Slug" character varying(64) NOT NULL,
    "Name" character varying(128) NOT NULL,
    "State" character varying(32) NOT NULL,
    "MaxMessageSizeBytes" bigint NOT NULL,
    "MaxRecipientsPerSubmission" integer NOT NULL,
    "MaxSmtpSessionDurationSeconds" integer NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "UpdatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_tenants" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_tenants_max_duration" CHECK ("MaxSmtpSessionDurationSeconds" > 0),
    CONSTRAINT "CK_tenants_max_message_size" CHECK ("MaxMessageSizeBytes" > 0),
    CONSTRAINT "CK_tenants_max_recipients" CHECK ("MaxRecipientsPerSubmission" > 0)
);

CREATE TABLE submission.environments (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "Name" character varying(64) NOT NULL,
    "State" character varying(32) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "UpdatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_environments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_environments_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES submission.tenants ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.tenant_quotas (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "PeriodType" character varying(32) NOT NULL,
    "PeriodStartUtc" timestamp with time zone NOT NULL,
    "PeriodEndUtc" timestamp with time zone NOT NULL,
    "AllocatedLimit" bigint NOT NULL,
    "UsedCount" bigint NOT NULL,
    CONSTRAINT "PK_tenant_quotas" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_tenant_quotas_allocated_limit" CHECK ("AllocatedLimit" >= 0),
    CONSTRAINT "CK_tenant_quotas_used_count" CHECK ("UsedCount" >= 0),
    CONSTRAINT "FK_tenant_quotas_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES submission.tenants ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.credentials (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "EnvironmentId" uuid NOT NULL,
    "Name" character varying(128) NOT NULL,
    "Type" character varying(32) NOT NULL,
    "State" character varying(32) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "RevokedAtUtc" timestamp with time zone,
    CONSTRAINT "PK_credentials" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_credentials_environments_EnvironmentId" FOREIGN KEY ("EnvironmentId") REFERENCES submission.environments ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_credentials_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES submission.tenants ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.environment_quotas (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "EnvironmentId" uuid NOT NULL,
    "PeriodType" character varying(32) NOT NULL,
    "PeriodStartUtc" timestamp with time zone NOT NULL,
    "PeriodEndUtc" timestamp with time zone NOT NULL,
    "AllocatedLimit" bigint NOT NULL,
    "UsedCount" bigint NOT NULL,
    CONSTRAINT "PK_environment_quotas" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_environment_quotas_allocated_limit" CHECK ("AllocatedLimit" >= 0),
    CONSTRAINT "CK_environment_quotas_used_count" CHECK ("UsedCount" >= 0),
    CONSTRAINT "FK_environment_quotas_environments_EnvironmentId" FOREIGN KEY ("EnvironmentId") REFERENCES submission.environments ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.suppressions (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "EnvironmentId" uuid NOT NULL,
    "Email" character varying(320) NOT NULL,
    "Reason" character varying(256) NOT NULL,
    "Source" character varying(64) NOT NULL,
    "SmtpCode" integer,
    "EnhancedStatusCode" character varying(32),
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "ExpiresAtUtc" timestamp with time zone,
    CONSTRAINT "PK_suppressions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_suppressions_environments_EnvironmentId" FOREIGN KEY ("EnvironmentId") REFERENCES submission.environments ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.credential_verifiers (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "CredentialId" uuid NOT NULL,
    "Version" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "Mechanism" character varying(64) NOT NULL,
    "Salt" bytea NOT NULL,
    "IterationCount" integer NOT NULL,
    "StoredKey" bytea NOT NULL,
    "ServerKey" bytea NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "RevokedAtUtc" timestamp with time zone,
    CONSTRAINT "PK_credential_verifiers" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_credential_verifiers_credentials_CredentialId" FOREIGN KEY ("CredentialId") REFERENCES submission.credentials ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.messages (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "EnvironmentId" uuid NOT NULL,
    "CredentialId" uuid,
    "EnvelopeSender" character varying(320) NOT NULL,
    "Subject" character varying(998),
    "AcceptedAtUtc" timestamp with time zone NOT NULL,
    "SubmissionIp" character varying(45),
    "ClientEhlo" character varying(255),
    "TlsCipher" character varying(64),
    "AuthenticatedUser" character varying(128),
    CONSTRAINT "PK_messages" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_messages_credentials_CredentialId" FOREIGN KEY ("CredentialId") REFERENCES submission.credentials ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_messages_environments_EnvironmentId" FOREIGN KEY ("EnvironmentId") REFERENCES submission.environments ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_messages_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES submission.tenants ("Id") ON DELETE RESTRICT
);

CREATE TABLE submission.sender_grants (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "CredentialId" uuid NOT NULL,
    "Type" character varying(32) NOT NULL,
    "Value" character varying(256) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_sender_grants" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_sender_grants_credentials_CredentialId" FOREIGN KEY ("CredentialId") REFERENCES submission.credentials ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.attachment_references (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "MessageId" uuid NOT NULL,
    "OpaqueObjectKey" character varying(512) NOT NULL,
    "FileName" character varying(255),
    "ContentType" character varying(128) NOT NULL,
    "SizeBytes" bigint NOT NULL,
    "HashAlgorithm" character varying(32) NOT NULL,
    "HashValue" character varying(128) NOT NULL,
    "StorageVersion" character varying(128),
    "OrderIndex" integer NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_attachment_references" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_attachment_references_messages_MessageId" FOREIGN KEY ("MessageId") REFERENCES submission.messages ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.deliveries (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "EnvironmentId" uuid NOT NULL,
    "MessageId" uuid NOT NULL,
    "RecipientEmail" character varying(320) NOT NULL,
    "State" character varying(32) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_deliveries" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_deliveries_environments_EnvironmentId" FOREIGN KEY ("EnvironmentId") REFERENCES submission.environments ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_deliveries_messages_MessageId" FOREIGN KEY ("MessageId") REFERENCES submission.messages ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_deliveries_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES submission.tenants ("Id") ON DELETE RESTRICT
);

CREATE TABLE submission.message_body_parts (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "MessageId" uuid NOT NULL,
    "OrderIndex" integer NOT NULL,
    "MediaType" character varying(128) NOT NULL,
    "Charset" character varying(64),
    "ContentTransferEncoding" character varying(64),
    "ContentId" character varying(256),
    "ContentDisposition" character varying(64),
    "Content" text NOT NULL,
    CONSTRAINT "PK_message_body_parts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_message_body_parts_messages_MessageId" FOREIGN KEY ("MessageId") REFERENCES submission.messages ("Id") ON DELETE CASCADE
);

CREATE TABLE submission.message_headers (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "MessageId" uuid NOT NULL,
    "OrderIndex" integer NOT NULL,
    "Name" character varying(256) NOT NULL,
    "Value" text NOT NULL,
    CONSTRAINT "PK_message_headers" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_message_headers_messages_MessageId" FOREIGN KEY ("MessageId") REFERENCES submission.messages ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_attachment_references_MessageId_OrderIndex" ON submission.attachment_references ("MessageId", "OrderIndex");

CREATE UNIQUE INDEX "IX_credential_verifiers_CredentialId_Version" ON submission.credential_verifiers ("CredentialId", "Version");

CREATE UNIQUE INDEX "IX_credentials_EnvironmentId_Name" ON submission.credentials ("EnvironmentId", "Name");

CREATE INDEX "IX_credentials_TenantId" ON submission.credentials ("TenantId");

CREATE INDEX "IX_deliveries_EnvironmentId" ON submission.deliveries ("EnvironmentId");

CREATE UNIQUE INDEX "IX_deliveries_MessageId_RecipientEmail" ON submission.deliveries ("MessageId", "RecipientEmail");

CREATE INDEX "IX_deliveries_TenantId" ON submission.deliveries ("TenantId");

CREATE UNIQUE INDEX "IX_environment_quotas_EnvironmentId_PeriodType_PeriodStartUtc" ON submission.environment_quotas ("EnvironmentId", "PeriodType", "PeriodStartUtc");

CREATE UNIQUE INDEX "IX_environments_TenantId_Name" ON submission.environments ("TenantId", "Name");

CREATE INDEX "IX_message_body_parts_MessageId_OrderIndex" ON submission.message_body_parts ("MessageId", "OrderIndex");

CREATE INDEX "IX_message_headers_MessageId_OrderIndex" ON submission.message_headers ("MessageId", "OrderIndex");

CREATE INDEX "IX_messages_CredentialId" ON submission.messages ("CredentialId");

CREATE INDEX "IX_messages_EnvironmentId" ON submission.messages ("EnvironmentId");

CREATE INDEX "IX_messages_TenantId_AcceptedAtUtc" ON submission.messages ("TenantId", "AcceptedAtUtc");

CREATE UNIQUE INDEX "IX_sender_grants_CredentialId_Type_Value" ON submission.sender_grants ("CredentialId", "Type", "Value");

CREATE UNIQUE INDEX "IX_suppressions_EnvironmentId_Email" ON submission.suppressions ("EnvironmentId", "Email");

CREATE UNIQUE INDEX "IX_tenant_quotas_TenantId_PeriodType_PeriodStartUtc" ON submission.tenant_quotas ("TenantId", "PeriodType", "PeriodStartUtc");

CREATE UNIQUE INDEX "IX_tenants_Slug" ON submission.tenants ("Slug");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260918211556_InitialSubmissionModel', '10.0.11');

COMMIT;

