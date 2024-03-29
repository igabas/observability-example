ARG DOTNET_TAG=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_TAG} AS base
WORKDIR /app
EXPOSE 8080 8084

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_TAG} AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ObservabilityExample.HttpService/ObservabilityExample.HttpService.csproj", "src/ObservabilityExample.HttpService/"]
RUN dotnet restore "src/ObservabilityExample.HttpService/ObservabilityExample.HttpService.csproj"
COPY . .
WORKDIR "/src/src/ObservabilityExample.HttpService"
RUN dotnet build "ObservabilityExample.HttpService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ObservabilityExample.HttpService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ObservabilityExample.HttpService.dll"]
