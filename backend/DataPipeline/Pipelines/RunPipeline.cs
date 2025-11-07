using DataPipeline.DataPipeline.Load;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Db.Models;
using Microsoft.Extensions.Logging;

namespace DataPipeline.Pipelines;

public class RunPipeline<T>(
    IDataProvider<T> provider,
    IDataTransformer<T> transformer,
    IDataLoader<PlayerModel> playerLoader,
    IDataLoader<ExternalIdModel> idloader,

    ILogger<T> logger) : IPipeline
{
    public async Task RunAsync()
    {
        var runner = typeof(T).Name;
        logger.LogInformation("beginning Pipeline for {t}.", runner);
        try
        {
            var dataExtract = await provider.ExtractDataAsync();
            var transformedData = transformer.Transform(dataExtract);

            if (transformedData?.PlayerData?.Count > 0)
            {
                await playerLoader.LoadData(transformedData.PlayerData);
            }

            if (transformedData?.ExternalIdPlayerData?.Count > 0)
            {
                await idloader.LoadData(transformedData.ExternalIdPlayerData);
            }

        }
        catch (Exception e)
        {
            logger.LogError("{x} failed: {e}", runner, e);
            throw;
        }
    }

}