namespace Api.Models.Response;

/// <summary>
/// Tipo de visita para combos (catálogo VisitType).
/// </summary>
public class VisitTypeDropdownItemResponse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string NameEs { get; set; } = string.Empty;

    public string NameEN { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public int? RuleMaxWeeksFromProgramStart { get; set; }
}
