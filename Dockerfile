# ==============================================================================
# Velaris SMTP Submission Service - Multi-stage Container Image
#
# Non-functional Requirements:
# - RNF-CTR-001: Containerized Deployment (OCI-compatible)
# - RNF-CTR-002: Ephemeral Instances (stateless host execution)
# - RNF-CTR-003: Non-Root Execution (runs as built-in non-root user $APP_UID)
# - RNF-CTR-004: Privilege Reduction (non-root, minimal privileges)
# - RNF-CTR-005: Read-Only Root Filesystem compatible
# - RNF-CTR-007: Health/Readiness strategy:
#     The SMTP Submission service exposes its implicit TLS SMTP endpoint on port 465.
#     Platform orchestrators (e.g., Kubernetes / ECS) perform TCP socket probes on port 465
#     for liveness and readiness to ensure the listener is active and responsive.
# - RNF-CTR-008: Graceful Shutdown (Generic Host handles SIGTERM and drains in-flight requests)
# - RNF-CTR-009: Immutable Images
# - RNF-CTR-010: Image Supply Chain
# ==============================================================================

# Stage 1: Build and Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution, configuration and source code
COPY global.json Directory.Build.props Directory.Packages.props Velaris.slnx ./
COPY src/ ./src/
COPY tests/ ./tests/

# Restore dependencies across the solution using locked mode
RUN dotnet restore --locked-mode

# Build and publish release binaries
RUN dotnet publish src/Hosts/Velaris.SmtpSubmission.Host/Velaris.SmtpSubmission.Host.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime image (non-root)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Run as non-root user (UID 1654 in official Microsoft .NET images)
USER $APP_UID

# Port 465 for SMTP Submission with implicit TLS
EXPOSE 465

ENTRYPOINT ["dotnet", "Velaris.SmtpSubmission.Host.dll"]
