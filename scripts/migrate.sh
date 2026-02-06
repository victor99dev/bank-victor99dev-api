#!/bin/sh
set -e

echo "[migrator] starting..."
echo "[migrator] ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"
echo "[migrator] ConnectionStrings__DefaultConnection=$ConnectionStrings__DefaultConnection"

export PATH="$PATH:/root/.dotnet/tools"

echo "[migrator] dotnet-ef version:"
dotnet ef --version

echo "[migrator] running migrations..."
dotnet ef database update \
  --project src/bank.victor99dev.Infrastructure/bank.victor99dev.Infrastructure.csproj \
  --startup-project src/bank.victor99dev.Api/bank.victor99dev.Api.csproj \
  --context AppDbContext

echo "[migrator] done"
