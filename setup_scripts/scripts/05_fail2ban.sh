#!/bin/bash
source ./config.env

echo "=== CONFIGURING FAIL2BAN ==="

systemctl stop fail2ban
rm -f /var/lib/fail2ban/fail2ban.sqlite3

SCRIPT_PATH="/usr/local/bin/ssh-alert.sh"
echo ">> Creating master alert script at $SCRIPT_PATH..."

cat > "$SCRIPT_PATH" <<EOF
#!/bin/bash
# Fail2Ban passes the IP as the first argument (\$1)
IP=\$1
TARGET_EMAIL="$ADMIN_EMAIL"
HOSTNAME=\$(hostname)
TIMESTAMP=\$(date)

MSG="SECURITY ALERT: High volume of SSH connection attempts from \$IP on \$HOSTNAME"

# A. System Log (Permanent Record)
logger -t ssh-alert "\$MSG"

# B. Direct Terminal Broadcast (The "Force" Method)
for term in /dev/pts/[0-9]*; do
    if [ -c "\$term" ]; then
        echo -e "\n\033[1;31m*****************************************************\033[0m" > "\$term"
        echo -e "\033[1;31m\$MSG\033[0m" > "\$term"
        echo -e "\033[1;31m*****************************************************\033[0m\n" > "\$term"
    fi
done

# C. Email Alert (For Admin)
echo "Fail2Ban Notification:
Time: \$TIMESTAMP
Server: \$HOSTNAME
Alert: \$MSG
Action: The IP \$IP has triggered the 5-attempt limit.
" | mail -s "[Fail2Ban] SSH Alert from \$HOSTNAME" "\$TARGET_EMAIL"

EOF

chmod 755 "$SCRIPT_PATH"

echo ">> Creating Fail2Ban action definition..."
cat > /etc/fail2ban/action.d/notify-only.conf <<EOF
[Definition]
actionstart =
actionstop =
actioncheck =
actionban = /usr/local/bin/ssh-alert.sh <ip>
actionunban =
EOF

echo ">> Configuring jail.local..."
cat > /etc/fail2ban/jail.local <<EOF
[sshd]
enabled = true
port = ssh
backend = systemd
journalmatch = _SYSTEMD_UNIT=ssh.service + _COMM=sshd + _COMM=sshd-session

mode = aggressive

maxretry = 5
findtime = 60
bantime = 600

action = notify-only

EOF

echo ">> Restarting Fail2Ban..."
systemctl restart fail2ban

if systemctl is-active --quiet fail2ban; then
    echo "SUCCESS: Fail2Ban is running."
else
    echo "ERROR: Fail2Ban failed to start. Checking logs..."
    journalctl -u fail2ban -n 20 --no-pager
    exit 1
fi

echo "=== FAIL2BAN APPLIED ==="
