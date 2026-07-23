#!/bin/bash
set -e

echo "=== STARTING STAGING ENVIRONMENT ==="

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PM_DIR="$ROOT_DIR/port_management"

COMPOSE_BASE="$PM_DIR/docker-compose.yml"
COMPOSE_STAGING="$PM_DIR/docker-compose-staging.yml"

echo "Ensuring setup is completed..."
bash "$ROOT_DIR/setup-staging.sh"

echo ""
echo "--- Starting full docker stack ---"
docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_STAGING" up -d

echo ""
echo "=== STAGING IS RUNNING ==="
docker ps --format "table {{.Names}}\t{{.Status}}"

IP=$(hostname -I | awk '{print $1}')

echo ""
echo "Frontend → http://$IP/"
echo "API      → http://$IP/api"
echo "Prolog   → http://$IP/prolog"
echo ""

