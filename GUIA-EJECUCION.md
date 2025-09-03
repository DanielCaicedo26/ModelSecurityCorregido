# Guía de Ejecución - ModelSecurityDa

## Resumen del Proyecto

**ModelSecurityDa** es una aplicación de seguridad de modelos y facturación desarrollada en ASP.NET Core 9.0 que se ejecuta en contenedores Docker con Ubuntu.

### Características principales:
- ✅ API de seguridad y facturación con Swagger
- ✅ Multi-base de datos: SQL Server, PostgreSQL, MySQL
- ✅ Contenedor Docker con Ubuntu 22.04 + Nginx + .NET 9.0
- ✅ Switching dinámico de BD usando header `X-Database-Engine`
- ✅ Autenticación JWT
- ✅ Proxy inverso con Nginx

## Estructura del Proyecto

```
ModelSecurityCorregido/
├── Web2/                    # Aplicación ASP.NET Core
├── Bussines/               # Lógica de negocio
├── Data/                   # Repositorios y contextos
├── Utilities/              # Utilidades
├── docker-compose.yml      # Orquestación de servicios
├── Dockerfile             # Definición del contenedor
├── nginx.conf             # Configuración Nginx
├── default.conf           # Configuración sitio Nginx
├── supervisord.conf       # Configuración Supervisor
└── start.sh              # Script de inicio
```

## Pasos de Ejecución

### 1. Verificar Requisitos Previos

```bash
# Verificar Docker
docker --version
docker-compose --version

# Verificar puertos disponibles
netstat -ano | findstr :80
netstat -ano | findstr :3306
netstat -ano | findstr :1433
netstat -ano | findstr :5432
```

### 2. Preparar Configuración

**Archivo importante:** `docker-compose.yml`

- **Puerto 80**: Cambiar a 8888 si está ocupado
- **Puerto 3306**: MySQL puede conflictuar si ya tienes uno instalado
- **Dependencias**: Comentar MySQL si hay conflictos

### 3. Levantar las Bases de Datos

```bash
# Desde el directorio del proyecto
cd C:\Users\User\OneDrive\Desktop\ModelSecurityCorregido

# Levantar solo las bases de datos
docker-compose up sqlserver postgres -d
```

### 4. Verificar Estado de las Bases de Datos

```bash
# Ver contenedores corriendo
docker ps

# Verificar logs de las BD
docker logs sqlserver-db
docker logs postgres-db
```

### 5. Levantar la Aplicación

```bash
# Levantar la aplicación (después de que las BD estén saludables)
docker-compose up app -d
```

### 6. Verificar que Todo Funcione

```bash
# Test de health check
curl http://localhost:8888/health

# Test de la API
curl http://localhost:8888/api/Person -H "x-database-engine: sqlserver"
curl http://localhost:8888/api/Person -H "x-database-engine: postgres"
```

## URLs de Acceso

Una vez que todo esté funcionando:

- **Health Check**: `http://localhost:8888/health`
- **API Principal**: `http://localhost:8888/api/Person`
- **Swagger**: `http://localhost:8888/swagger`
- **API Directa .NET**: `http://localhost:5001/api/Person`

## Headers Importantes

Para cambiar de base de datos, usar el header:

```
X-Database-Engine: sqlserver   # SQL Server
X-Database-Engine: postgres    # PostgreSQL  
X-Database-Engine: mysql       # MySQL (si está disponible)
```

## Comandos Útiles

### Ver Logs en Tiempo Real
```bash
# Logs del contenedor de la aplicación
docker logs modelsecurityda-nginx -f

# Logs de una base de datos específica
docker logs sqlserver-db -f
docker logs postgres-db -f
```

### Reiniciar Servicios
```bash
# Reiniciar todo
docker-compose down
docker-compose up -d

# Reiniciar solo la aplicación
docker-compose restart app
```

### Parar Todo
```bash
# Parar todos los servicios
docker-compose down

# Parar y eliminar volúmenes
docker-compose down -v
```

## Configuración de Base de Datos

Las conexiones están configuradas en el `docker-compose.yml`:

```yaml
ConnectionStrings__SqlServer: Server=sqlserver-db,1433;Database=ModelSecurityDa;User Id=sa;Password=StrongPwd!123;Encrypt=True;TrustServerCertificate=True;
ConnectionStrings__Postgres: Host=postgres-db;Port=5432;Database=modelsecurityda;Username=postgres;Password=StrongPwd!123;
ConnectionStrings__MySql: Server=mysql-db;Port=3306;Database=modelsecurityda;User=mysqluser;Password=StrongPwd!123;
```

**Nota**: Los nombres de host (`sqlserver-db`, `postgres-db`, `mysql-db`) son importantes para la comunicación entre contenedores.
