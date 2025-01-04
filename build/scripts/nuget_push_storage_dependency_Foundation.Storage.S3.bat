@echo off

IF EXIST "*.nupkg" DEL "*.nupkg"

REM pack
dotnet.exe clean "../../src/Storage/S3/Nwpie.Foundation.Storage.S3.csproj" -c Release
dotnet.exe pack "../../src/Storage/S3/Nwpie.Foundation.Storage.S3.csproj" -c Release -o %cd%

REM push
%NUGET_HOME%/nuget.exe push -Source "nwpie.nuget" -ApiKey Nwpie "*.nupkg"

DEL *.nupkg

timeout 5