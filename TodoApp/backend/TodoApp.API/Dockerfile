# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Əvvəlcə yalnız csproj kopyalanır ki, restore layer-i cache-lənsin
COPY TodoApp.API.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

USER app

# Connection string image-ə yazılmır — runtime-da env ilə verilir (ConnectionStrings__Default)
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080

# Render PORT env verirsə, Program.cs onu istifadə edir; yoxdursa 8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TodoApp.API.dll"]
