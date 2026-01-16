#!/usr/bin/env python
# -*- coding: utf-8 -*-

import pyodbc
import pandas as pd
import os
import sys
import argparse
from colorama import init, Fore, Style
import configparser
import re

# Inicializar colorama para colores en la terminal
init()

def parse_flyway_config(config_file):
    """Parsea el archivo de configuración de Flyway para obtener los datos de conexión"""
    config = {}
    
    with open(config_file, 'r') as file:
        for line in file:
            if line.startswith('flyway.url='):
                url_match = re.search(r'jdbc:sqlserver://([^:]+):(\d+);databaseName=([^;]+)', line)
                if url_match:
                    config['server'] = url_match.group(1)
                    config['port'] = url_match.group(2)
                    config['database'] = url_match.group(3)
            elif line.startswith('flyway.user='):
                config['username'] = line.split('=')[1].strip()
            elif line.startswith('flyway.password='):
                config['password'] = line.split('=')[1].strip()
    
    return config

def get_connection(server, database, username, password, encrypt=True):
    """Establece conexión con la base de datos SQL Server"""
    try:
        connection_string = (
            f"DRIVER={{ODBC Driver 18 for SQL Server}};"
            f"SERVER={server};"
            f"DATABASE={database};"
            f"UID={username};"
            f"PWD={password};"
        )
        
        # En ODBC Driver 18, Encrypt debe ser "yes" o "no" (string), no True/False (boolean)
        if encrypt:
            connection_string += "Encrypt=yes;TrustServerCertificate=yes;"
        else:
            connection_string += "Encrypt=no;TrustServerCertificate=yes;"
        
        print(f"Conectando a {server}/{database}...", file=sys.stderr)
        conn = pyodbc.connect(connection_string)
        print(f"Conexión exitosa a {server}/{database}", file=sys.stderr)
        return conn
    except Exception as e:
        print(f"Error al conectar a {server}/{database}: {str(e)}", file=sys.stderr)
        return None

def find_and_clean_duplicate_users(conn, auto_clean=False):
    """Encuentra y elimina usuarios duplicados en AspNetUsers"""
    cursor = conn.cursor()
    
    print(f"\n{Fore.CYAN}=== BUSCANDO USUARIOS DUPLICADOS POR EMAIL ==={Style.RESET_ALL}")
    
    # Buscar duplicados por email
    query_email = """
    WITH DuplicateEmails AS (
        SELECT 
            Email,
            COUNT(*) AS EmailCount
        FROM AspNetUsers
        WHERE Email IS NOT NULL
        GROUP BY Email
        HAVING COUNT(*) > 1
    )
    SELECT 
        Email,
        EmailCount AS Cantidad
    FROM DuplicateEmails
    ORDER BY EmailCount DESC;
    """
    
    cursor.execute(query_email)
    email_duplicates = cursor.fetchall()
    
    if not email_duplicates:
        print(f"{Fore.GREEN}No se encontraron usuarios con emails duplicados.{Style.RESET_ALL}")
    else:
        print(f"{Fore.YELLOW}Se encontraron {len(email_duplicates)} emails duplicados:{Style.RESET_ALL}")
        
        for row in email_duplicates:
            email = row.Email
            print(f"\n  - Email: {email}, Cantidad: {row.Cantidad}")
            
            # Obtener detalles de los usuarios con este email
            users = get_detailed_user_info(conn, email=email, print_details=True)
            
            if auto_clean and users:
                # Determinar qué usuario mantener (el más completo/reciente)
                keep_user = select_user_to_keep(users)
                
                if keep_user:
                    print(f"\n{Fore.GREEN}Manteniendo usuario: {keep_user.Id} ({keep_user.UserName}){Style.RESET_ALL}")
                    
                    # Eliminar los demás usuarios
                    delete_duplicate_users(conn, email=email, keep_id=keep_user.Id)
    
    print(f"\n{Fore.CYAN}=== BUSCANDO USUARIOS DUPLICADOS POR USERNAME ==={Style.RESET_ALL}")
    
    # Buscar duplicados por username
    query_username = """
    WITH DuplicateUserNames AS (
        SELECT 
            UserName,
            COUNT(*) AS UserNameCount
        FROM AspNetUsers
        WHERE UserName IS NOT NULL
        GROUP BY UserName
        HAVING COUNT(*) > 1
    )
    SELECT 
        UserName,
        UserNameCount AS Cantidad
    FROM DuplicateUserNames
    ORDER BY UserNameCount DESC;
    """
    
    cursor.execute(query_username)
    username_duplicates = cursor.fetchall()
    
    if not username_duplicates:
        print(f"{Fore.GREEN}No se encontraron usuarios con usernames duplicados.{Style.RESET_ALL}")
    else:
        print(f"{Fore.YELLOW}Se encontraron {len(username_duplicates)} usernames duplicados:{Style.RESET_ALL}")
        
        for row in username_duplicates:
            username = row.UserName
            print(f"\n  - UserName: {username}, Cantidad: {row.Cantidad}")
            
            # Obtener detalles de los usuarios con este username
            users = get_detailed_user_info(conn, username=username, print_details=True)
            
            if auto_clean and users:
                # Determinar qué usuario mantener (el más completo/reciente)
                keep_user = select_user_to_keep(users)
                
                if keep_user:
                    print(f"\n{Fore.GREEN}Manteniendo usuario: {keep_user.Id} ({keep_user.UserName}){Style.RESET_ALL}")
                    
                    # Eliminar los demás usuarios
                    delete_duplicate_users(conn, username=username, keep_id=keep_user.Id)
    
    return email_duplicates, username_duplicates

def get_detailed_user_info(conn, email=None, username=None, print_details=False):
    """Obtiene información detallada de los usuarios duplicados"""
    cursor = conn.cursor()
    
    if email:
        if print_details:
            print(f"\n{Fore.CYAN}=== DETALLES DE USUARIOS CON EMAIL: {email} ==={Style.RESET_ALL}")
        
        query = """
        SELECT 
            u.Id,
            u.UserName,
            u.Email,
            u.FirstName,
            u.MiddleName,
            u.FatherLastName,
            u.MotherLastName,
            u.EmailConfirmed,
            u.PhoneNumber,
            u.IsActive,
            u.CreatedAt,
            u.UpdatedAt,
            u.AgencyId,
            (SELECT COUNT(*) FROM AspNetUserRoles WHERE UserId = u.Id) AS RolesCount,
            (SELECT STRING_AGG(r.Name, ', ') FROM AspNetRoles r 
             INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId 
             WHERE ur.UserId = u.Id) AS Roles
        FROM AspNetUsers u
        WHERE u.Email = ?
        ORDER BY u.IsActive DESC, u.CreatedAt DESC;
        """
        
        cursor.execute(query, email)
    elif username:
        if print_details:
            print(f"\n{Fore.CYAN}=== DETALLES DE USUARIOS CON USERNAME: {username} ==={Style.RESET_ALL}")
        
        query = """
        SELECT 
            u.Id,
            u.UserName,
            u.Email,
            u.FirstName,
            u.MiddleName,
            u.FatherLastName,
            u.MotherLastName,
            u.EmailConfirmed,
            u.PhoneNumber,
            u.IsActive,
            u.CreatedAt,
            u.UpdatedAt,
            u.AgencyId,
            (SELECT COUNT(*) FROM AspNetUserRoles WHERE UserId = u.Id) AS RolesCount,
            (SELECT STRING_AGG(r.Name, ', ') FROM AspNetRoles r 
             INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId 
             WHERE ur.UserId = u.Id) AS Roles
        FROM AspNetUsers u
        WHERE u.UserName = ?
        ORDER BY u.IsActive DESC, u.CreatedAt DESC;
        """
        
        cursor.execute(query, username)
    else:
        return []
    
    users = cursor.fetchall()
    
    if not users:
        if print_details:
            print(f"{Fore.YELLOW}No se encontraron usuarios.{Style.RESET_ALL}")
        return []
    
    if print_details:
        for i, user in enumerate(users):
            print(f"\n{Fore.GREEN}Usuario {i+1}:{Style.RESET_ALL}")
            print(f"  ID: {user.Id}")
            print(f"  UserName: {user.UserName}")
            print(f"  Email: {user.Email}")
            print(f"  Nombre: {user.FirstName} {user.MiddleName or ''} {user.FatherLastName} {user.MotherLastName}")
            print(f"  Email Confirmado: {user.EmailConfirmed}")
            print(f"  Teléfono: {user.PhoneNumber}")
            print(f"  Activo: {user.IsActive}")
            print(f"  Creado: {user.CreatedAt}")
            print(f"  Actualizado: {user.UpdatedAt}")
            print(f"  AgencyId: {user.AgencyId}")
            print(f"  Cantidad de Roles: {user.RolesCount}")
            print(f"  Roles: {user.Roles or 'Ninguno'}")
    
    return users

def select_user_to_keep(users):
    """Selecciona el usuario a mantener entre los duplicados"""
    if not users:
        return None
    
    # Criterios de selección (en orden de prioridad):
    # 1. Usuario activo
    # 2. Usuario con más roles
    # 3. Usuario más reciente
    
    # Primero filtrar por usuarios activos
    active_users = [u for u in users if u.IsActive]
    
    if active_users:
        # Si hay usuarios activos, seleccionar entre ellos
        candidates = active_users
    else:
        # Si no hay usuarios activos, usar todos
        candidates = users
    
    # Ordenar por cantidad de roles (descendente) y fecha de creación (descendente)
    sorted_users = sorted(candidates, key=lambda u: (-u.RolesCount if u.RolesCount else 0, u.CreatedAt or '1900-01-01'), reverse=True)
    
    # Retornar el primer usuario (el que tiene más roles o es más reciente)
    return sorted_users[0] if sorted_users else None

def delete_duplicate_users(conn, email=None, username=None, keep_id=None):
    """Elimina usuarios duplicados, manteniendo uno"""
    if not email and not username:
        print(f"{Fore.RED}Debe especificar un email o username para eliminar duplicados.{Style.RESET_ALL}")
        return False
    
    if not keep_id:
        print(f"{Fore.RED}Debe especificar el ID del usuario a mantener.{Style.RESET_ALL}")
        return False
    
    try:
        cursor = conn.cursor()
        
        # Iniciar transacción
        conn.autocommit = False
        
        print(f"{Fore.YELLOW}Iniciando eliminación de usuarios duplicados...{Style.RESET_ALL}")
        
        # 1. Primero verificar si hay referencias en otras tablas que no sean AspNetUserRoles, AspNetUserClaims, AspNetUserLogins
        # Estas tablas pueden tener referencias a los usuarios que vamos a eliminar
        tables_to_check = [
            "UserAgencyAssignment",
            "UserProgram",
            "AgencyProgram"  # Si tiene una columna UserId
        ]
        
        user_ids_to_delete = []
        
        if email:
            query = "SELECT Id FROM AspNetUsers WHERE Email = ? AND Id <> ?"
            cursor.execute(query, email, keep_id)
            user_ids_to_delete = [row.Id for row in cursor.fetchall()]
        elif username:
            query = "SELECT Id FROM AspNetUsers WHERE UserName = ? AND Id <> ?"
            cursor.execute(query, username, keep_id)
            user_ids_to_delete = [row.Id for row in cursor.fetchall()]
        
        if not user_ids_to_delete:
            print(f"{Fore.YELLOW}No se encontraron usuarios para eliminar.{Style.RESET_ALL}")
            return True
        
        print(f"{Fore.YELLOW}Se eliminarán {len(user_ids_to_delete)} usuarios duplicados.{Style.RESET_ALL}")
        
        # Verificar referencias en otras tablas
        for table in tables_to_check:
            try:
                # Verificar si la tabla existe y tiene una columna UserId
                query = f"""
                IF EXISTS (
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.TABLES t
                    JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
                    WHERE t.TABLE_NAME = '{table}' AND c.COLUMN_NAME = 'UserId'
                )
                BEGIN
                    SELECT '{table}' AS TableName, UserId, COUNT(*) AS RefCount
                    FROM {table}
                    WHERE UserId IN ({','.join(["'" + uid + "'" for uid in user_ids_to_delete])})
                    GROUP BY UserId
                END
                """
                cursor.execute(query)
                
                refs = cursor.fetchall()
                if refs:
                    print(f"{Fore.YELLOW}Referencias encontradas en {table}:{Style.RESET_ALL}")
                    for ref in refs:
                        print(f"  - UserId: {ref.UserId}, Referencias: {ref.RefCount}")
                    
                    # Actualizar referencias para que apunten al usuario que vamos a mantener
                    update_query = f"""
                    UPDATE {table}
                    SET UserId = ?
                    WHERE UserId IN ({','.join(['?' for _ in user_ids_to_delete])})
                    """
                    cursor.execute(update_query, keep_id, *user_ids_to_delete)
                    print(f"{Fore.GREEN}Referencias actualizadas en {table}: {cursor.rowcount} filas{Style.RESET_ALL}")
            except Exception as e:
                print(f"{Fore.YELLOW}Error al verificar referencias en {table}: {str(e)}{Style.RESET_ALL}")
                # Continuar con las siguientes tablas
        
        # 2. Eliminar registros en AspNetUserRoles
        query = f"""
        DELETE FROM AspNetUserRoles
        WHERE UserId IN ({','.join(['?' for _ in user_ids_to_delete])})
        """
        cursor.execute(query, *user_ids_to_delete)
        print(f"{Fore.GREEN}Roles eliminados: {cursor.rowcount}{Style.RESET_ALL}")
        
        # 3. Eliminar registros en AspNetUserClaims
        query = f"""
        DELETE FROM AspNetUserClaims
        WHERE UserId IN ({','.join(['?' for _ in user_ids_to_delete])})
        """
        cursor.execute(query, *user_ids_to_delete)
        print(f"{Fore.GREEN}Claims eliminados: {cursor.rowcount}{Style.RESET_ALL}")
        
        # 4. Eliminar registros en AspNetUserLogins
        query = f"""
        DELETE FROM AspNetUserLogins
        WHERE UserId IN ({','.join(['?' for _ in user_ids_to_delete])})
        """
        cursor.execute(query, *user_ids_to_delete)
        print(f"{Fore.GREEN}Logins eliminados: {cursor.rowcount}{Style.RESET_ALL}")
        
        # 5. Finalmente, eliminar los usuarios duplicados
        if email:
            query = "DELETE FROM AspNetUsers WHERE Email = ? AND Id <> ?"
            cursor.execute(query, email, keep_id)
        elif username:
            query = "DELETE FROM AspNetUsers WHERE UserName = ? AND Id <> ?"
            cursor.execute(query, username, keep_id)
        
        print(f"{Fore.GREEN}Usuarios eliminados: {cursor.rowcount}{Style.RESET_ALL}")
        
        # Confirmar transacción
        conn.commit()
        print(f"{Fore.GREEN}Transacción completada con éxito.{Style.RESET_ALL}")
        
        return True
    except Exception as e:
        conn.rollback()
        print(f"{Fore.RED}Error al eliminar usuarios duplicados: {str(e)}{Style.RESET_ALL}")
        return False
    finally:
        conn.autocommit = True

def main():
    parser = argparse.ArgumentParser(description='Herramienta para gestionar usuarios duplicados en AspNetIdentity')
    parser.add_argument('--azure', action='store_true', help='Usar configuración de Azure DB')
    parser.add_argument('--local', action='store_true', help='Usar configuración de Local DB')
    parser.add_argument('--find', action='store_true', help='Buscar usuarios duplicados')
    parser.add_argument('--clean', action='store_true', help='Eliminar automáticamente usuarios duplicados')
    parser.add_argument('--email', type=str, help='Email específico para buscar o eliminar duplicados')
    parser.add_argument('--username', type=str, help='Username específico para buscar o eliminar duplicados')
    parser.add_argument('--keep-id', type=str, help='ID del usuario a mantener al eliminar duplicados')
    
    args = parser.parse_args()
    
    # Si no se especifica ninguna opción, mostrar ayuda
    if not (args.azure or args.local or args.find or args.clean or args.email or args.username):
        parser.print_help()
        return
    
    # Configuración de bases de datos (mismas que en compare_databases.py)
    azure_config = {
        'server': 'aesanserver.database.windows.net',
        'database': 'aesan2',
        'username': 'aesan',
        'password': '4ubi4XFygLMxXU',
        'encrypt': True
    }
    
    local_config = {
        'server': '192.168.1.144',
        'database': 'NUTRE',
        'username': 'sa',
        'password': 'd@rio152325',
        'encrypt': False
    }
    
    # Determinar qué configuración usar
    if args.azure:
        db_config = azure_config
        db_name = "Azure DB (aesan2)"
    else:  # Por defecto usar local
        db_config = local_config
        db_name = "Local DB (NUTRE)"
    
    # Establecer conexión
    conn = get_connection(**db_config)
    
    if not conn:
        print(f"{Fore.RED}No se pudo establecer la conexión. Saliendo.{Style.RESET_ALL}")
        return
    
    try:
        # Buscar y limpiar usuarios duplicados
        if args.find or args.clean:
            find_and_clean_duplicate_users(conn, auto_clean=args.clean)
        
        # Procesar un email o username específico
        if args.email or args.username:
            if args.email:
                users = get_detailed_user_info(conn, email=args.email, print_details=True)
            else:
                users = get_detailed_user_info(conn, username=args.username, print_details=True)
            
            if args.clean:
                if args.keep_id:
                    keep_id = args.keep_id
                    # Verificar que el ID existe
                    for user in users:
                        if user.Id == keep_id:
                            break
                    else:
                        print(f"{Fore.RED}El ID {keep_id} no existe entre los usuarios encontrados.{Style.RESET_ALL}")
                        return
                else:
                    # Seleccionar automáticamente el usuario a mantener
                    keep_user = select_user_to_keep(users)
                    if keep_user:
                        keep_id = keep_user.Id
                        print(f"\n{Fore.GREEN}Seleccionado automáticamente para mantener: {keep_id} ({keep_user.UserName}){Style.RESET_ALL}")
                    else:
                        print(f"{Fore.RED}No se pudo determinar qué usuario mantener.{Style.RESET_ALL}")
                        return
                
                # Eliminar duplicados
                if args.email:
                    delete_duplicate_users(conn, email=args.email, keep_id=keep_id)
                else:
                    delete_duplicate_users(conn, username=args.username, keep_id=keep_id)
    
    finally:
        # Cerrar conexión
        if conn:
            conn.close()

if __name__ == "__main__":
    main() 