FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/BookGuess.Web/BookGuess.Web.csproj", "src/BookGuess.Web/"]
RUN dotnet restore "src/BookGuess.Web/BookGuess.Web.csproj"
COPY . .
WORKDIR "/src/src/BookGuess.Web"
RUN dotnet build "BookGuess.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookGuess.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookGuess.Web.dll"]
