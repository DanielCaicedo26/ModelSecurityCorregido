@echo off
echo ======================================
echo   ModelSecurityDa - Docker Shutdown
echo ======================================

echo.
echo Deteniendo todos los contenedores...
docker-compose down

echo.
echo Contenedores detenidos exitosamente.
echo.

set /p cleanup="¿Deseas eliminar también los volúmenes de datos? (y/N): "
if /i "%cleanup%"=="y" (
    echo.
    echo Eliminando volúmenes de datos...
    docker-compose down -v
    docker volume prune -f
    echo Volúmenes eliminados.
) else (
    echo Los datos se mantienen para el próximo inicio.
)

echo.
echo ======================================
echo   Shutdown completado
echo ======================================

pause