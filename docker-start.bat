@echo off
echo ======================================
echo   ModelSecurityDa - Docker Startup
echo ======================================

echo.
echo Construyendo e iniciando contenedores...
docker-compose up --build -d

echo.
echo Esperando que las bases de datos inicien...
timeout /t 60 /nobreak > nul

echo.
echo Estado de los servicios:
docker-compose ps

echo.
echo ======================================
echo   Servicios disponibles:
echo ======================================
echo   API: http://localhost:5112
echo   Swagger: http://localhost:5112/swagger
echo   Health Check: http://localhost:5112/health
echo.
echo   SQL Server: localhost:1433
echo   PostgreSQL: localhost:5432  
echo   MySQL: localhost:3306
echo ======================================

echo.
echo Para cambiar de base de datos, usa:
echo   POST http://localhost:5112/api/database/set-engine/sqlserver
echo   POST http://localhost:5112/api/database/set-engine/postgres
echo   POST http://localhost:5112/api/database/set-engine/mysql

pause