#!/usr/bin/env bash
set -euo pipefail

URL="http://localhost:8082/group/fetchAllGroups/Bot?getParticipants=false"

API_KEY="BQYHJGJHJ"

OUT="group_info.txt"

# 3) Fetch JSON, extract id and subject, one "record" per line:
#    - If there are multiple objects in the top-level array, each becomes a new line
#    - Each line is: "<id> => <subject>"
curl -sS "$URL" -X GET -H "apikey: $API_KEY" | jq -r '.[] | "\(.id) => \(.subject)"' > "$OUT"

echo "Wrote: $OUT"
