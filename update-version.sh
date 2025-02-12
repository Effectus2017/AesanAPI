#!/bin/bash

# Obtener la última versión de Git (sin la "v" al inicio)
VERSION=$(git describe --tags --abbrev=0 | sed 's/^v//')

# Reemplazar la versión en appsettings.json
jq --arg ver "$VERSION" '.ApiSettings.Version = $ver' appsettings.json > appsettings.tmp.json && mv appsettings.tmp.json appsettings.json

# Reemplazar la versión en el archivo .csproj
sed -i -E "s/<Version>[0-9]+\.[0-9]+\.[0-9]+<\/Version>/<Version>$VERSION<\/Version>/" Api/Api.csproj

echo "✅ Versión actualizada en package.json, appsettings.json y Api.csproj: $VERSION"
