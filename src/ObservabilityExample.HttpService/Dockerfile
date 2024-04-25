ARG DOTNET_TAG=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_TAG} AS base
WORKDIR /app
EXPOSE 8080 8084


# build
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_TAG} AS build
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

COPY ["src/ObservabilityExample.HttpService/ObservabilityExample.HttpService.csproj", "/src/ObservabilityExample.HttpService/"]
# package and build info
COPY ["Directory.Build.props", "/src/"]
COPY ["Directory.Packages.props", "/src/"]

RUN dotnet restore "/src/ObservabilityExample.HttpService/ObservabilityExample.HttpService.csproj"

COPY ./src/ /src/
WORKDIR "/src/ObservabilityExample.HttpService"
RUN dotnet build "./ObservabilityExample.HttpService.csproj" -c Release -o /app/build


# publish
FROM build AS publish
RUN dotnet publish "./ObservabilityExample.HttpService.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# final layer
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ObservabilityExample.HttpService.dll"]
