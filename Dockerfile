FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

RUN dotnet tool install -g dotnet-script \ 
 && export PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app

COPY whatsapp-random-bot.csx /app/whatsapp-random-bot.csx

ENV PATH="$PATH:/root/.dotnet/tools"

CMD ["dotnet-script", "whatsapp-random-bot.csx", "-c", "release"]