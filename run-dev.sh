#!/bin/bash
set -e

echo "=== STARTING DEV ENVIRONMENT ==="

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PM_DIR="$ROOT_DIR/port_management"

COMPOSE_BASE="$PM_DIR/docker-compose.yml"
COMPOSE_DEV="$PM_DIR/docker-compose-dev.yml"

BACKEND_API="$PM_DIR/backend/Api"
FRONTEND_DIR="$PM_DIR/frontend"

bash "$ROOT_DIR/setup-dev.sh"

echo ""
echo "→ Starting docker services (postgres + prolog)..."
docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_DEV" up -d

echo "→ Starting backend..."
cd "$BACKEND_API"
dotnet run &
BACKEND_PID=$!
cd "$ROOT_DIR"

sleep 1

echo "→ Starting frontend..."
cd "$FRONTEND_DIR"
npm run dev &
FRONTEND_PID=$!
cd "$ROOT_DIR"

cleanup() {
    echo ""
    echo "=== CTRL+C — STOPPING DEV ENVIRONMENT ==="

    echo "→ Stopping backend..."
    kill $BACKEND_PID 2>/dev/null || true

    echo "→ Stopping frontend..."
    kill $FRONTEND_PID 2>/dev/null || true

    sleep 1
    exit 0
}

trap cleanup INT

echo ""
echo "Dev environment running:"
echo "  Backend   → http://localhost:5164"
echo "  Frontend  → http://localhost:5173"
echo ""
echo "Press CTRL+C to stop everything."

wait $BACKEND_PID
wait $FRONTEND_PID

