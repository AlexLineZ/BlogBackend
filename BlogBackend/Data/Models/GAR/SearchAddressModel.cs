
namespace BlogBackend.Models.GAR;

public class SearchAddressModel
{
    public long? ObjectId { get; set; }
    public Guid ObjectGuid { get; set; }
    public String? Text { get; set; }
    public GarAddressLevel? ObjectLevel { get; set; }
    public String? ObjectLevelText { get; set; }
    
    public SearchAddressModel() {}

    public SearchAddressModel(long? objectId, Guid objectGuid, String text, GarAddressLevel objectLevel,
        String objectLevelText)
    {
        ObjectId = objectId;
        ObjectGuid = objectGuid;
        Text = text;
        ObjectLevel = objectLevel;
        ObjectLevelText = objectLevelText;
    }
}