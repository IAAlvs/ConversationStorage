FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ConversationStorageApi/ConversationStorageApi.csproj .
RUN dotnet restore
COPY ./src/ConversationStorageApi .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ConversationStorageApi.dll"]
