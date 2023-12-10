# Use the official .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY MediaService/*.csproj ./MediaService/
RUN dotnet restore ./MediaService/MediaService.csproj

# Copy the remaining source code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image as the final base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Set the working directory inside the container
WORKDIR /app

# Copy the published output from the build environment
COPY --from=build-env /app/out .

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "MediaService.dll"]