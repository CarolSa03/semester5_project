#!/bin/bash
source ./config.env

echo "=== CONFIGURING SSH SECURITY ==="

SSH_CFG="/etc/ssh/sshd_config"
PAM_SSHD="/etc/pam.d/sshd"

# --- PART A: SSHD CONFIG ---
# Usage: safe_config "KeyName" "FullReplacementLine"
safe_config() {
    local key=$1
    local replacement=$2
    
    # LOGIC FIX:
    # 1. Search for: Start of line (^), Optional Hashes (#*), Optional Spaces ( *), then the Key
    if grep -qE "^#* *${key}" "$SSH_CFG"; then
        # 2. Sed Fix: We use ^#* to match commented lines correctly.
        #    We replace the ENTIRE matching line with the replacement.
        sed -i "s|^#* *${key}.*|${replacement}|" "$SSH_CFG"
    else
        # 3. If not found, append to end
        echo "$replacement" >> "$SSH_CFG"
    fi
}

echo ">> Hardening sshd_config..."

# 1. Enable Public Key (Pass only the Key Name, then the Full Line)
safe_config "PubkeyAuthentication" "PubkeyAuthentication yes"

# 2. Enable Root Login
safe_config "PermitRootLogin" "PermitRootLogin yes"

# 3. Enable PAM & Challenge Response
safe_config "UsePAM" "UsePAM yes"
safe_config "ChallengeResponseAuthentication" "ChallengeResponseAuthentication yes"

# 4. CRITICAL FIXES (The Loop Breakers)
safe_config "KbdInteractiveAuthentication" "KbdInteractiveAuthentication yes"
safe_config "PasswordAuthentication" "PasswordAuthentication no"


# --- PART B: PAM SSHD ---
echo ">> Configuring PAM for SSH..."

# 1. Clean up OLD lines to prevent duplicates
sed -i '/pam_google_authenticator.so/d' "$PAM_SSHD"
sed -i '/pam_succeed_if.so uid eq 0/d' "$PAM_SSHD"
sed -i '/pam_faildelay.so/d' "$PAM_SSHD"

# 2. Re-add lines in correct order (Root Bypass -> MFA)
echo "auth [success=2 default=ignore] pam_succeed_if.so uid eq 0" >> "$PAM_SSHD"
echo "auth optional pam_faildelay.so delay=10000000" >> "$PAM_SSHD"
echo "auth required pam_google_authenticator.so nullok" >> "$PAM_SSHD"

# 3. Ensure Time Limits are enabled
if ! grep -q "pam_time.so" "$PAM_SSHD"; then
    echo "account required pam_time.so" >> "$PAM_SSHD"
fi

# --- PART C: RESTART & VERIFY ---
echo ">> Restarting SSH..."
systemctl restart ssh

echo "=== SSH SECURITY APPLIED ==="
