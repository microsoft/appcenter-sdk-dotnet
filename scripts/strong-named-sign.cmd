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

if not exist %pathToAssemblies% (
    echo Error: The path to assemblies does not exist: %pathToAssemblies%
    exit /b 1
)

set programFilesVsDevCmdDir=%PROGRAMFILES%\Microsoft Visual Studio\%VSVERSION%\Enterprise\Common7\Tools\VsDevCmd.bat
set programFilesVsDevCmdDir86=%PROGRAMFILES(x86)%\Microsoft Visual Studio\%VSVERSION%\Enterprise\Common7\Tools\VsDevCmd.bat
 
if exist %programFilesVsDevCmdDir86% (
    set VSDEVCMDDIR=%programFilesVsDevCmdDir86%
)
 
if exist %programFilesVsDevCmdDir% (
    set VSDEVCMDDIR=%programFilesVsDevCmdDir%
)

if not defined VSDEVCMDDIR (
    echo Error: VsDevCmd.bat not found for version %VSVERSION%.
    exit /b 1
)

echo "Start VS Dev Command CMD from path: %VSDEVCMDDIR%"

call "%VSDEVCMDDIR%"
if errorlevel 1 (
    echo Error: Failed to execute VsDevCmd.bat
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