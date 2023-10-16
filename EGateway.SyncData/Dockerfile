#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EGateway.SyncData/EGateway.SyncData.csproj", "EGateway.SyncData/"]
RUN dotnet restore "EGateway.SyncData/EGateway.SyncData.csproj"
COPY . .
WORKDIR "/src/EGateway.SyncData"
RUN dotnet build "EGateway.SyncData.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EGateway.SyncData.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EGateway.SyncData.dll"]