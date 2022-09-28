:: Copyright (c) Microsoft Corporation. All rights reserved.
:: Licensed under the MIT License.

:: This script is used for test-sign or partially signed assemblies with the private key.

:: The PATH_TO_PRIVATE_KEY contains path to the private key file.

:: Usage: 
::   .\strong-named-sign-test.cmd [pathToFile]
:: Where: 
::   [pathToFile] - path to assemblies (ex. bin\Release\).

setlocal
set pathToFile=%1

call "%VSAPPIDDIR%..\Tools\VsDevCmd.bat"

call :testSignAssemble %pathToFile%Microsoft.AppCenter.Crashes.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Test.Windows.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Test.WindowsDesktop.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Crashes.Test.Windows.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Test.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Analytics.Test.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Analytics.Test.Windows.dll
EXIT /B

:testSignAssemble
echo "Start processing the file: %~1"
if exist %~1 (
    sn -TS "%~1" "%PATH_TO_PRIVATE_KEY%"
) else (
    echo "File %~1 doesn't exist".
)
