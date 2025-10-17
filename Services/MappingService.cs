using Api.Models;
using Api.Models.Response;
using Api.Services.Mappers;

namespace Api.Services;

/// <summary>
/// Servicio centralizado para todos los mapeos de datos
/// Proporciona una interfaz unificada para mapear resultados dinámicos a DTOs
/// </summary>
public class MappingService
{
    private readonly AgencyMapper _agencyMapper;
    private readonly SiteMapper _siteMapper;

    public MappingService(AgencyMapper agencyMapper, SiteMapper siteMapper)
    {
        _agencyMapper = agencyMapper ?? throw new ArgumentNullException(nameof(agencyMapper));
        _siteMapper = siteMapper ?? throw new ArgumentNullException(nameof(siteMapper));
    }
    #region Agency Mappings

    /// <summary>
    /// Mapea una agencia desde un resultado dinámico a un DTOAgency
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgency</returns>
    public DTOAgency MapAgency(dynamic item)
    {
        return _agencyMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea los datos de inscripción de agencia desde un resultado dinámico a un DTOAgencyInscription
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgencyInscription</returns>
    public DTOAgencyInscription? MapAgencyInscription(dynamic item)
    {
        return AgencyMapper.MapInscriptionFromResult(item);
    }

    /// <summary>
    /// Mapea una agencia desde un resultado dinámico a un objeto con campos necesarios para listas y tablas de UI
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto con campos necesarios para la UI</returns>
    public dynamic MapAgencyList(dynamic item)
    {
        return _agencyMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de agencias desde resultados dinámicos a objetos para listas y tablas de UI
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de objetos con campos necesarios para la UI</returns>
    public IEnumerable<dynamic> MapAgencyList(IEnumerable<dynamic> items)
    {
        if (items == null)
        {
            return new List<dynamic>();
        }

        return items.Select(MapAgencyList).Where(item => item != null);
    }

    #endregion

    #region Program Mappings

    /// <summary>
    /// Mapea los programas de una agencia desde resultados dinámicos a una lista de DTOProgram
    /// </summary>
    /// <param name="programs">Resultados dinámicos de los programas</param>
    /// <returns>Lista de DTOProgram</returns>
    public List<DTOProgram> MapPrograms(IEnumerable<dynamic> programs)
    {
        return ProgramMapper.MapFromResult(programs);
    }

    /// <summary>
    /// Mapea un programa individual desde un resultado dinámico a un DTOProgram
    /// </summary>
    /// <param name="program">Resultado dinámico del programa</param>
    /// <returns>DTOProgram</returns>
    public DTOProgram? MapProgram(dynamic program)
    {
        return ProgramMapper.MapSingleProgramFromResult(program);
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una lista de programas (versión simplificada)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOProgram</returns>
    public DTOProgram MapProgramList(dynamic result)
    {
        return ProgramMapper.MapListFromResult(result);
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un programa completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOProgram</returns>
    public DTOProgram MapProgramFromResult(dynamic result)
    {
        return ProgramMapper.MapFromResult(result);
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un programa de inscripción
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOProgramInscription</returns>
    public DTOProgramInscription MapProgramInscription(dynamic result)
    {
        return ProgramMapper.MapInscriptionFromResult(result);
    }

    #endregion

    #region Staff Mappings

    /// <summary>
    /// Mapea una lista de staff desde un resultado dinámico
    /// </summary>
    /// <param name="result">Resultado dinámico</param>
    /// <returns>Objeto mapeado para listas</returns>
    public dynamic MapStaffList(dynamic result)
    {
        return StaffMapper.MapListFromResult(result);
    }

    /// <summary>
    /// Mapea un staff individual desde un resultado dinámico
    /// </summary>
    /// <param name="result">Resultado dinámico</param>
    /// <returns>Objeto mapeado</returns>
    public dynamic MapStaff(dynamic result)
    {
        return StaffMapper.MapFromResult(result);
    }

    /// <summary>
    /// Mapea los detalles de un staff desde un resultado dinámico a un DTOStaff
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOStaff</returns>
    public DTOStaff MapStaffDetails(dynamic item)
    {
        return StaffMapper.MapDetailsFromResult(item, this);
    }

    #endregion

    #region School Mappings

    /// <summary>
    /// Mapea una escuela desde un resultado dinámico a un SchoolResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolResponse</returns>
    public SchoolResponse MapSchool(dynamic item)
    {
        return new SchoolResponse
        {
            Id = item.Id,
            AgencyId = item.AgencyId,
            Name = item.Name,
            SchoolCode = item.SchoolCode,
            SchoolNumber = item.SchoolNumber,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            Agency = new DTOAgency
            {
                Id = item.AgencyId,
                Name = item.AgencyName,
                Code = item.AgencyCode
            }
        };
    }

    /// <summary>
    /// Mapea una asignación School-Site desde un resultado dinámico a un SchoolSiteTableResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolSiteTableResponse</returns>
    public SchoolSiteTableResponse MapSchoolSite(dynamic item)
    {
        return new SchoolSiteTableResponse
        {
            Id = item.Id,
            SchoolId = item.SchoolId,
            SiteId = item.SiteId,
            AssignmentDate = item.AssignmentDate,
            Comment = item.Comment,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            SiteName = item.SiteName,
            SiteCode = item.SiteCode,
            SiteNumber = item.SiteNumber,
            Address = item.Address,
            SiteIsActive = item.IsActive
        };
    }

    #endregion

    #region Site Mappings

    /// <summary>
    /// Mapea un sitio desde un resultado dinámico a un SiteResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteResponse</returns>
    public SiteResponse MapSite(dynamic item)
    {
        return _siteMapper.MapResponseFromResult(item);
    }

    /// <summary>
    /// Mapea un sitio desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteResponse para listas</returns>
    public SiteResponse MapSiteList(dynamic item)
    {
        return SiteMapper.MapResponseListFromResult(item);
    }

    /// <summary>
    /// Mapea un sitio desde un resultado dinámico para tablas optimizadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteTableResponse para tablas</returns>
    public SiteTableResponse MapSiteTable(dynamic item)
    {
        return SiteMapper.MapTableResponseFromResult(item);
    }

    /// <summary>
    /// Mapea un servicio de sitio desde un resultado dinámico a un SiteServiceResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteServiceResponse</returns>
    public SiteServiceResponse MapSiteService(dynamic item)
    {
        return SiteMapper.MapSiteServiceFromResult(item);
    }

    /// <summary>
    /// Mapea información de Day Care Home desde un resultado dinámico a un SiteDayCareHomeResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteDayCareHomeResponse</returns>
    public SiteDayCareHomeResponse MapSiteDayCareHome(dynamic item)
    {
        return SiteMapper.MapSiteDayCareHomeFromResult(item);
    }

    /// <summary>
    /// Mapea un participante de sitio desde un resultado dinámico a un SiteParticipantResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteParticipantResponse</returns>
    public SiteParticipantResponse MapSiteParticipant(dynamic item)
    {
        return SiteMapper.MapSiteParticipantFromResult(item);
    }

    /// <summary>
    /// Mapea un sitio desde un resultado dinámico para elementos de lista
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteListItemResponse para listas</returns>
    public SiteListItemResponse MapSiteListItem(dynamic item)
    {
        return SiteMapper.MapResponseListFromResult(item);
    }

    /// <summary>
    /// Mapea un nivel educativo desde un resultado dinámico a un DTOEducationLevel
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOEducationLevel</returns>
    public DTOEducationLevel MapEducationLevel(dynamic item)
    {
        return SiteMapper.MapEducationLevelFromResult(item);
    }

    #endregion

    #region Option Selection Mappings

    /// <summary>
    /// Mapea una opción de selección desde un resultado dinámico a un DTOOptionSelection (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOptionSelection</returns>
    public DTOOptionSelection MapOptionSelectionList(dynamic item)
    {
        return OptionSelectionMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea una opción de selección desde un resultado dinámico a un DTOOptionSelection completo
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOptionSelection</returns>
    public DTOOptionSelection MapOptionSelection(dynamic item)
    {
        return OptionSelectionMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de opciones de selección desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOOptionSelection</returns>
    public List<DTOOptionSelection> MapOptionSelections(IEnumerable<dynamic> items)
    {
        return items.Select(MapOptionSelection).ToList();
    }

    #endregion

    #region Agency User Mappings

    /// <summary>
    /// Mapea un usuario de agencia desde un resultado dinámico a un DTOAgencyUser (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgencyUser</returns>
    public DTOAgencyUser MapAgencyUserList(dynamic item)
    {
        return AgencyUserMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un usuario de agencia desde un resultado dinámico a un DTOAgencyUser completo
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgencyUser</returns>
    public DTOAgencyUser MapAgencyUser(dynamic item)
    {
        return AgencyUserMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de usuarios de agencia desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOAgencyUser</returns>
    public List<DTOAgencyUser> MapAgencyUsers(IEnumerable<dynamic> items)
    {
        return items.Select(MapAgencyUser).ToList();
    }

    #endregion

    #region User Mappings

    /// <summary>
    /// Mapea un usuario desde un resultado dinámico a un DTOUser
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOUser</returns>
    public DTOUser MapUser(dynamic item)
    {
        return UserMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de usuarios desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOUser</returns>
    public List<DTOUser> MapUsers(IEnumerable<dynamic> items)
    {
        return items.Select(MapUser).ToList();
    }

    #endregion

    #region Area Type Mappings

    /// <summary>
    /// Mapea un tipo de área desde un resultado dinámico a un DTOAreaType (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAreaType</returns>
    public DTOAreaType MapAreaTypeList(dynamic item)
    {
        return AreaTypeMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un tipo de área desde un resultado dinámico a un DTOAreaType completo
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAreaType</returns>
    public DTOAreaType MapAreaType(dynamic item)
    {
        return AreaTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de área desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOAreaType</returns>
    public List<DTOAreaType> MapAreaTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapAreaType).ToList();
    }

    #endregion

    #region Delivery Type Mappings

    /// <summary>
    /// Mapea un tipo de entrega desde un resultado dinámico a un DeliveryTypeResponse (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DeliveryTypeResponse</returns>
    public DeliveryTypeResponse MapDeliveryTypeList(dynamic item)
    {
        return DeliveryTypeMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un tipo de entrega desde un resultado dinámico a un DeliveryTypeResponse completo
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DeliveryTypeResponse</returns>
    public DeliveryTypeResponse MapDeliveryType(dynamic item)
    {
        return DeliveryTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de entrega desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DeliveryTypeResponse</returns>
    public List<DeliveryTypeResponse> MapDeliveryTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapDeliveryType).ToList();
    }

    #endregion

    #region Center Type Mappings

    /// <summary>
    /// Mapea un tipo de centro desde un resultado dinámico a un DTOCenterType (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOCenterType</returns>
    public DTOCenterType MapCenterTypeList(dynamic item)
    {
        return CenterTypeMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un tipo de centro desde un resultado dinámico a un DTOCenterType completo
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOCenterType</returns>
    public DTOCenterType MapCenterType(dynamic item)
    {
        return CenterTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de centro desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOCenterType</returns>
    public List<DTOCenterType> MapCenterTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapCenterType).ToList();
    }

    #endregion

    #region Organization Type Mappings

    /// <summary>
    /// Mapea un tipo de organización desde un resultado dinámico a un DTOOrganizationType (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOrganizationType</returns>
    public DTOOrganizationType MapOrganizationTypeList(dynamic item)
    {
        return OrganizationTypeMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un tipo de organización desde un resultado dinámico a un DTOOrganizationType completo
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOrganizationType</returns>
    public DTOOrganizationType MapOrganizationType(dynamic item)
    {
        return OrganizationTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de organización desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOOrganizationType</returns>
    public List<DTOOrganizationType> MapOrganizationTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapOrganizationType).ToList();
    }

    #endregion

    #region Staff Type Mappings

    /// <summary>
    /// Mapea un tipo de personal desde un resultado dinámico a un objeto StaffType (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto StaffType mapeado</returns>
    public dynamic MapStaffTypeList(dynamic item)
    {
        return StaffTypeMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un tipo de personal desde un resultado dinámico a un objeto StaffType completo
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto StaffType mapeado</returns>
    public dynamic MapStaffType(dynamic item)
    {
        return StaffTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de personal desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de objetos StaffType mapeados</returns>
    public List<dynamic> MapStaffTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapStaffType).ToList();
    }

    #endregion

    #region Agency Status Mappings

    /// <summary>
    /// Mapea un estado de agencia desde un resultado dinámico a un DTOAgencyStatus
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgencyStatus</returns>
    public DTOAgencyStatus MapAgencyStatus(dynamic item)
    {
        return AgencyStatusMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de estados de agencia desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOAgencyStatus</returns>
    public List<DTOAgencyStatus> MapAgencyStatuses(IEnumerable<dynamic> items)
    {
        return items.Select(MapAgencyStatus).ToList();
    }

    #endregion

    #region Geo Mappings

    /// <summary>
    /// Mapea una ciudad desde un resultado dinámico a un DTOCity
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOCity</returns>
    public DTOCity MapCity(dynamic item)
    {
        return GeoMapper.MapCityFromResult(item);
    }

    /// <summary>
    /// Mapea una región desde un resultado dinámico a un DTORegion
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTORegion</returns>
    public DTORegion MapRegion(dynamic item)
    {
        return GeoMapper.MapRegionFromResult(item);
    }

    /// <summary>
    /// Mapea una ciudad desde un resultado dinámico a un DTOCity (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOCity</returns>
    public DTOCity MapCityList(dynamic item)
    {
        return GeoMapper.MapCityListFromResult(item);
    }

    /// <summary>
    /// Mapea una región desde un resultado dinámico a un DTORegion (versión simplificada para listas)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTORegion</returns>
    public DTORegion MapRegionList(dynamic item)
    {
        return GeoMapper.MapRegionListFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de ciudades desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOCity</returns>
    public List<DTOCity> MapCities(IEnumerable<dynamic> items)
    {
        return items.Select(MapCityList).ToList();
    }

    /// <summary>
    /// Mapea una lista de regiones desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTORegion</returns>
    public List<DTORegion> MapRegions(IEnumerable<dynamic> items)
    {
        return items.Select(MapRegionList).ToList();
    }

    #endregion

    #region StaffClassification Mappings

    /// <summary>
    /// Mapea una clasificación de staff desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto mapeado para listas</returns>
    public dynamic MapStaffClassificationList(dynamic item)
    {
        return StaffClassificationMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea una clasificación de staff desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto mapeado para listas paginadas</returns>
    public dynamic MapStaffClassification(dynamic item)
    {
        return StaffClassificationMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de clasificaciones de staff desde resultados dinámicos para listas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de objetos mapeados para listas</returns>
    public List<dynamic> MapStaffClassificationLists(IEnumerable<dynamic> items)
    {
        return items.Select(MapStaffClassificationList).ToList();
    }

    /// <summary>
    /// Mapea una lista de clasificaciones de staff desde resultados dinámicos para listas paginadas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de objetos mapeados para listas paginadas</returns>
    public List<dynamic> MapStaffClassifications(IEnumerable<dynamic> items)
    {
        return items.Select(MapStaffClassification).ToList();
    }

    #endregion

    #region KitchenType Mappings

    /// <summary>
    /// Mapea un tipo de cocina desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOKitchenType para listas</returns>
    public DTOKitchenType MapKitchenTypeList(dynamic item)
    {
        return KitchenTypeMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un tipo de cocina desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOKitchenType para listas paginadas</returns>
    public DTOKitchenType MapKitchenType(dynamic item)
    {
        return KitchenTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de cocina desde resultados dinámicos para listas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOKitchenType para listas</returns>
    public List<DTOKitchenType> MapKitchenTypeLists(IEnumerable<dynamic> items)
    {
        return items.Select(MapKitchenTypeList).ToList();
    }

    /// <summary>
    /// Mapea una lista de tipos de cocina desde resultados dinámicos para listas paginadas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOKitchenType para listas paginadas</returns>
    public List<DTOKitchenType> MapKitchenTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapKitchenType).ToList();
    }

    #endregion

    #region GroupType Mappings

    /// <summary>
    /// Mapea un tipo de grupo desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOGroupType para listas</returns>
    public DTOGroupType MapGroupTypeList(dynamic item)
    {
        return GroupTypeMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un tipo de grupo desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOGroupType para listas paginadas</returns>
    public DTOGroupType MapGroupType(dynamic item)
    {
        return GroupTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de grupo desde resultados dinámicos para listas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOGroupType para listas</returns>
    public List<DTOGroupType> MapGroupTypeLists(IEnumerable<dynamic> items)
    {
        return items.Select(MapGroupTypeList).ToList();
    }

    /// <summary>
    /// Mapea una lista de tipos de grupo desde resultados dinámicos para listas paginadas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOGroupType para listas paginadas</returns>
    public List<DTOGroupType> MapGroupTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapGroupType).ToList();
    }

    #endregion

    #region OperatingPeriod Mappings

    /// <summary>
    /// Mapea un período operativo desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPeriod para listas</returns>
    public DTOOperatingPeriod MapOperatingPeriodList(dynamic item)
    {
        return OperatingPeriodMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea un período operativo desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPeriod para listas paginadas</returns>
    public DTOOperatingPeriod MapOperatingPeriod(dynamic item)
    {
        return OperatingPeriodMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de períodos operativos desde resultados dinámicos para listas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOOperatingPeriod para listas</returns>
    public List<DTOOperatingPeriod> MapOperatingPeriodLists(IEnumerable<dynamic> items)
    {
        return items.Select(MapOperatingPeriodList).ToList();
    }

    /// <summary>
    /// Mapea una lista de períodos operativos desde resultados dinámicos para listas paginadas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOOperatingPeriod para listas paginadas</returns>
    public List<DTOOperatingPeriod> MapOperatingPeriods(IEnumerable<dynamic> items)
    {
        return items.Select(MapOperatingPeriod).ToList();
    }

    #endregion

    #region OperatingPolicy Mappings

    /// <summary>
    /// Mapea una política operativa desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPolicy para listas</returns>
    public DTOOperatingPolicy MapOperatingPolicyList(dynamic item)
    {
        return OperatingPolicyMapper.MapListFromResult(item);
    }

    /// <summary>
    /// Mapea una política operativa desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPolicy para listas paginadas</returns>
    public DTOOperatingPolicy MapOperatingPolicy(dynamic item)
    {
        return OperatingPolicyMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de políticas operativas desde resultados dinámicos para listas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOOperatingPolicy para listas</returns>
    public List<DTOOperatingPolicy> MapOperatingPolicyLists(IEnumerable<dynamic> items)
    {
        return items.Select(MapOperatingPolicyList).ToList();
    }

    /// <summary>
    /// Mapea una lista de políticas operativas desde resultados dinámicos para listas paginadas
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOOperatingPolicy para listas paginadas</returns>
    public List<DTOOperatingPolicy> MapOperatingPolicies(IEnumerable<dynamic> items)
    {
        return items.Select(MapOperatingPolicy).ToList();
    }

    #endregion

    #region StaffRelationship Mappings

    /// <summary>
    /// Mapea una relación de staff desde un resultado dinámico
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOStaffRelationship</returns>
    public DTOStaffRelationship MapStaffRelationship(dynamic item)
    {
        return StaffRelationshipMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de relaciones de staff desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOStaffRelationship</returns>
    public List<DTOStaffRelationship> MapStaffRelationships(IEnumerable<dynamic> items)
    {
        return items.Select(MapStaffRelationship).ToList();
    }

    #endregion

    #region SponsorType Mappings

    /// <summary>
    /// Mapea un tipo de auspiciador desde un resultado dinámico
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOSponsorType</returns>
    public DTOSponsorType MapSponsorType(dynamic item)
    {
        return SponsorTypeMapper.MapFromResult(item);
    }

    /// <summary>
    /// Mapea una lista de tipos de auspiciador desde resultados dinámicos
    /// </summary>
    /// <param name="items">Resultados dinámicos</param>
    /// <returns>Lista de DTOSponsorType</returns>
    public List<DTOSponsorType> MapSponsorTypes(IEnumerable<dynamic> items)
    {
        return items.Select(MapSponsorType).ToList();
    }

    #endregion

    #region Common Relationship Mappings

    /// <summary>
    /// Mapea un estado de agencia desde campos individuales
    /// </summary>
    /// <param name="statusId">ID del estado</param>
    /// <param name="statusName">Nombre del estado</param>
    /// <returns>DTOAgencyStatus o null</returns>
    public DTOAgencyStatus? MapAgencyStatus(int? statusId, string? statusName)
    {
        if (statusId == null) return null;

        return new DTOAgencyStatus
        {
            Id = statusId.Value,
            Name = statusName ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea una selección de opción desde campos individuales
    /// </summary>
    /// <param name="id">ID de la opción</param>
    /// <param name="name">Nombre de la opción</param>
    /// <param name="nameEN">Nombre en inglés de la opción</param>
    /// <param name="optionKey">Clave de la opción</param>
    /// <returns>DTOOptionSelection o null</returns>
    public DTOOptionSelection? MapOptionSelection(int? id, string? name, string? nameEN = null, string? optionKey = null)
    {
        if (id == null) return null;

        return new DTOOptionSelection
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty,
            OptionKey = optionKey ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de organización desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOOrganizationType o null</returns>
    public DTOOrganizationType? MapOrganizationType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOOrganizationType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de cocina desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOKitchenType o null</returns>
    public DTOKitchenType? MapKitchenType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOKitchenType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de grupo desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOGroupType o null</returns>
    public DTOGroupType? MapGroupType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOGroupType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de entrega desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DeliveryTypeResponse o null</returns>
    public DeliveryTypeResponse? MapDeliveryType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DeliveryTypeResponse
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de auspiciador desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOSponsorType o null</returns>
    public DTOSponsorType? MapSponsorType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOSponsorType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de centro desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOCenterType o null</returns>
    public DTOCenterType? MapCenterType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOCenterType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de área desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOAreaType o null</returns>
    public DTOAreaType? MapAreaType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOAreaType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de staff desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOStaffType o null</returns>
    public DTOStaffType? MapStaffType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOStaffType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEn = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo de aplicante desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOApplicantType o null</returns>
    public DTOApplicantType? MapApplicantType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOApplicantType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea un tipo residencial desde campos individuales
    /// </summary>
    /// <param name="id">ID del tipo</param>
    /// <param name="name">Nombre del tipo</param>
    /// <param name="nameEN">Nombre en inglés del tipo</param>
    /// <returns>DTOResidentialType o null</returns>
    public DTOResidentialType? MapResidentialType(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOResidentialType
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea una política operativa desde campos individuales
    /// </summary>
    /// <param name="id">ID de la política</param>
    /// <param name="name">Nombre de la política</param>
    /// <param name="nameEN">Nombre en inglés de la política</param>
    /// <returns>DTOOperatingPolicy o null</returns>
    public DTOOperatingPolicy? MapOperatingPolicy(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOOperatingPolicy
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEN = nameEN ?? string.Empty
        };
    }

    /// <summary>
    /// Mapea una clasificación de staff desde campos individuales
    /// </summary>
    /// <param name="id">ID de la clasificación</param>
    /// <param name="name">Nombre de la clasificación</param>
    /// <param name="nameEN">Nombre en inglés de la clasificación</param>
    /// <returns>DTOStaffClassification o null</returns>
    public DTOStaffClassification? MapStaffClassification(int? id, string? name, string? nameEN = null)
    {
        if (id == null) return null;

        return new DTOStaffClassification
        {
            Id = id.Value,
            Name = name ?? string.Empty,
            NameEn = nameEN ?? string.Empty
        };
    }

    #endregion
}
