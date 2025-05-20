# Guía de patrón CRUD para modelos (API + Angular)

## Descripción general

Cuando se indique el nombre de un modelo (por ejemplo, "KitchenType"), debes replicar el siguiente workflow para implementar el CRUD completo, siguiendo el patrón de AgencyStatus/KitchenType. No inventes lógica nueva salvo que se indique explícitamente. Usa siempre las convenciones y estructuras existentes.

---

### 1. Backend (.NET + SQL Server)

#### 1.1. Base de Datos

- Crear la tabla en la base de datos siguiendo las convenciones de nombres y tipos.
  - **Ejemplo:** `Database/Tables/KitchenType/KitchenType.sql`
- Crear los Stored Procedures para:
  - Insertar (Add)  
    **Ejemplo:** `SP_KitchenType_Add.sql`
  - Actualizar (Update)  
    **Ejemplo:** `SP_KitchenType_Update.sql`
  - Eliminar (Delete, preferentemente lógico)  
    **Ejemplo:** `SP_KitchenType_Delete.sql`
  - Obtener por ID (GetById)  
    **Ejemplo:** `SP_KitchenType_GetById.sql`
  - Listar todos (GetAll)  
    **Ejemplo:** `SP_KitchenType_GetAll.sql`
- Ubicar scripts en `Database/Tables/<Modelo>/`.

#### 1.2. Capa de Datos y Lógica

- Crear el modelo C# en `Models/Tables/`.
  - **Ejemplo:** `Models/Tables/KitchenType.cs`
- Crear el DTO en `Models/DTO/` si aplica.
  - **Ejemplo:** `Models/DTO/DTOKitchenType.cs`
- Crear la interfaz y la implementación del repositorio en `Repositories/`.
  - **Ejemplo:** `Repositories/IKitchenTypeRepository.cs`, `Repositories/KitchenTypeRepository.cs`
- Crear el controlador en `Controllers/`.
  - **Ejemplo:** `Controllers/KitchenTypeController.cs`
- Seguir el patrón de inyección de dependencias y manejo de errores.
- Agregar en Startup la relación de Interface y Repository
  - **Ejemplo:**
    ```csharp
    services.AddScoped<IKitchenTypeRepository, KitchenTypeRepository>();
    ```

#### 1.3. Controlador (Controller)

El controlador debe seguir el patrón de AgencyStatusController:

- Rutas explícitas y descriptivas para cada acción (ej: "insert-<modelo>", "get-all-<modelo>-from-db", etc.).
- Usar siempre constructor principal.
- Usar DTOs para la entrada y salida de datos.
- Validar el ModelState antes de ejecutar la lógica.
- Manejar errores con bloques try/catch y loguear excepciones.
- Devolver respuestas HTTP adecuadas:
  - Ok() para lecturas exitosas
  - CreatedAtAction() para inserciones
  - NoContent() para actualizaciones/eliminaciones exitosas
  - NotFound() y BadRequest() según corresponda
- Documentar cada endpoint con [SwaggerOperation] y comentarios XML.
- Inyectar el repositorio por constructor.
- No debe contener lógica de negocio, solo orquestar la llamada al repositorio y el manejo de la respuesta.

#### Ejemplo de estructura para el controlador

```csharp
[Route("<modelo>-status")]
public class <Modelo>StatusController : ControllerBase
{
    private readonly I<Modelo>StatusRepository _repository;
    private readonly ILogger<<Modelo>StatusController> _logger;

    public <Modelo>StatusController(I<Modelo>StatusRepository repository, ILogger<<Modelo>StatusController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("get-<modelo>-status-by-id")]
    [SwaggerOperation(Summary = "Obtiene un estado de {modelo} por su ID", Description = "Devuelve un estado de {modelo} basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var item = await _repository.GetById(id);
            if (item == null)
            {
                return NotFound($"Estado de {modelo} con ID {id} no encontrado");
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el estado de {modelo} con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el estado de {modelo}");
        }
    }

    [HttpGet("get-all-<modelo>-status-from-db")]
    [SwaggerOperation(Summary = "...", Description = "...")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var items = await _repository.GetAll(queryParameters...);
                return Ok(items);
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ...");
            return StatusCode(500, "Error interno del servidor ...");
        }
    }

    // ... (repetir para GetById, Insert, Update, Delete, etc.)
}
```

---

### 2. Frontend (Angular)

#### 2.1. Estructura

- Crear una carpeta bajo `src/app/modules/admin-portal/<modelo>/` (o `agency-portal`/`monitor-portal` según corresponda).
  - **Ejemplo:** `src/app/modules/admin-portal/kitchen-type/`
- Incluir componentes para:
  - Listado (`list/`)  
    **Ejemplo:** `src/app/modules/admin-portal/kitchen-type/list/`
  - Alta (`add/`)  
    **Ejemplo:** `src/app/modules/admin-portal/kitchen-type/add/`
  - Edición (`edit/`)  
    **Ejemplo:** `src/app/modules/admin-portal/kitchen-type/edit/`
- Crear el servicio Angular para consumir la API, si no existe.
  - **Ejemplo:** `src/app/shared/services/kitchen-type.service.ts`
- Crear el archivo de rutas del módulo: `<modelo>.routes.ts`
- **Crear el archivo `resolvers.ts`** en la carpeta del módulo para la carga inicial de datos y detalles, siguiendo el patrón de kitchen-type.
- **No crear archivos `module.ts`** en la carpeta del módulo.
- **No crear archivos `.scss`**: los estilos deben implementarse exclusivamente con Tailwind CSS en los templates HTML.

#### 2.1.1. Estructura de carpeta y archivos para el listado (list)

- Dentro de `<modelo>/list/` deben existir:
  - `list.component.ts`: Componente principal del listado
  - `list.component.html`: Template del listado
  - `columns-schema.ts`: Definición de columnas para la tabla

##### Ejemplo de estructura:

```
<modelo>/list/
  ├── list.component.ts
  ├── list.component.html
  └── columns-schema.ts
```

##### Detalles de implementación:

- **list.component.ts**

  - Standalone component (Angular 15+)
  - Importa y usa:
    - `GenericHeaderComponent` para el header y búsqueda
    - `GenericTableComponent` para la tabla
    - `MatPaginatorModule` para paginación
    - ReactiveFormsModule y FormsModule
    - Servicio del modelo (ej: `KitchenTypeService`)
    - Utilidades de Angular Material (opcional)
    - `TranslocoModule` para i18n
  - Implementa lógica de:
    - Búsqueda reactiva
    - Paginación
    - Navegación a alta/edición
    - Suscripción a observable de datos del servicio
    - Limpieza de suscripciones con `takeUntil` y `Subject`
  - Ejemplo de propiedades:
    - `headerConfig: GenericHeaderConfig`
    - `tableConfig: GenericTableConfig`
    - Métodos: `onSearch`, `getAll`, `getPaginator`, `onClean`, `onTableEdit`, `onAdd`

- **list.component.html**

  - Estructura base:
    ```html
    <div class="absolute inset-0 flex flex-col min-w-0 overflow-hidden">
      <app-generic-header
        [config]="headerConfig"
        [handler]="this"
      ></app-generic-header>
      <div class="flex-auto overflow-y-auto" cdkScrollable>
        <app-generic-table
          [config]="tableConfig"
          [handler]="this"
        ></app-generic-table>
      </div>
      <mat-paginator
        class="hidden sm:block"
        [pageSizeOptions]="tableConfig.pageSizeOptions"
        [pageSize]="tableConfig.pageSize"
        [length]="tableConfig.length"
        (page)="getPaginator($event)"
        showFirstLastButtons
      >
      </mat-paginator>
    </div>
    ```
  - Usar solo clases Tailwind para estilos.

- **columns-schema.ts**

  - Exporta un array de columnas para la tabla, siguiendo la interfaz `ColumnSchema`.
  - Ejemplo:
    ```typescript
    export const COLUMNS_SCHEMA: ColumnSchema[] = [
      { key: "id", type: "text", label: "<modelo>.list.table.columns.id" },
      { key: "name", type: "text", label: "<modelo>.list.table.columns.name" },
      // ... otras columnas ...
      {
        key: "actions",
        type: "button",
        label: "<modelo>.list.table.columns.actions",
        buttons: [{ key: "edit", label: "<modelo>.list.table.buttons.edit" }],
      },
    ];
    ```

- **Notas:**
  - El patrón debe replicar exactamente la estructura y lógica de kitchen-type/agency-status.
  - No agregar archivos ni lógica extra fuera de lo especificado.

#### 2.1.2. Estructura de carpeta y archivos para alta (add) y edición (edit)

- Dentro de `<modelo>/add/` y `<modelo>/edit/` deben existir:
  - `add.component.ts` y `add.component.html`
  - `edit.component.ts` y `edit.component.html`

##### Ejemplo de estructura:

```
<modelo>/add/
  ├── add.component.ts
  └── add.component.html
<modelo>/edit/
  ├── edit.component.ts
  └── edit.component.html
```

##### Detalles de implementación:

- **add.component.ts** y **edit.component.ts**

  - Standalone component (Angular 15+)
  - Importa y usa:
    - `GenericHeaderComponent` para el header y botones de acción
    - ReactiveFormsModule y FormsModule
    - Servicio del modelo (ej: `KitchenTypeService`)
    - Utilidades de Angular Material (ej: `MatFormFieldModule`, `MatInputModule`, `MatCheckboxModule`, `MatButtonModule`, `MatSelectModule`)
    - `TranslocoModule` para i18n
    - `CustomRouterService` para navegación
    - (Opcional) `MatSnackBar` para notificaciones
    - (Opcional) `FuseConfirmationService` para diálogos de confirmación
  - Implementa lógica de:
    - Inicialización reactiva del formulario (`formGroup` en `headerConfig`)
    - Validación de campos
    - Manejo de guardado (`onSave`) y cancelación (`onCancel`)
    - Lógica de orden/posición si aplica (ver kitchen-type)
    - Navegación tras guardar/cancelar
    - Consumo de datos del resolver para edición

- **add.component.html** y **edit.component.html**

  - Estructura base:
    ```html
    <div class="absolute inset-0 flex flex-col min-w-0">
      <app-generic-header
        [config]="headerConfig"
        [handler]="this"
      ></app-generic-header>
      <div class="flex-auto overflow-y-auto sm:p-6 sm:w-auto" cdkScrollable>
        <form [formGroup]="headerConfig.formGroup">
          <!-- Campos del formulario -->
        </form>
      </div>
    </div>
    ```
  - Usar solo clases Tailwind para estilos.
  - Los campos del formulario deben replicar el patrón de kitchen-type/agency-status (ejemplo: mat-form-field, mat-checkbox, mat-select, etc.)

- **Notas:**
  - El patrón debe replicar exactamente la estructura y lógica de kitchen-type/agency-status.
  - No agregar archivos ni lógica extra fuera de lo especificado.

#### 2.2. Internacionalización

- Agregar traducciones en los archivos de i18n correspondientes.
  - **Ejemplo:** `src/assets/i18n/es/kitchen-types.json`

#### 2.3. UI/UX

- Usar los componentes y estilos existentes (tablas, formularios, diálogos).
- Validar formularios y mostrar mensajes de error consistentes.
- **Todos los estilos deben implementarse con Tailwind CSS.**

---

### 3. Documentación y Progreso

- Actualizar la documentación técnica si aplica.
- Registrar el avance en `progress.md` y actualizar el Memory Bank si corresponde.

---

### 4. Notas

- No inventar lógica nueva: replicar el patrón AgencyStatus/KitchenType.
- Si el modelo requiere lógica especial, documentarla explícitamente.
- Mantener la trazabilidad de cambios en el changelog correspondiente.

---

**Resumen:**
Al recibir el nombre de un modelo, ejecuta este workflow completo para implementar el CRUD en backend y frontend, siguiendo siempre las convenciones y estructuras existentes en el proyecto. No crear archivos innecesarios fuera de los aquí especificados.
