# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
pushd $APPCENTER_SOURCE_DIRECTORY
nugetFileName="../../../NuGet.config"
contentValue="<?xml version=\""1.0\"" encoding=\""utf-8\""?>
 <configuration>
   <packageSources>
     <add key=\""nuget\"" value=\""https://api.nuget.org/v3/index.json\"" />
     <add key=\""VSTSAC\"" value=\""https://msmobilecenter.pkgs.visualstudio.com/_packaging/AppCenter/nuget/v3/index.json\"" />
   </packageSources>
   <activePackageSource>
     <add key=\""All\"" value=\""\('Aggregate source'\)\"" />
   </activePackageSource>
   <packageSourceCredentials>
     <VSTSAC>
       <add key=\""Username\"" value=\""mobilecenter\"" />
       <add key=\""ClearTextPassword\"" value=\""$NUGET_PASSWORD\"" />
     </VSTSAC>
   </packageSourceCredentials>
 </configuration>"

if [ -e $nugetFileName ]; then
    rm $nugetFileName
fi
echo $contentValue >> $nugetFileName
./scripts/update-app-secrets.sh PROD
./build.sh -t=externals-ios
popd
