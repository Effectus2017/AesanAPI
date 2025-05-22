# Guía de Uso: Herramienta para Eliminar Usuarios Duplicados en AspNetIdentity

## Introducción

Esta guía describe cómo utilizar la herramienta `eliminar_usuarios_duplicados.py` para identificar y eliminar usuarios duplicados en las tablas de AspNetIdentity de SQL Server. La herramienta permite encontrar usuarios con el mismo email o username, analizar sus detalles y eliminar los duplicados manteniendo el usuario más adecuado.

## Requisitos Previos

- Python 3.6 o superior
- Paquetes requeridos: `pyodbc`, `pandas`, `colorama`
- Driver ODBC para SQL Server instalado

### Instalación de Dependencias

```bash
# Activar entorno virtual (si existe)
source venv/bin/activate  # En Linux/macOS
# o
venv\Scripts\activate     # En Windows

# Instalar dependencias
pip install pyodbc pandas colorama
```

### Instalación del Driver ODBC en macOS

Si aún no tienes instalado el driver ODBC para SQL Server, sigue las instrucciones en la [Guía de Comparación de Bases de Datos](GUIA-COMPARACION-BASES-DATOS.md#instalación-del-driver-odbc-en-macos).

## Comandos Básicos

### Ver Ayuda

```bash
python Database/tools/eliminar_usuarios_duplicados.py
```

### Buscar Usuarios Duplicados

```bash
# Buscar usuarios duplicados en la base de datos local
python Database/tools/eliminar_usuarios_duplicados.py --local --find

# Buscar usuarios duplicados en la base de datos de Azure
python Database/tools/eliminar_usuarios_duplicados.py --azure --find
```

### Eliminar Usuarios Duplicados Automáticamente

```bash
# Eliminar automáticamente todos los usuarios duplicados en la base de datos local
python Database/tools/eliminar_usuarios_duplicados.py --local --find --clean

# Eliminar automáticamente todos los usuarios duplicados en la base de datos de Azure
python Database/tools/eliminar_usuarios_duplicados.py --azure --find --clean
```

### Trabajar con un Usuario Específico

```bash
# Buscar usuarios con un email específico
python Database/tools/eliminar_usuarios_duplicados.py --local --email "usuario@ejemplo.com"

# Buscar usuarios con un username específico
python Database/tools/eliminar_usuarios_duplicados.py --local --username "nombreusuario"

# Eliminar duplicados de un email específico
python Database/tools/eliminar_usuarios_duplicados.py --local --email "usuario@ejemplo.com" --clean

# Eliminar duplicados de un username específico
python Database/tools/eliminar_usuarios_duplicados.py --local --username "nombreusuario" --clean
```

### Especificar Manualmente el Usuario a Mantener

```bash
# Eliminar duplicados manteniendo un usuario específico por ID
python Database/tools/eliminar_usuarios_duplicados.py --local --email "usuario@ejemplo.com" --keep-id "id-del-usuario" --clean
```

## Ejemplos de Uso

### Escenario 1: Análisis de Usuarios Duplicados

```bash
# Ver qué usuarios tienen emails o usernames duplicados
python Database/tools/eliminar_usuarios_duplicados.py --local --find
```

Este comando mostrará:

- Usuarios con emails duplicados
- Usuarios con usernames duplicados
- Detalles completos de cada usuario duplicado

### Escenario 2: Limpieza Automática de Duplicados

```bash
# Eliminar automáticamente todos los usuarios duplicados
python Database/tools/eliminar_usuarios_duplicados.py --local --find --clean
```

Este comando:

- Buscará todos los usuarios duplicados
- Seleccionará automáticamente qué usuario mantener basándose en criterios predefinidos
- Eliminará los usuarios duplicados manteniendo todas las referencias

### Escenario 3: Análisis y Limpieza de un Usuario Específico

```bash
# Analizar y eliminar duplicados de un email específico
python Database/tools/eliminar_usuarios_duplicados.py --local --email "usuario@ejemplo.com" --clean
```

Este comando:

- Buscará todos los usuarios con el email especificado
- Mostrará detalles completos de cada usuario
- Seleccionará automáticamente qué usuario mantener
- Eliminará los usuarios duplicados

### Escenario 4: Control Manual de la Eliminación

```bash
# Eliminar duplicados manteniendo un usuario específico
python Database/tools/eliminar_usuarios_duplicados.py --local --email "usuario@ejemplo.com" --keep-id "id-del-usuario" --clean
```

Este comando:

- Buscará todos los usuarios con el email especificado
- Mantendrá el usuario con el ID especificado
- Eliminará los demás usuarios duplicados

## Criterios de Selección Automática

Cuando se utiliza la opción `--clean` sin especificar `--keep-id`, la herramienta selecciona automáticamente qué usuario mantener basándose en estos criterios (en orden de prioridad):

1. **Usuarios activos**: Se priorizan los usuarios con `IsActive = 1`
2. **Usuarios con más roles**: Entre los usuarios activos, se mantiene el que tenga más roles asignados
3. **Usuarios más recientes**: Si hay empate en los criterios anteriores, se mantiene el usuario creado más recientemente

## Manejo de Referencias

La herramienta maneja correctamente las referencias en las siguientes tablas:

- `AspNetUserRoles`: Roles asignados a los usuarios
- `AspNetUserClaims`: Claims de los usuarios
- `AspNetUserLogins`: Información de inicio de sesión
- `UserAgencyAssignment`: Asignaciones de agencias a usuarios
- `UserProgram`: Programas asignados a usuarios
- `AgencyProgram`: Si tiene referencias a usuarios

Cuando se elimina un usuario duplicado, la herramienta:

1. Actualiza las referencias en tablas relacionadas para que apunten al usuario que se va a mantener
2. Elimina los registros en las tablas de AspNetIdentity relacionados con los usuarios duplicados
3. Finalmente elimina los usuarios duplicados

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

Para cambiar estas configuraciones, modifica las variables `azure_config` y `local_config` en el script.

## Solución de Problemas

### Error de Driver ODBC

Si aparece el error "Can't open lib 'ODBC Driver 18 for SQL Server'", consulta la sección de solución de problemas en la [Guía de Comparación de Bases de Datos](GUIA-COMPARACION-BASES-DATOS.md#error-de-driver-odbc).

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

### Error al Eliminar Usuarios

Si ocurren errores durante la eliminación de usuarios:

1. Verifica que no haya referencias a los usuarios en otras tablas no contempladas por el script
2. Asegúrate de que el usuario tiene permisos suficientes para eliminar registros
3. Revisa los mensajes de error para identificar el problema específico

## Prevención de Duplicados Futuros

Para evitar la creación de usuarios duplicados en el futuro, considera agregar índices únicos a las tablas de AspNetIdentity:

```sql
-- Índice único para Email
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_Email' AND object_id = OBJECT_ID('AspNetUsers'))
CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_Email ON AspNetUsers(Email) WHERE Email IS NOT NULL;

-- Índice único para UserName
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_UserName' AND object_id = OBJECT_ID('AspNetUsers'))
CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_UserName ON AspNetUsers(UserName) WHERE UserName IS NOT NULL;
```

## Buenas Prácticas

1. **Hacer backup antes de eliminar**: Siempre realiza una copia de seguridad de la base de datos antes de ejecutar eliminaciones masivas.

2. **Verificar antes de eliminar**: Usa la opción `--find` sin `--clean` para revisar qué usuarios se eliminarán.

3. **Entornos de prueba**: Prueba primero en un entorno de desarrollo o pruebas antes de ejecutar en producción.

4. **Monitoreo**: Después de la eliminación, verifica que las aplicaciones que utilizan la base de datos funcionen correctamente.

---

Esta guía proporciona una referencia completa para utilizar la herramienta de eliminación de usuarios duplicados en AspNetIdentity. Para más información o soporte, contacta al equipo de desarrollo.
