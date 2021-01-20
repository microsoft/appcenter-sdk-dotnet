:: Copyright (c) Microsoft Corporation. All rights reserved.
:: Licensed under the MIT License.

:: Note: This script is used to skip a stong-naming check for Puppet apps.

:: This script uses the VS140COMNTOOLS environment variable which contains path 
:: to the Visual Studio Tools. By default, the path should be like this 
:: "C:\Program Files (x86)\Microsoft Visual Studio [version]\Common7\Tools\" 
:: and should contains VsDevCmd.bat file.

:: If this VS140COMNTOOLS environment variable doesn't contains this file 
:: you should replace this path with the right Visual Studio Tools folder 
:: or some other path where places the VsDevCmd.bat file.  

:: Usage in Admin mode: 
::   .\strong-named-sign-delay.cmd [action] [assembliesPath]
:: Where: 
::   [action] - 'Vr' for register assemblies or 'Vu' for unregister assemblies.
::   [assembliesPath] - path to assemblies (ex. bin\Release\).

setlocal
set action=%1
set pathToFile=%2

call "%VS140COMNTOOLS%VsDevCmd.bat"

call :registerOrUnregisterFiles %pathToFile%Microsoft.AppCenter.dll
call :registerOrUnregisterFiles %pathToFile%Microsoft.AppCenter.Analytics.dll
call :registerOrUnregisterFiles %pathToFile%Microsoft.AppCenter.Crashes.dll
call :registerOrUnregisterFiles %pathToFile%Microsoft.AppCenter.Distribute.dll
EXIT /B

:registerOrUnregisterFiles
echo "Start to processing the file: %~1"
if exist %~1 (
    sn -%action% "%~1"
) else (
    echo "File %~1 doesn't exist".
)
goto:EOF