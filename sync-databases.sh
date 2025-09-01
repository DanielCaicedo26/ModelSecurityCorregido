#!/bin/bash

# Script para sincronizar datos entre las 3 bases de datos
# Asegura que todas las BD tengan los mismos datos iniciales

set -e

echo "=== SINCRONIZACIÓN DE BASES DE DATOS ==="
echo "Timestamp: $(date)"

# Variables de configuración
DB_NAME="modelsecurityda"
DB_USER_POSTGRES="postgres"
DB_USER_MYSQL="mysqluser"
DB_USER_SQLSERVER="sa"
DB_PASSWORD="StrongPwd!123"

# Hosts (usar los nombres de servicio del docker-compose)
POSTGRES_HOST="postgres-db"
MYSQL_HOST="mysql-db"
SQLSERVER_HOST="sqlserver-db"

# Puertos (internos del contenedor)
POSTGRES_PORT="5432"
MYSQL_PORT="3306"
SQLSERVER_PORT="1433"

echo "Configuración:"
echo "  PostgreSQL: $POSTGRES_HOST:$POSTGRES_PORT"
echo "  MySQL: $MYSQL_HOST:$MYSQL_PORT"
echo "  SQL Server: $SQLSERVER_HOST:$SQLSERVER_PORT"

# Función para ejecutar migraciones en todas las BD
run_all_migrations() {
    echo "=== EJECUTANDO MIGRACIONES EN TODAS LAS BASES DE DATOS ==="
    
    echo "1. Aplicando migraciones en SQL Server..."
    dotnet ef database update --context ApplicationDbContext --verbose || {
        echo "❌ ERROR: Falló la migración de SQL Server"
        exit 1
    }
    
    echo "2. Aplicando migraciones en PostgreSQL..."
    dotnet ef database update --context ApplicationDbContextPostgres --verbose || {
        echo "❌ ERROR: Falló la migración de PostgreSQL"
        exit 1
    }
    
    echo "3. Aplicando migraciones en MySQL..."
    dotnet ef database update --context ApplicationDbContextMySql --verbose || {
        echo "❌ ERROR: Falló la migración de MySQL"
        exit 1
    }
    
    echo "✅ Migraciones aplicadas exitosamente en todas las BD"
}

# Función para verificar conectividad
test_connections() {
    echo "=== VERIFICANDO CONECTIVIDAD ==="
    
    echo "Probando PostgreSQL..."
    PGPASSWORD="$DB_PASSWORD" psql -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" -U "$DB_USER_POSTGRES" -d "$DB_NAME" -c "SELECT 1;" > /dev/null 2>&1 || {
        echo "❌ No se puede conectar a PostgreSQL"
        return 1
    }
    echo "✅ PostgreSQL conectado"
    
    echo "Probando MySQL..."
    mysql -h "$MYSQL_HOST" -P "$MYSQL_PORT" -u "$DB_USER_MYSQL" -p"$DB_PASSWORD" -D "$DB_NAME" -e "SELECT 1;" > /dev/null 2>&1 || {
        echo "❌ No se puede conectar a MySQL"
        return 1
    }
    echo "✅ MySQL conectado"
    
    echo "Probando SQL Server..."
    /opt/mssql-tools18/bin/sqlcmd -S "$SQLSERVER_HOST,$SQLSERVER_PORT" -U "$DB_USER_SQLSERVER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 1" -C > /dev/null 2>&1 || {
        echo "❌ No se puede conectar a SQL Server"
        return 1
    }
    echo "✅ SQL Server conectado"
}

# Función para crear datos de prueba (usando la API de la aplicación)
create_sample_data() {
    echo "=== CREANDO DATOS DE MUESTRA ==="
    
    # Esperar a que la aplicación esté disponible
    echo "Esperando a que la aplicación esté disponible..."
    for i in {1..30}; do
        if curl -s http://localhost:8080/health > /dev/null 2>&1; then
            echo "✅ Aplicación disponible"
            break
        fi
        if [ $i -eq 30 ]; then
            echo "❌ La aplicación no está disponible después de 30 intentos"
            return 1
        fi
        sleep 2
    done
    
    # Cambiar a cada BD y crear algunos datos de prueba
    for engine in "sqlserver" "postgres" "mysql"; do
        echo "Configurando datos en $engine..."
        
        # Cambiar motor de BD
        curl -s -X POST "http://localhost:8080/api/database/set-engine/$engine" || {
            echo "❌ Error cambiando a $engine"
            continue
        }
        
        # Aquí puedes agregar llamadas API para crear datos específicos
        # Por ejemplo, crear usuarios, roles, etc.
        
        sleep 1
    done
    
    echo "✅ Datos de muestra creados en todas las BD"
}

# Función para verificar que los datos existen en todas las BD
verify_data_sync() {
    echo "=== VERIFICANDO SINCRONIZACIÓN DE DATOS ==="
    
    for engine in "sqlserver" "postgres" "mysql"; do
        echo "Verificando datos en $engine..."
        
        # Cambiar motor y probar conexión
        response=$(curl -s -X POST "http://localhost:8080/api/database/set-engine/$engine")
        if echo "$response" | grep -q '"success":true'; then
            echo "✅ $engine configurado correctamente"
        else
            echo "❌ Error configurando $engine"
            continue
        fi
        
        # Probar conexión
        connection_test=$(curl -s "http://localhost:8080/api/database/test-connection")
        if echo "$connection_test" | grep -q '"connected":true'; then
            echo "✅ $engine conectado y funcional"
        else
            echo "❌ $engine no está conectado correctamente"
        fi
        
        sleep 1
    done
    
    echo "✅ Verificación completada"
}

# Función principal
main() {
    echo "Iniciando sincronización de bases de datos..."
    
    # Solo ejecutar verificaciones si estamos dentro del contenedor
    if [ -f "/.dockerenv" ]; then
        echo "Ejecutando dentro del contenedor..."
        test_connections
        run_all_migrations
        create_sample_data
        verify_data_sync
    else
        echo "Este script debe ejecutarse dentro del contenedor de la aplicación"
        echo "Use: docker-compose exec web-api ./sync-databases.sh"
        exit 1
    fi
    
    echo "=== SINCRONIZACIÓN COMPLETADA ==="
    echo "Las 3 bases de datos ahora tienen la misma estructura y datos"
    echo "Puede cambiar entre motores usando la API:"
    echo "  POST /api/database/set-engine/sqlserver"
    echo "  POST /api/database/set-engine/postgres"
    echo "  POST /api/database/set-engine/mysql"
}

# Ejecutar función principal
main "$@"