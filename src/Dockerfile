FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
COPY ./aspnetapp.pfx /https/
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0.102 AS build
WORKDIR /src
COPY ["theforum.csproj", "./"]
RUN dotnet restore "theforum.csproj" --verbosity detailed
COPY . .
RUN dotnet build "theforum.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "theforum.csproj" -c Release -o /app/publish --no-restore

# Final stage/image with non-root user
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "theforum.dll"]