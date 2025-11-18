using DataPipeline.DataPipeline.Extract;
using DataPipeline.DataPipeline.Load;
using DataPipeline.DataPipeline.Transform;
using DataPipeline.Helpers;
using DataPipeline.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataPipeline.Pipelines;

public class ExternalIdsPipeline(
    ExternalIdsExtract provider,
    ExternalIdTransform transformer,
    ExternalIdsLoader loader,

    ILogger<ExternalIdsPipeline> logger) : IPipeline
{
    public async Task RunAsync()
    {
        var pipeline = "ExternalIdsPipeline";

        try
        {
            logger.LogInformation("beginning {x}", pipeline);

            var dataExtract = await provider.ExtractDataAsync();
            var transformedData = transformer.Transform(dataExtract);
            var res = await loader.LoadData(transformedData.ExternalIdPlayerData!);

            logger.LogSuccessPipeline(pipeline, dataExtract.Count, null, res.AddCount, null, null);
        }
        catch (Exception e)
        {
            logger.LogError("{pipeline} failed: {e}", pipeline, e);
            throw;
        }
    }

}