# FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
# WORKDIR /app
# # Setup NodeJs
# RUN apt-get update && \
#     apt-get install -y wget && \
#     apt-get install -y gnupg2 && \
#     wget -qO- https://deb.nodesource.com/setup_8.x | bash - && \
#     apt-get install -y build-essential nodejs
# # End setup
# EXPOSE 80
# EXPOSE 443

# FROM microsoft/dotnet:2.2-sdk AS build
# WORKDIR /src
# COPY webapp/webapp.csproj webapp/
# RUN dotnet restore webapp/webapp.csproj
# COPY . .
# WORKDIR /src/webapp
# RUN dotnet build webapp.csproj -c Release -o /app

# FROM node:8 as modules
# WORKDIR /npm
# COPY webapp/ClientApp/package.json /npm
# RUN npm install

# FROM build AS publish
# RUN dotnet publish webapp.csproj -c Release -o /app

# FROM base as development
# WORKDIR /modules
# COPY --from=modules /npm .
# WORKDIR /app

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
# ENTRYPOINT ["dotnet", "webapp.dll"]

# MINIMIZED

# FROM microsoft/dotnet:2.2-sdk-alpine AS build
# WORKDIR /src
# COPY webapp.csproj webapp/
# RUN dotnet restore ./webapp/webapp.csproj
# WORKDIR /src/webapp
# COPY . .
# RUN dotnet build webapp.csproj -c Release -o /app

# FROM build AS publish
# WORKDIR /src/webapp
# RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
# RUN dotnet publish webapp.csproj -c Release -o out -r linux-musl-x64 /p:ShowLinkerSizeComparison=true /p:CrossGenDuringPublish=false


# FROM microsoft/dotnet:2.2-runtime-deps-alpine AS runtime
# ENV DOTNET_USE_POLLING_FILE_WATCHER=true
# WORKDIR /app
# COPY --from=publish /src/webapp/out ./
# ENTRYPOINT ["dotnet", "./webapp.dll"]

# FROM LINK - https://github.com/dotnet/dotnet-docker/blob/master/samples/dotnetapp/Dockerfile.alpine-x64-selfcontained

# build frontend for production
FROM mhart/alpine-node:10 as frontend
WORKDIR /app/Assets
COPY ./Assets .
WORKDIR /app/ClientApp
COPY ./ClientApp .
RUN npm install
RUN npm run build:prod


FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY webapp.csproj ./webapp/
# COPY utils/*.csproj ./utils/
WORKDIR /app/webapp
RUN dotnet restore

# copy and build app and libraries
WORKDIR /app/
COPY . ./webapp/
# COPY utils/. ./utils/

# copy production version of frontend
WORKDIR /app/webapp/ClientApp
COPY --from=frontend /app/ClientApp ./
WORKDIR /app/webapp/wwwroot
COPY --from=frontend /app/wwwroot ./

WORKDIR /app/webapp

# TODO - rebuild client-side assets in production mode
# add IL Linker package
# RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
# RUN dotnet publish -c Release -r linux-musl-x64 -o out /p:ShowLinkerSizeComparison=true /p:CrossGenDuringPublish=false
RUN dotnet publish -c Release -r linux-musl-x64 -o out 

FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-alpine AS runtime
RUN apk add nodejs-current
WORKDIR /app
COPY --from=build /app/webapp/out ./
ENTRYPOINT ["./webapp", "-no-restor", "--urls", "http://0.0.0.0:5001"]