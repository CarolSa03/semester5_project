#!/bin/bash
source ./config.env
apt-get update
apt-get install -y docker-compose-plugin rsync acl libpam-pwquality libpam-google-authenticator fail2ban bsd-mailx curl tar jq
