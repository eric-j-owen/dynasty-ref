using ClientApi.Dtos.Espn;

namespace ClientApi.Services;

internal static class DataMapperService
{
    internal static MappedTeamStats? EspnTeamStats(EspnTeamStatsResponseDto data)
    {
        var team = data.Results.Stats.Categories;
        var opp = data.Results.Opponent;

        if (team == null || opp == null)
        {
            Console.WriteLine("data is null");
            return null;
        }

        var categories = new
        {
            passing = team.FirstOrDefault(c => c.Name == "passing")?.Stats,
            rushing = team.FirstOrDefault(c => c.Name == "rushing")?.Stats,
            scoring = team.FirstOrDefault(c => c.Name == "scoring")?.Stats,
            defensive = opp.FirstOrDefault(c => c.Name == "defensive")?.Stats,
            interceptions = opp.FirstOrDefault(c => c.Name == "defensiveInterceptions")?.Stats
        };

        return new MappedTeamStats
        {
            Offense = new OffenseStats
            {
                TouchdownsPerGame = categories.scoring?.FirstOrDefault(x => x.Name == "totalTouchdowns")?.PerGameValue,
                PassingTouchdownsPerGame = categories.passing?.FirstOrDefault(x => x.Name == "passingTouchdowns")?.PerGameValue,
                RushingTouchdowns = categories.rushing?.FirstOrDefault(x => x.Name == "rushingTouchdowns")?.Value,
                PassingYardsPerGame = categories.scoring?.FirstOrDefault(x => x.Name == "totalPointsPerGame")?.Value,
                RushingYardsPerGame = categories.rushing?.FirstOrDefault(x => x.Name == "rushingYardsPerGame")?.Value
            },

            Opponent = new OppStats
            {
                InterceptionsPerGame = categories.interceptions?.FirstOrDefault(x => x.Name == "interceptions")?.PerGameValue,
                PassesDefendedPerGame = categories.defensive?.FirstOrDefault(x => x.Name == "passesDefended")?.PerGameValue,
                SacksPerGame = categories.defensive?.FirstOrDefault(x => x.Name == "sacks")?.PerGameValue,
                TacklesPerGame = categories.defensive?.FirstOrDefault(x => x.Name == "totalTackles")?.PerGameValue,
                TacklesForLossPerGame = categories.defensive?.FirstOrDefault(x => x.Name == "tacklesForLoss")?.PerGameValue,
                StuffsPerGame = categories.defensive?.FirstOrDefault(x => x.Name == "stuffs")?.PerGameValue,

            }
        };
    }
}