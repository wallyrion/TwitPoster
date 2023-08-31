FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-preview AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/nightly/sdk:8.0-preview AS build
WORKDIR /src
COPY ["TwitPoster.Web/TwitPoster.Web.csproj", "TwitPoster.Web/"]
COPY ["TwitPoster.BLL/TwitPoster.BLL.csproj", "TwitPoster.BLL/"]
COPY ["TwitPoster.DAL/TwitPoster.DAL.csproj", "TwitPoster.DAL/"]
COPY ["TwitPoster.Contracts/TwitPoster.Contracts.csproj", "TwitPoster.Contracts/"]
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
