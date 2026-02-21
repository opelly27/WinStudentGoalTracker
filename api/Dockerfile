FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY api.csproj ./
RUN dotnet restore api.csproj

COPY . ./
RUN dotnet publish api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8005
EXPOSE 8005

ENTRYPOINT ["dotnet", "api.dll"]
