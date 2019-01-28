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
# ENV ASPNETCORE_URLS http://*:80

# FROM microsoft/dotnet:2.2-sdk AS build
# WORKDIR /src
# COPY ["webapp/webapp.csproj", "webapp/"]
# RUN dotnet restore "webapp/webapp.csproj"
# COPY . .
# WORKDIR "/src/webapp"
# RUN dotnet build "webapp.csproj" -c Release -o /app

# FROM build AS publish
# WORKDIR /src/webapp/ClientApp
# RUN npm rebuild node-sass --force
# RUN npm run build
# WORKDIR /src/webapp
# RUN dotnet publish "webapp.csproj" -c Release -o /app
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
# ENTRYPOINT ["dotnet", "webapp.dll"]

FROM microsoft/dotnet:latest
COPY . /app
WORKDIR /app
RUN dotnet restore
RUN dotnet build
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_8.x | bash - && \
    apt-get install -y build-essential nodejs
WORKDIR /app/ClientApp
RUN npm rebuild node-sass --force
WORKDIR /app
EXPOSE 60882
ENTRYPOINT ["dotnet", "run", "-no-restor", "--urls", "https://0.0.0.0:5000"]


# FROM microsoft/dotnet:2.2-sdk-alpine AS build
# WORKDIR /app

# # copy csproj and restore as distinct layers
# COPY webapp/*.csproj ./webapp/
# #COPY utils/*.csproj ./utils/
# WORKDIR /app/dotnetapp
# RUN dotnet restore

# # copy and publish app and libraries
# WORKDIR /app/
# COPY webapp/. ./webapp/
# #COPY utils/. ./utils/
# WORKDIR /app/webapp
# RUN dotnet publish -c Release -o out


# # test application -- see: dotnet-docker-unit-testing.md
# # FROM build AS testrunner
# # WORKDIR /app/tests
# # COPY tests/. .
# # ENTRYPOINT ["dotnet", "test", "--logger:trx"]


# FROM microsoft/dotnet:2.2-runtime-alpine AS runtime
# WORKDIR /app
# COPY --from=build /app/webapp/out ./
# ENTRYPOINT ["dotnet", "webapp.dll"]