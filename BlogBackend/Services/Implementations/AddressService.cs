using BlogBackend.Data;
using BlogBackend.Exceptions;
using BlogBackend.Helpers;
using BlogBackend.Models.GAR;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogBackend.Services.Implementations;

public class AddressService : IAddressService
{
    private readonly Gar70Context _garDbContext;

    public AddressService(Gar70Context garDbContext)
    {
        _garDbContext = garDbContext;
    }

    public async Task<List<SearchAddressModel>> Search(Int64 parentObjectId, string? query)
    {
        var hierarchyList = _garDbContext.AsAdmHierarchies
            .Where(x => x.Parentobjid == parentObjectId && x.Isactive == 1)
            .AsQueryable();

        var addressList = new List<SearchAddressModel>();

        if (!hierarchyList.IsNullOrEmpty())
        {
            addressList = FindChildInAddrObj(hierarchyList);

            if (addressList.IsNullOrEmpty())
            {
                addressList = FindChildInHouses(hierarchyList);
            }
        }

        if (query != null)
        {
            addressList = addressList
                .Where(x => x.Text != null 
                            && x.Text.ToUpper().Contains(query.ToUpper()))
                .ToList();
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
                GetHouseName(house),
                GarAddressLevel.Building,
                AddressHelper.GetAddressName(10)
            );
            addressList.Add(searchHouseModel);

            var houseHierarchy = await _garDbContext.AsAdmHierarchies
                .Where(x => x.Objectid == house.Objectid && x.Isactive == 1)
                .FirstOrDefaultAsync();

            if (houseHierarchy!= null) {}
            var parentGuid = _garDbContext.AsAddrObjs.FirstOrDefault(x =>
                x.Objectid == houseHierarchy.Parentobjid && x.Isactive == 1 && x.Isactual == 1).Objectguid;

            objectGuid = parentGuid;
        }

        var address = _garDbContext.AsAddrObjs.FirstOrDefault(x =>
            x.Objectguid == objectGuid && x.Isactive == 1 && x.Isactual == 1);

        if (address == null)
        {
            throw new ResourceNotFoundException($"Object with GUID {objectGuid} is not found");
        }

        var searchAddressModel = new SearchAddressModel(
            address.Objectid,
            address.Objectguid,
            address.Typename + " " + address.Name,
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

    private List<SearchAddressModel> FindChildInAddrObj(IQueryable<AsAdmHierarchy> hierarchyList)
    {
        var addressQuery = 
            from hierarchyItem in hierarchyList
            join address in _garDbContext.AsAddrObjs
                on hierarchyItem.Objectid equals address.Objectid
            where address.Isactive == 1 && address.Isactual == 1
            select new SearchAddressModel
            {
                ObjectId = address.Objectid,
                ObjectGuid = address.Objectguid,
                Text = $"{address.Typename} {address.Name}",
                ObjectLevel = AddressHelper.GetGarAddressLevel(Convert.ToInt32(address.Level)),
                ObjectLevelText = AddressHelper.GetAddressName(Convert.ToInt32(address.Level))
            };

        return addressQuery.ToList();
    }

    private List<SearchAddressModel> FindChildInHouses(IQueryable<AsAdmHierarchy> hierarchyList)
    {
        var addressQuery = 
            from hierarchyItem in hierarchyList
            join address in _garDbContext.AsHouses
                on hierarchyItem.Objectid equals address.Objectid
            where address.Isactive == 1 && address.Isactual == 1
            select new SearchAddressModel
            {
                ObjectId = address.Objectid,
                ObjectGuid = address.Objectguid,
                Text = GetHouseName(address),
                ObjectLevel = GarAddressLevel.Building,
                ObjectLevelText = AddressHelper.GetAddressName(10)
            };

        return addressQuery.ToList();
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
                address.Typename + " " + address.Name,
                AddressHelper.GetGarAddressLevel(Convert.ToInt32(address.Level)),
                AddressHelper.GetAddressName(Convert.ToInt32(address.Level))
            )
            : null;
    }
    
    private static string GetHouseName(AsHouse house)
    {
        var chainHouseNumber = new List<String>();

        if (!String.IsNullOrEmpty(house.Housenum))
        {
            chainHouseNumber.Add(house.Housenum);
        }

        if (house.Addtype1 != null)
        {
            chainHouseNumber.Add(AddressHelper.GetHouseType(house.Addtype1));
            chainHouseNumber.Add(house.Addnum1 ?? String.Empty);
        }

        if (house.Addtype2 != null)
        {
            chainHouseNumber.Add(AddressHelper.GetHouseType(house.Addtype2));
            chainHouseNumber.Add(house.Addnum2 ?? String.Empty);
        }

        return String.Join(" ", chainHouseNumber).Trim();
    }
}
