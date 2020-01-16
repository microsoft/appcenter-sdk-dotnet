#!/bin/bash

GITHUB_API_URL_TEMPLATE="https://api.github.com/repos/annakocheshkova/appcenter-sdk-dotnet/%s?access_token=%s%s"
REQUEST_URL_PULL="$(printf $GITHUB_API_URL_TEMPLATE 'pulls' $GithubToken)"

resp="$(curl -s -X POST $REQUEST_URL_PULL -d '{
      "title": "Start new '${SDK_NEW_VERSION}' version",
      "body": "Start new '${SDK_NEW_VERSION}' version",
      "head": "test-branch",
      "base": "master"
    }')"
url="$(echo $resp | jq -r '.url')"

if [ -z $url ] || [ "$url" == "" ] || [ "$url" == "null" ]; then
    echo "Cannot create a pull request"
    echo "Response:" $url
else
    echo "A pull request has been created at $url"
fi

exit 1
