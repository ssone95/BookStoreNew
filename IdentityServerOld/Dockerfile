# Step 1 - build application

FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app
COPY . IdentityServer.csproj
RUN dotnet restore IdentityServer.csproj

RUN mkdir -p /app/src
WORKDIR /app/src
COPY . .
RUN dotnet publish -c Release -o out

# Step 2 - prepare and run application
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/src/out .
EXPOSE 5000 5001
ENTRYPOINT [ "dotnet", "IdentityServer.dll", "--urls", "http://0.0.0.0:80" ]