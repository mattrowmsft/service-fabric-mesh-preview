#!/bin/sh 
DOCKER_REPO=$1
WEB_TAG=$2
WORKER_TAG=$3
echo docker tag azure-mesh-fireworks-web:dev-alpine $DOCKER_REPO/azure-mesh-fireworks-web:$WEB_TAG-alpine
docker tag azure-mesh-fireworks-web:dev-alpine $DOCKER_REPO/azure-mesh-fireworks-web:$WEB_TAG-alpine
echo docker push $DOCKER_REPO/azure-mesh-fireworks-web:$WEB_TAG-alpine
docker push $DOCKER_REPO/azure-mesh-fireworks-web:$WEB_TAG-alpine
echo docker tag azure-mesh-fireworks-worker-v1:dev-stretch $DOCKER_REPO/azure-mesh-fireworks-worker-v1:$WORKER_TAG-stretch
docker tag azure-mesh-fireworks-worker-v1:dev-stretch $DOCKER_REPO/azure-mesh-fireworks-worker-v1:$WORKER_TAG-stretch
echo docker push $DOCKER_REPO/azure-mesh-fireworks-worker-v1:$WORKER_TAG-stretch
docker push $DOCKER_REPO/azure-mesh-fireworks-worker-v1:$WORKER_TAG-stretch
echo docker tag azure-mesh-fireworks-worker-v2:dev-stretch $DOCKER_REPO/azure-mesh-fireworks-worker-v2:$WORKER_TAG-stretch
docker tag azure-mesh-fireworks-worker-v2:dev-stretch $DOCKER_REPO/azure-mesh-fireworks-worker-v2:$WORKER_TAG-stretch
echo docker push $DOCKER_REPO/azure-mesh-fireworks-worker-v2:$WORKER_TAG-stretch
docker push $DOCKER_REPO/azure-mesh-fireworks-worker-v2:$WORKER_TAG-stretch
