using AutoMapper;
using Business.GameManager;
using Business.Models;
using Game;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IGameManager _gameManager;
    
    public MatchController(IMapper mapper, IGameManager gameManager)
    {
        _mapper = mapper;
        _gameManager = gameManager;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OkResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CreateMatch(CreateMatchRequest request)
    {
        try
        {
            var hostId = Guid.Parse(request.HostId);
            var result = await _gameManager.CreateMatch(_mapper.Map<MatchHistoryModel>(request));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
