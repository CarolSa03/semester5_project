#!/bin/bash
source ./config.env

echo "=== SETTING UP FILESYSTEM ==="

# 1. Create Directories
mkdir -p "$APP_DIR"
mkdir -p "$DB_DATA_DIR"
mkdir -p "$PUBLIC_DIR"
mkdir -p "$IPS_DIR"

# 2. Create Admin Group
if ! getent group "$ADMIN_GROUP" > /dev/null; then groupadd "$ADMIN_GROUP"; fi

# 3. Permission Configuration

# A. Project Folder (/srv/project)
echo "Setting ownership of $APP_DIR to $RUNNER_USER..."
chown -R "$RUNNER_USER":"$RUNNER_USER" "$APP_DIR"
chmod -R 775 "$APP_DIR"

# B. Postgres Data (/srv/postgres-data)
echo "Setting permissions for DB Data..."
chown root:root "$DB_DATA_DIR"
chmod 755 "$DB_DATA_DIR"

# C. Public Folder & Allowed IPs
chown -R root:"$ADMIN_GROUP" "$PUBLIC_DIR"
chown -R root:"$ADMIN_GROUP" "$IPS_DIR"
chmod 775 "$PUBLIC_DIR"
chmod 775 "$IPS_DIR"

# 4. Create Default Configs if missing
if [ ! -f "$IPS_DIR/default_allowed.conf" ]; then
    echo -e "allow 10.0.0.0/8;\nallow 172.16.0.0/12;\nallow 192.168.0.0/16;\nallow 127.0.0.1;\ndeny all;" > "$IPS_DIR/default_allowed.conf"
fi

if [ ! -f "$PUBLIC_DIR/readme.txt" ]; then
    echo "Public shared folder working." > "$PUBLIC_DIR/readme.txt"
fi

# 5. Prepare for Symlink
if [ -d "/srv/actions" ] && [ ! -L "/srv/actions" ]; then
    echo "WARNING: /srv/actions exists as a directory. Renaming to backup..."
    mv /srv/actions "/srv/actions_backup_$(date +%s)"
fi

echo "Filesystem ready."
