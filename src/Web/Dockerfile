FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /src
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /

COPY . .
RUN dotnet restore "ClearSkyMapsSimple.sln"

WORKDIR /src/Web
RUN dotnet publish --no-restore -c Release -o /app

FROM build as tests
WORKDIR /src/Web.Tests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.dll"]