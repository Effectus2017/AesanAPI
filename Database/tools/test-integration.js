// =============================================
// Script de Prueba para la Integración con Cursor
// Autor: Equipo de Desarrollo
// Fecha: 2025-03-07
// =============================================

// Este script simula la ejecución del script de integración con Cursor
// para probar su funcionamiento sin necesidad de configurar Cursor.

const path = require('path');
const { spawn } = require('child_process');

// Configuración
const integrationScriptPath = path.join(__dirname, 'cursor-integration.js');

// Función para ejecutar el script de integración
function runIntegrationScript(filePath) {
    console.log(`Probando integración con archivo: ${filePath}`);

    // Ejecutar el script de integración
    const process = spawn('node', [integrationScriptPath, filePath], {
        stdio: 'inherit'
    });

    process.on('close', (code) => {
        console.log(`Script de integración finalizado con código: ${code}`);
    });
}

// Función principal
function main() {
    // Obtener la ruta del archivo de prueba
    const testFilePath = process.argv[2];

    if (!testFilePath) {
        console.error('Uso: node test-integration.js <ruta-al-archivo-sql>');
        console.error('Ejemplo: node test-integration.js ../StoredProcedures/GetAgencyById.sql');
        return;
    }

    // Normalizar la ruta del archivo
    const normalizedPath = path.resolve(__dirname, testFilePath);

    // Ejecutar el script de integración
    runIntegrationScript(normalizedPath);
}

// Ejecutar la función principal
main(); 