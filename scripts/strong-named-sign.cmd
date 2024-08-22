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

for /f "delims=" %%i in ('powershell -Command "Get-ChildItem -Path C:\ -Recurse -Directory -Filter 'Microsoft Visual Studio' -ErrorAction SilentlyContinue -Force | Select-Object -First 1 -ExpandProperty FullName"') do set vsPath=%%i

if not defined vsPath (
    echo Error: Could not find Microsoft Visual Studio directory.
    exit /b 1
)

set VSDEVCMDDIR=%vsPath%\%VSVERSION%\Enterprise\Common7\IDE\Tools\VsDevCmd.bat

if not exist "%VSDEVCMDDIR%" (
    echo Error: VsDevCmd.bat not found at %VSDEVCMDDIR%
    exit /b 1
)

call "%VSDEVCMDDIR%"
if errorlevel 1 (
    echo Error: Failed to execute VsDevCmd.bat
    exit /b 1
)

if not exist "%pathToAssemblies%" (
    echo Error: The path to assemblies does not exist: %pathToAssemblies%
    exit /b 1
)

FOR /D %%d IN (%pathToAssemblies%\*) DO (
    pushd %%d
    (FOR %%z IN (*.dll) DO call :signAssemble %%d\%%z)
    popd
)

exit /b

:signAssemble
echo "Start processing the file: %~1"
if exist %~1 (
    sn -R "%~1" %privateKey%
    if errorlevel 1 (
        echo Error: Failed to sign %~1
        exit /b 1
    )
) else (
    echo "File %~1 doesn't exist."
    exit /b 1
)
