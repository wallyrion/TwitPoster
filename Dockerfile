FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TwitPoster.Web/TwitPoster.Web.csproj", "TwitPoster.Web/"]
COPY ["TwitPoster.BLL/TwitPoster.BLL.csproj", "TwitPoster.BLL/"]
COPY ["TwitPoster.DAL/TwitPoster.DAL.csproj", "TwitPoster.DAL/"]
RUN dotnet restore "TwitPoster.Web/TwitPoster.Web.csproj"
COPY . .
WORKDIR "/src/TwitPoster.Web"
RUN dotnet build "TwitPoster.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitPoster.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitPoster.Web.dll"]
