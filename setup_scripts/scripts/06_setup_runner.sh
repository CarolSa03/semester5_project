#!/bin/bash
source ./config.env

echo "=== GITHUB RUNNER SETUP ==="
echo "Selected User: $RUNNER_USER"

# === PATH CONFIGURATION ===
if [ "$RUNNER_USER" == "root" ]; then
    export RUNNER_ALLOW_RUNASROOT=1
    RUNNER_DIR="/root/actions-runner"
    IS_ROOT=true
    echo ">> Running in NUCLEAR MODE (Root)"
else
    RUNNER_DIR="/home/$RUNNER_USER/actions-runner"
    IS_ROOT=false
    echo ">> Running in SAFE MODE (User)"
    
    usermod -aG docker "$RUNNER_USER"
    mkdir -p "$APP_DIR"
    chown -R "$RUNNER_USER":"$RUNNER_USER" "$APP_DIR"
    chmod -R 775 "$APP_DIR"
fi

# === DOWNLOAD ===
if [ -d "$RUNNER_DIR" ]; then
    echo "Runner directory exists."
else
    echo "Creating Runner directory: $RUNNER_DIR"
    mkdir -p "$RUNNER_DIR"
    
    if [ "$IS_ROOT" = false ]; then
        chown "$RUNNER_USER":"$RUNNER_USER" "$RUNNER_DIR"
    fi

    echo "Downloading Runner..."
    run_cmd() {
        if [ "$IS_ROOT" = true ]; then
            bash -c "$1"
        else
            su - "$RUNNER_USER" -c "$1"
        fi
    }

    run_cmd "mkdir -p ~/actions-runner && cd ~/actions-runner && curl -o actions-runner-linux-x64-${RUNNER_VERSION}.tar.gz -L https://github.com/actions/runner/releases/download/v${RUNNER_VERSION}/actions-runner-linux-x64-${RUNNER_VERSION}.tar.gz"
    run_cmd "cd ~/actions-runner && tar xzf ./actions-runner-linux-x64-${RUNNER_VERSION}.tar.gz"
fi

# === MANUAL DEPENDENCY FIX ===
# We skipped the GitHub script because it breaks on Debian 13.
# We assume you ran: apt-get install -y libssl3 libicu-dev krb5-locales zlib1g
echo "Skipping default dependency check (Incompatible with Debian 13)..."

# === SYMLINK SETUP ===
echo "=== SYMLINK SETUP ==="
if [ ! -e "/srv/actions" ]; then
    echo "Linking /srv/actions -> $RUNNER_DIR"
    ln -s "$RUNNER_DIR" /srv/actions
else
    echo "/srv/actions link already exists."
fi

# === CHECK IF ALREADY CONFIGURED ===
if [ -f "$RUNNER_DIR/.runner" ]; then
    echo "---------------------------------------------------"
    echo "RUNNER ALREADY CONFIGURED. SKIPPING REGISTRATION."
    echo "---------------------------------------------------"
    
    # Just try to ensure the service is running
    cd "$RUNNER_DIR"
    ./svc.sh status || ./svc.sh start
    exit 0
fi

# === REGISTRATION ===
echo ""
echo "!!! ACTION REQUIRED !!!"
echo "Paste your GitHub Runner Token below."
read -p "Token: " RUNNER_TOKEN
read -p "Repo URL: " REPO_URL
echo ""

echo "Registering Runner..."

# Define the command string
CMD="./config.sh --url $REPO_URL --token $RUNNER_TOKEN --labels debian13-staging --unattended --replace"

if [ "$IS_ROOT" = true ]; then
    cd "$RUNNER_DIR"
    $CMD
else
    su - "$RUNNER_USER" -c "cd ~/actions-runner && $CMD"
fi

# ERROR CHECKING: Did config.sh succeed?
if [ ! -f "$RUNNER_DIR/svc.sh" ]; then
    echo "ERROR: Registration failed. svc.sh was not created."
    echo "Likely cause: Invalid Token or Network issue."
    exit 1
fi

echo "Installing Service..."
cd "$RUNNER_DIR"
./svc.sh install "$RUNNER_USER"
./svc.sh start

echo "SUCCESS! Runner is active."
