FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["TriggMine.ChatBot.Core/TriggMine.ChatBot.Core.csproj", "TriggMine.ChatBot.Core/"]
COPY ["TriggMine.ChatBot.Repository/TriggMine.ChatBot.Repository.csproj", "TriggMine.ChatBot.Repository/"]
COPY ["TriggMine.ChatBot.Shared/TriggMine.ChatBot.Shared.csproj", "TriggMine.ChatBot.Shared/"]
RUN dotnet restore "TriggMine.ChatBot.Core/TriggMine.ChatBot.Core.csproj"
COPY . .
WORKDIR "/src/TriggMine.ChatBot.Core"
RUN dotnet build "TriggMine.ChatBot.Core.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TriggMine.ChatBot.Core.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TriggMine.ChatBot.Core.dll"]