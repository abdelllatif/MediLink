# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/MediLink.API/MediLink.API.csproj", "src/MediLink.API/"]
COPY ["src/MediLink.Application/MediLink.Application.csproj", "src/MediLink.Application/"]
COPY ["src/MediLink.Domain/MediLink.Domain.csproj", "src/MediLink.Domain/"]
COPY ["src/MediLink.Infrastructure/MediLink.Infrastructure.csproj", "src/MediLink.Infrastructure/"]
COPY ["src/MediLink.Shared/MediLink.Shared.csproj", "src/MediLink.Shared/"]

# Restore dependencies
RUN dotnet restore "src/MediLink.API/MediLink.API.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/src/MediLink.API"
RUN dotnet build "MediLink.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MediLink.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "MediLink.API.dll"]
