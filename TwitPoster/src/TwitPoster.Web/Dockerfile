#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TwitPoster.Web/TwitPoster.Web.csproj", "src/TwitPoster.Web/"]
COPY ["src/TwitPoster.BLL/TwitPoster.BLL.csproj", "src/TwitPoster.BLL/"]
COPY ["src/TwitPoster.DAL/TwitPoster.DAL.csproj", "src/TwitPoster.DAL/"]
RUN dotnet restore "src/TwitPoster.Web/TwitPoster.Web.csproj"
COPY . .
WORKDIR "/src/src/TwitPoster.Web"
RUN dotnet build "TwitPoster.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "TwitPoster.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitPoster.Web.dll"]