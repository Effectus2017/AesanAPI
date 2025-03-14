// =============================================
// Script de Integración de Auto-Despliegue con Cursor
// Autor: Equipo de Desarrollo
// Fecha: 2025-03-07
// =============================================

// Este script está diseñado para ser ejecutado por Cursor cuando se guarda un archivo SQL
// en el directorio de procedimientos almacenados.

const fs = require('fs');
const path = require('path');
const { exec } = require('child_process');
const os = require('os');

// Configuración
const isWindows = os.platform() === 'win32';
const projectRoot = path.resolve(__dirname, '../../..');
const databaseDir = path.resolve(__dirname, '..');
const spDir = path.join(databaseDir, 'StoredProcedures');

// Función para verificar si un archivo es un procedimiento almacenado
function isStoredProcedure(filePath) {
    // Verificar si el archivo está en el directorio de SPs
    if (!filePath.startsWith(spDir)) {
        return false;
    }

    // Verificar si el archivo tiene extensión .sql
    if (path.extname(filePath) !== '.sql') {
        return false;
    }

    // Verificar si el contenido del archivo contiene "CREATE PROCEDURE" o "CREATE OR ALTER PROCEDURE"
    try {
        const content = fs.readFileSync(filePath, 'utf8');
        return content.includes('CREATE PROCEDURE') || content.includes('CREATE OR ALTER PROCEDURE');
    } catch (error) {
        console.error(`Error al leer el archivo ${filePath}: ${error.message}`);
        return false;
    }
}

// Función para ejecutar el script de auto-despliegue
function runAutoDeployScript(filePath) {
    console.log(`Detectado cambio en procedimiento almacenado: ${filePath}`);

    // Determinar qué script ejecutar según el sistema operativo
    const scriptPath = isWindows
        ? path.join(__dirname, 'auto-deploy-sp.ps1')
        : path.join(__dirname, 'auto-deploy-sp.sh');

    // Comando a ejecutar
    const command = isWindows
        ? `powershell -ExecutionPolicy Bypass -File "${scriptPath}"`
        : `"${scriptPath}" local`;

    console.log(`Ejecutando: ${command}`);

    // Ejecutar el script
    exec(command, (error, stdout, stderr) => {
        if (error) {
            console.error(`Error al ejecutar el script: ${error.message}`);
            return;
        }

        if (stderr) {
            console.error(`Error en la salida del script: ${stderr}`);
            return;
        }

        console.log(`Salida del script: ${stdout}`);
    });
}

// Función principal
function main() {
    // Obtener el archivo que se acaba de guardar (proporcionado por Cursor)
    const savedFilePath = process.argv[2];

    if (!savedFilePath) {
        console.error('No se proporcionó un archivo');
        return;
    }

    // Normalizar la ruta del archivo
    const normalizedPath = path.resolve(savedFilePath);

    // Verificar si el archivo es un procedimiento almacenado
    if (isStoredProcedure(normalizedPath)) {
        // Ejecutar el script de auto-despliegue
        runAutoDeployScript(normalizedPath);
    } else {
        console.log(`El archivo ${normalizedPath} no es un procedimiento almacenado o no está en el directorio correcto`);
    }
}

// Ejecutar la función principal
main(); 