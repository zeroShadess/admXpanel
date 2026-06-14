FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AdminPanel.csproj", "./"]
RUN dotnet restore "AdminPanel.csproj"
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render.com RENDER env var'ı otomatik set eder
ENV ASPNETCORE_ENVIRONMENT=Production
ENV RENDER=true

EXPOSE 10000
ENTRYPOINT ["dotnet", "AdminPanel.dll"]
