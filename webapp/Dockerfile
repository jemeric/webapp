FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS http://*:80
RUN apt-get -qq update && apt-get -qqy --no-install-recommends install wget gnupg \
    git \
    unzip
RUN curl -sL https://deb.nodesource.com/setup_8.x |  bash -
RUN apt-get install -y nodejs

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["webapp/webapp.csproj", "webapp/"]
RUN dotnet restore "webapp/webapp.csproj"
COPY . .
WORKDIR "/src/webapp"
RUN dotnet build "webapp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "webapp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "webapp.dll"]