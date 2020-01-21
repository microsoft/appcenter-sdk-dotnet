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
BASE_BRANCH_NAME$3
GITHUB_ACCESS_TOKEN=$4

if [ -z "${GITHUB_REPO_OWNER}" ]; then
    GITHUB_REPO_OWNER="microsoft"
fi
GITHUB_REPO_NAME="appcenter-sdk-dotnet"
REQUEST_URL_PULL="https://api.github.com/repos/$GITHUB_REPO_OWNER/$GITHUB_REPO_NAME/pulls?access_token=$GITHUB_ACCESS_TOKEN"

resp="$(curl -s -X POST $REQUEST_URL_PULL -d '{
      "title": "'${PULL_REQUEST_TITLE}'",
      "head": "'${HEAD_BRANCH_NAME}'",
      "base": "'${BASE_BRANCH_NAME}'"
    }')"
url="$(echo $resp | jq -r '.url')"

if [ -z $url ] || [ "$url" == "" ] || [ "$url" == "null" ]; then
    echo "Cannot create a pull request"
    echo "Response:" $url
    exit 1
else
    echo "A pull request has been created at $url"
fi

