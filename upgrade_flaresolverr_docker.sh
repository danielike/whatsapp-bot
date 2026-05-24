#!/bin/bash
docker pull ghcr.io/flaresolverr/flaresolverr:latest
docker container stop flaresolverr
docker container remove flaresolverr
docker run -d --name=flaresolverr -p 8191:8191 -e LOG_LEVEL=info --restart unless-stopped ghcr.io/flaresolverr/flaresolverr:latest
