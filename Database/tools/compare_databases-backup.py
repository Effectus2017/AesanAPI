#!/usr/bin/env python
# -*- coding: utf-8 -*-

import pyodbc
import pandas as pd
import os
import sys
from tabulate import tabulate
import argparse
from colorama import init, Fore, Style
import difflib
import re

# Inicializar colorama para colores en la terminal
init()

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

def get_tables(conn):
    """Obtiene la lista de tablas en la base de datos"""
    cursor = conn.cursor()
    query = """
    SELECT 
        t.name AS TableName,
        SCHEMA_NAME(t.schema_id) AS SchemaName
    FROM 
        sys.tables t
    ORDER BY 
        SchemaName, TableName
    """
    cursor.execute(query)
    tables = [(row.SchemaName, row.TableName) for row in cursor.fetchall()]
    cursor.close()
    return tables

def get_table_columns(conn, schema_name, table_name):
    """Obtiene la estructura de columnas de una tabla"""
    cursor = conn.cursor()
    query = """
    SELECT 
        c.name AS ColumnName,
        t.name AS DataType,
        c.max_length,
        c.precision,
        c.scale,
        c.is_nullable,
        c.is_identity,
        ISNULL(ep.value, '') AS Description
    FROM 
        sys.columns c
    JOIN 
        sys.types t ON c.user_type_id = t.user_type_id
    LEFT JOIN 
        sys.extended_properties ep ON ep.major_id = c.object_id AND ep.minor_id = c.column_id AND ep.name = 'MS_Description'
    WHERE 
        c.object_id = OBJECT_ID(?)
    ORDER BY 
        c.column_id
    """
    cursor.execute(query, f"{schema_name}.{table_name}")
    columns = []
    for row in cursor.fetchall():
        data_type = row.DataType
        if data_type in ('varchar', 'nvarchar', 'char', 'nchar'):
            if row.max_length == -1:
                data_type += '(MAX)'
            else:
                if data_type.startswith('n'):
                    data_type += f'({row.max_length // 2})'
                else:
                    data_type += f'({row.max_length})'
        elif data_type in ('decimal', 'numeric'):
            data_type += f'({row.precision},{row.scale})'
            
        columns.append({
            'name': row.ColumnName,
            'type': data_type,
            'nullable': row.is_nullable,
            'identity': row.is_identity,
            'description': row.Description
        })
    cursor.close()
    return columns

def get_stored_procedures(conn):
    """Obtiene la lista de procedimientos almacenados"""
    cursor = conn.cursor()
    query = """
    SELECT 
        SCHEMA_NAME(p.schema_id) AS SchemaName,
        p.name AS ProcedureName,
        OBJECT_DEFINITION(p.object_id) AS Definition,
        p.create_date,
        p.modify_date
    FROM 
        sys.procedures p
    ORDER BY 
        SchemaName, ProcedureName
    """
    cursor.execute(query)
    procedures = []
    for row in cursor.fetchall():
        procedures.append({
            'schema': row.SchemaName,
            'name': row.ProcedureName,
            'definition': row.Definition,
            'create_date': row.create_date,
            'modify_date': row.modify_date
        })
    cursor.close()
    return procedures

def compare_tables(db1_tables, db2_tables):
    """Compara las tablas entre dos bases de datos"""
    db1_tables_set = set([(schema, table) for schema, table in db1_tables])
    db2_tables_set = set([(schema, table) for schema, table in db2_tables])
    
    only_in_db1 = db1_tables_set - db2_tables_set
    only_in_db2 = db2_tables_set - db1_tables_set
    common_tables = db1_tables_set.intersection(db2_tables_set)
    
    return {
        'only_in_db1': sorted(list(only_in_db1)),
        'only_in_db2': sorted(list(only_in_db2)),
        'common': sorted(list(common_tables))
    }

def compare_table_structure(db1_conn, db2_conn, schema_name, table_name):
    """Compara la estructura de una tabla entre dos bases de datos"""
    db1_columns = get_table_columns(db1_conn, schema_name, table_name)
    db2_columns = get_table_columns(db2_conn, schema_name, table_name)
    
    db1_cols_dict = {col['name'].lower(): col for col in db1_columns}  # Usar lowercase para comparación insensible a mayúsculas
    db2_cols_dict = {col['name'].lower(): col for col in db2_columns}
    
    db1_col_names = set(db1_cols_dict.keys())
    db2_col_names = set(db2_cols_dict.keys())
    
    only_in_db1 = db1_col_names - db2_col_names
    only_in_db2 = db2_col_names - db1_col_names
    common_cols = db1_col_names.intersection(db2_col_names)
    
    different_cols = []
    for col_name in common_cols:
        db1_col = db1_cols_dict[col_name]
        db2_col = db2_cols_dict[col_name]
        
        differences = []
        
        # Comparar tipo de datos
        if db1_col['type'] != db2_col['type']:
            differences.append(f"tipo: {db1_col['type']} vs {db2_col['type']}")
        
        # Comparar nulabilidad
        if db1_col['nullable'] != db2_col['nullable']:
            differences.append(f"nulabilidad: {'NULL' if db1_col['nullable'] else 'NOT NULL'} vs {'NULL' if db2_col['nullable'] else 'NOT NULL'}")
        
        # Comparar identidad
        if db1_col['identity'] != db2_col['identity']:
            differences.append(f"identidad: {'IDENTITY' if db1_col['identity'] else 'NOT IDENTITY'} vs {'IDENTITY' if db2_col['identity'] else 'NOT IDENTITY'}")
        
        # Si hay diferencias, agregar a la lista
        if differences:
            different_cols.append({
                'name': db1_col['name'],  # Usar el nombre original, no el lowercase
                'db1': db1_col,
                'db2': db2_col,
                'differences': differences
            })
    
    return {
        'only_in_db1': [db1_cols_dict[col] for col in only_in_db1],
        'only_in_db2': [db2_cols_dict[col] for col in only_in_db2],
        'different': different_cols
    }

def normalize_sp_definition(definition):
    """Normaliza la definición de un SP para comparación"""
    if not definition:
        return ""
    
    # Eliminar comentarios
    definition = re.sub(r'--.*?\n', '\n', definition)
    definition = re.sub(r'/\*.*?\*/', '', definition, flags=re.DOTALL)
    
    # Normalizar espacios en blanco
    definition = re.sub(r'\s+', ' ', definition)
    definition = re.sub(r'\s*,\s*', ', ', definition)
    definition = re.sub(r'\s*=\s*', '=', definition)
    definition = re.sub(r'\s*\(\s*', '(', definition)
    definition = re.sub(r'\s*\)\s*', ')', definition)
    
    # Convertir a minúsculas para comparación insensible a mayúsculas/minúsculas
    # (opcional, comenta esta línea si quieres comparación sensible a mayúsculas/minúsculas)
    # definition = definition.lower()
    
    return definition.strip()

def compare_stored_procedures(db1_procs, db2_procs, show_diff=False):
    """Compara los procedimientos almacenados entre dos bases de datos"""
    db1_procs_dict = {f"{proc['schema']}.{proc['name']}": proc for proc in db1_procs}
    db2_procs_dict = {f"{proc['schema']}.{proc['name']}": proc for proc in db2_procs}
    
    db1_proc_names = set(db1_procs_dict.keys())
    db2_proc_names = set(db2_procs_dict.keys())
    
    only_in_db1 = db1_proc_names - db2_proc_names
    only_in_db2 = db2_proc_names - db1_proc_names
    common_procs = db1_proc_names.intersection(db2_proc_names)
    
    different_procs = []
    for proc_name in common_procs:
        db1_proc = db1_procs_dict[proc_name]
        db2_proc = db2_procs_dict[proc_name]
        
        # Normalizar definiciones para comparación
        db1_def = normalize_sp_definition(db1_proc['definition'])
        db2_def = normalize_sp_definition(db2_proc['definition'])
        
        if db1_def != db2_def:
            # Calcular diferencias si se solicita
            diff = None
            if show_diff:
                diff = list(difflib.unified_diff(
                    db1_def.splitlines(),
                    db2_def.splitlines(),
                    fromfile=f"DB1: {proc_name}",
                    tofile=f"DB2: {proc_name}",
                    lineterm=''
                ))
            
            different_procs.append({
                'name': proc_name,
                'db1_date': db1_proc['modify_date'],
                'db2_date': db2_proc['modify_date'],
                'db1_def': db1_proc['definition'],
                'db2_def': db2_proc['definition'],
                'diff': diff
            })
    
    return {
        'only_in_db1': sorted(list(only_in_db1)),
        'only_in_db2': sorted(list(only_in_db2)),
        'different': different_procs
    }

def print_table_comparison(comparison, db1_name, db2_name):
    """Imprime la comparación de tablas"""
    print(f"\n{Fore.CYAN}=== COMPARACIÓN DE TABLAS ==={Style.RESET_ALL}")
    
    if comparison['only_in_db1']:
        print(f"\n{Fore.YELLOW}Tablas solo en {db1_name} ({len(comparison['only_in_db1'])}):{Style.RESET_ALL}")
        for schema, table in comparison['only_in_db1']:
            print(f"  - {schema}.{table}")
    
    if comparison['only_in_db2']:
        print(f"\n{Fore.YELLOW}Tablas solo en {db2_name} ({len(comparison['only_in_db2'])}):{Style.RESET_ALL}")
        for schema, table in comparison['only_in_db2']:
            print(f"  - {schema}.{table}")
    
    print(f"\n{Fore.GREEN}Tablas en común: {len(comparison['common'])}{Style.RESET_ALL}")

def print_table_structure_comparison(comparison, schema_name, table_name):
    """Imprime la comparación de estructura de una tabla"""
    has_differences = (comparison['only_in_db1'] or comparison['only_in_db2'] or comparison['different'])
    
    if has_differences:
        print(f"\n{Fore.CYAN}=== COMPARACIÓN DE ESTRUCTURA: {schema_name}.{table_name} ==={Style.RESET_ALL}")
        
        if comparison['only_in_db1']:
            print(f"\n{Fore.YELLOW}Columnas solo en DB1 ({len(comparison['only_in_db1'])}):{Style.RESET_ALL}")
            for col in comparison['only_in_db1']:
                print(f"  - {col['name']} ({col['type']}, {'NULL' if col['nullable'] else 'NOT NULL'}, {'IDENTITY' if col['identity'] else 'NOT IDENTITY'})")
        
        if comparison['only_in_db2']:
            print(f"\n{Fore.YELLOW}Columnas solo en DB2 ({len(comparison['only_in_db2'])}):{Style.RESET_ALL}")
            for col in comparison['only_in_db2']:
                print(f"  - {col['name']} ({col['type']}, {'NULL' if col['nullable'] else 'NOT NULL'}, {'IDENTITY' if col['identity'] else 'NOT IDENTITY'})")
        
        if comparison['different']:
            print(f"\n{Fore.RED}Columnas con diferencias ({len(comparison['different'])}):{Style.RESET_ALL}")
            for diff in comparison['different']:
                db1_col = diff['db1']
                db2_col = diff['db2']
                print(f"  - {diff['name']}:")
                for difference in diff['differences']:
                    print(f"    * {difference}")

def print_stored_procedure_comparison(comparison, db1_name, db2_name, show_diff=False, export_path=None):
    """Imprime la comparación de procedimientos almacenados"""
    print(f"\n{Fore.CYAN}=== COMPARACIÓN DE PROCEDIMIENTOS ALMACENADOS ==={Style.RESET_ALL}")
    
    if comparison['only_in_db1']:
        print(f"\n{Fore.YELLOW}Procedimientos solo en {db1_name} ({len(comparison['only_in_db1'])}):{Style.RESET_ALL}")
        for proc in comparison['only_in_db1']:
            print(f"  - {proc}")
    
    if comparison['only_in_db2']:
        print(f"\n{Fore.YELLOW}Procedimientos solo en {db2_name} ({len(comparison['only_in_db2'])}):{Style.RESET_ALL}")
        for proc in comparison['only_in_db2']:
            print(f"  - {proc}")
    
    if comparison['different']:
        print(f"\n{Fore.RED}Procedimientos con diferencias ({len(comparison['different'])}):{Style.RESET_ALL}")
        for diff in comparison['different']:
            print(f"  - {diff['name']}")
            print(f"    {db1_name} modificado: {diff['db1_date']}")
            print(f"    {db2_name} modificado: {diff['db2_date']}")
            
            # Exportar a archivos si se solicita
            if export_path:
                proc_name = diff['name'].replace('.', '_')
                os.makedirs(export_path, exist_ok=True)
                
                # Exportar SP de DB1
                with open(f"{export_path}/{proc_name}_db1.sql", 'w', encoding='utf-8') as f:
                    f.write(diff['db1_def'])
                
                # Exportar SP de DB2
                with open(f"{export_path}/{proc_name}_db2.sql", 'w', encoding='utf-8') as f:
                    f.write(diff['db2_def'])
                
                print(f"    Exportado a {export_path}/{proc_name}_db1.sql y {export_path}/{proc_name}_db2.sql")
            
            # Mostrar diferencias si se solicita
            if show_diff and diff['diff']:
                print("\n    Diferencias:")
                for line in diff['diff']:
                    if line.startswith('+'):
                        print(f"    {Fore.GREEN}{line}{Style.RESET_ALL}")
                    elif line.startswith('-'):
                        print(f"    {Fore.RED}{line}{Style.RESET_ALL}")
                    else:
                        print(f"    {line}")
                print()

def main():
    parser = argparse.ArgumentParser(description='Comparar dos bases de datos SQL Server')
    parser.add_argument('--azure', action='store_true', help='Usar configuración de Azure DB')
    parser.add_argument('--local', action='store_true', help='Usar configuración de Local DB')
    parser.add_argument('--tables', action='store_true', help='Comparar tablas')
    parser.add_argument('--procedures', action='store_true', help='Comparar procedimientos almacenados')
    parser.add_argument('--all', action='store_true', help='Comparar todo')
    parser.add_argument('--table-details', action='store_true', help='Mostrar detalles de las tablas comunes')
    parser.add_argument('--all-tables', action='store_true', help='Mostrar todas las tablas comunes, incluso sin diferencias')
    parser.add_argument('--output', type=str, help='Archivo de salida para guardar los resultados')
    parser.add_argument('--show-sp-diff', action='store_true', help='Mostrar diferencias en el código de los procedimientos almacenados')
    parser.add_argument('--export-sp', type=str, help='Directorio para exportar los procedimientos almacenados diferentes')
    parser.add_argument('--sp-name', type=str, help='Nombre específico de un procedimiento almacenado para comparar')
    
    args = parser.parse_args()
    
    # Si no se especifica ninguna opción, mostrar ayuda
    if not (args.azure or args.local or args.tables or args.procedures or args.all or args.table_details or args.all_tables or args.show_sp_diff or args.export_sp or args.sp_name):
        parser.print_help()
        return
    
    # Configurar salida a archivo si se especifica
    if args.output:
        sys.stdout = open(args.output, 'w', encoding='utf-8')
    
    # Configuración de bases de datos
    # Azure DB
    db1_config = {
        'server': 'aesanserver.database.windows.net',
        'database': 'aesan2',
        'username': 'aesan',
        'password': '4ubi4XFygLMxXU',
        'encrypt': True
    }
    
    # Local DB
    # db2_config = {
    #     'server': '192.168.1.144',
    #     'database': 'NUTRE',
    #     'username': 'sa',
    #     'password': 'd@rio152325',
    #     'encrypt': False
    # }

    # Azure DB NUTRE 2
    db2_config = {
        'server': 'testaes.database.windows.net',
        'database': 'aesbd',
        'username': 'serverdbaes',
        'password': 'EsU8jWyEgLHK',
        'encrypt': True
    }
    
    # Nombres para mostrar
    db1_name = "Azure DB (aesan2)"
    # db2_name = "Local DB (NUTRE)"
    db2_name = "Azure DB (NUTRE 2)"
    
    # Establecer conexiones
    db1_conn = get_connection(**db1_config)
    db2_conn = get_connection(**db2_config)
    
    if not db1_conn or not db2_conn:
        print("No se pudieron establecer ambas conexiones. Saliendo.")
        return
    
    try:
        # Comparar tablas
        if args.tables or args.all:
            db1_tables = get_tables(db1_conn)
            db2_tables = get_tables(db2_conn)
            
            table_comparison = compare_tables(db1_tables, db2_tables)
            print_table_comparison(table_comparison, db1_name, db2_name)
            
            # Comparar estructura de tablas comunes
            if args.table_details:
                print(f"\n{Fore.CYAN}=== COMPARACIÓN DETALLADA DE ESTRUCTURAS DE TABLAS ==={Style.RESET_ALL}")
                
                # Contador para tablas con diferencias
                tables_with_differences = 0
                tables_without_differences = 0
                
                for schema, table in table_comparison['common']:
                    structure_comparison = compare_table_structure(db1_conn, db2_conn, schema, table)
                    
                    # Verificar si hay diferencias
                    has_differences = (structure_comparison['only_in_db1'] or 
                                      structure_comparison['only_in_db2'] or 
                                      structure_comparison['different'])
                    
                    if has_differences:
                        tables_with_differences += 1
                        print_table_structure_comparison(structure_comparison, schema, table)
                    elif args.all_tables:
                        tables_without_differences += 1
                        print(f"\n{Fore.GREEN}Tabla sin diferencias: {schema}.{table}{Style.RESET_ALL}")
                
                if tables_with_differences == 0:
                    print(f"\n{Fore.GREEN}No se encontraron diferencias en la estructura de las tablas comunes.{Style.RESET_ALL}")
                else:
                    print(f"\n{Fore.YELLOW}Se encontraron diferencias en {tables_with_differences} tablas.{Style.RESET_ALL}")
                    print(f"{Fore.GREEN}Tablas sin diferencias: {tables_without_differences}{Style.RESET_ALL}")
        
        # Comparar procedimientos almacenados
        if args.procedures or args.all or args.sp_name:
            db1_procs = get_stored_procedures(db1_conn)
            db2_procs = get_stored_procedures(db2_conn)
            
            # Si se especifica un SP específico, filtrar solo ese
            if args.sp_name:
                db1_procs = [p for p in db1_procs if p['name'] == args.sp_name or f"{p['schema']}.{p['name']}" == args.sp_name]
                db2_procs = [p for p in db2_procs if p['name'] == args.sp_name or f"{p['schema']}.{p['name']}" == args.sp_name]
                
                if not db1_procs and not db2_procs:
                    print(f"No se encontró el procedimiento almacenado '{args.sp_name}' en ninguna base de datos.")
                    return
            
            proc_comparison = compare_stored_procedures(db1_procs, db2_procs, show_diff=args.show_sp_diff)
            print_stored_procedure_comparison(
                proc_comparison, 
                db1_name, 
                db2_name, 
                show_diff=args.show_sp_diff,
                export_path=args.export_sp
            )
            
    finally:
        # Cerrar conexiones
        if db1_conn:
            db1_conn.close()
        if db2_conn:
            db2_conn.close()

if __name__ == "__main__":
    main()
