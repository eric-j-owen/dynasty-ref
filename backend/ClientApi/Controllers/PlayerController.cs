using ClientApi.Dtos;
using ClientApi.Dtos.Players;
using ClientApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController(PlayerService playerService) : ControllerBase
{
    private readonly PlayerService _playerService = playerService;

    [HttpGet]
    [Route("rankings")]
    public async Task<ActionResult<PaginatedResults<PlayerRankingDto>>> GetRankings(
        [FromQuery] string? searchName,
        [FromQuery] string? positions,
        [FromQuery] string sortDataSource = "keeptradecut",
        [FromQuery] bool isSuperFlex = true,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 15

    )
    {

        var result = await _playerService.GetPlayerRankings(searchName, positions, sortDataSource, isSuperFlex, page, pageSize);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet]
    [Route("{playerId}")]
    public async Task<ActionResult<PlayerDetailsDto>> GetPlayerDetails(int playerId)
    {
        var result = await _playerService.GetPlayerDetails(playerId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

}