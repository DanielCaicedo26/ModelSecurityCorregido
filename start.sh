#!/bin/bash

# Crear directorios necesarios
mkdir -p /var/log/supervisor
mkdir -p /var/log/nginx

# Configurar Nginx
rm -f /etc/nginx/sites-enabled/default
ln -s /etc/nginx/sites-available/default /etc/nginx/sites-enabled/

# Verificar configuración de Nginx
nginx -t

# Iniciar supervisor que manejará Nginx y la aplicación .NET
exec /usr/bin/supervisord -c /etc/supervisor/conf.d/supervisord.conf