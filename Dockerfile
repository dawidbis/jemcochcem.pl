FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY jemcochcemprojekt.sln .
COPY FitApp/FitApp.csproj FitApp/
COPY FitApp.Tests/FitApp.Tests.csproj FitApp.Tests/
RUN dotnet restore FitApp/FitApp.csproj
COPY . .
RUN dotnet publish FitApp/FitApp.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
CMD ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet FitApp.dll