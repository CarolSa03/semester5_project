#!/bin/bash
set -e

echo "=== FULL DEV SETUP ==="

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PM_DIR="$ROOT_DIR/port_management"

BACKEND_SOLUTION="$PM_DIR/backend/PortManagement.sln"
BACKEND_API="$PM_DIR/backend/Api"
INFRASTRUCTURE="$PM_DIR/backend/Infrastructure"
FRONTEND_DIR="$PM_DIR/frontend"

COMPOSE_BASE="$PM_DIR/docker-compose.yml"
COMPOSE_DEV="$PM_DIR/docker-compose-dev.yml"

# -----------------------------
# 1. Tool checks
# -----------------------------
check() {
    if ! command -v "$1" >/dev/null 2>&1; then
        echo "Missing: $1"
        MISSING=1
    else
        echo "Found: $1"
    fi
}

echo "Checking required tools..."
check docker
check docker compose
check dotnet
check npm
check node

if [ "$MISSING" = "1" ]; then
    echo ""
    echo "You must install the missing tools above before proceeding."
    exit 1
fi

# -----------------------------
# 2. dotnet-ef install
# -----------------------------
if ! command -v dotnet-ef >/dev/null 2>&1; then
    echo "Installing dotnet-ef..."
    dotnet tool install --global dotnet-ef
else
    echo "dotnet-ef installed"
fi

# -----------------------------
# 3. Restore backend packages
# -----------------------------
echo ""
echo "Restoring backend packages..."
dotnet restore "$BACKEND_SOLUTION"

# -----------------------------
# 4. Install frontend deps
# -----------------------------
echo ""
echo "Installing frontend dependencies..."
npm install --prefix "$FRONTEND_DIR"

# -----------------------------
# 5. Start docker so DB exists for migrations
# -----------------------------
echo ""
echo "Starting docker (postgres + prolog) for migrations..."
docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_DEV" up -d --build

sleep 2

# -----------------------------
# 6. Apply EF migrations automatically
# -----------------------------
echo ""
echo "Applying EF migrations to the development database..."
cd "$BACKEND_API"
dotnet ef database update --project "$INFRASTRUCTURE" --startup-project .

cd "$ROOT_DIR"

# -----------------------------
# 7. Pull docker images
# -----------------------------
echo ""
echo "Pulling docker images..."
docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_DEV" pull || true

echo ""
echo "====================================="
echo "   DEV SETUP COMPLETED SUCCESSFULLY"
echo "====================================="
echo ""
echo "You can now run:  ./run-dev.sh"

