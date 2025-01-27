using AutoMapper;
using Business.GameManager;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController
{
    private readonly IMapper _mapper;
    private readonly IGameManager _gameManager;
    
    public MatchController(IMapper mapper, IGameManager gameManager)
    {
        _mapper = mapper;
        _gameManager = gameManager;
    }
}
