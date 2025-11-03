using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Db.Models;
using Microsoft.Extensions.Logging;

namespace DataPipeline.Pipelines;

public class PlayerPipeline(
    IDataProvider<SleeperPlayer> provider,
    IDataTransformer<SleeperPlayer> transformer,
    IDataLoader<Player> loader,
    ILogger<PlayerPipeline> logger) : IPipeline
{
    public async Task RunAsync()
    {
        try
        {
            var dataExtract = await provider.ExtractDataAsync();
            var transformedData = transformer.Transform(dataExtract);

            if (transformedData?.PlayerData?.Count > 0)
            {
                await loader.LoadData(transformedData.PlayerData);
            }

        }
        catch (Exception e)
        {
            logger.LogError("sleeperplayerpipeline failed {e}", e);
            throw;
        }
    }

}