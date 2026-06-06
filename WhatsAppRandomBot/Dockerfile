FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WhatsAppRandomBot/WhatsAppRandomBot.csproj", "WhatsAppRandomBot/"]
RUN dotnet restore "WhatsAppRandomBot/WhatsAppRandomBot.csproj"
COPY . .
WORKDIR "/src/WhatsAppRandomBot"
RUN dotnet build "./WhatsAppRandomBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WhatsAppRandomBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WhatsAppRandomBot.dll"]
