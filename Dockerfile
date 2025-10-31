# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file and restore first to leverage layer caching
COPY ["ImageCompressor.sln", "."]
COPY ["ImageCompressor.WebApp/ImageCompressor.WebApp.csproj", "ImageCompressor.WebApp/"]
COPY ["ImageCompressor.Library/ImageCompressor.Library.csproj", "ImageCompressor.Library/"]
RUN dotnet restore "ImageCompressor.WebApp/ImageCompressor.WebApp.csproj"

# Copy everything and publish
COPY . .
WORKDIR /src/ImageCompressor.WebApp
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ImageCompressor.WebApp.dll"]
