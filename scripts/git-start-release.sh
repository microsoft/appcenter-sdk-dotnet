#! /bin/bash

branch_name="release/$SDK_NEW_VERSION"
git fetch origin
git push origin HEAD:$branch_name
echo "git push origin HEAD:$branch_name"
git fetch origin
git checkout --track -b $branch_name origin/$branch_name
git pull
echo "git commit $commit_message"
commit_message="Start new $NEW_SDK_VERSION release"
git add . -A
git commit -m "$commit_message"
git push