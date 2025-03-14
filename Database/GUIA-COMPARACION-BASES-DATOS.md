# Guía de Uso: Herramienta de Comparación de Bases de Datos SQL Server

## Introducción

Esta guía describe cómo utilizar la herramienta de comparación de bases de datos SQL Server (`compare_databases.py`) para identificar diferencias entre dos bases de datos. La herramienta permite comparar tablas, estructuras de columnas y procedimientos almacenados.

## Requisitos Previos

- Python 3.6 o superior
- Paquetes requeridos: `pyodbc`, `pandas`, `tabulate`, `colorama`
- Driver ODBC para SQL Server instalado

### Instalación de Dependencias

```bash
# Activar entorno virtual (si existe)
source venv/bin/activate  # En Linux/macOS
# o
venv\Scripts\activate     # En Windows

# Instalar dependencias
pip install pyodbc pandas tabulate colorama
```

### Instalación del Driver ODBC en macOS

```bash
# Instalar Homebrew (si no está instalado)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Instalar unixODBC y FreeTDS
brew install unixodbc freetds

# Instalar el driver oficial de Microsoft
# Para Apple Silicon (M1/M2)
curl -O https://download.microsoft.com/download/1/f/f/1fffb537-26ab-4947-a46a-7a45c27f6f77/msodbcsql18-18.3.2.1-aarch64.pkg
sudo installer -pkg msodbcsql18-18.3.2.1-aarch64.pkg -target /

# Para Intel Mac
curl -O https://download.microsoft.com/download/1/f/f/1fffb537-26ab-4947-a46a-7a45c27f6f77/msodbcsql18-18.3.2.1-x86_64.pkg
sudo installer -pkg msodbcsql18-18.3.2.1-x86_64.pkg -target /
```

## Comandos Básicos

### Ver Ayuda

```bash
python Database/tools/compare_databases.py
```

### Comparar Tablas

```bash
# Listar tablas que existen solo en una base de datos
python Database/tools/compare_databases.py --tables

# Comparar estructura de tablas con diferencias
python Database/tools/compare_databases.py --tables --table-details

# Comparar todas las tablas, incluso las que no tienen diferencias
python Database/tools/compare_databases.py --tables --table-details --all-tables
```

### Comparar Procedimientos Almacenados

```bash
# Listar procedimientos almacenados que existen solo en una base de datos
python Database/tools/compare_databases.py --procedures

# Mostrar diferencias en el código de los procedimientos
python Database/tools/compare_databases.py --procedures --show-sp-diff

# Exportar procedimientos diferentes a archivos
python Database/tools/compare_databases.py --procedures --export-sp ./sp_diff

# Comparar un procedimiento específico
python Database/tools/compare_databases.py --sp-name dbo.NombreProcedimiento --show-sp-diff
```

### Comparación Completa

```bash
# Comparar tablas y procedimientos
python Database/tools/compare_databases.py --all

# Comparación completa con todos los detalles
python Database/tools/compare_databases.py --all --table-details --show-sp-diff
```

### Exportar Resultados

```bash
# Guardar resultados en un archivo
python Database/tools/compare_databases.py --all --output resultados.txt

# Exportar resultados y procedimientos diferentes
python Database/tools/compare_databases.py --all --show-sp-diff --export-sp ./diff_output --output resultados.txt
```

## Ejemplos de Uso

### Escenario 1: Verificación Rápida de Diferencias

```bash
# Ver qué tablas y procedimientos son diferentes
python Database/tools/compare_databases.py --all
```

Este comando mostrará:

- Tablas que existen solo en una base de datos
- Procedimientos almacenados que existen solo en una base de datos
- Procedimientos almacenados que tienen diferencias (sin mostrar el código)

### Escenario 2: Análisis Detallado de Diferencias en Tablas

```bash
# Ver diferencias detalladas en la estructura de las tablas
python Database/tools/compare_databases.py --tables --table-details
```

Este comando mostrará:

- Tablas que existen solo en una base de datos
- Para cada tabla común con diferencias:
  - Columnas que existen solo en una base de datos
  - Columnas con diferencias en tipo de datos, nulabilidad o identidad

### Escenario 3: Análisis Detallado de Procedimientos Almacenados

```bash
# Ver diferencias en el código de los procedimientos
python Database/tools/compare_databases.py --procedures --show-sp-diff
```

Este comando mostrará:

- Procedimientos que existen solo en una base de datos
- Para cada procedimiento común con diferencias:
  - Fechas de modificación en ambas bases de datos
  - Diferencias específicas en el código (similar a git diff)

### Escenario 4: Exportar Procedimientos para Comparación Externa

```bash
# Exportar procedimientos diferentes a archivos
python Database/tools/compare_databases.py --procedures --export-sp ./sp_diff
```

Este comando:

- Exportará los procedimientos diferentes a archivos separados en el directorio `./sp_diff`
- Creará dos archivos por cada procedimiento (uno por cada base de datos)
- Permitirá usar herramientas externas para comparar los archivos

### Escenario 5: Análisis Completo y Documentación

```bash
# Análisis completo con exportación de resultados
python Database/tools/compare_databases.py --all --table-details --show-sp-diff --export-sp ./diff_output --output informe_diferencias.txt
```

Este comando:

- Realizará una comparación completa de tablas y procedimientos
- Mostrará detalles de las diferencias en tablas y procedimientos
- Exportará los procedimientos diferentes a archivos
- Guardará todos los resultados en un archivo de texto para documentación

## Configuración de Bases de Datos

La herramienta está configurada para conectarse a:

1. **Base de datos Azure**:

   - Servidor: aesanserver.database.windows.net
   - Base de datos: aesan2
   - Usuario: aesan
   - Contraseña: 4ubi4XFygLMxXU

2. **Base de datos Local**:
   - Servidor: 192.168.1.144
   - Base de datos: NUTRE
   - Usuario: sa
   - Contraseña: d@rio152325

Para cambiar estas configuraciones, modifica las variables `db1_config` y `db2_config` en el script.

## Solución de Problemas

### Error de Driver ODBC

Si aparece el error "Can't open lib 'ODBC Driver 18 for SQL Server'":

1. Verifica los drivers ODBC instalados:

   ```bash
   odbcinst -j
   cat /usr/local/etc/odbcinst.ini
   ```

2. Modifica el script para usar un driver disponible:
   ```python
   # Cambiar esta línea en get_connection()
   connection_string = f"DRIVER={{ODBC Driver 17 for SQL Server}};"
   # o
   connection_string = f"DRIVER={{FreeTDS}};"
   ```

### Error de Conexión

Si hay problemas para conectarse a las bases de datos:

1. Verifica la accesibilidad de red:

   ```bash
   ping aesanserver.database.windows.net
   ping 192.168.1.144
   ```

2. Verifica las credenciales y permisos de usuario

3. Para servidores locales, verifica que SQL Server:
   - Tenga habilitadas las conexiones TCP/IP
   - Esté escuchando en el puerto correcto (generalmente 1433)
   - Permita autenticación SQL Server

### Error de Encriptación

Si aparece el error "Invalid value specified for connection string attribute 'Encrypt'":

```python
# Usar estos valores en la cadena de conexión
connection_string += "Encrypt=yes;TrustServerCertificate=yes;"  # Para Azure
connection_string += "Encrypt=no;TrustServerCertificate=yes;"   # Para servidor local
```

## Herramientas Externas para Comparar Archivos

Después de exportar los procedimientos almacenados a archivos, puedes usar estas herramientas para una comparación más detallada:

1. **Visual Studio Code**:

   - Abre VS Code
   - Abre el primer archivo
   - Haz clic derecho y selecciona "Select for Compare"
   - Abre el segundo archivo
   - Haz clic derecho y selecciona "Compare with Selected"

2. **Beyond Compare**:

   - Abre Beyond Compare
   - Selecciona "Compare Files"
   - Selecciona los dos archivos a comparar
   - Haz clic en "Compare"

3. **Meld**:

   ```bash
   meld ./sp_diff/procedimiento_db1.sql ./sp_diff/procedimiento_db2.sql
   ```

4. **Comando diff**:
   ```bash
   diff -u ./sp_diff/procedimiento_db1.sql ./sp_diff/procedimiento_db2.sql
   ```

---

Esta guía proporciona una referencia completa para utilizar la herramienta de comparación de bases de datos SQL Server. Para más información o soporte, contacta al equipo de desarrollo.
