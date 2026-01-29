using ClientApi.Dtos;
using ClientApi.Dtos.Espn;
using ClientApi.Dtos.Players;
using ClientApi.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Consts;
using System.Net;

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
            return Ok(response);
        }

        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(new
            {
                instance = HttpContext.Request.Path,
                e.StatusCode,
                e.Message,
            });
        }
        catch (Exception)
        {
            return Problem(detail: "an error occurred", instance: HttpContext.Request.Path);
        }

    }
}