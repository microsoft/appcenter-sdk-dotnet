#!/bin/bash
# Replace app-secret's and tokens values in test apps. Update Puppet apps by default.

# Usage:
# ./scripts/update-app-secrets.sh <INT|PROD>

# Dynamic constants.
appPrefix="INT"

# Prepare constants.
if [ "$1" == "PROD" ]; then
  appPrefix="PROD"
fi

# Constants for iOS and Android.
declare -a platformConstants=(
    "XAMARIN_FORMS_ANDROID_${appPrefix}"
    "XAMARIN_FORMS_ANDROID_TARGET_TOKEN_${appPrefix}"
    "XAMARIN_FORMS_IOS_${appPrefix}"
    "XAMARIN_FORMS_IOS_TARGET_TOKEN_${appPrefix}"
    "XAMARIN_FORMS_MACOS_${appPrefix}"
    "XAMARIN_ANDROID_${appPrefix}"
    "XAMARIN_IOS_${appPrefix}"
    "XAMARIN_MACOS_${appPrefix}")

# Files which should be changed.
declare -a targetFiles=("Apps/Contoso.Android.Puppet/Contoso.Android.Puppet.csproj" 
    "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.Droid/Contoso.Forms.Demo.Droid.csproj"
    "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.MacOS/Contoso.Forms.Demo.MacOS.csproj"
    "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.iOS/Contoso.Forms.Demo.iOS.csproj"
    "Apps/Contoso.Forms.Puppet/Contoso.Forms.Puppet.Droid/Contoso.Forms.Puppet.Droid.csproj"
    "Apps/Contoso.Forms.Puppet/Contoso.Forms.Puppet.MacOS/Contoso.Forms.Puppet.MacOS.csproj"
    "Apps/Contoso.Forms.Puppet/Contoso.Forms.Puppet.iOS/Contoso.Forms.Puppet.iOS.csproj"
    "Apps/Contoso.MacOS.Puppet/Contoso.MacOS.Puppet.csproj"
    "Apps/Contoso.iOS.Puppet/Contoso.iOS.Puppet.csproj")

# Print info about current job.
echo "Insert secrets for $appPrefix apps."

# Update files from array.
for constant in "${platformConstants[@]}"
do
    for file in "${targetFiles[@]}"
    do
        # Replace secret value from enviroment variables.
        sed -i '' "s/{$constant}/"${!constant}"/g" $file
    done
done
