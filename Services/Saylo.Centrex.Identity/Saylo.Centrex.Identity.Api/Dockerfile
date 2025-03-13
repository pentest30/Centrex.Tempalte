FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS prepare-restore-files
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-subset --version 0.3.2
WORKDIR /src
COPY . .
RUN dotnet subset restore "Services/Saylo.Centrex.Identity/Saylo.Centrex.Identity.Api/Saylo.Centrex.Identity.Api.csproj" --root-directory /src --output restore/

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --from=prepare-restore-files /src/restore .
RUN dotnet restore "Services/Saylo.Centrex.Identity/Saylo.Centrex.Identity.Api/Saylo.Centrex.Identity.Api.csproj"
COPY . .
WORKDIR "Services/Saylo.Centrex.Identity/Saylo.Centrex.Identity.Api"
RUN dotnet build "Saylo.Centrex.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore 

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Saylo.Centrex.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Saylo.Centrex.Identity.Api.dll"]
