ARG DOTNET_TAG=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_TAG} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8084

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_TAG} AS build
WORKDIR /src
COPY ["src/ObservabilityExample.GrpcService/ObservabilityExample.GrpcService.csproj", "src/ObservabilityExample.GrpcService/"]
RUN dotnet restore "src/ObservabilityExample.GrpcService/ObservabilityExample.GrpcService.csproj"
COPY . .
WORKDIR "/src/src/ObservabilityExample.GrpcService"
RUN dotnet build "ObservabilityExample.GrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ObservabilityExample.GrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ObservabilityExample.GrpcService.dll"]
