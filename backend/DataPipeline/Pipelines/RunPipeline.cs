using System.Text.Json;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Db.Models;
using Microsoft.Extensions.Logging;

namespace DataPipeline.Pipelines;

public class RunPipeline<T>(
    IDataProvider<T> provider,
    IDataTransformer<T> transformer,
    IDataLoader<PlayerModel> playerLoader,
    IDataLoader<ExternalIdWithLookupDto> idloader,

    ILogger<T> logger) : IPipeline
{
    public async Task RunAsync()
    {
        var runner = typeof(T).Name;
        logger.LogInformation("beginning Pipeline for {t}.", runner);
        try
        {
            logger.LogInformation("beginning data extract");
            var dataExtract = await provider.ExtractDataAsync();
            logger.LogInformation("extract count: {x}", dataExtract.Count);

            logger.LogInformation("beginning data transform");
            var transformedData = transformer.Transform(dataExtract);

            if (transformedData?.PlayerData?.Count > 0)
            {

                logger.LogInformation("transformed {x} records", transformedData.PlayerData.Count);

                logger.LogInformation("beginning data load");
                logger.LogInformation("found {x} records of incomplete data", transformedData.IncompletePlayerData!.Count);
                logger.LogDebug("data {x}", JsonSerializer.Serialize(transformedData.IncompletePlayerData));

                var res = await playerLoader.LoadData(transformedData.PlayerData);

                logger.LogInformation("updated {x} players", res.UpdateCount);
                logger.LogInformation("added {x} new players", res.AddCount);
            }

            if (transformedData?.ExternalIdPlayerData?.Count > 0)
            {
                logger.LogInformation("transformed {x} records", transformedData.ExternalIdPlayerData.Count);

                logger.LogInformation("beginning data load");
                var res = await idloader.LoadData(transformedData.ExternalIdPlayerData);
                logger.LogInformation("added {x} new ids", res.AddCount);
            }

            logger.LogInformation("{r} success", runner);

        }
        catch (Exception e)
        {
            logger.LogError("{r} failed: {e}", runner, e);
            throw;
        }
    }

}