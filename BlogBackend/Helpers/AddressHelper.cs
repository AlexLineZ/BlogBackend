using BlogBackend.Models.GAR;

namespace BlogBackend.Helpers;

public static class AddressHelper
{
    public static GarAddressLevel GetGarAddressLevel(int? level)
    {
        GarAddressLevel name = level switch
        {
            1 => GarAddressLevel.Region,
            2 => GarAddressLevel.AdministrativeArea,
            3 => GarAddressLevel.MunicipalArea,
            4 => GarAddressLevel.RuralUrbanSettlement,
            5 => GarAddressLevel.City,
            6 => GarAddressLevel.Locality,
            7 => GarAddressLevel.ElementOfPlanningStructure,
            8 => GarAddressLevel.ElementOfRoadNetwork,
            9 => GarAddressLevel.Land,
            10 => GarAddressLevel.Building,
            11 => GarAddressLevel.Room,
            12 => GarAddressLevel.RoomInRooms,
            13 => GarAddressLevel.AutonomousRegionLevel,
            14 => GarAddressLevel.IntracityLevel,
            15 => GarAddressLevel.AdditionalTerritoriesLevel,
            16 => GarAddressLevel.LevelOfObjectsInAdditionalTerritories,
            17 => GarAddressLevel.CarPlace,
            _ => GarAddressLevel.Region
        };
        
        return name;
    }

    public static String GetAddressName(int? level)
    {
        var name = level switch
        {
            1 => "Субъект РФ",
            2 => "Административный район",
            3 => "Муниципальный район",
            4 => "Сельское/городское поселение",
            5 => "Город",
            6 => "Населенный пункт",
            7 => "Элемент планировочной структуры",
            8 => "Элемент улично-дорожной сети",
            9 => "Земельный участок",
            10 => "Здание (сооружение)",
            11 => "Помещение",
            12 => "Помещения в пределах помещения",
            13 => "Уровень автономного округа",
            14 => "Уровень внутригородской территории",
            15 => "Уровень дополнительных территорий",
            16 => "Уровень объектов на дополнительных территориях",
            17 => "Машиноместо",
            _ => String.Empty
        };
        
        return name;
    }
    
    public static string GetHouseType(int? type)
    {
        var name = type switch
        {
            1 => "корпус",
            2 => "строение",
            3 => "сооружение",
            4 => "литера",
            _ => String.Empty
        };
        return name;
    }
}