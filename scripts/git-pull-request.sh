#!/bin/bash

resp="$(curl -s -X POST -H 'Content-Type: application/json' -H 'Accept: application/vnd.github.v3+json'  -H 'User-Agent: Microsoft' -H 'Authorization: Token $GithubToken'  -d '{
      "title": "'Start new ${SDK_NEW_VERSION} release'",
      "body": "'Start new ${SDK_NEW_VERSION} release'",
      "head": "test-branch",
      "base": "master"
    }' https://api.github.com/repos/annakocheshkova/appcenter-sdk-dotnet/pulls)"

url="$(echo $resp | jq -r '.url')"

if [ -z $url ] || [ "$url" == "" ] || [ "$url" == "null" ]; then
    echo "Cannot create a pull request"
    echo "Response:" $url
else
    echo "A pull request has been created at $url"
fi