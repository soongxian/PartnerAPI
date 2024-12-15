FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY PartnerAPI/PartnerAPI.csproj ./PartnerAPI/
RUN dotnet restore ./PartnerAPI/PartnerAPI.csproj

COPY PartnerAPI/ ./PartnerAPI/
WORKDIR /app/PartnerAPI
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/PartnerAPI/out .

EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "PartnerAPI.dll"]
