using DataPipeline.DataPipeline.Extract;
using DataPipeline.DataPipeline.Load;
using DataPipeline.Helpers;
using DataPipeline.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataPipeline.Pipelines;

public class PlayerValuesPipeline(
    PlayerValuesExtract provider,
    PlayerValuesLoader loader,


    ILogger<PlayerValuesPipeline> logger) : IPipeline
{
    public async Task RunAsync()
    {
        var pipeline = "PlayerValuesPipeline";

        try
        {

            logger.LogInformation("beginning {x}", pipeline);

            var dataExtract = await provider.ExtractDataAsync();
            var res = await loader.LoadData(dataExtract);

            logger.LogSuccessPipeline(pipeline, dataExtract.Count, null, res.AddCount, null, res.DeleteCount);

        }
        catch (Exception e)
        {
            logger.LogError("failed: {e}", e);
            throw;
        }
    }

}