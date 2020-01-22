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

GITHUB_ACCESS_TOKEN=$1

if [ -z "${GITHUB_REPO_OWNER}" ]; then
    GITHUB_REPO_OWNER="microsoft"
fi
GITHUB_REPO_NAME="appcenter-sdk-dotnet"
GITHUB_API_URL="https://api.github.com/repos/$GITHUB_REPO_OWNER/$GITHUB_REPO_NAME"

# Get opened milestone
GET_MILESTONES_URL="$GITHUB_API_URL/milestones?state=open"
MILESTONE_TITLE=$(date +%B)
echo "Getting opened milestone..."
echo $GET_MILESTONES_URL
resp="$(curl -s -X GET $GET_MILESTONES_URL)"
MILESTONE_NUMBER="$(echo $resp | jq '.[] | select(.title=='\"$MILESTONE_TITLE\"') | .number')" || echo $resp >&2
if [ -z $MILESTONE_NUMBER ] || [ "$MILESTONE_NUMBER" == "" ]; then
    echo "There is no opened milestone"
    exit
else
    echo "A milestone $MILESTONE_NUMBER has been fetched"
fi

# Get opened issues
GET_ISSUES_URL="$GITHUB_API_URL/issues?milestone=$MILESTONE_NUMBER&state=open"
echo "Getting opened issues..."
echo $GET_ISSUES_URL
resp="$(curl -s -X GET $GET_ISSUES_URL)"
OPENED_ISSUES=$(echo $resp | jq '.[] | .number') || echo $resp >&2
if [[ -z $OPENED_ISSUES ]] || [[ ${#OPENED_ISSUES[@]} -eq 0 ]]; then
    echo "There are no opened issues"
    exit
else
    echo "Opened issues:"
    echo $OPENED_ISSUES
fi

# Comment and close issues
for ISSUE_NUMBER in $OPENED_ISSUES; do
    POST_COMMENT_URL="$GITHUB_API_URL/issues/$ISSUE_NUMBER/comments?access_token=$GITHUB_ACCESS_TOKEN"
    read -d '' POST_COMMENT_MESSAGE << EOF
Hi there!
We've included fixes in the latest SDK release for this. Closing the issue but please reopen if you run into this again.
EOF
    echo "Adding a comment to $ISSUE_NUMBER issue..."
    echo $POST_COMMENT_URL
    body="$(jq -n --arg body "$POST_COMMENT_MESSAGE" '{ body: $body }')"
    curl -fsS -o /dev/null -X POST $POST_COMMENT_URL -d "$body" || exit

    CLOSE_ISSUE_URL="$GITHUB_API_URL/issues/$ISSUE_NUMBER?access_token=$GITHUB_ACCESS_TOKEN"
    echo "Closing $ISSUE_NUMBER issue..."
    echo $CLOSE_ISSUE_URL
    curl -fsS -o /dev/null -X PATCH $CLOSE_ISSUE_URL -d "{ \"state\": \"closed\" }" || exit
done

# Close milestone
CLOSE_MILESTONE_URL="$GITHUB_API_URL/milestones/$MILESTONE_NUMBER?access_token=$GITHUB_ACCESS_TOKEN"
echo "Closing $MILESTONE_NUMBER milestone..."
echo $CLOSE_MILESTONE_URL
curl -fsS -o /dev/null -X PATCH $CLOSE_MILESTONE_URL -d "{ \"state\": \"closed\" }" || exit