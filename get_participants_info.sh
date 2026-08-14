#!/usr/bin/env bash
set -euo pipefail

GROUP_ID="120363428730667729"

URL="http://localhost:8082/group/participants/Bot?groupJid=$GROUP_ID"

API_KEY="BQYHJGJHJ"

OUT="participants_info.txt"

# 3) Fetch JSON, extract id and subject, one "record" per line:
#    - If there are multiple objects in the top-level array, each becomes a new line
#    - Each line is: "<id> => <admin>"
curl -sS "$URL" -X GET -H "apikey: $API_KEY" | jq -r '.participants[] | "\(.id) => \(.admin)"' > "$OUT"

echo "Wrote: $OUT"
