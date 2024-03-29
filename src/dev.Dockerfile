FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
# Copy the certificate for HTTPS during development
COPY ./aspnetapp.pfx /https/
EXPOSE 80
EXPOSE 443

# Install curl
RUN apt-get update && \
    apt-get install -y curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# The SDK is used here instead of the runtime to enable building and watching for changes
FROM mcr.microsoft.com/dotnet/sdk:7.0.102 AS build
WORKDIR /src
COPY ["theforum.csproj", "./"]
RUN dotnet restore "theforum.csproj" --verbosity detailed
COPY . .
RUN dotnet build "theforum.csproj" -c Debug -o /app/build

# Publish is still performed to prepare the app, but we use Debug configuration
FROM build AS publish
RUN dotnet publish "theforum.csproj" -c Debug -o /app/publish

# Final stage/image for development
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the ASP.NET Core environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Use dotnet watch to enable hot reloading
ENTRYPOINT ["dotnet", "watch", "run"]
