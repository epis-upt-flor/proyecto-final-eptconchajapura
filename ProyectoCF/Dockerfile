FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "ProyectoCF.dll"]
