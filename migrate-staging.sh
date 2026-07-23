#!/bin/bash
set -e

echo "=== RUNNING STAGING DATABASE MIGRATIONS (via SDK container) ==="

BACKEND_DIR="/srv/project/port_management/backend"
NETWORK_NAME="portnet"
CONNECTION_STRING="Host=postgres;Port=5432;Database=portdb;Username=dbadmin;Password=password123!"

# Check that the docker network exists
if ! docker network ls --format '{{.Name}}' | grep -qx "$NETWORK_NAME"; then
  echo "ERROR: Docker network '$NETWORK_NAME' does not exist."
  exit 1
fi

docker run --rm \
  --network "$NETWORK_NAME" \
  -v "$BACKEND_DIR:/src" \
  mcr.microsoft.com/dotnet/sdk:9.0 \
  bash -c '
    echo ">>> Installing EF Tools"
    dotnet tool install --global dotnet-ef --version 9.*
    export PATH="/root/.dotnet/tools:$PATH"

    echo ">>> Restoring solution"
    cd /src/Api
    dotnet restore ../PortManagement.sln

    echo ">>> Running EF migrations"
    dotnet ef database update \
      --project ../Infrastructure \
      --startup-project . \
      --connection "'"$CONNECTION_STRING"'"
  '

echo "=== MIGRATIONS APPLIED SUCCESSFULLY ==="

