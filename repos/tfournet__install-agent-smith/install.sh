#!/usr/bin/env bash
#
# install.sh — one-file installer for RewstApp/agent-smith-go
#
# Usage:
#   curl -sSL https://raw.githubusercontent.com/<you>/<repo>/main/install.sh \
#     | bash -s -- <ORG_ID> <SECRET>
#

set -euo pipefail

if [ "$#" -ne 2 ]; then
  cat >&2 <<EOF
Usage: curl …/install.sh | bash -s -- <ORG_ID> <SECRET>

Or, locally:
  bash install.sh ACME my_secret
EOF
  exit 1
fi

ORG_ID="$1"
SECRET="$2"

# 1) detect platform & extension
unameOut="$(uname -s | tr '[:upper:]' '[:lower:]')"
case "$unameOut" in
  *mingw*|*msys*|*cygwin*)
    PLATFORM="win"
    EXT="exe"
    ;;
  *darwin*)
    PLATFORM="macos"
    EXT="bin"
    ;;
  *)
    PLATFORM="linux"
    EXT="bin"
    ;;
esac

# 2) construct asset name & URL
ASSET="rewst_agent_config.${PLATFORM}.${EXT}"
URL="https://github.com/RewstApp/agent-smith-go/releases/latest/download/${ASSET}"

# 3) download, make executable, and run
TMP="$(mktemp)"
trap 'rm -f "$TMP"' EXIT

echo "👉 Downloading ${ASSET} from GitHub…"
curl -sSL "$URL" -o "$TMP"

echo "👉 Running installer…"
chmod +x "$TMP"
exec "$TMP" --org_id="$ORG_ID" --config-secret="$SECRET"