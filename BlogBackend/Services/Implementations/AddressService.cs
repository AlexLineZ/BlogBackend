using BlogBackend.Data;
using BlogBackend.Exceptions;
using BlogBackend.Helpers;
using BlogBackend.Models.GAR;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogBackend.Services.Implementations;

public class AddressService: IAddressService
{
    private readonly Gar70Context _garDbContext;

    public AddressService(Gar70Context garDbContext)
    {
        _garDbContext = garDbContext;
    }
    
    public async Task<List<SearchAddressModel>> Search(Int64 parentObjectId, string? query)
    {
        var hierarchyList = await _garDbContext.AsAdmHierarchies
            .Where(x => x.Parentobjid == parentObjectId && x.Isactive == 1)
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

        if (query != null)
        {
            addressList = addressList.Where(x => x.Text.Contains(query)).ToList();
        }
        
        return addressList;
    }

    public async Task<List<SearchAddressModel>> Chain(Guid objectGuid)
    {
        var house = _garDbContext.AsHouses.FirstOrDefault(x =>
            x.Objectguid == objectGuid && x.Isactive == 1 && x.Isactual == 1);

        var addressList = new List<SearchAddressModel>();
        
        if (house != null)
        {
            var searchHouseModel = new SearchAddressModel(
                house.Objectid,
                house.Objectguid,
                house.Housenum,
                GarAddressLevel.Building,
                AddressHelper.GetAddressName(10)
            );
            addressList.Add(searchHouseModel);
            
            var houseHierarchy = await _garDbContext.AsAdmHierarchies
                .Where(x => x.Objectid == house.Objectid && x.Isactive == 1)
                .FirstOrDefaultAsync();
            
            var parentGuid = _garDbContext.AsAddrObjs.FirstOrDefault(x =>
                x.Objectid == houseHierarchy.Parentobjid && x.Isactive == 1 && x.Isactual == 1).Objectguid;

            objectGuid = parentGuid;
        }
        
        var address =  _garDbContext.AsAddrObjs.FirstOrDefault(x =>
            x.Objectguid == objectGuid && x.Isactive == 1 && x.Isactual == 1);

        if (address == null)
        {
            throw new ResourceNotFoundException($"Object with GUID {objectGuid} is not found");
        }
            
        var searchAddressModel = new SearchAddressModel(
            address.Objectid,
            address.Objectguid,
            address.Name,
            AddressHelper.GetGarAddressLevel(Convert.ToInt32(address.Level)),
            AddressHelper.GetAddressName(Convert.ToInt32(address.Level))
        );
            
        while (searchAddressModel != null)
        {
            addressList.Add(searchAddressModel);
            searchAddressModel = await FindParentInAddrObjOneObject(searchAddressModel.ObjectId);
        }

        addressList.Reverse();
        
        return addressList;
    }

    private async Task<List<SearchAddressModel>> FindChildInAddrObj(List<AsAdmHierarchy> hierarchyList)
    {
        var addressList = new List<SearchAddressModel>();

        foreach (var hierarchyItem in hierarchyList)
        {
            var address = await _garDbContext.AsAddrObjs
                .Where(a => a.Objectid == hierarchyItem.Objectid && a.Isactive == 1 && a.Isactual == 1)
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
                .Where(a => a.Objectid == hierarchyItem.Objectid && a.Isactive == 1 && a.Isactual == 1)
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

    private async Task<SearchAddressModel?> FindParentInAddrObjOneObject(long? objectId)
    {
        var hierarchyItem = await _garDbContext.AsAdmHierarchies
            .Where(x => x.Objectid == objectId && x.Isactive == 1)
            .FirstOrDefaultAsync();

        if (hierarchyItem == null || !hierarchyItem.Parentobjid.HasValue)
        {
            return null;
        }

        var address = await _garDbContext.AsAddrObjs
            .Where(a => a.Objectid == hierarchyItem.Parentobjid && a.Isactive == 1 && a.Isactual == 1)
            .FirstOrDefaultAsync();

        return address != null
            ? new SearchAddressModel(
                address.Objectid,
                address.Objectguid,
                address.Name,
                AddressHelper.GetGarAddressLevel(Convert.ToInt32(address.Level)),
                AddressHelper.GetAddressName(Convert.ToInt32(address.Level))
            )
            : null;
    }

}