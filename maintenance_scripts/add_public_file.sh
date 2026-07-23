#!/bin/bash
set -e

echo "=== Adding example file to /srv/project/public ==="

sudo tee /srv/project/public/readme.txt >/dev/null <<EOF
Public shared folder working.
$(date)
EOF

sudo chmod 664 /srv/project/public/readme.txt

echo "=== Access via http://<server-ip>/public/readme.txt ==="
