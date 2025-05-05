# systemPatterns.md

## Patrón de uso de parámetros en repositorios

Todos los métodos de inserción, actualización y borrado que interactúan con procedimientos almacenados deben:

- Usar siempre `DynamicParameters` de Dapper para pasar parámetros.
- Especificar explícitamente el tipo de dato (`DbType`) y la dirección (`ParameterDirection`) para cada parámetro, tanto de entrada como de salida.
- Este patrón previene errores de conversión, mejora la claridad y la mantenibilidad del código.

**Ejemplo:**

```csharp
var p = new DynamicParameters();
p.Add("@Nombre", request.Nombre, DbType.String, ParameterDirection.Input);
p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
```

Este estándar debe aplicarse a todos los repositorios del proyecto.
