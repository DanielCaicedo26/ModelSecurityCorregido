# Etapa base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /daniel
EXPOSE 8080
EXPOSE 8081

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

# Copiar todo el código fuente
COPY . .
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
COPY --from=publish /app/publish .

# Crear usuario no-root para seguridad
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Web2.dll"]