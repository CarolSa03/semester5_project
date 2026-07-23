#!/bin/bash
set -e

echo "=== CURRENT SPACE ==="
df -h /

echo ""
echo "=== 1. CLEANING DOCKER (Aggressive) ==="
# -a: Remove all unused images not just dangling ones
# -f: Force without prompt
# --volumes: Remove unused anonymous volumes
docker system prune -a -f --volumes

echo ""
echo "=== 2. CLEANING .NET NUGET CACHE ==="
# This folder grows huge (GBs) when running dotnet restore/migrations locally
rm -rf /root/.nuget/packages/*
rm -rf /home/u1220780/.nuget/packages/*
rm -rf /tmp/NuGetScratch

echo ""
echo "=== 3. CLEANING APT CACHE ==="
apt-get clean
apt-get autoremove -y

echo ""
echo "=== 4. CLEANING SYSTEM LOGS ==="
# Vacuum logs older than 1 day
journalctl --vacuum-time=1d

echo ""
echo "=== 5. CLEANING GITHUB RUNNER TEMP FILES ==="
# Clean the runner's internal diag logs
rm -rf /root/actions-runner/_diag/*
# Clean the runner's work directory (Repo checkout) - it will re-clone next time
rm -rf /root/actions-runner/_work/*

echo ""
echo "=== NEW SPACE STATUS ==="
df -h /
