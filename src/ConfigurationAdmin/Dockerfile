FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ConfigurationReaderCase.sln", "./"]
COPY ["src/ConfigurationAdmin/ConfigurationAdmin.csproj", "src/ConfigurationAdmin/"]
COPY ["src/ConfigurationReader/ConfigurationReader.csproj", "src/ConfigurationReader/"]

RUN dotnet restore "src/ConfigurationAdmin/ConfigurationAdmin.csproj"

COPY . .

RUN dotnet publish "src/ConfigurationAdmin/ConfigurationAdmin.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ConfigurationAdmin.dll"]