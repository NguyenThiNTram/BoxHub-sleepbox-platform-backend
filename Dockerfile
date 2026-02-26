# ========================
# Base runtime
# ========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Render inject PORT
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ========================
# Build stage
# ========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BoxHub.API/BoxHub.API.csproj", "BoxHub.API/"]
COPY ["BoxHub.Infrastructure/BoxHub.Infrastructure.csproj", "BoxHub.Infrastructure/"]
COPY ["BoxHub.Domain/BoxHub.Domain.csproj", "BoxHub.Domain/"]
COPY ["BoxHub.Application/BoxHub.Application.csproj", "BoxHub.Application/"]

RUN dotnet restore "BoxHub.API/BoxHub.API.csproj"

COPY . .
WORKDIR "/src/BoxHub.API"
RUN dotnet publish "BoxHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# ========================
# Final stage
# ========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BoxHub.API.dll"]