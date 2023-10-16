FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["EGateway.sln", "EGateway.sln"]
COPY ["EGateway.Api/", "EGateway.Api/"]
COPY ["EGateway.Model/", "EGateway.Model/"]
COPY ["EGateway.Common/", "EGateway.Common/"]
COPY ["EGateway.Business/", "EGateway.Business/"]
COPY ["EGateway.ViewModel/", "EGateway.ViewModel/"]
COPY ["EGateway.DataAccess/", "EGateway.DataAccess/"]

RUN dotnet restore "EGateway.sln"
COPY . .
WORKDIR "/src/EGateway.Api"
RUN dotnet build "EGateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EGateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EGateway.Api.dll"]