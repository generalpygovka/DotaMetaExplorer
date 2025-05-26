# WebApi/Dockerfile (.NET 8)
# 1) збірка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# копіюємо csproj та відновлюємо пакети
COPY *.csproj .
RUN dotnet restore

# копіюємо весь код і публікуємо
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 2) рантайм
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# відкриваємо порт
EXPOSE 80

ENTRYPOINT ["dotnet", "DotaMetaExplorer.dll"]