#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Note: Run this from within the root directory

version=$1

./build.sh "version.cake" --Target="StartNewVersion" --NewVersion="$version"
