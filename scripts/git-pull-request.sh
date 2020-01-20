#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

help() {
  echo "Usage: $0 <github-access-token>"
}

if [ -z $1 ]; then
  help
  exit 1
fi

github_access_token=$1

if [ -z "${GITHUB_REPO_OWNER}" ]; then
    GITHUB_REPO_OWNER="microsoft"
fi
GITHUB_REPO_NAME="appcenter-sdk-dotnet"
REQUEST_URL_PULL="https://api.github.com/repos/$GITHUB_REPO_OWNER/$GITHUB_REPO_NAME/pulls?access_token=$github_access_token"

resp="$(curl -s -X POST $REQUEST_URL_PULL -d '{
      "title": "Start new '${NEW_SDK_VERSION}' version",
      "body": "Start new '${NEW_SDK_VERSION}' version",
      "head": "release/'${NEW_SDK_VERSION}'",
      "base": "master"
    }')"
url="$(echo $resp | jq -r '.url')"

if [ -z $url ] || [ "$url" == "" ] || [ "$url" == "null" ]; then
    echo "Cannot create a pull request"
    echo "Response:" $url
    exit 1
else
    echo "A pull request has been created at $url"
fi

