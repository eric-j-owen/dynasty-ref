using ClientApi.Dtos;
using ClientApi.Dtos.Espn;
using ClientApi.Dtos.Players;
using ClientApi.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Consts;

namespace ClientApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController(PlayerService playerService, EspnService espnService) : ControllerBase
{
    private readonly PlayerService _playerService = playerService;
    private readonly EspnService _espnService = espnService;

    [HttpGet]
    [Route("rankings")]
    public async Task<ActionResult<PaginatedResults<PlayerRankingDto>>> GetRankings(
        [FromQuery] string? searchName,
        [FromQuery] string? positions,
        [FromQuery] DataSource sortDataSource = DataSource.FantasyCalc,
        [FromQuery] bool isSuperFlex = true,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 15

    )
    {

        var result = await _playerService.GetPlayerRankings(searchName, positions, isSuperFlex, sortDataSource, page, pageSize);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // [HttpGet]
    // [Route("{playerId}")]
    // public async Task<ActionResult<PlayerDetailsDto>> GetPlayerDetails(int playerId)
    // {
    //     var result = await _playerService.GetPlayerDetails(playerId);

    //     if (result == null)
    //     {
    //         return NotFound();
    //     }

    //     return Ok(result);
    // }

    [HttpGet]
    [Route("stats/{espnId}")]
    public async Task<ActionResult<EspnAthleteStatsResponse>> GetPlayerEspnStats(int espnId)
    {
        try
        {
            var response = await _espnService.GetPlayerStats(espnId);
            if (response == null)
            {
                return NotFound(new
                {
                    detail = $"{espnId} not found",
                    instance = HttpContext.Request.Path
                });
            }

            return Ok(response);
        }
        catch (Exception e)
        {
            return Problem(detail: $"{e}", instance: HttpContext.Request.Path);
        }

    }
}