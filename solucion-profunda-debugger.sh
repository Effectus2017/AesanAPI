#!/bin/bash

echo "🔧 SOLUCIÓN PROFUNDA DEL DEBUGGER DE C#"
echo "======================================="
echo ""

# Función para mostrar progreso
show_progress() {
    echo "✅ $1"
    sleep 1
}

# 1. Cerrar VS Code completamente
echo "🔄 Cerrando VS Code completamente..."
pkill -f "Visual Studio Code" 2>/dev/null || true
pkill -f "Code" 2>/dev/null || true
sleep 3
show_progress "VS Code cerrado"

# 2. Limpiar TODOS los caches
echo "🧹 Limpiando todos los caches..."
rm -rf ~/.vscode/extensions/ms-dotnettools.*/bin/ 2>/dev/null || true
rm -rf ~/.vscode/extensions/ms-dotnettools.*/out/ 2>/dev/null || true
rm -rf ~/.omnisharp/ 2>/dev/null || true
rm -rf ~/.vscode/extensions/ms-dotnettools.*/.omnisharp/ 2>/dev/null || true
rm -rf ~/.vscode/logs/ 2>/dev/null || true
rm -rf ~/.vscode/CachedExtensions/ 2>/dev/null || true
show_progress "Caches limpiados"

# 3. Matar todos los procesos relacionados
echo "🔄 Terminando procesos relacionados..."
pkill -f omnisharp 2>/dev/null || true
pkill -f "dotnet.*omnisharp" 2>/dev/null || true
pkill -f "dotnet.*Api" 2>/dev/null || true
show_progress "Procesos terminados"

# 4. Verificar .NET SDK
echo "🔍 Verificando .NET SDK..."
DOTNET_VERSION=$(dotnet --version 2>/dev/null)
if [ $? -eq 0 ]; then
    show_progress "NET SDK $DOTNET_VERSION encontrado"
else
    echo "❌ Error: .NET SDK no encontrado"
    exit 1
fi

# 5. Reconstruir proyecto completamente
echo "🔨 Reconstruyendo proyecto completamente..."
dotnet clean > /dev/null 2>&1
rm -rf bin/ obj/ 2>/dev/null || true
dotnet restore > /dev/null 2>&1
dotnet build > /dev/null 2>&1
if [ $? -eq 0 ]; then
    show_progress "Proyecto reconstruido correctamente"
else
    echo "❌ Error en la reconstrucción del proyecto"
    exit 1
fi

# 6. Verificar extensiones
echo "🔍 Verificando extensiones de C#..."
C_SHARP_EXT=$(code --list-extensions 2>/dev/null | grep "ms-dotnettools.csharp" | wc -l)
CSDEVKIT_EXT=$(code --list-extensions 2>/dev/null | grep "ms-dotnettools.csdevkit" | wc -l)

if [ $CSDEVKIT_EXT -gt 0 ]; then
    show_progress "C# Dev Kit encontrado"
else
    echo "⚠️  Instalando C# Dev Kit..."
    code --install-extension ms-dotnettools.csdevkit > /dev/null 2>&1
    show_progress "C# Dev Kit instalado"
fi

# 7. Crear configuración de OmniSharp
echo "⚙️ Configurando OmniSharp..."
mkdir -p .omnisharp
cat > .omnisharp/omnisharp.json << 'EOF'
{
  "FormattingOptions": {
    "EnableEditorConfigSupport": true,
    "OrganizeImports": true
  },
  "RoslynExtensionsOptions": {
    "EnableAnalyzersSupport": true,
    "EnableImportCompletion": true,
    "EnableAsyncCompletion": true
  },
  "Sdk": {
    "IncludePrereleases": false
  }
}
EOF
show_progress "OmniSharp configurado"

echo ""
echo "🎉 ¡SOLUCIÓN PROFUNDA COMPLETADA!"
echo "================================="
echo ""
echo "📋 INSTRUCCIONES FINALES:"
echo ""
echo "1. 🚀 Abre VS Code desde este directorio:"
echo "   code ."
echo ""
echo "2. ⏳ ESPERA completamente a que VS Code cargue (puede tomar 1-2 minutos)"
echo ""
echo "3. 🔧 Reinicia OmniSharp:"
echo "   - Presiona Ctrl+Shift+P"
echo "   - Escribe: 'C#: Restart OmniSharp'"
echo "   - Presiona Enter"
echo "   - ESPERA a que termine (verás el indicador en la barra inferior)"
echo ""
echo "4. 🐛 Prueba el debugger:"
echo "   - Presiona F5"
echo "   - O ve a Run and Debug (Ctrl+Shift+D)"
echo ""
echo "5. 🧪 Si aún no funciona, prueba ejecutando:"
echo "   dotnet run --environment Development"
echo ""
echo "✅ Esta solución debería resolver definitivamente el problema!"
