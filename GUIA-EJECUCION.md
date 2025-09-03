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

## Troubleshooting (Solución de Problemas)

### Problema: "longitud del contenido: 0" en el navegador

**Síntomas:**
- La API devuelve respuestas vacías
- Status 500 Internal Server Error
- Content-Length: 0

**Causas y Soluciones:**

1. **Base de datos desconectada**
   ```bash
   # Verificar health check
   curl http://localhost:8888/health
   
   # Si muestra "Unhealthy", revisar conexiones BD
   docker logs sqlserver-db
   docker logs postgres-db
   ```

2. **Contenedores no están en la misma red**
   ```bash
   # Usar docker-compose en lugar de docker run individual
   docker-compose up -d
   
   # NO hacer: docker run -p 8888:80 modelsecurityda-app
   ```

3. **Conflictos de puertos**
   ```bash
   # Verificar puertos ocupados
   netstat -ano | findstr :80
   netstat -ano | findstr :3306
   
   # Cambiar puertos en docker-compose.yml si es necesario
   ```

### Problema: "502 Bad Gateway"

**Síntomas:**
- Nginx muestra 502 Bad Gateway
- La aplicación no responde

**Soluciones:**
```bash
# 1. Verificar que la aplicación .NET esté corriendo
docker exec modelsecurityda-nginx ps aux | grep dotnet

# 2. Verificar logs del contenedor
docker logs modelsecurityda-nginx --tail 20

# 3. Reiniciar el contenedor
docker-compose restart app
```

### Problema: MySQL no inicia (Puerto 3306 ocupado)

**Síntomas:**
- Error: "bind: Solo se permite un uso de cada dirección de socket"
- MySQL ya instalado en el host

**Soluciones:**

**Opción 1: Deshabilitar dependencia de MySQL**
```yaml
# En docker-compose.yml, comentar:
depends_on:
  # mysql:
  #   condition: service_healthy
```

**Opción 2: Cambiar puerto de MySQL**
```yaml
mysql:
  ports:
    - "3307:3306"  # Cambiar de 3306 a 3307
```

**Opción 3: Parar MySQL local**
```bash
# Windows (como administrador)
net stop mysql80

# O usar solo PostgreSQL y SQL Server
docker-compose up app sqlserver postgres -d
```

### Problema: Puerto 80 ocupado

**Síntomas:**
- Error: "listen tcp 0.0.0.0:80: bind: Intento de acceso a un socket no permitido"

**Solución:**
```yaml
# En docker-compose.yml cambiar:
ports:
  - "8888:80"  # En lugar de "80:80"
  - "5001:5000" # En lugar de "5000:5000"
```

### Problema: Aplicación no se conecta a las BD

**Síntomas:**
- Health check muestra "Unhealthy"
- API devuelve errores 500

**Verificaciones:**
```bash
# 1. Verificar que las BD estén saludables
docker ps

# 2. Verificar conexiones desde el contenedor
docker exec modelsecurityda-nginx ping sqlserver-db
docker exec modelsecurityda-nginx ping postgres-db

# 3. Verificar variables de entorno
docker exec modelsecurityda-nginx env | grep ConnectionStrings
```

### Problema: Swagger no se carga

**Síntomas:**
- /swagger devuelve página en blanco
- /swagger redirige incorrectamente

**Soluciones:**
```bash
# Acceder directamente por el puerto de .NET
curl http://localhost:5001/swagger

# O verificar configuración de Nginx
docker exec modelsecurityda-nginx cat /etc/nginx/sites-available/default
```

## Comandos de Diagnóstico

### Ver estado completo
```bash
# Estado de contenedores
docker ps -a

# Uso de puertos
netstat -ano | findstr :8888
netstat -ano | findstr :5001
netstat -ano | findstr :1433
netstat -ano | findstr :5432

# Logs completos
docker-compose logs
```

### Limpiar y empezar de nuevo
```bash
# Parar todo
docker-compose down

# Limpiar contenedores huérfanos
docker container prune

# Limpiar redes no usadas
docker network prune

# Reiniciar todo
docker-compose up -d
```

## Configuraciones Importantes Realizadas

### 1. Modificaciones en docker-compose.yml

**Puertos cambiados:**
- `80:80` → `8888:80`
- `5000:5000` → `5001:5000`

**Dependencia de MySQL comentada:**
```yaml
depends_on:
  sqlserver:
    condition: service_healthy
  postgres:
    condition: service_healthy
  # mysql:
  #   condition: service_healthy
```

### 2. Configuración de Red

Los contenedores están en la red `modelsecurity-network` y se comunican usando:
- `sqlserver-db:1433`
- `postgres-db:5432`
- `mysql-db:3306`

### 3. Variables de Entorno

Las connection strings están configuradas para usar nombres de contenedores:
```
ConnectionStrings__SqlServer=Server=sqlserver-db,1433;Database=ModelSecurityDa;User Id=sa;Password=StrongPwd!123;Encrypt=True;TrustServerCertificate=True;
```

## Verificación Final

Para confirmar que todo funciona:

```bash
# 1. Health check
curl http://localhost:8888/health
# Debe devolver: {"status":"Healthy","database":"Connected"...}

# 2. API con SQL Server
curl http://localhost:8888/api/Person -H "x-database-engine: sqlserver"
# Debe devolver: JSON con array de personas

# 3. API con PostgreSQL
curl http://localhost:8888/api/Person -H "x-database-engine: postgres"
# Debe devolver: JSON con array de personas

# 4. Swagger
curl http://localhost:8888/swagger
# Debe devolver: HTML de Swagger UI
```

Si todos estos comandos funcionan, ¡la aplicación está correctamente configurada! 🎉
