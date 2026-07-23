#!/bin/bash
echo "=== STOPPING DEV ENVIRONMENT ==="

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PM_DIR="$ROOT_DIR/port_management"

COMPOSE_BASE="$PM_DIR/docker-compose.yml"
COMPOSE_DEV="$PM_DIR/docker-compose-dev.yml"

echo "Stopping docker containers..."
docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_DEV" down

echo "Killing dotnet processes..."
pkill -f "dotnet run" 2>/dev/null || true
pkill -f "dotnet watch" 2>/dev/null || true

echo "Killing npm/node processes..."
#pkill -f "npm" 2>/dev/null || true
#pkill -f "node" 2>/dev/null || true

echo "Dev stopped."

