#!/bin/bash
set -e

echo "=== STAGING SETUP START ==="

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PM_DIR="$ROOT_DIR/port_management"

BACKEND_DIR="$PM_DIR/backend"
API_DIR="$PM_DIR/backend/Api"
INFRA_DIR="$PM_DIR/backend/Infrastructure"

COMPOSE_BASE="$PM_DIR/docker-compose.yml"
COMPOSE_STAGING="$PM_DIR/docker-compose-staging.yml"

echo ""
echo "--- Checking required tools ---"

REQUIRED_TOOLS=(docker git dotnet)
missing=0

for t in "${REQUIRED_TOOLS[@]}"; do
    if ! command -v "$t" >/dev/null 2>&1; then
        echo "Missing: $t"
        missing=1
    else
        echo " Found: $t"
    fi
done

if [ "$missing" -eq 1 ]; then
    echo "Install missing tools before continuing."
    exit 1
fi

echo ""
echo "--- Restoring .NET local tools ---"
cd "$BACKEND_DIR"
dotnet tool restore

echo ""
echo "--- Restoring backend solution packages ---"
dotnet restore

echo ""
echo "--- Starting postgres & prolog (for migration run) ---"
docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_STAGING" up -d postgres prolog

echo "Waiting 8 seconds for Postgres to become ready..."
sleep 8

echo ""
echo "--- Applying EF migrations to STAGING database ---"
export ASPNETCORE_ENVIRONMENT="Staging"
export ConnectionStrings__DefaultPostgres="Host=localhost;Port=5432;Database=portdb;Username=dbadmin;Password=password123!"

cd "$API_DIR"
dotnet ef database update --project "$INFRA_DIR" --startup-project "$API_DIR"

echo ""
echo "--- Building ALL Docker images (first time) ---"
cd "$PM_DIR"
docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_STAGING" build

echo ""
echo "=== STAGING SETUP COMPLETE ==="

