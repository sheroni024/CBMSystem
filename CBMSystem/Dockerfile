# Use official ASP.NET runtime image as base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Expose port via environment variable (default to 8080 if not set)
ARG PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CBMSystem/CBMSystem.csproj", "CBMSystem/"]
RUN dotnet restore "./CBMSystem/CBMSystem.csproj"

COPY . .
WORKDIR "/src/CBMSystem"
RUN dotnet build "./CBMSystem.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CBMSystem.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Use the environment port variable for Render
ENV PORT 8080
ENTRYPOINT ["dotnet", "CBMSystem.dll"]
