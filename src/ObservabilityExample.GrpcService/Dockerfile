ARG DOTNET_TAG=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_TAG} AS base
WORKDIR /app
EXPOSE 8080 8084


# build
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_TAG} AS build
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

COPY ["src/ObservabilityExample.GrpcService/ObservabilityExample.GrpcService.csproj", "/src/ObservabilityExample.GrpcService/"]
# package and build info
COPY ["Directory.Build.props", "/src/"]
COPY ["Directory.Packages.props", "/src/"]

RUN dotnet restore "/src/ObservabilityExample.GrpcService/ObservabilityExample.GrpcService.csproj"

COPY ./src/ /src/
WORKDIR "/src/ObservabilityExample.GrpcService"
RUN dotnet build "./ObservabilityExample.GrpcService.csproj" -c Release -o /app/build


# publish
FROM build AS publish
RUN dotnet publish "./ObservabilityExample.GrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# final layer
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ObservabilityExample.GrpcService.dll"]
