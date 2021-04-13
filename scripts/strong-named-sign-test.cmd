:: Copyright (c) Microsoft Corporation. All rights reserved.
:: Licensed under the MIT License.

:: This script is used for test-sign or partially signed assemblies with the private key.

:: This script uses the VS140COMNTOOLS environment variable which contains path 
:: to the Visual Studio Tools. By default, the path should be like this 
:: "C:\Program Files (x86)\Microsoft Visual Studio [version]\Common7\Tools\" 
:: and should contain VsDevCmd.bat file.

:: If this VS140COMNTOOLS environment variable doesn't contains this file 
:: you should replace this path with the right Visual Studio Tools folder 
:: or some other path containing the VsDevCmd.bat file.  

:: The PATH_TO_PRIVATE_KEY contains path to the private key file.

:: Usage: 
::   .\strong-named-sign-test.cmd [pathToFile]
:: Where: 
::   [pathToFile] - path to assemblies (ex. bin\Release\).

setlocal
set pathToFile=%1

call "%VS140COMNTOOLS%VsDevCmd.bat"
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Crashes.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Test.Windows.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Test.WindowsDesktop.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Crashes.Test.Windows.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.NET.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Analytics.NET.dll
call :testSignAssemble %pathToFile%Microsoft.AppCenter.Analytics.Test.Windows.dll
EXIT /B

:testSignAssemble
echo "Start processing the file: %~1"
if exist %~1 (
    sn -TS "%~1" "%PATH_TO_PRIVATE_KEY%"
) else (
    echo "File %~1 doesn't exist".
)
