@echo off

IF EXIST "*.nupkg" DEL "*.nupkg"

REM pack
dotnet.exe clean "../../../src/Measurement/Common/Nwpie.Foundation.Measurement.Common.csproj" -c Release
dotnet.exe pack "../../../src/Measurement/Common/Nwpie.Foundation.Measurement.Common.csproj" -c Release -o %cd%

REM push
%NUGET_HOME%/nuget.exe push -Source "nwpie.nuget" -ApiKey Nwpie "*.nupkg"

DEL *.nupkg

timeout 5