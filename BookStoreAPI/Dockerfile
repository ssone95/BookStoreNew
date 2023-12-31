#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN dotnet dev-certs https
WORKDIR /src
COPY ["BookStoreAPI/BookStoreAPI.csproj", "BookStoreAPI/"]
RUN dotnet restore "BookStoreAPI/BookStoreAPI.csproj"
COPY . .
WORKDIR "/src/BookStoreAPI"
RUN dotnet build "BookStoreAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStoreAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "BookStoreAPI.dll" ]