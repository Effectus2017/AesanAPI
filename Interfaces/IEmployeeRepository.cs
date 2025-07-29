using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IEmployeeRepository
{
    /// <summary>
    /// Obtiene un empleado por su ID
    /// </summary>
    /// <param name="id">El ID del empleado</param>
    /// <returns>El empleado</returns>
    Task<dynamic> GetEmployeeById(int id);

    /// <summary>
    /// Obtiene todos los empleados de la base de datos
    /// </summary>
    /// <param name="take">El número de empleados a obtener</param>
    /// <param name="skip">El número de empleados a saltar</param>
    /// <param name="name">El nombre del empleado a buscar</param>
    /// <param name="alls">Si se deben obtener todos los empleados</param>
    /// <param name="isList">Si es para lista simple (dropdowns)</param>
    /// <returns>Los empleados</returns>
    Task<dynamic> GetAllEmployeesFromDb(int take, int skip, string name, bool alls, bool isList);

    /// <summary>
    /// Inserta un nuevo empleado en la base de datos
    /// </summary>
    /// <param name="employeeRequest">Datos del empleado a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    Task<bool> InsertEmployee(EmployeeRequest employeeRequest);

    /// <summary>
    /// Actualiza un empleado existente en la base de datos
    /// </summary>
    /// <param name="employeeRequest">Datos del empleado a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateEmployee(EmployeeRequest employeeRequest);

    /// <summary>
    /// Elimina un empleado de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del empleado a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteEmployee(int id);

    /// <summary>
    /// Convierte un empleado en usuario del sistema
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>True si se convirtió correctamente</returns>
    Task<bool> ConvertEmployeeToUser(int employeeId, string userId);

    /// <summary>
    /// Verifica si existe un empleado principal
    /// </summary>
    /// <returns>True si existe un empleado principal</returns>
    Task<bool> HasMainEmployee();

    /// <summary>
    /// Actualiza el estado activo de un empleado
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    /// <param name="isActive">Nuevo estado activo</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateEmployeeActiveStatus(int employeeId, bool isActive);
}