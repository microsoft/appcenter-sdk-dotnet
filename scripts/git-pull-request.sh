#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

help() {
  echo "Usage: $0 <title> <head-branch> <base-branch> <github-access-token>"
}

if [ -z $1 ]; then
  help
  exit 1
fi

PULL_REQUEST_TITLE=$1
HEAD_BRANCH_NAME=$2
BASE_BRANCH_NAME=$3
GITHUB_ACCESS_TOKEN=$4

if [ -z "${GITHUB_REPO_OWNER}" ]; then
    GITHUB_REPO_OWNER="microsoft"
fi
GITHUB_REPO_NAME="appcenter-sdk-dotnet"
GITHUB_API_URL="https://api.github.com/repos/$GITHUB_REPO_OWNER/$GITHUB_REPO_NAME"

# Create pull request
CREATE_PULL_REQUEST_URL="$GITHUB_API_URL/pulls?access_token=$GITHUB_ACCESS_TOKEN"
resp="$(curl -s -X POST $CREATE_PULL_REQUEST_URL -d '{
      "title": "'${PULL_REQUEST_TITLE}'",
      "head": "'${HEAD_BRANCH_NAME}'",
      "base": "'${BASE_BRANCH_NAME}'"
    }')"
url="$(echo $resp | jq -r '.url')"

if [ -z $url ] || [ "$url" == "" ] || [ "$url" == "null" ]; then
    echo "Cannot create a pull request"
    echo "Response:" $resp
    exit 1
else
    echo "A pull request has been created at $url"
fi

