using DataPipeline.Interfaces;
using Db.Models;
using DataPipeline.DTOs;
using Shared.Consts;
using System.Text.Json;
using DataPipeline.Helpers;

namespace DataPipeline.DataTransformers;

public class SleeperPlayerTransformer : IDataTransformer<SleeperPlayer>
{
    private static readonly HashSet<string> _positions = [.. Enum.GetNames<PlayerConsts.IncludedPosition>()];
    public TransformResult Transform(List<SleeperPlayer> data)
    {

        List<Player> players = [];
        List<IncompletePlayerData> incompleteData = [];

        var validPlayers = from record in data
                           where
                                record.Positions != null &&
                                record.Positions.Any(_positions.Contains) &&
                                record.SleeperId != null
                           select record;

        foreach (var player in validPlayers)
        {
            if
            (
                string.IsNullOrEmpty(player.SearchFullName) ||
                string.IsNullOrEmpty(player.FirstName) ||
                string.IsNullOrEmpty(player.LastName)
            )
            {
                incompleteData.Add(new IncompletePlayerData
                {
                    RawData = JsonSerializer.Serialize(player),
                    Reason = ApiConsts.IncompleteDataReason.MissingName,
                });

                continue;
            }

            var parsedPositions = player.Positions!
                .Where(_positions.Contains)
                .Select(Enum.Parse<PlayerConsts.IncludedPosition>)
                .ToArray();

            var newPlayer = new Player
            {
                NormalizedName = NormalizeField.Name(player.SearchFullName),
                FirstName = player.FirstName,
                LastName = player.LastName,
                Positions = parsedPositions,
            };

            var sleeperId = new ExternalIdPlayerLookup
            {
                DataSource = DataSource.Sleeper,
                SourceId = player.SleeperId!,
                Player = newPlayer
            };

            newPlayer.AddExternalId(sleeperId);

            players.Add(newPlayer);
        }

        return new TransformResult(players, null, null, incompleteData);
    }
}