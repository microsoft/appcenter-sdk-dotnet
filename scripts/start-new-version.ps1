# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Note: Run this from within the root directory

[CmdletBinding()]
Param(
    [string]$Version
)

.\build.ps1 "version.cake" --Target="StartNewVersion" --NewVersion="$Version"
