FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_8.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY webapp/webapp.csproj webapp/
RUN dotnet restore webapp/webapp.csproj
COPY . .
WORKDIR /src/webapp
RUN dotnet build webapp.csproj -c Release -o /app

FROM node:8 as modules
WORKDIR /npm
COPY webapp/ClientApp/package*.json /npm/
RUN npm install

FROM build AS publish
RUN dotnet publish webapp.csproj -c Release -o /app

FROM base as development
WORKDIR /modules
COPY --from=modules /npm .
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "webapp.dll"]
