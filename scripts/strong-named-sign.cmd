:: Copyright (c) Microsoft Corporation. All rights reserved.
:: Licensed under the MIT License.

:: This script is used for the strong-name signing of our assemblies.

:: This script uses the VS140COMNTOOLS environment variable which contains path 
:: to the Visual Studio Tools. By default, the path should be like this 
:: "C:\Program Files (x86)\Microsoft Visual Studio [version]\Common7\Tools\" 
:: and should contain VsDevCmd.bat file.

:: If this VS140COMNTOOLS environment variable doesn't contains this file 
:: you should replace this path with the right Visual Studio Tools folder 
:: or some other path containing the VsDevCmd.bat file.  

:: Usage in Admin mode: 
::   .\strong-named-sign.cmd [pathToAssemblies] [privateKey]
:: Where: 
::   [pathToAssemblies] - path to assemblies.
::   [privateKey] - private key for strong-name sign.

setlocal
set pathToAssemblies=%1
set privateKey=%2

call "%VS140COMNTOOLS%VsDevCmd.bat"
FOR /D %%d IN (%pathToAssemblies%\*) DO pushd %%d & (FOR %%z IN (*.dll) DO call :signAssemble %%d\%%z) & popd
EXIT /B

:signAssemble
echo "Start processing the file: %~1"
if exist %~1 (
    sn -R "%~1" %privateKey%
) else (
    echo "File %~1 doesn't exist".
)
