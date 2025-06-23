FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./JobTracking.API/*.csproj .

CMD ["dotnet", "watch", "run", "-v", "--non-interactive", "--project", "MusicallyAPI/MusicallyAPI.csproj"]
