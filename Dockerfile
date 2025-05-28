# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["oratorio-backend.csproj", "./"]
RUN dotnet restore "./oratorio-backend.csproj"
COPY . .
RUN dotnet publish "oratorio-backend.csproj" -c Release -o /app

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .  
ENTRYPOINT ["dotnet", "oratorio-backend.dll"]
