using BlogBackend.Data;
using BlogBackend.Helpers;
using BlogBackend.Models.GAR;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogBackend.Services.Implementations;

public class AddressService: IAddressService
{
    private readonly AppDbContext _dbContext;
    private readonly Gar70Context _garDbContext;

    public AddressService(AppDbContext dbContext, Gar70Context garDbContext)
    {
        _dbContext = dbContext;
        _garDbContext = garDbContext;
    }
    
    public async Task<List<SearchAddressModel>> Search(Int64 parentObjectId, string? query)
    {
        var hierarchyList = await _garDbContext.AsAdmHierarchies
            .Where(x => x.Parentobjid == parentObjectId)
            .ToListAsync();

        var addressList = new List<SearchAddressModel>();

        if (!hierarchyList.IsNullOrEmpty())
        {
            addressList = await FindChildInAddrObj(hierarchyList);

            if (addressList.IsNullOrEmpty())
            {
                addressList = await FindChildInHouses(hierarchyList);
            }
        }

        return addressList;
    }

    public async Task<List<SearchAddressModel>> Chain(String objectGuid)
    {
        var hierarchyList = _garDbContext.AsAdmHierarchies
            .Where(x => x.Areacode == objectGuid)
            .ToList();

        var list = hierarchyList.Select(x =>
                new SearchAddressModel(x.Objectid, new Guid(), "", GarAddressLevel.Region, ""))
            .ToList();
        
        return list;
    }

    private async Task<List<SearchAddressModel>> FindChildInAddrObj(List<AsAdmHierarchy> hierarchyList)
    {
        var addressList = new List<SearchAddressModel>();

        foreach (var hierarchyItem in hierarchyList)
        {
            var address = await _garDbContext.AsAddrObjs
                .Where(a => a.Objectid == hierarchyItem.Objectid)
                .FirstOrDefaultAsync();

            if (address != null)
            {
                var searchAddressModel = new SearchAddressModel(
                    address.Objectid,
                    address.Objectguid,
                    address.Name,
                    AddressHelper.GetGarAddressLevel(Convert.ToInt32(address.Level)),
                    AddressHelper.GetAddressName(Convert.ToInt32(address.Level))
                );

                addressList.Add(searchAddressModel);
            }
        }

        return addressList;
    }
    
    private async Task<List<SearchAddressModel>> FindChildInHouses(List<AsAdmHierarchy> hierarchyList)
    {
        var addressList = new List<SearchAddressModel>();

        foreach (var hierarchyItem in hierarchyList)
        {
            var address = await _garDbContext.AsHouses
                .Where(a => a.Objectid == hierarchyItem.Objectid)
                .FirstOrDefaultAsync();

            if (address != null)
            {
                var searchAddressModel = new SearchAddressModel(
                    address.Objectid,
                    address.Objectguid,
                    address.Housenum,
                    GarAddressLevel.Building,
                    AddressHelper.GetAddressName(10)
                );

                addressList.Add(searchAddressModel);
            }
        }

        return addressList;
    }
}