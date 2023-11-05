FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
EXPOSE 5010/tcp
ENV ASPNETCORE_URLS http://*:5010
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Essence.WebAPI.dll"]