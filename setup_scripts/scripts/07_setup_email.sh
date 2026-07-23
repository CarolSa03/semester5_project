#!/bin/bash
source ./config.env

echo "=== CONFIGURING EMAIL (POSTFIX) ==="

# Prevent interactive popups
export DEBIAN_FRONTEND=noninteractive

# 1. Install
apt-get update
apt-get install -y postfix mailutils libsasl2-modules

# 2. Configure Postfix Global Settings
postconf -e "relayhost = $EMAIL_RELAY_HOST"
postconf -e "smtp_sasl_auth_enable = yes"
postconf -e "smtp_sasl_security_options = noanonymous"
postconf -e "smtp_sasl_password_maps = hash:/etc/postfix/sasl_passwd"
postconf -e "smtp_use_tls = yes"
postconf -e "smtp_tls_security_level = encrypt"
postconf -e "smtp_tls_CAfile = /etc/ssl/certs/ca-certificates.crt"

# 3. Credentials
echo ">> Setting credentials..."
# Note: We use > to overwrite, ensuring no duplicates
echo "$EMAIL_RELAY_HOST    $EMAIL_USERNAME:$EMAIL_APP_PASSWORD" > /etc/postfix/sasl_passwd

# 4. Security & Hash
chmod 600 /etc/postfix/sasl_passwd
postmap /etc/postfix/sasl_passwd

# 5. Restart
systemctl restart postfix

echo "=== EMAIL SETUP COMPLETE ==="
