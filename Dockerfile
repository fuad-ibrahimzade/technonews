#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TechnologyNews/TechnologyNews.csproj", "TechnologyNews/"]
RUN dotnet restore "TechnologyNews/TechnologyNews.csproj"
COPY . .
WORKDIR "/src/TechnologyNews"
RUN apt-get install --yes curl
RUN curl --silent --location https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install --yes nodejs
RUN dotnet build "TechnologyNews.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechnologyNews.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TechnologyNews.dll