
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["LibraryServer.csproj", "."]
RUN dotnet restore "./LibraryServer.csproj"
COPY . .

RUN dotnet publish "./LibraryServer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LibraryServer.dll"]
