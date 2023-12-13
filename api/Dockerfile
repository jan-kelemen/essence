FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /api
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /api
EXPOSE 5010/tcp
ENV ASPNETCORE_URLS http://*:5010
COPY --from=build-env /api/out .
ENTRYPOINT ["dotnet", "Essence.WebAPI.dll"]
