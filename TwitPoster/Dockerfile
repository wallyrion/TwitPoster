FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-preview AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/nightly/sdk:8.0-preview AS build
WORKDIR /src
COPY ["src/TwitPoster.Web/TwitPoster.Web.csproj", "TwitPoster.Web/"]
COPY ["src/TwitPoster.BLL/TwitPoster.BLL.csproj", "TwitPoster.BLL/"]
COPY ["src/TwitPoster.DAL/TwitPoster.DAL.csproj", "TwitPoster.DAL/"]
RUN dotnet restore "TwitPoster.Web/TwitPoster.Web.csproj"
COPY src .
WORKDIR "/src/TwitPoster.Web"
RUN dotnet build "TwitPoster.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitPoster.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitPoster.Web.dll"]
