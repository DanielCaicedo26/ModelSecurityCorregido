
#!/bin/sh
set -e

echo "=== INICIANDO APLICACIÓN ==="
echo "Timestamp: $(date)"


# Variables de entorno para la base de datos
DB_ENGINE="${DB_ENGINE:-sqlserver}"
DB_SERVER="${DB_SERVER:-nombre-contenedor-bd}"
DB_NAME="${DB_NAME:-ModelSecurityDa}"
DB_USER="${DB_USER:-sa}"
DB_PASSWORD="${DB_PASSWORD:-StrongPwd!123}"


echo "Configuración de BD:"
echo "  Motor: $DB_ENGINE"
echo "  Servidor: $DB_SERVER"
echo "  Base de datos: $DB_NAME"
echo "  Usuario: $DB_USER"


# Función para verificar conectividad con el motor seleccionado
wait_for_db() {
    echo "Esperando a que la base de datos ($DB_ENGINE) esté disponible..."
    local max_attempts=30
    local attempt=1
    while [ $attempt -le $max_attempts ]; do
        echo "Intento $attempt/$max_attempts de conectar a $DB_ENGINE..."
        case "$DB_ENGINE" in
            sqlserver)
                if /opt/mssql-tools18/bin/sqlcmd -S "$DB_SERVER,1433" -U "$DB_USER" -P "$DB_PASSWORD" -Q "SELECT 1" -C > /dev/null 2>&1; then
                    echo "✓ SQL Server está listo!"
                    return 0
                fi
                ;;
            postgres)
                if PGPASSWORD="$DB_PASSWORD" psql -h "$DB_SERVER" -U "$DB_USER" -d "$DB_NAME" -c "SELECT 1;" > /dev/null 2>&1; then
                    echo "✓ PostgreSQL está listo!"
                    return 0
                fi
                ;;
            mysql)
                if mysqladmin ping -h "$DB_SERVER" -u "$DB_USER" -p"$DB_PASSWORD" --silent; then
                    echo "✓ MySQL está listo!"
                    return 0
                fi
                ;;
            *)
                echo "❌ Motor de base de datos no soportado: $DB_ENGINE"
                exit 1
                ;;
        esac
        echo "$DB_ENGINE no disponible, esperando 10 segundos..."
        sleep 10
        attempt=$((attempt + 1))
    done
    echo "❌ ERROR: No se pudo conectar a $DB_ENGINE después de $max_attempts intentos"
    exit 1
}


# Función para ejecutar migraciones (opcional, aquí solo ejemplo para SQL Server)
run_migrations() {
    echo "=== EJECUTANDO MIGRACIONES (opcional) ==="
    case "$DB_ENGINE" in
        sqlserver)
            echo "Verificando/creando base de datos en SQL Server..."
            /opt/mssql-tools18/bin/sqlcmd -S "$DB_SERVER,1433" -U "$DB_USER" -P "$DB_PASSWORD" -C -Q "
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '$DB_NAME')
            BEGIN
                CREATE DATABASE [$DB_NAME]
                PRINT 'Base de datos $DB_NAME creada'
            END
            ELSE
                PRINT 'Base de datos $DB_NAME ya existe'
            " || { echo "❌ ERROR: No se pudo crear/verificar la base de datos"; exit 1; }
            ;;
        postgres)
            echo "Verificando/creando base de datos en PostgreSQL..."
            PGPASSWORD="$DB_PASSWORD" psql -h "$DB_SERVER" -U "$DB_USER" -tc "SELECT 1 FROM pg_database WHERE datname = '$DB_NAME';" | grep -q 1 || \
            PGPASSWORD="$DB_PASSWORD" psql -h "$DB_SERVER" -U "$DB_USER" -c "CREATE DATABASE \"$DB_NAME\";" || { echo "❌ ERROR: No se pudo crear/verificar la base de datos"; exit 1; }
            ;;
        mysql)
            echo "Verificando/creando base de datos en MySQL..."
            mysql -h "$DB_SERVER" -u "$DB_USER" -p"$DB_PASSWORD" -e "CREATE DATABASE IF NOT EXISTS \`$DB_NAME\`;" || { echo "❌ ERROR: No se pudo crear/verificar la base de datos"; exit 1; }
            ;;
        *)
            echo "❌ Motor de base de datos no soportado para migraciones: $DB_ENGINE"
            exit 1
            ;;
    esac
    echo "✓ Base de datos verificada/creada exitosamente"
    echo "Las migraciones de Entity Framework se aplicarán automáticamente cuando la aplicación inicie"
}


# Esperar a la base de datos
wait_for_db

# Ejecutar migraciones
run_migrations

echo "=== INICIANDO APLICACIÓN WEB ==="
echo "La aplicación estará disponible en http://localhost:8080"

# Iniciar la aplicación
exec "$@"