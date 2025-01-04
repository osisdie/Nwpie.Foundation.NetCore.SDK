@echo off

IF EXIST "*.nupkg" DEL "*.nupkg"

REM pack
dotnet.exe clean "../../../src/Logging/Log4net/Nwpie.Foundation.Logging.Log4net.csproj" -c Release
dotnet.exe pack "../../../src/Logging/Log4net/Nwpie.Foundation.Logging.Log4net.csproj" -c Release -o %cd%

REM push
%NUGET_HOME%/nuget.exe push -Source "nwpie.nuget" -ApiKey Nwpie "*.nupkg"

DEL *.nupkg

timeout 5