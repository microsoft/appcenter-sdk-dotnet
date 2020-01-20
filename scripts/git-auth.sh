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

git config --global url."https://api:$github_access_token@github.com/".insteadOf "https://github.com/"
git config --global url."https://ssh:$github_access_token@github.com/".insteadOf "ssh://git@github.com/"
git config --global url."https://git:$github_access_token@github.com/".insteadOf "git@github.com:"
git config --global user.email "appcentersdk@microsoft.com"
git config --global user.name "App Center"