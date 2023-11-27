# TODO: Later remove the following text:
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Express" -p 1433:1433 -d --name=sql mcr.microsoft.com/mssql/server:latest

Server=localhost,1433;Initial Catalog=MyDb;Integrated Security=True;User Id=sa;Password=MyPass@word;

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Express" -p 1439:1433 -d --name=sql mcr.microsoft.com/mssql/server:latest

Server=localhost,1439;Initial Catalog=MyDb;Integrated Security=True;User Id=sa;Password=MyPass@word;


# Certificates setup (for Docker)
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\bookstore.pfx -p Secret123$
dotnet dev-certs https --trust

# Then in Package Manager Console
dotnet user-secrets -p BookStoreApi\BookStoreApi.csproj set "Kestrel:Certificates:Development:Password" "Secret123$"
dotnet user-secrets -p IdentityServerOld\IdentityServerOld.csproj set "Kestrel:Certificates:Development:Password" "Secret123$"