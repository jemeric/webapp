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

FROM microsoft/dotnet:2.2-sdk-alpine AS build
WORKDIR /src
COPY webapp.csproj webapp/
RUN dotnet restore ./webapp/webapp.csproj
WORKDIR /src/webapp
COPY . .
RUN dotnet build webapp.csproj -c Release -o /app



# copy csproj and restore as distinct layers
# COPY *.sln .
# COPY nuget.config . 
# COPY superzeit/*.csproj ./superzeit/
# RUN dotnet restore
# copy everything else and build app
# COPY . .
# WORKDIR /app/superzeit
# RUN dotnet build
 
# FROM build AS publish
# WORKDIR /app/superzeit
# # add IL Linker package
# RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
# RUN dotnet publish -c Release -o out -r linux-musl-x64 /p:ShowLinkerSizeComparison=true

FROM build AS publish
WORKDIR /src/webapp
RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
RUN dotnet publish webapp.csproj -c Release -o out -r linux-musl-x64 /p:ShowLinkerSizeComparison=true /p:CrossGenDuringPublish=false


FROM microsoft/dotnet:2.2-runtime-deps-alpine AS runtime
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
WORKDIR /app
COPY --from=publish /src/webapp/out ./
ENTRYPOINT ["dotnet", "webapp.dll"]

