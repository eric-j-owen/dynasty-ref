using DataPipeline.DTOs;
using Db.Models;
namespace DataPipeline.Interfaces;

public interface IDataTransformer<T>
{
    TransformResult Transform(List<T> data);
}

public record TransformResult(
    List<PlayerModel>? PlayerData = null,
    List<ExternalIdWithLookupDto>? ExternalIdPlayerData = null,
    List<PlayerValueModel>? PlayerValueData = null
);