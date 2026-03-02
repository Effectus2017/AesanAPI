namespace Api.Models.Response;

/// <summary>
/// Contenedor para respuestas paginadas. Usado en endpoints de listado que devuelven { data, count }.
/// </summary>
/// <typeparam name="T">Tipo de cada elemento (ej. XxxTableResponse).</typeparam>
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int Count { get; set; }
}
