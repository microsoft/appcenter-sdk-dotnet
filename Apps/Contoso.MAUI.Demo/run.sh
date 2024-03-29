#!/usr/bin/env bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

echo running with app secret: $MAUI_ANDROID_PROD

MAUI_ANDROID_PROD=$MAUI_ANDROID_PROD dotnet build -t:Run -f net7.0-$@