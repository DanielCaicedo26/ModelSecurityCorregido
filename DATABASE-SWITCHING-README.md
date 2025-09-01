# Cambio Dinámico de Base de Datos 🔄

Esta aplicación soporta el cambio dinámico entre 3 motores de base de datos:
- **SQL Server** (Puerto 1434)
- **PostgreSQL** (Puerto 5433)
- **MySQL** (Puerto 3307)

## 🚀 Inicio Rápido

### 1. Levantar todos los servicios
```bash
docker-compose -f docker-compose.light.yml up -d
```

### 2. Verificar que todos los contenedores estén ejecutándose
```bash
docker-compose -f docker-compose.light.yml ps
```

### 3. Esperar a que la aplicación complete las migraciones
La aplicación automáticamente:
- ✅ Migra todas las 3 bases de datos al inicio
- ✅ Crea el mismo esquema en SQL Server, PostgreSQL y MySQL
- ✅ Prepara la aplicación para cambio dinámico

## 🔧 API Endpoints para Cambio de Base de Datos

### Ver motor actual
```bash
curl http://localhost:5113/api/database/current-engine
```

### Cambiar a SQL Server
```bash
curl -X POST http://localhost:5113/api/database/set-engine/sqlserver
```

### Cambiar a PostgreSQL
```bash
curl -X POST http://localhost:5113/api/database/set-engine/postgres
```

### Cambiar a MySQL
```bash
curl -X POST http://localhost:5113/api/database/set-engine/mysql
```

### Probar conexión actual
```bash
curl http://localhost:5113/api/database/test-connection
```

### Ver motores disponibles
```bash
curl http://localhost:5113/api/database/available-engines
```

## 📊 Swagger UI

Accede a la documentación interactiva en:
```
http://localhost:5113/swagger
```

Aquí podrás probar todos los endpoints de cambio de base de datos desde la interfaz web.

## 🔍 Verificación Manual

### Conectar directamente a cada base de datos:

**PostgreSQL:**
```bash
docker exec -it postgres-db psql -U postgres -d modelsecurityda
```

**MySQL:**
```bash
docker exec -it mysql-db mysql -u mysqluser -p modelsecurityda
# Password: StrongPwd!123
```

**SQL Server:**
```bash
docker exec -it sqlserver-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongPwd!123 -C
```

## 🛠️ Scripts Adicionales

### Sincronizar datos entre bases de datos
```bash
docker-compose exec web-api ./sync-databases.sh
```

Este script:
- ✅ Verifica conectividad a las 3 BD
- ✅ Aplica migraciones en todas
- ✅ Crea datos de muestra (si es necesario)
- ✅ Verifica sincronización

## 📝 Uso en Aplicación

1. **Al iniciar**: La aplicación usa el motor especificado en `DB_ENGINE` (por defecto: postgres)
2. **En tiempo de ejecución**: Usa la API para cambiar entre motores
3. **Persistencia**: Los cambios son temporales (reinicio vuelve al motor por defecto)

## ⚠️ Consideraciones Importantes

### Datos
- Cada base de datos mantiene sus propios datos
- El cambio de motor **NO** sincroniza datos automáticamente
- Para datos consistentes, usa el script `sync-databases.sh`

### Rendimiento
- El cambio de motor es **inmediato**
- Sin necesidad de reiniciar la aplicación
- Cada motor mantiene su propio pool de conexiones

### Conexiones
- La aplicación mantiene contextos separados para cada motor
- Las conexiones se crean bajo demanda
- Healthchecks verifican la disponibilidad de cada motor

## 🐛 Troubleshooting

### Error de conexión
```bash
# Verificar estado de contenedores
docker-compose -f docker-compose.light.yml ps

# Ver logs específicos
docker-compose -f docker-compose.light.yml logs postgres-db
docker-compose -f docker-compose.light.yml logs mysql-db
docker-compose -f docker-compose.light.yml logs sqlserver-db
```

### Error en migraciones
```bash
# Ver logs de la aplicación
docker-compose -f docker-compose.light.yml logs web-api

# Ejecutar manualmente migraciones
docker-compose exec web-api ./sync-databases.sh
```

### Puertos ocupados
Los puertos han sido configurados para evitar conflictos:
- API: `5113` (era 5112)
- PostgreSQL: `5433` (era 5432)
- MySQL: `3307` (era 3306)
- SQL Server: `1434` (era 1433)

## 🎯 Casos de Uso

### Desarrollo
- Probar compatibilidad entre motores
- Validar migraciones en múltiples BD
- Benchmark de rendimiento por motor

### Testing
- Tests de integración con diferentes motores
- Validación de queries específicas por BD
- Testing de failover entre motores

### Producción
- Migración gradual entre motores
- A/B testing con diferentes BD
- Backup/restore entre diferentes motores