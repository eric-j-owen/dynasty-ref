using DataPipeline.DataPipeline.Extract;
using DataPipeline.DataPipeline.Load;
using DataPipeline.DataPipeline.Transform;
using DataPipeline.Helpers;
using DataPipeline.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataPipeline.Pipelines;

public class PlayerPipeline(
    SleeperPlayersExtract provider,
    SleeperPlayerTransformer transformer,
    PlayerUpsertLoader loader,

    ILogger<PlayerPipeline> logger) : IPipeline
{
    public async Task RunAsync()
    {
        var pipeline = "PlayerPipeline";

        try
        {
            logger.LogInformation("beginning {x}", pipeline);

            var dataExtract = await provider.ExtractDataAsync();
            var transformedData = transformer.Transform(dataExtract);
            var res = await loader.LoadData(transformedData.PlayerData!);

            logger.LogSuccessPipeline(pipeline, dataExtract.Count, res.UpdateCount, res.AddCount, transformedData.IncompletePlayerData!.Count, null);
        }
        catch (Exception e)
        {
            logger.LogError("{pipeline} failed: {e}", pipeline, e);
            throw;
        }
    }

}