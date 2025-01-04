@echo off

IF EXIST "*.nupkg" DEL "*.nupkg"

REM pack
dotnet.exe clean "../../src/Caching/LocalCache/Nwpie.Foundation.Caching.LocalCache.csproj" -c Release
dotnet.exe pack "../../src/Caching/LocalCache/Nwpie.Foundation.Caching.LocalCache.csproj" -c Release -o %cd%

REM push
%NUGET_HOME%/nuget.exe push -Source "nwpie.nuget" -ApiKey Nwpie "*.nupkg"

DEL *.nupkg

timeout 5