# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Note: This script is used to skip a stong-naming check for Puppet apps.

# This script uses the VS140COMNTOOLS environment variable which contains path 
# to the Visual Studio Tools. By default, the path should be like this 
# "C:\Program Files (x86)\Microsoft Visual Studio [version]\Common7\Tools\" 
# and should contains VsDevCmd.bat file.
# If this VS140COMNTOOLS environment variable doesn't contains this file 
# you should replace this path with the right Visual Studio Tools folder 
# or some other path where places VsDevCmd.bat file.  

# Usage: run as Admin .\strong-named-sign-delay.ps1 delaySign [outputDir]
# Where assemblePath - path to assemble (ex. bin\Release\name.dll)

param ($assemblePath) 

Write-Host Start to signing $assemblePath assemble.
cmd "$($Env:VS140COMNTOOLS)VsDevCmd.bat&sn -Vr $assemblePath"
if (-not $?)
{
    Write-Host Something went wrong during sign assemble.
}
