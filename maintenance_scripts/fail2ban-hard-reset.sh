# Aggressive way to reset bans on fail2ban
systemctl stop fail2ban
rm -f /var/lib/fail2ban/fail2ban.sqlite3
systemctl start fail2ban
