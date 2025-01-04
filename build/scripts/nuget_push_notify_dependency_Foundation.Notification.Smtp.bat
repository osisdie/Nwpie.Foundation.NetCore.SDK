@echo off

IF EXIST "*.nupkg" DEL "*.nupkg"

REM pack
dotnet.exe clean "../../src/Notification/Smtp/Nwpie.Foundation.Notification.Smtp.csproj" -c Release
dotnet.exe pack "../../src/Notification/Smtp/Nwpie.Foundation.Notification.Smtp.csproj" -c Release -o %cd%

REM push
%NUGET_HOME%/nuget.exe push -Source "nwpie.nuget" -ApiKey Nwpie "*.nupkg"

DEL *.nupkg

timeout 5