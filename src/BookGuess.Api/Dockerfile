FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/BookGuess.Api/BookGuess.Api.csproj", "BookGuess.Api/"]
COPY ["src/BookGuess.Application/BookGuess.Application.csproj", "BookGuess.Application/"]
COPY ["src/BookGuess.Domain/BookGuess.Domain.csproj", "BookGuess.Domain/"]
COPY ["src/BookGuess.Infrastructure/BookGuess.Infrastructure.csproj", "BookGuess.Infrastructure/"]
RUN dotnet restore "BookGuess.Api/BookGuess.Api.csproj"
COPY src/ .
RUN dotnet publish "BookGuess.Api/BookGuess.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookGuess.Api.dll"]
