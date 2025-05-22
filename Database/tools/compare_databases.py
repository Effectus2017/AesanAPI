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
import datetime
import glob
from collections import defaultdict, deque
import traceback
import numpy as np

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

def clean_old_sp_versions(conn, db_name, dry_run=True):
    """Elimina versiones viejas de SPs versionados (NNN_NombreSP) en la base de datos dada y registra los SPs más recientes que se conservarán en un log."""
    sps = get_stored_procedures(conn)
    pattern = re.compile(r'^(?P<version>\d{3})_(?P<base>.+)$')
    versioned = {}
    for sp in sps:
        m = pattern.match(sp['name'])
        if m:
            # Normalizar el nombre base: minúsculas y sin espacios
            base = f"{sp['schema'].lower()}.{m.group('base').replace(' ', '').lower()}"
            version = int(m.group('version'))
            versioned.setdefault(base, []).append((version, sp))
    total_deleted = 0
    failed_drops = []
    # Preparar log de SPs que se conservarán
    log_lines = []
    fecha = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')
    log_filename = f"sp_cleanup_keep_log_{db_name.replace(' ', '_')}_{fecha}.txt"
    for base, versions in versioned.items():
        versions.sort(reverse=True)  # De mayor a menor
        keep = versions[0][1]  # Mantener la versión más alta
        to_delete = [sp for version, sp in versions[1:]]
        log_lines.append(f"[{db_name}] Se conservará: [{keep['schema']}].[{keep['name']}] (versión más reciente)")
        if dry_run:
            print(f"[DRY RUN] {db_name}: Se conservará la versión más alta: [{keep['schema']}].[{keep['name']}] (versión más reciente)")
        for sp in to_delete:
            drop_sql = f"DROP PROCEDURE [{sp['schema']}].[{sp['name']}]"
            if dry_run:
                print(f"[DRY RUN] {db_name}: {drop_sql}")
            else:
                print(f"Eliminando en {db_name}: {drop_sql}")
                cursor = conn.cursor()
                try:
                    cursor.execute(drop_sql)
                    print(f"  -> Eliminado: {sp['schema']}.{sp['name']}")
                    total_deleted += 1
                except Exception as e:
                    print(f"  -> ERROR eliminando {sp['schema']}.{sp['name']}: {e}")
                    failed_drops.append(f"{sp['schema']}.{sp['name']} - {e}")
                finally:
                    cursor.close()
        # Commit tras cada grupo (por seguridad)
        if not dry_run:
            try:
                conn.commit()
            except Exception as e:
                print(f"[WARN] Error al hacer commit: {e}")
    # Guardar el log de SPs que se conservarán
    with open(log_filename, 'w', encoding='utf-8') as logf:
        logf.write('\n'.join(log_lines))
    print(f"\n[LOG] Registro de SPs que se conservarán guardado en: {log_filename}\n")
    if not dry_run:
        print(f"Total SPs eliminados en {db_name}: {total_deleted}")
        if failed_drops:
            print(f"\n[ERROR] No se pudieron eliminar los siguientes SPs:")
            for fail in failed_drops:
                print(f"  - {fail}")

def sync_missing_sps(db1_conn, db2_conn):
    """Sincroniza los SPs que existen en db1 pero faltan en db2 ejecutando los scripts SQL correspondientes en db2."""
    print(f"\n{Fore.CYAN}=== SINCRONIZACIÓN DE SPs FALTANTES EN DB2 ==={Style.RESET_ALL}")
    db1_procs = get_stored_procedures(db1_conn)
    db2_procs = get_stored_procedures(db2_conn)
    db1_proc_names = set(f"{p['schema']}.{p['name']}" for p in db1_procs)
    db2_proc_names = set(f"{p['schema']}.{p['name']}" for p in db2_procs)
    missing_in_db2 = db1_proc_names - db2_proc_names
    if not missing_in_db2:
        print(f"{Fore.GREEN}No hay SPs faltantes en DB2.{Style.RESET_ALL}")
        return
    print(f"{Fore.YELLOW}Procedimientos a sincronizar en DB2: {len(missing_in_db2)}{Style.RESET_ALL}")
    # Buscar archivos SQL en Api/Database/Tables/*/*.sql
    sql_files = glob.glob(os.path.join(os.path.dirname(__file__), '../Tables/*/*.sql'))
    sql_files_map = {os.path.basename(f).replace('.sql',''): f for f in sql_files}
    for sp_fullname in missing_in_db2:
        sp_name = sp_fullname.split('.')[-1]
        sql_file = sql_files_map.get(sp_name)
        if not sql_file:
            print(f"{Fore.RED}No se encontró archivo SQL para {sp_fullname}{Style.RESET_ALL}")
            continue
        print(f"Sincronizando {sp_fullname} desde archivo {os.path.basename(sql_file)}...")
        with open(sql_file, 'r', encoding='utf-8') as f:
            sql = f.read()
        # Separar por bloques usando 'GO' como separador
        blocks = [block.strip() for block in re.split(r'^\s*GO\s*$', sql, flags=re.MULTILINE) if block.strip()]
        try:
            cursor = db2_conn.cursor()
            for block in blocks:
                cursor.execute(block)
            cursor.commit()
            cursor.close()
            print(f"{Fore.GREEN}SP {sp_fullname} sincronizado correctamente.{Style.RESET_ALL}")
        except Exception as e:
            print(f"{Fore.RED}Error al sincronizar {sp_fullname}: {e}{Style.RESET_ALL}")
            print(f"Se detiene la sincronización.")
            break

def move_old_sp_files_local():
    """
    Herramienta de comparación y mantenimiento de bases de datos SQL Server

    NUEVO: Limpieza y organización de archivos locales de SPs
    --------------------------------------------------------

    Opción: --move-old-sp-files-local

    Permite mover automáticamente las versiones viejas de archivos de procedimientos almacenados (SPs) locales a una subcarpeta Deprecated dentro de cada entidad.

    - Busca archivos versionados (NNN_NombreSP.sql) en cada carpeta de entidad bajo Api/Database/Tables.
    - Mantiene solo la versión más alta de cada SP.
    - Mueve las versiones viejas a la subcarpeta Deprecated (creándola si no existe).
    - Renombra los archivos movidos como {Nombre}-Deprecated.sql.

    USO:
      python compare_databases.py --move-old-sp-files-local

    Limitaciones:
    - Solo afecta archivos locales, no la base de datos.
    - No elimina archivos, solo los mueve y renombra.
    - No revisa dependencias entre SPs.
    """
    print(f"\n{Fore.CYAN}=== MOVIENDO ARCHIVOS DE SPs VERSIONADOS ANTIGUOS A Deprecated/ ==={Style.RESET_ALL}")
    base_dir = os.path.join(os.path.dirname(__file__), '../Tables')
    for entity_dir in os.listdir(base_dir):
        entity_path = os.path.join(base_dir, entity_dir)
        if not os.path.isdir(entity_path):
            continue
        # Buscar archivos NNN_NombreSP.sql
        sp_files = [f for f in os.listdir(entity_path) if re.match(r'\d{3}_.+\.sql$', f)]
        if not sp_files:
            continue
        # Agrupar por base name (sin versión)
        sp_map = {}
        for f in sp_files:
            m = re.match(r'(?P<version>\d{3})_(?P<base>.+)\.sql$', f)
            if m:
                base = m.group('base')
                version = int(m.group('version'))
                sp_map.setdefault(base, []).append((version, f))
        for base, versions in sp_map.items():
            versions.sort(reverse=True)
            keep = versions[0][1]
            to_move = [f for v, f in versions[1:]]
            if not to_move:
                continue
            deprecated_dir = os.path.join(entity_path, 'Deprecated')
            os.makedirs(deprecated_dir, exist_ok=True)
            for f in to_move:
                src = os.path.join(entity_path, f)
                dst = os.path.join(deprecated_dir, f.replace('.sql', '-Deprecated.sql'))
                print(f"Moviendo {src} -> {dst}")
                os.rename(src, dst)
    print(f"\n{Fore.GREEN}Archivos antiguos movidos y renombrados correctamente.{Style.RESET_ALL}")

def get_foreign_key_dependencies(conn):
    """
    Devuelve un diccionario {tabla: set(tablas_de_las_que_depende)} para todas las tablas con FKs.
    """
    cursor = conn.cursor()
    query = '''
    SELECT 
        fk_tab.schema_id AS fk_schema_id,
        fk_tab.name AS fk_table,
        pk_tab.schema_id AS pk_schema_id,
        pk_tab.name AS pk_table
    FROM sys.foreign_keys fk
    INNER JOIN sys.tables fk_tab ON fk.parent_object_id = fk_tab.object_id
    INNER JOIN sys.tables pk_tab ON fk.referenced_object_id = pk_tab.object_id
    '''
    cursor.execute(query)
    dependencies = {}
    for row in cursor.fetchall():
        fk_schema = cursor.execute(f"SELECT SCHEMA_NAME({row.fk_schema_id})").fetchval()
        pk_schema = cursor.execute(f"SELECT SCHEMA_NAME({row.pk_schema_id})").fetchval()
        fk_full = f"{fk_schema}.{row.fk_table}"
        pk_full = f"{pk_schema}.{row.pk_table}"
        dependencies.setdefault(fk_full, set()).add(pk_full)
    cursor.close()
    return dependencies

def topological_sort_for_delete(tables, dependencies):
    """
    Ordena las tablas para borrado: primero hijos, luego padres.
    tables: lista de (schema, table)
    dependencies: dict {tabla: set(tablas_de_las_que_depende)}
    Devuelve lista de (schema, table) en orden de borrado.
    """
    table_names = [f"{schema}.{table}" for schema, table in tables]
    table_set = set(table_names)
    # Grafo inverso: tabla -> hijos (solo tablas en table_set)
    children = defaultdict(set)
    for child, parents in dependencies.items():
        if child not in table_set:
            continue
        for parent in parents:
            if parent in table_set:
                children[parent].add(child)
    # Contar hijos
    in_degree = {t: 0 for t in table_names}
    for t in table_names:
        in_degree[t] = len(children[t])
    # Cola de tablas sin hijos (hojas)
    queue = deque([t for t in table_names if in_degree[t] == 0])
    result = []
    visited = set()
    while queue:
        t = queue.popleft()
        result.append(t)
        visited.add(t)
        # Para cada padre de t, reducir su in_degree
        for parent in dependencies.get(t, []):
            if parent in in_degree:
                in_degree[parent] -= 1
                if in_degree[parent] == 0 and parent not in visited:
                    queue.append(parent)
    # Si faltan tablas (sin dependencias), agregarlas al final
    for t in table_names:
        if t not in result:
            result.append(t)
    return [tuple(t.split('.', 1)) for t in result]

def topological_sort_for_insert(tables, dependencies):
    """
    Ordena las tablas para inserción: primero padres, luego hijos.
    tables: lista de (schema, table)
    dependencies: dict {tabla: set(tablas_de_las_que_depende)}
    Devuelve lista de (schema, table) en orden de inserción.
    """
    table_names = [f"{schema}.{table}" for schema, table in tables]
    table_set = set(table_names)
    # Grafo: tabla -> padres (solo tablas en table_set)
    parents = defaultdict(set)
    for child, deps in dependencies.items():
        if child not in table_set:
            continue
        for parent in deps:
            if parent in table_set:
                parents[child].add(parent)
    # Contar padres
    in_degree = {t: 0 for t in table_names}
    for t in table_names:
        in_degree[t] = len(parents[t])
    # Cola de tablas sin padres (raíces)
    queue = deque([t for t in table_names if in_degree[t] == 0])
    result = []
    visited = set()
    while queue:
        t = queue.popleft()
        result.append(t)
        visited.add(t)
        # Para cada hijo de t, reducir su in_degree
        for child, deps in parents.items():
            if t in deps and child in in_degree:
                in_degree[child] -= 1
                if in_degree[child] == 0 and child not in visited:
                    queue.append(child)
    # Si faltan tablas (sin dependencias), agregarlas al final
    for t in table_names:
        if t not in result:
            result.append(t)
    return [tuple(t.split('.', 1)) for t in result]

def has_identity_column(conn, schema, table):
    """
    Devuelve True si la tabla tiene una columna IDENTITY.
    """
    cursor = conn.cursor()
    query = f"""
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.tables t ON c.object_id = t.object_id
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE s.name = ? AND t.name = ? AND c.is_identity = 1
    """
    cursor.execute(query, (schema, table))
    result = cursor.fetchone()
    cursor.close()
    return result is not None

def get_datetime_columns(conn, schema, table):
    """
    Devuelve una lista de nombres de columnas que son de tipo datetime en la tabla destino.
    (No incluye datetime2, solo datetime)
    """
    cursor = conn.cursor()
    query = f"""
    SELECT c.name
    FROM sys.columns c
    INNER JOIN sys.tables t ON c.object_id = t.object_id
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
    WHERE s.name = ? AND t.name = ? AND ty.name = 'datetime'
    """
    cursor.execute(query, (schema, table))
    cols = [row[0] for row in cursor.fetchall()]
    cursor.close()
    return cols

def get_notnull_datetime_columns(conn, schema, table):
    """
    Devuelve una lista de nombres de columnas que son de tipo datetime en la tabla destino.
    (No incluye datetime2, solo datetime)
    """
    cursor = conn.cursor()
    query = '''
    SELECT c.name
    FROM sys.columns c
    INNER JOIN sys.tables t ON c.object_id = t.object_id
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
    WHERE s.name = ? AND t.name = ? AND ty.name = 'datetime' AND c.is_nullable = 0
    '''
    cursor.execute(query, (schema, table))
    cols = [row[0] for row in cursor.fetchall()]
    cursor.close()
    return cols

def migrate_all_table_data(db1_conn, db2_conn):
    """
    Migra los datos de todas las tablas comunes de db1 a db2.
    Borra primero los datos de destino y luego inserta todos los datos de origen.
    Si hay error de FK, muestra advertencia y continúa.
    Además, chequea y limpia valores nulos en columnas datetime que no aceptan nulos,
    y detecta datos huérfanos en tablas con FK antes de migrar cada tabla.
    """
    print(f"\n{Fore.RED}ADVERTENCIA: Esta operación eliminará TODOS los datos de las tablas destino antes de migrar.{Style.RESET_ALL}")
    confirm = input(f"\n¿Desea continuar? (escriba 'SI' para continuar): ")
    if confirm.strip().upper() != 'SI':
        print("Operación cancelada por el usuario.")
        return

    db1_tables = get_tables(db1_conn)
    db2_tables = get_tables(db2_conn)
    table_comparison = compare_tables(db1_tables, db2_tables)
    common_tables = table_comparison['common']

    # Calcular dependencias de FK y orden de borrado/inserción
    dependencies = get_foreign_key_dependencies(db2_conn)
    delete_order = topological_sort_for_delete(common_tables, dependencies)
    insert_order = topological_sort_for_insert(common_tables, dependencies)

    # Helper para obtener columnas NOT NULL de tipo datetime
    def get_notnull_datetime_columns(conn, schema, table):
        cursor = conn.cursor()
        query = '''
        SELECT c.name
        FROM sys.columns c
        INNER JOIN sys.tables t ON c.object_id = t.object_id
        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
        INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
        WHERE s.name = ? AND t.name = ? AND ty.name = 'datetime' AND c.is_nullable = 0
        '''
        cursor.execute(query, (schema, table))
        cols = [row[0] for row in cursor.fetchall()]
        cursor.close()
        return cols

    # Helper para obtener FKs de una tabla
    def get_foreign_keys(conn, schema, table):
        cursor = conn.cursor()
        query = '''
        SELECT
            fk.name AS fk_name,
            c.name AS column_name,
            pk_s.name AS pk_schema,
            pk_t.name AS pk_table,
            pk_c.name AS pk_column
        FROM sys.foreign_keys fk
        INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
        INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
        INNER JOIN sys.columns c ON fkc.parent_column_id = c.column_id AND c.object_id = t.object_id
        INNER JOIN sys.tables pk_t ON fk.referenced_object_id = pk_t.object_id
        INNER JOIN sys.schemas pk_s ON pk_t.schema_id = pk_s.schema_id
        INNER JOIN sys.columns pk_c ON fkc.referenced_column_id = pk_c.column_id AND pk_c.object_id = pk_t.object_id
        WHERE s.name = ? AND t.name = ?
        '''
        cursor.execute(query, (schema, table))
        fks = cursor.fetchall()
        cursor.close()
        return fks

    # Borrar datos destino en orden seguro
    print(f"\n{Fore.CYAN}Eliminando datos de tablas destino (orden calculado)...{Style.RESET_ALL}")
    for schema, table in delete_order:
        try:
            print(f"  - Borrando [{schema}].[{table}]...", end=' ')
            cursor = db2_conn.cursor()
            cursor.execute(f"DELETE FROM [{schema}].[{table}]")
            db2_conn.commit()
            cursor.close()
            print(f"{Fore.GREEN}OK{Style.RESET_ALL}")
        except Exception as e:
            print(f"{Fore.RED}ERROR: {e}{Style.RESET_ALL}")

    print(f"\n{Fore.CYAN}Migrando datos de origen a destino (orden calculado)...{Style.RESET_ALL}")
    for schema, table in insert_order:
        try:
            print(f"  - Migrando [{schema}].[{table}]...", end=' ')
            # Leer datos de origen
            df = pd.read_sql(f"SELECT * FROM [{schema}].[{table}]", db1_conn)
            if df.empty:
                print(f"{Fore.YELLOW}Sin datos{Style.RESET_ALL}")
                continue

            # --- 1. Chequeo de datos huérfanos en FKs ---
            fks = get_foreign_keys(db2_conn, schema, table)
            orphan_warnings = []
            for fk in fks:
                fk_col = fk.column_name
                pk_schema = fk.pk_schema
                pk_table = fk.pk_table
                pk_col = fk.pk_column
                # Obtener valores únicos de la FK en el df
                fk_values = df[fk_col].dropna().unique()
                if len(fk_values) == 0:
                    continue
                # Consultar los valores existentes en la tabla padre destino
                q = f"SELECT [{pk_col}] FROM [{pk_schema}].[{pk_table}] WHERE [{pk_col}] IN ({','.join(['?' for _ in fk_values])})"
                cursor = db2_conn.cursor()
                # Convertir todos los valores a tipos nativos de Python
                params = [int(x) if isinstance(x, np.integer) else x for x in fk_values]
                cursor.execute(q, tuple(params))
                existing = set(row[0] for row in cursor.fetchall())
                cursor.close()
                # Detectar huérfanos
                orphans = set(fk_values) - existing
                if orphans:
                    orphan_warnings.append((fk_col, pk_schema, pk_table, pk_col, orphans))
                    # Omitir filas huérfanas
                    df = df[~df[fk_col].isin(orphans)]
            if orphan_warnings:
                print(f"\n    {Fore.YELLOW}[ADVERTENCIA] Se encontraron datos huérfanos en FKs:{Style.RESET_ALL}")
                for fk_col, pk_schema, pk_table, pk_col, orphans in orphan_warnings:
                    print(f"    - FK {fk_col} referencia a {pk_schema}.{pk_table}({pk_col}) con valores huérfanos: {list(orphans)[:5]}{'...' if len(orphans)>5 else ''} (total: {len(orphans)})")
                print(f"    [LOG] Filas huérfanas omitidas: {sum(len(o[4]) for o in orphan_warnings)}")
            if df.empty:
                print(f"{Fore.YELLOW}Sin datos tras limpiar huérfanos{Style.RESET_ALL}")
                continue

            # --- 2. Omitir SIEMPRE todas las columnas datetime ---
            dt_cols = get_datetime_columns(db2_conn, schema, table)
            cols_to_drop = [col for col in dt_cols if col in df.columns]
            # Detectar columnas datetime NOT NULL omitidas
            dt_notnull_cols = get_notnull_datetime_columns(db2_conn, schema, table)
            dt_notnull_cols_to_fill = [col for col in dt_notnull_cols if col in cols_to_drop]
            if cols_to_drop:
                print(f"    [LOG] Columnas datetime omitidas en [{schema}].[{table}]: {cols_to_drop}")
                df = df.drop(columns=cols_to_drop)
            # Si hay columnas datetime NOT NULL omitidas, agregarlas con fecha de hoy
            if dt_notnull_cols_to_fill:
                now = datetime.datetime.utcnow().replace(microsecond=0)  # UTC sin microsegundos
                for col in dt_notnull_cols_to_fill:
                    print(f"    [LOG] Columna {col} (datetime NOT NULL) rellenada con fecha UTC actual: {now} en [{schema}].[{table}]")
                    df[col] = now
            if df.empty:
                print(f"{Fore.YELLOW}Sin datos tras omitir columnas datetime{Style.RESET_ALL}")
                continue

            # Convertir todos los valores a tipos nativos de Python antes de insertar
            def convert_value(val):
                if pd.isnull(val):
                    return None
                if isinstance(val, pd.Timestamp):
                    return val.to_pydatetime()
                if isinstance(val, (pd.Int64Dtype, pd.UInt64Dtype, pd.Int32Dtype, pd.UInt32Dtype, pd.Int16Dtype, pd.UInt16Dtype)):
                    return int(val)
                if isinstance(val, (np.integer, np.int64, np.int32, np.int16, np.uint64, np.uint32, np.uint16)):
                    return int(val)
                if isinstance(val, (np.floating, np.float64, np.float32, np.float16)):
                    return float(val)
                return val
            df = df.applymap(convert_value)
            df = df.astype(object)

            # LOG: Mostrar preview de los datos a insertar
            print(f"\n    [LOG] [{schema}].[{table}] - Filas a insertar: {len(df)}")
            print(f"    [LOG] Columnas: {list(df.columns)}")
            print(f"    [LOG] Primeras filas:\n{df.head(3).to_string(index=False)}")
            # Insertar en destino
            cols = ','.join(f'[{col}]' for col in df.columns)
            placeholders = ','.join(['?' for _ in df.columns])
            insert_sql = f"INSERT INTO [{schema}].[{table}] ({cols}) VALUES ({placeholders})"
            cursor = db2_conn.cursor()
            cursor.fast_executemany = True
            identity_on = False
            try:
                if has_identity_column(db2_conn, schema, table):
                    cursor.execute(f"SET IDENTITY_INSERT [{schema}].[{table}] ON")
                    identity_on = True
                cursor.executemany(insert_sql, df.values.tolist())
                db2_conn.commit()
                print(f"{Fore.GREEN}OK ({len(df)}){Style.RESET_ALL}")
            except Exception as e:
                print(f"{Fore.RED}ERROR: {e}{Style.RESET_ALL}")
                print(f"    [LOG] Tipo de error: {type(e)}")
                print(f"    [LOG] Traceback:")
                traceback.print_exc()
            finally:
                if identity_on:
                    try:
                        cursor.execute(f"SET IDENTITY_INSERT [{schema}].[{table}] OFF")
                    except Exception as e:
                        print(f"{Fore.RED}ERROR al desactivar IDENTITY_INSERT en [{schema}].[{table}]: {e}{Style.RESET_ALL}")
                cursor.close()
        except Exception as e:
            print(f"{Fore.RED}ERROR: {e}{Style.RESET_ALL}")
            print(f"    [LOG] Tipo de error: {type(e)}")
            print(f"    [LOG] Traceback:")
            traceback.print_exc()
    print(f"\n{Fore.GREEN}Migración de datos finalizada.{Style.RESET_ALL}")

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
    parser.add_argument('--clean-old-sp-versions', action='store_true', help='Eliminar versiones viejas de SPs versionados en ambas bases')
    parser.add_argument('--dry-run', dest='dry_run', action='store_true', help='Solo mostrar los SPs que se eliminarían, sin ejecutar DROP (por defecto activo)')
    parser.add_argument('--no-dry-run', dest='dry_run', action='store_false', help='Ejecutar realmente el borrado de SPs viejos')
    parser.set_defaults(dry_run=True)
    parser.add_argument('--sync-missing-sps', action='store_true', help='Sincronizar SPs que existen en la base fuente pero faltan en la base destino (solo DB2)')
    parser.add_argument('--move-old-sp-files-local', action='store_true', help='Mover versiones viejas de SPs locales a carpeta Deprecated y renombrar como -Deprecated.sql')
    parser.add_argument('--migrate-data', action='store_true', help='Migrar datos de todas las tablas comunes de db1 a db2 (borra primero los datos de destino)')
    
    args = parser.parse_args()
    
    # Si no se especifica ninguna opción, mostrar ayuda
    if not (args.azure or args.local or args.tables or args.procedures or args.all or args.table_details or args.all_tables or args.show_sp_diff or args.export_sp or args.sp_name or args.clean_old_sp_versions or args.sync_missing_sps or args.move_old_sp_files_local or args.migrate_data):
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
    db1_name = "Azure DB (NUTRE OLD)"
    # db2_name = "Local DB (NUTRE)"
    db2_name = "Azure DB (NUTRE NEW)"
    
    # Establecer conexiones
    db1_conn = get_connection(**db1_config)
    db2_conn = get_connection(**db2_config)
    
    if not db1_conn or not db2_conn:
        print("No se pudieron establecer ambas conexiones. Saliendo.")
        return
    
    try:
        # Limpiar versiones viejas de SPs si se solicita
        if args.clean_old_sp_versions:
            print(f"\n{Fore.CYAN}=== LIMPIEZA DE VERSIONES VIEJAS DE SPs (NNN_NombreSP) ==={Style.RESET_ALL}")
            clean_old_sp_versions(db1_conn, db1_name, dry_run=args.dry_run)
            clean_old_sp_versions(db2_conn, db2_name, dry_run=args.dry_run)
            return
        
        if args.sync_missing_sps:
            sync_missing_sps(db1_conn, db2_conn)
            return
        
        if args.move_old_sp_files_local:
            move_old_sp_files_local()
            return
        
        if args.migrate_data:
            migrate_all_table_data(db1_conn, db2_conn)
            return
        
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
