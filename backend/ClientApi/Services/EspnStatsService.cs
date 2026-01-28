using ClientApi.Dtos.Espn;
using Microsoft.Extensions.Caching.Memory;
using Shared.Consts;

namespace ClientApi.Services;

public class EspnService(HttpClient client, IMemoryCache cache)
{
    private readonly HttpClient _client = client;
    private readonly static string _baseUrl = ApiBaseUrl.Espn;
    private readonly IMemoryCache _cache = cache;
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromDays(7);
    private readonly static Dictionary<TeamAbbr, int> _teamIdsMap = new()
    {
        { TeamAbbr.ARI, 22 }, { TeamAbbr.ATL, 1 }, {TeamAbbr.BAL, 33}, {TeamAbbr.BUF, 2},
        { TeamAbbr.CAR, 29 }, { TeamAbbr.CHI, 3 }, {TeamAbbr.CIN, 4}, {TeamAbbr.CLE, 5},
        { TeamAbbr.DAL, 6 }, { TeamAbbr.DEN, 7 }, {TeamAbbr.DET, 8}, {TeamAbbr.GB, 9},
        { TeamAbbr.HOU, 34 }, { TeamAbbr.IND, 11 }, {TeamAbbr.JAX, 30}, {TeamAbbr.KC, 12},
        { TeamAbbr.LV, 13 }, { TeamAbbr.LAC, 24 }, {TeamAbbr.LAR, 14}, {TeamAbbr.MIA, 15},
        { TeamAbbr.MIN, 16 }, { TeamAbbr.NE, 17 }, {TeamAbbr.NO, 18}, {TeamAbbr.NYG, 19},
        { TeamAbbr.NYJ, 20 }, { TeamAbbr.PHI, 21 }, {TeamAbbr.PIT, 23}, {TeamAbbr.SF, 25},
        { TeamAbbr.SEA, 26 }, { TeamAbbr.TB, 27 }, {TeamAbbr.TEN, 10}, {TeamAbbr.WAS, 28},
    };

    public async Task<MappedTeamStats?> GetTeamStats(string teamAbbr)
    {
        // parse parameter string to team abbreviations enum
        if (!Enum.TryParse<TeamAbbr>(teamAbbr, true, out var parsedTeam))
        {
            Console.WriteLine("invalid team abbreviation");
            return null;
        }

        var key = $"team_stats_{parsedTeam}";

        //check cache
        if (_cache.TryGetValue(key, out MappedTeamStats? cached))
        {
            Console.WriteLine("returning cached");
            return cached;
        }

        //verify input team
        if (!_teamIdsMap.TryGetValue(parsedTeam, out var teamId))
        {
            Console.WriteLine("no espn id found for given team");
            return null;
        }

        // http + map response
        try
        {
            var url = $"{_baseUrl}/site/v2/sports/football/nfl/teams/{teamId}/statistics";
            var res = await _client.GetFromJsonAsync<EspnTeamStatsResponseDto>(url);
            if (res == null)
            {
                Console.WriteLine("response is null");
                return null;
            }

            var mapped = DataMapperService.EspnTeamStats(res);
            _cache.Set(key, mapped, _cacheDuration);
            return mapped;
        }
        catch (Exception e)
        {
            Console.WriteLine($"error from espnstatsservice: {e}");
            return null;
        }




    }

    public async Task<EspnAthleteStatsResponse?> GetPlayerStats(int espnId)
    {

        var url = $"{_baseUrl}/common/v3/sports/football/nfl/athletes/{espnId}/stats?seasontype=2";
        var response = await _client.GetFromJsonAsync<EspnAthleteStatsResponse>(url);
        return response;

    }


}