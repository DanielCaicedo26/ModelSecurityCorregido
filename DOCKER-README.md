# ModelSecurityDa - Docker Setup

Este proyecto incluye una configuración completa de Docker con soporte para múltiples bases de datos.

## 🚀 Inicio Rápido

### Prerrequisitos
- Docker Desktop instalado
- Git (para clonar el proyecto)

### Iniciar el proyecto
```bash
# Opción 1: Script automático (Windows)
docker-start.bat

# Opción 2: Comando manual
docker-compose up --build -d
```

### Servicios Disponibles

| Servicio | URL/Puerto | Descripción |
|----------|------------|-------------|
| **API** | http://localhost:5112 | API Principal |
| **Swagger** | http://localhost:5112/swagger | Documentación API |
| **Health Check** | http://localhost:5112/health | Estado del sistema |
| **SQL Server** | localhost:1433 | Base de datos SQL Server |
| **PostgreSQL** | localhost:5432 | Base de datos PostgreSQL |
| **MySQL** | localhost:3306 | Base de datos MySQL |

## 🎯 Cambio Dinámico de Base de Datos

La aplicación permite cambiar entre diferentes bases de datos en tiempo real:

### Cambiar a SQL Server
```bash
curl -X POST "http://localhost:5112/api/database/set-engine/sqlserver"
```

### Cambiar a PostgreSQL
```bash
curl -X POST "http://localhost:5112/api/database/set-engine/postgres"
```

### Cambiar a MySQL
```bash
curl -X POST "http://localhost:5112/api/database/set-engine/mysql"
```

### Ver Motor Actual
```bash
curl "http://localhost:5112/api/database/current-engine"
```

## 📊 Monitoreo

### Ver logs de todos los servicios
```bash
docker-compose logs -f
```

### Ver logs específicos
```bash
# API
docker-compose logs -f web-api

# SQL Server
docker-compose logs -f sqlserver

# PostgreSQL  
docker-compose logs -f postgres

# MySQL
docker-compose logs -f mysql
```

### Verificar estado de salud
```bash
curl "http://localhost:5112/health"
```

## 🗄️ Gestión de Datos

### Datos Persistentes
Los datos se almacenan en volúmenes de Docker:
- `sqlserver_data`: Datos de SQL Server
- `postgres_data`: Datos de PostgreSQL  
- `mysql_data`: Datos de MySQL

### Backup de Datos
Los volúmenes persisten entre reinicios del contenedor.

### Limpiar Datos
```bash
# Detener servicios y eliminar volúmenes
docker-compose down -v

# O usar el script (Windows)
docker-stop.bat
```

## 🔧 Configuración

### Variables de Entorno
Las configuraciones principales están en `docker-compose.yml`:

```yaml
environment:
  ConnectionStrings__SqlServer: Server=sqlserver-db,1433;Database=...
  ConnectionStrings__Postgres: Host=postgres-db;Port=5432;Database=...
  ConnectionStrings__MySql: Server=mysql-db;Port=3306;Database=...
  DB_ENGINE: sqlserver  # Motor por defecto
```

### Credenciales por Defecto

| Base de Datos | Usuario | Contraseña |
|---------------|---------|------------|
| SQL Server | sa | StrongPwd!123 |
| PostgreSQL | postgres | StrongPwd!123 |
| MySQL | mysqluser | StrongPwd!123 |

## 🛠️ Comandos Útiles

### Desarrollo
```bash
# Reconstruir solo la API
docker-compose build web-api

# Reiniciar un servicio específico
docker-compose restart web-api

# Ver estado de servicios
docker-compose ps
```

### Troubleshooting
```bash
# Limpiar todo y empezar de cero
docker-compose down -v
docker system prune -f
docker-compose up --build -d

# Ver logs de errores
docker-compose logs web-api | grep -i error
```

## 📋 Testing

### Probar Endpoints
```bash
# Health check
curl http://localhost:5112/health

# Probar conexión a BD actual
curl http://localhost:5112/api/database/test-connection

# Ver motores disponibles
curl http://localhost:5112/api/database/available-engines
```

### Probar Cambio de Base de Datos
1. Cambiar motor: `POST /api/database/set-engine/postgres`
2. Crear datos en PostgreSQL
3. Cambiar motor: `POST /api/database/set-engine/mysql`  
4. Crear datos en MySQL
5. Verificar que los datos están separados por base de datos

## 🚨 Troubleshooting

### Problemas Comunes

**Error: Puerto ya en uso**
```bash
# Ver qué proceso usa el puerto
netstat -ano | findstr :5112

# Cambiar puerto en docker-compose.yml
ports:
  - "5113:8080"  # Usar puerto diferente
```

**Base de datos no conecta**
```bash
# Verificar que el contenedor esté corriendo
docker-compose ps

# Ver logs de la base de datos
docker-compose logs sqlserver
```

**API no inicia**
```bash
# Ver logs detallados
docker-compose logs web-api

# Verificar migraciones
docker-compose exec web-api dotnet ef database update
```

## 📞 Soporte

- Logs detallados: `docker-logs.bat`
- Health check: http://localhost:5112/health
- Swagger UI: http://localhost:5112/swagger