FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY src/bank.victor99dev.Domain/bank.victor99dev.Domain.csproj src/bank.victor99dev.Domain/
COPY src/bank.victor99dev.Application/bank.victor99dev.Application.csproj src/bank.victor99dev.Application/
COPY src/bank.victor99dev.Infrastructure/bank.victor99dev.Infrastructure.csproj src/bank.victor99dev.Infrastructure/
COPY src/bank.victor99dev.Api/bank.victor99dev.Api.csproj src/bank.victor99dev.Api/

RUN dotnet restore src/bank.victor99dev.Api/bank.victor99dev.Api.csproj

COPY . .

RUN dotnet publish src/bank.victor99dev.Api/bank.victor99dev.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "bank.victor99dev.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator
WORKDIR /src

COPY . .

RUN chmod +x /src/scripts/migrate.sh

RUN dotnet tool install --global dotnet-ef --version 8.*
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["/bin/sh", "/src/scripts/migrate.sh"]