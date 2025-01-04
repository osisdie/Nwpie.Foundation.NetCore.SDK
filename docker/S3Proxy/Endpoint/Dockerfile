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
# RUN bash -c 'cat samples/Serverless/S3Proxy/Endpoint/platform.json | head'

# --------------------------
# Build & Publish
# --------------------------
RUN dotnet restore "samples/Serverless/S3Proxy/Endpoint/Nwpie.Foundation.S3Proxy.Endpoint.csproj" --configfile NuGet.Config
ARG CONFIGURATION=Release
RUN dotnet publish "samples/Serverless/S3Proxy/Endpoint/Nwpie.Foundation.S3Proxy.Endpoint.csproj" -c ${CONFIGURATION} -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Nwpie.Foundation.S3Proxy.Endpoint.dll"]
