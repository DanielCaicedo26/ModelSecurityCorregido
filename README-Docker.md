# Contenedor Docker con Ubuntu + Nginx + .NET 9.0

Este proyecto está configurado para ejecutarse en un contenedor Docker con Ubuntu, Nginx como proxy inverso y .NET 9.0 runtime.

## Arquitectura

- **Ubuntu 22.04**: Sistema operativo base
- **Nginx**: Proxy inverso que maneja las requests HTTP en el puerto 80
- **ASP.NET Core 9.0**: API backend ejecutándose en el puerto 5000
- **Supervisor**: Process manager que administra Nginx y la aplicación .NET

## Comandos para ejecutar

### Construcción y ejecución simple
```bash
# Construir la imagen
docker build -t modelsecurityda-app .

# Ejecutar el contenedor
docker run -p 80:80 -p 5000:5000 modelsecurityda-app
```

### Con docker-compose (recomendado)
```bash
# Ejecutar solo la aplicación
docker-compose up app

# Ejecutar con todas las bases de datos
docker-compose --profile database up

# En background
docker-compose up -d app
```

## Puertos expuestos

- **Puerto 80**: Nginx (acceso principal)
- **Puerto 5000**: API .NET directa (opcional)

## Endpoints disponibles

- `http://localhost/` - API principal
- `http://localhost/health` - Health check
- `http://localhost/swagger` - Documentación Swagger

## Variables de entorno importantes

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://+:5000`
- `ConnectionStrings__*` - Configuraciones de base de datos
- `JwtSettings__*` - Configuraciones JWT

## Archivos de configuración

- `Dockerfile` - Definición del contenedor
- `nginx.conf` - Configuración principal de Nginx
- `default.conf` - Configuración del sitio Nginx
- `supervisord.conf` - Configuración de Supervisor
- `start.sh` - Script de inicio del contenedor
- `docker-compose.yml` - Orquestación de servicios

## Logs

Los logs se almacenan en:
- Nginx: `/var/log/nginx/`
- Supervisor: `/var/log/supervisor/`
- Aplicación .NET: A través de supervisor

Para ver logs en tiempo real:
```bash
docker logs -f <container-name>
```

## Notas importantes

1. El contenedor se ejecuta automáticamente al iniciarse
2. Supervisor reinicia automáticamente los servicios si fallan
3. Nginx está configurado para manejar CORS
4. La aplicación está configurada para producción