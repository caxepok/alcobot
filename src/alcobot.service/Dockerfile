FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["alcobot.service/alcobot.service.csproj", "alcobot.service/"]
RUN dotnet restore "alcobot.service/alcobot.service.csproj"
COPY . .
WORKDIR "/src/alcobot.service"
RUN dotnet build "alcobot.service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "alcobot.service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV "AppSettings:BotApiKey" "YOUR_BOT_TOKEN_HERE"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "alcobot.service.dll"]
