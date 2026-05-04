FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/UsersProducts.Api/UsersProducts.Api.csproj src/UsersProducts.Api/

RUN dotnet restore src/UsersProducts.Api/UsersProducts.Api.csproj

COPY . .

RUN dotnet publish src/UsersProducts.Api/UsersProducts.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UsersProducts.Api.dll"]