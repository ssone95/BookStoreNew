# Step 1 - build application

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN dotnet dev-certs https
WORKDIR /src
COPY ["IdentityServerOld/IdentityServerOld.csproj", "IdentityServerOld/"]
RUN dotnet restore "IdentityServerOld/IdentityServerOld.csproj"
COPY . .
WORKDIR "/src/IdentityServerOld"
RUN dotnet build "IdentityServerOld.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServerOld.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "IdentityServerOld.dll" ]

#
#FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#COPY . IdentityServer.csproj
#RUN dotnet restore IdentityServer.csproj
#
#RUN mkdir -p /app/src
#WORKDIR /app/src
#COPY . .
#RUN dotnet publish -c Release -o out
#
## Step 2 - prepare and run application
#FROM mcr.microsoft.com/dotnet/aspnet:7.0
#WORKDIR /app
#COPY --from=build /app/src/out .
#ENTRYPOINT [ "dotnet", "IdentityServerOld.dll", "--urls", "http://0.0.0.0:80" ]