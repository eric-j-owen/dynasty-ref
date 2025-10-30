using Db.Models;
namespace DataPipeline.Interfaces;

interface IDataTransformer<T>
{
    TransformResult Transform(List<T> data);
}

public record TransformResult(
    List<Player>? PlayerData = null,
    List<ExternalIdPlayerLookup>? ExternalIdPlayerData = null,
    List<PlayerValue>? PlayerValueData = null,
    List<IncompletePlayerData>? IncompletePlayerData = null
);