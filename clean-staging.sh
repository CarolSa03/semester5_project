#!/bin/bash
set -e

echo "=== STOPPING STAGING ENVIRONMENT ==="

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PM_DIR="$ROOT_DIR/port_management"

COMPOSE_BASE="$PM_DIR/docker-compose.yml"
COMPOSE_STAGING="$PM_DIR/docker-compose-staging.yml"

docker compose -f "$COMPOSE_BASE" -f "$COMPOSE_STAGING" down

echo "=== STAGING STOPPED ==="

