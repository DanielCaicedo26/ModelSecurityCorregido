# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY *.sln .
COPY Web2/*.csproj ./Web2/
COPY Bussines/*.csproj ./Bussines/
COPY Data/*.csproj ./Data/
COPY ModelSecurityDa/*.csproj ./ModelSecurityDa/
COPY Utilities/*.csproj ./Utilities/

# Restaurar dependencias
RUN dotnet restore

# Copiar el resto del código
COPY . .

# Publicar la aplicación
WORKDIR /src/Web2
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime con Ubuntu, Nginx y .NET 9.0
FROM ubuntu:22.04 AS runtime

# Configurar variables de entorno
ENV DEBIAN_FRONTEND=noninteractive
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Actualizar sistema e instalar dependencias
RUN apt-get update && apt-get install -y \
    wget \
    nginx \
    supervisor \
    ca-certificates \
    libc6 \
    libgcc-s1 \
    libgssapi-krb5-2 \
    libicu70 \
    libssl3 \
    libstdc++6 \
    zlib1g \
    && rm -rf /var/lib/apt/lists/*

# Instalar .NET 9.0 Runtime
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y aspnetcore-runtime-9.0 \
    && rm -rf /var/lib/apt/lists/*

# Crear directorio para la aplicación
WORKDIR /app

# Copiar la aplicación publicada
COPY --from=build /app/publish .

# Configurar Nginx
COPY nginx.conf /etc/nginx/nginx.conf
COPY default.conf /etc/nginx/sites-available/default

# Configurar supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Crear directorios necesarios
RUN mkdir -p /var/log/supervisor /var/log/nginx

# Crear script de inicio directamente
RUN echo '#!/bin/bash\n\
mkdir -p /var/log/supervisor\n\
mkdir -p /var/log/nginx\n\
rm -f /etc/nginx/sites-enabled/default\n\
ln -s /etc/nginx/sites-available/default /etc/nginx/sites-enabled/\n\
nginx -t\n\
exec /usr/bin/supervisord -c /etc/supervisor/conf.d/supervisord.conf' > /start.sh && \
chmod +x /start.sh

# Exponer puertos
EXPOSE 80 5000

# Comando de inicio
CMD ["/start.sh"]