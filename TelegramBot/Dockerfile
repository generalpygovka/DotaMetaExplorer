# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# скопировать только проекты
COPY DotaMetaExplorer/DotaMetaExplorer.csproj DotaMetaExplorer/
COPY TelegramBot/TelegramBot.csproj TelegramBot/

# восстановить пакеты только для бота (он подтянет DotaMetaExplorer по ссылке)
RUN dotnet restore TelegramBot/TelegramBot.csproj

# скопировать всё
COPY . .

# опубликовать именно проект бота
WORKDIR /src/TelegramBot
RUN dotnet publish -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TelegramBot.dll"]