#================================================================
# About API
#================================================================
# - Document
#   - Development   https://api-dev.kevinw.net/s3proxy/metadata
#   - Production    https://api.kevinw.net/s3proxy/metadata
# - HealthCheck
#   - Development   https://api-dev.kevinw.net/s3proxy/health
#   - Production    https://api.kevinw.net/s3proxy/health

#================================================================
# About Dockerbuild
#================================================================
# export VERSION=xxx
# export ACCOUNT=xxx
# export NUGET_USERNAME=xxx
# export NUGET_PAT=xxx

#!/bin/bash
if [ -z ${VERSION+x} ]; then echo "VERSION is unset" && exit 1; else echo "VERSION is set to '$VERSION'"; fi
if [ -z ${ACCOUNT+x} ]; then echo "ACCOUNT is unset" && exit 1; else echo "ACCOUNT is set to '$ACCOUNT'"; fi
if [ -z ${NUGET_USERNAME+x} ]; then echo "NUGET_USERNAME is unset" && exit 1; else echo "NUGET_USERNAME is set to 'NUGET_USERNAME'"; fi
if [ -z ${NUGET_PAT+x} ]; then echo "NUGET_PAT is unset" && exit 1; else echo "NUGET_PAT is set to '$NUGET_PAT'"; fi

ECR_HOST="${ACCOUNT}.dkr.ecr.us-west-2.amazonaws.com"
ECR_REPO=s3proxy
CONFIGURATION=${CONFIGURATION:-Release}
VERSION=${VERSION:-1.0.0.0}
NUGET_ENDPOINT=${NUGET_ENDPOINT:-"https://pkgs.dev.azure.com/creativeark/_packaging/nwpie.nuget/nuget/v3/index.json"}
ECR_HOST_WITH_TAG=${ECR_HOST}/${ECR_REPO}:${VERSION}
ECR_HOST_WITH_LATEST_TAG=${ECR_HOST}/${ECR_REPO}:latest

for i in {\
ACCOUNT,\
ECR_HOST,ECR_REPO,CONFIGURATION,VERSION,\
NUGET_USERNAME,NUGET_ENDPOINT,NUGET_PAT,\
ECR_HOST_WITH_TAG,ECR_HOST_WITH_LATEST_TAG\
}; do
  echo "$i = ${!i}"
done

docker build \
  --build-arg CONFIGURATION=$CONFIGURATION \
  --build-arg VERSION=$VERSION \
  --build-arg NUGET_USERNAME=${NUGET_USERNAME} \
  --build-arg NUGET_ENDPOINT=$NUGET_ENDPOINT \
  --build-arg NUGET_PAT=${NUGET_PAT} \
  . -t $ECR_HOST_WITH_TAG -t $ECR_HOST_WITH_LATEST_TAG \
  -f docker/S3Proxy/Endpoint/Dockerfile

#================================================================
# AWS Login
#================================================================
# aws ecr get-login --no-include-email --region us-west-2
# echo <your-password> | docker login ${ECR_HOST} -u AWS --password-stdin
# export AWS_PROFILE="default"
echo $(aws ecr get-authorization-token --region us-west-2 --output text --query 'authorizationData[].authorizationToken' | base64 -d | cut -d: -f2) | docker login -u AWS $ECR_HOST --password-stdin

#================================================================
# push image to ECR
#================================================================
docker push $ECR_HOST_WITH_TAG
docker push $ECR_HOST_WITH_LATEST_TAG
