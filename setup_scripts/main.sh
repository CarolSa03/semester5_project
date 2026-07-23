#!/bin/bash
set -e

# Load Config
source ./config.env

echo "=== STARTING MODULAR SETUP ==="

# Check for root
if [ "$EUID" -ne 0 ]; then 
  echo "Please run as root (sudo bash main.sh)"
  exit 1
fi

# Make scripts executable
chmod +x scripts/*.sh

echo ">> Running 01_packages.sh..."
./scripts/01_packages.sh

echo ">> Running 02_filesystem.sh..."
./scripts/02_filesystem.sh

echo ">> Running 03_security_pam.sh..."
./scripts/03_security_pam.sh

echo ">> Running 04_security_ssh.sh..."
./scripts/04_security_ssh.sh

echo ">> Running 05_fail2ban.sh..."
./scripts/05_fail2ban.sh

echo ">> Running 06_setup_runner.sh..."
./scripts/06_setup_runner.sh

echo ">> Running 07_setup_email.sh..."
./scripts/07_setup_email.sh

echo "=== ALL DONE! ==="
echo "Please verify your GitHub Runner is 'Idle' in the repo settings."
