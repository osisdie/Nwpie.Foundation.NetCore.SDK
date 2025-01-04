FROM mcr.microsoft.com/dotnet/core/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:8.0 AS build
WORKDIR /build
COPY . .
ARG NUGET_USERNAME
ARG NUGET_ENDPOINT
ARG NUGET_PAT
RUN bash -c 'cat azure-nuget.xml | sed "s|NUGET_ENDPOINT|$NUGET_ENDPOINT|" | sed "s|NUGET_USERNAME|$NUGET_USERNAME|" | sed "s|NUGET_PAT|$NUGET_PAT|" > ./NuGet.Config'
# RUN bash -c 'cat ./NuGet.Config | head'

# --------------------------
# COPY Dependency Files
# --------------------------
# console trace
# RUN bash -c 'cat samples/MiniSite/ESProxy/Endpoint/platform.json | head'

# --------------------------
# Build & Publish
# --------------------------
RUN dotnet restore "samples/MiniSite/ESProxy/Endpoint/Nwpie.MiniSite.ES.Endpoint.csproj" --configfile NuGet.Config
ARG CONFIGURATION=Release
RUN dotnet publish "samples/MiniSite/ESProxy/Endpoint/Nwpie.MiniSite.ES.Endpoint.csproj" -c ${CONFIGURATION} -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Nwpie.MiniSite.ES.Endpoint.dll"]
