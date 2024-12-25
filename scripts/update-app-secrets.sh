#!/bin/bash
# Replace app-secret's and tokens values in test MAUI app.

# Usage:
# ./scripts/update-app-secrets.sh

# Constants for iOS and Android.
declare -a platformConstants=(
    "MAUI_ANDROID_PROD"
    "MAUI_ANDROID_TARGET_TOKEN_PROD"
    "MAUI_IOS_PROD"
    "MAUI_IOS_TARGET_TOKEN_PROD")

# File which should be changed.
targetFile=("Apps/Contoso.MAUI.Demo/App.xaml.cs")

echo "Insert secrets for MAUI app."

# Update files from array.
for constant in "${platformConstants[@]}"; do
    # Replace secret value from enviroment variables.
    sed -i '' "s/{$constant}/"${!constant}"/g" $targetFile
done
