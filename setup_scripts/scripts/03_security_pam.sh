#!/bin/bash
source ./config.env

echo "=== CONFIGURING PAM SECURITY ==="

# 1. PASSWORD COMPLEXITY
# ------------------------------
echo ">> Applying Password Complexity..."
cat > /etc/security/pwquality.conf <<EOF
minlen = 12
dcredit = -1
ucredit = -1
lcredit = -1
ocredit = -1
enforce_for_root = 0
EOF

# Only insert into common-password if it's missing
if ! grep -q "pam_pwquality.so" /etc/pam.d/common-password; then
    # Insert at line 1 (standard for Debian)
    sed -i '1ipassword requisite pam_pwquality.so retry=3' /etc/pam.d/common-password
else
    echo "   Complexity already configured."
fi

# 2. ACCOUNT LOCKOUT
# --------------------------
echo ">> Applying Account Lockout (5 attempts)..."
AUTH_FILE="/etc/pam.d/common-auth"

# We check if pam_faillock is ALREADY there. If yes, we touch nothing.
if grep -q "pam_faillock.so" "$AUTH_FILE"; then
    echo "   Lockout already configured. Skipping to prevent corruption."
else
    # Safety Check: Ensure pam_unix exists so we know where to inject
    if grep -q "pam_unix.so" "$AUTH_FILE"; then
        echo "   Injecting pam_faillock rules..."
        # 1. Insert Pre-Auth BEFORE pam_unix
        sed -i '/pam_unix.so/i auth required pam_faillock.so preauth silent audit deny=5 unlock_time=0' "$AUTH_FILE"
        # 2. Insert Auth-Fail AFTER pam_unix
        sed -i '/pam_unix.so/a auth [default=die] pam_faillock.so authfail audit deny=5 unlock_time=0' "$AUTH_FILE"
	# 3. Guarantee the success has 2 skips instead of 1.
	sed -i 's/success=1 default=ignore/success=2 default=ignore/' "$AUTH_FILE"
    else
        echo "ERROR: Could not find pam_unix.so in $AUTH_FILE. Manual check required."
    fi
fi

# 3. TIME LIMITS
# ---------------------
echo ">> Applying Time Limits..."

# A. Configure the File
TIME_FILE="/etc/security/time.conf"
sed -i '/^sshd/d' "$TIME_FILE"

#Commented for testing purposes
#echo "sshd;*;!root;Al0800-2200" >> "$TIME_FILE"
echo "sshd;*;!root;Al0000-2400" >> "$TIME_FILE"

# B. Enable the Module in PAM
if ! grep -q "pam_time.so" /etc/pam.d/sshd; then
    echo "account required pam_time.so" >> /etc/pam.d/sshd
else
    echo "   Time module already enabled."
fi

echo "=== PAM SECURITY APPLIED ==="
