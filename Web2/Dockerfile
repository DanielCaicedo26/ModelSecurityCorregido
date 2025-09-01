
# Etapa base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["Web2/Web2.csproj", "Web2/"]
COPY ["Bussines/Bussines.csproj", "Bussines/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["ModelSecurityDa/Entity.csproj", "ModelSecurityDa/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]

RUN dotnet restore "Web2/Web2.csproj"

COPY . .
# Copiar herramientas locales dotnet-ef
COPY Web2/.config ./Web2/.config
WORKDIR "/src/Web2"

# Construir el proyecto
RUN dotnet build "Web2.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Web2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app


# Instalar herramientas necesarias para SQL Server, PostgreSQL y MySQL, y .NET EF Tools
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    gnupg \
    ca-certificates \
    wget \
    unixodbc-dev \
    postgresql-client \
    libpq-dev \
    mariadb-client \
    libmariadb-dev-compat \
    && curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg \
    && echo "deb [arch=amd64,arm64,armhf signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/repos/microsoft-debian-bookworm-prod bookworm main" > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools18 \
    && rm -rf /var/lib/apt/lists/*


# Instalar .NET SDK para herramientas EF y dotnet-ef global
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-9.0 \
    && dotnet tool install --global dotnet-ef \
    && rm -rf /var/lib/apt/lists/*

# Configurar PATH para herramientas y dotnet-ef global
ENV PATH="${PATH}:/opt/mssql-tools18/bin:/root/.dotnet/tools"


# Copiar archivos publicados
COPY --from=publish /app/publish .

# Ejecutar migraciones antes de cambiar al usuario no-root
RUN dotnet ef database update --no-build || true

# Crear usuario no-root para seguridad (después de configurar herramientas)
RUN adduser --disabled-password --gecos "" appuser \
    && chown -R appuser:appuser /app

# Healthcheck opcional
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=5 \
   CMD curl --fail http://localhost:8080/health || exit 1

# Cambiar al usuario no-root
USER appuser

ENTRYPOINT ["dotnet", "Web2.dll"]
