using DataPipeline.Interfaces;
using Db.Models;
using DataPipeline.DTOs;
using Shared.Consts;
using System.Text.Json;
using DataPipeline.Helpers;
using Microsoft.Extensions.Logging;

namespace DataPipeline.DataPipeline.Transform;

public class SleeperPlayerTransformer(ILogger<SleeperPlayerTransformer> logger) : IDataTransformer<SleeperPlayerDto>
{
    private static readonly HashSet<string> _positions = [.. Enum.GetNames<IncludedPosition>()];

    public TransformResult Transform(List<SleeperPlayerDto> data)
    {

        List<PlayerModel> players = [];
        List<SleeperPlayerDto> incompleteData = [];

        logger.LogInformation("beginning data transform on {x} records", data.Count);

        var validPlayers = (from record in data
                            where
                                 record.Positions != null &&
                                 record.Positions.Any(_positions.Contains) &&
                                 record.SleeperId != null
                            select record).ToList();

        foreach (var player in validPlayers)
        {
            if
            (
                string.IsNullOrEmpty(player.SearchFullName) ||
                string.IsNullOrEmpty(player.FirstName) ||
                string.IsNullOrEmpty(player.LastName)
            )
            {
                incompleteData.Add(player);
                continue;
            }

            var normalizedPositions = player.Positions!
                .Select(NormalizeField.Position)
                .ToArray();

            var normalizedTeam = NormalizeField.Team(player.Team);

            var newPlayer = new PlayerModel
            {
                NormalizedName = NormalizeField.Name(player.SearchFullName),
                FirstName = player.FirstName,
                LastName = player.LastName,
                Positions = normalizedPositions,
                Team = normalizedTeam,
                LastUpdated = DateTime.UtcNow
            };

            var sleeperId = new ExternalIdModel
            {
                DataSource = DataSource.Sleeper,
                SourceId = player.SleeperId!,
                Player = newPlayer
            };

            newPlayer.AddExternalId(sleeperId);

            players.Add(newPlayer);

        }

        logger.LogInformation("transformed {x} players", players.Count);
        logger.LogWarning("found {x} incomplete player data records {players}", incompleteData.Count, JsonSerializer.Serialize(incompleteData));

        return new TransformResult(players);
    }
}