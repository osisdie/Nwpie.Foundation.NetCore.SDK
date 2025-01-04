@echo off

IF EXIST "*.nupkg" DEL "*.nupkg"

REM pack
dotnet.exe clean "../../src/DataAccess/Mapper/Nwpie.Foundation.DataAccess.Mapper.csproj" -c Release
dotnet.exe pack "../../src/DataAccess/Mapper/Nwpie.Foundation.DataAccess.Mapper.csproj" -c Release -o %cd%

REM push
%NUGET_HOME%/nuget.exe push -Source "nwpie.nuget" -ApiKey Nwpie "*.nupkg"

DEL *.nupkg

timeout 5