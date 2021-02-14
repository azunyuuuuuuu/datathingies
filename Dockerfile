# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# install npm
RUN curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - \
 && apt-get install -y nodejs \
 && npm install -g webpack webpack-cli

# copy csproj and restore as distinct layers
COPY *.csproj /src/
RUN dotnet restore

# copy everything else and build app
COPY . /src/
RUN cd StaticAssets && npm install
RUN dotnet publish -c Release -o /output

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY --from=build /output ./

VOLUME [ "/app/_data" ]

EXPOSE "80"
ENTRYPOINT ["dotnet", "datathingies.dll"]