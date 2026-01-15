using ClientApi.Dtos.Espn;
using Microsoft.Extensions.Caching.Memory;
using Shared.Consts;

namespace ClientApi.Services;


/*

athlete stats
common/v3/sports/football/nfl/athletes/4262921/stats

team ids
site/v2/sports/football/nfl/teams

team stats 
site/v2/sports/football/nfl/teams/22/statistics

*/
public class EspnService(HttpClient client, IMemoryCache cache)
{
    private readonly HttpClient _client = client;
    private readonly static string baseUrl = ApiBaseUrl.Espn;
    private readonly IMemoryCache _cache = cache;
    private static readonly TimeSpan cacheDuration = TimeSpan.FromDays(7);
    private readonly static Dictionary<TeamAbbr, int> _teamIdsMap = new() { { TeamAbbr.ARI, 22 } };

    public async Task<TeamStatsResponseDto?> GetTeamStats(string teamAbbr)
    {
        // parse parameter string to team abbreviations enum
        if (!Enum.TryParse<TeamAbbr>(teamAbbr, true, out var parsedTeam))
        {
            Console.WriteLine("invalid team abbreviation");
            return null;
        }

        var key = $"team_stats_{parsedTeam}";

        if (_cache.TryGetValue(key, out TeamStatsResponseDto? cached))
        {
            Console.WriteLine("returning cached");
            return cached;
        }

        if (!_teamIdsMap.TryGetValue(parsedTeam, out var teamId))
        {
            Console.WriteLine("no espn id found for given team");
            return null;
        }

        try
        {
            var url = $"{baseUrl}/site/v2/sports/football/nfl/teams/{teamId}/statistics";
            var res = await _client.GetFromJsonAsync<TeamStatsResponseDto>(url);
            if (res != null)
            {
                _cache.Set(key, res, cacheDuration);
            }
            else
            {
                Console.WriteLine("response is null");
            }
            return res;


        }
        catch (Exception e)
        {
            Console.WriteLine($"error from espnstatsservice: {e}");
            return null;
        }




    }

    // public async Task GetPlayerStats(int espnId)
    // {

    // }


}