# Base image with ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image with SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy solution and project files
COPY Foodfacilities.sln ./
COPY Foodfacilities/Foodfacilities.csproj ./Foodfacilities/
COPY Foodfacilities.Tests/Foodfacilities.Tests.csproj ./Foodfacilities.Tests/

# Restore dependencies
RUN dotnet restore Foodfacilities.sln

# Copy the rest of the source code
COPY . .

# Build the solution
RUN dotnet build -c $BUILD_CONFIGURATION

# Run tests
RUN dotnet test Foodfacilities.Tests/Foodfacilities.Tests.csproj --no-build -c Release --verbosity normal

# Publish the main app
RUN dotnet publish Foodfacilities/Foodfacilities.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for runtime
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Foodfacilities.dll"]
