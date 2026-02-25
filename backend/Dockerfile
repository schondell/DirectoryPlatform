FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DirectoryPlatform/DirectoryPlatform.sln .
COPY DirectoryPlatform/src/Core/DirectoryPlatform.Core.csproj src/Core/
COPY DirectoryPlatform/src/Contracts/DirectoryPlatform.Contracts.csproj src/Contracts/
COPY DirectoryPlatform/src/Application/DirectoryPlatform.Application.csproj src/Application/
COPY DirectoryPlatform/src/Infrastructure/DirectoryPlatform.Infrastructure.csproj src/Infrastructure/
COPY DirectoryPlatform/src/API/DirectoryPlatform.API.csproj src/API/
RUN dotnet restore

COPY DirectoryPlatform/ .
RUN dotnet publish src/API/DirectoryPlatform.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DirectoryPlatform.API.dll"]
