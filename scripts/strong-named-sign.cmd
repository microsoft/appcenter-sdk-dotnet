:: Copyright (c) Microsoft Corporation. All rights reserved.
:: Licensed under the MIT License.

:: This script is used for the strong-name signing of our assemblies.

:: Usage in Admin mode: 
::   .\strong-named-sign.cmd [pathToAssemblies] [privateKey]
:: Where: 
::   [pathToAssemblies] - path to assemblies.
::   [privateKey] - private key for strong-name sign.

setlocal
set pathToAssemblies=%1
set privateKey=%2

call "%VSAPPIDDIR%..\Tools\VsDevCmd.bat"

FOR /D %%d IN (%pathToAssemblies%\*) DO pushd %%d & (FOR %%z IN (*.dll) DO call :signAssemble %%d\%%z) & popd
EXIT /B

:signAssemble
echo "Start processing the file: %~1"
if exist %~1 (
    sn -R "%~1" %privateKey%
) else (
    echo "File %~1 doesn't exist".
)
