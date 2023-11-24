using BlogBackend.Models.GAR;

namespace BlogBackend.Services.Interfaces;

public interface IAddressService
{
    Task<List<SearchAddressModel>> Search(Int64 parentObjectId, string? query);
    Task<List<SearchAddressModel>> Chain(String objectGuid);
}