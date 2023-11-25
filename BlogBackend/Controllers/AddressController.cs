using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[ApiController]
[Route("api/address")]
public class AddressController: ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Search(Int64 parentObjectId, string? query)
    {
        var list = await _addressService.Search(parentObjectId, query);
        return Ok(list);
    }

    [HttpGet]
    [Route("chain")]
    public async Task<IActionResult> Chain(Guid objectGuid)
    {
        var list = await _addressService.Chain(objectGuid);
        return Ok(list);
    }
}