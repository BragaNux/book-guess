FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/BookGuess.AIService/BookGuess.AIService.csproj", "BookGuess.AIService/"]
RUN dotnet restore "BookGuess.AIService/BookGuess.AIService.csproj"
COPY src/BookGuess.AIService/ BookGuess.AIService/
RUN dotnet publish "BookGuess.AIService/BookGuess.AIService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookGuess.AIService.dll"]
