#!/bin/sh
echo docker build src -f src/web/linux.Dockerfile -t azure-mesh-fireworks-web:dev-alpine
docker build src -f src/web/linux.Dockerfile -t azure-mesh-fireworks-web:dev-alpine

echo docker build src -f src/worker/linux.Dockerfile -t azure-mesh-fireworks-worker-v1:dev-stretch
docker build src -f src/worker/linux.Dockerfile -t azure-mesh-fireworks-worker-v1:dev-stretch

echo docker build src -f src/worker/linux.v2.Dockerfile -t azure-mesh-fireworks-worker-v2:dev-stretch
docker build src -f src/worker/linux.v2.Dockerfile -t azure-mesh-fireworks-worker-v2:dev-stretch
