using AutoMapper;
using Business.Models;
using Business.User;
using Game;
using Microsoft.AspNetCore.Mvc;


namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    
    public UserController(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransferMoneyResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferMoneyAsync(TransferMoneyRequest request)
    {
        var result = _mapper.Map<TransferMoneyResponse>( 
            await _userService.TransferMoneyAsync(_mapper.Map<TransferMoneyModel>(request)));;

        return Ok(result);
    }
}