-- Script para verificar tablas en las diferentes bases de datos

-- SQL Server
SELECT 'SQL_SERVER' as DB_TYPE, TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE_TABLE' 
AND TABLE_NAME != '__EFMigrationsHistory'
ORDER BY TABLE_NAME;

-- PostgreSQL (ejecutar por separado)
-- SELECT 'POSTGRES' as db_type, table_name 
-- FROM information_schema.tables 
-- WHERE table_schema = 'public' 
-- AND table_name != '__EFMigrationsHistory'
-- ORDER BY table_name;

-- MySQL (ejecutar por separado)
-- SELECT 'MYSQL' as db_type, TABLE_NAME 
-- FROM INFORMATION_SCHEMA.TABLES 
-- WHERE TABLE_SCHEMA = DATABASE() 
-- AND TABLE_NAME != '__EFMigrationsHistory'
-- ORDER BY TABLE_NAME;