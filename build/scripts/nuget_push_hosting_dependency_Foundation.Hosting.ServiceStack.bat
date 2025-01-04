@echo off

IF EXIST "*.nupkg" DEL "*.nupkg"

REM pack
dotnet.exe clean "../../src/Hosting/ServiceStack/Nwpie.Foundation.Hosting.ServiceStack.csproj" -c Release
dotnet.exe pack "../../src/Hosting/ServiceStack/Nwpie.Foundation.Hosting.ServiceStack.csproj" -c Release -o %cd%

REM push
%NUGET_HOME%/nuget.exe push -Source "nwpie.nuget" -ApiKey Nwpie "*.nupkg"

DEL *.nupkg

timeout 5