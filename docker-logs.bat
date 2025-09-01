@echo off
echo ======================================
echo   ModelSecurityDa - Docker Logs
echo ======================================

echo.
echo Selecciona el servicio para ver logs:
echo   1. API (modelsecurityda-api)
echo   2. SQL Server (sqlserver-db)
echo   3. PostgreSQL (postgres-db)
echo   4. MySQL (mysql-db)
echo   5. Todos los servicios
echo.

set /p choice="Ingresa tu opción (1-5): "

if "%choice%"=="1" (
    echo.
    echo === LOGS DE LA API ===
    docker-compose logs -f web-api
) else if "%choice%"=="2" (
    echo.
    echo === LOGS DE SQL SERVER ===
    docker-compose logs -f sqlserver
) else if "%choice%"=="3" (
    echo.
    echo === LOGS DE POSTGRESQL ===
    docker-compose logs -f postgres
) else if "%choice%"=="4" (
    echo.
    echo === LOGS DE MYSQL ===
    docker-compose logs -f mysql
) else if "%choice%"=="5" (
    echo.
    echo === LOGS DE TODOS LOS SERVICIOS ===
    docker-compose logs -f
) else (
    echo Opción inválida.
    pause
)