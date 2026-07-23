# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY PortManagement.sln ./
COPY Api/*.csproj ./Api/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Tests/*.csproj ./Tests/

RUN dotnet restore PortManagement.sln

COPY . .

RUN dotnet publish Api/PortManagement.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS="http://0.0.0.0:8080"

EXPOSE 8080
ENTRYPOINT ["dotnet", "PortManagement.Api.dll"]

