# building using: docker build --rm -t webapp:prod ../../webapp
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

# TODO - use trimmer + linker but the linker doesn't work with some of the dependencies in this app :(
# add IL Linker package
#RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
#RUN dotnet publish -c Release -r linux-musl-x64 -o out /p:ShowLinkerSizeComparison=true /p:CrossGenDuringPublish=false

RUN dotnet add package Microsoft.Packaging.Tools.Trimming --version 1.1.0-preview1-26619-01
RUN dotnet publish -c Release -r linux-musl-x64 -o out /p:TrimUnusedDependencies=true

FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-alpine AS runtime
RUN apk add nodejs-current
WORKDIR /app
COPY --from=build /app/webapp/out ./
ENV ASPNETCORE_URLS http://+:5001
EXPOSE 5001
ENTRYPOINT ["./webapp", "-no-restor", "--urls", "http://0.0.0.0:5001"]