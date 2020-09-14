FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /src
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /

# For some reaason in doesn't work
#COPY "ClearSkyMapsSimple.sln" "ClearSkyMapsSimple.sln"
#COPY "src/Web/Web.csproj" "src/Web/Web.csproj"
#COPY "src/Web.UnitTests/Web.UnitTests.csproj" "src/Web.UnitTests/Web.UnitTests.csproj"
#COPY "src/Web.IntegrationTests/Web.IntegrationTests.csproj" "src/Web.IntegrationTests/Web.IntegrationTests.csproj"

COPY . .
RUN dotnet restore  "ClearSkyMapsSimple.sln" --disable-parallel

WORKDIR /src/Web
RUN pwsh compilepwa.ps1
RUN dotnet publish --no-restore --no-cache -c Release -o /app

FROM build as unit-tests
WORKDIR /src/Web.UnitTests

FROM build as integration-tests
WORKDIR /src/Web.IntegrationTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.dll"]