using System;

namespace Api.Models;

public class DTOAgencyUser
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsActive { get; set; }
    
    // CAMPOS NUEVOS
    public string? AgencyAssignmentType { get; set; }
    public string? RoleId { get; set; }  // Desde JOIN, no se almacena en AgencyUsers
    public string? RoleName { get; set; }  // Desde JOIN, solo para display
    
    // Campos existentes
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
}