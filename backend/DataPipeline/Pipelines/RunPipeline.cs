using System.Text.Json;
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
    IDataLoader<ExternalIdWithLookupDto> idloader,

    ILogger<T> logger) : IPipeline
{
    public async Task RunAsync()
    {
        var runner = typeof(T).Name;
        var dataCount = (0, 0, 0); //updated, added, incomplete

        logger.LogInformation("beginning Pipeline for {t}.", runner);
        try
        {
            logger.LogInformation("beginning data extract");
            var dataExtract = await provider.ExtractDataAsync();

            logger.LogInformation("beginning data transform");
            var transformedData = transformer.Transform(dataExtract);

            logger.LogInformation("beginning data load");
            // player loader
            if (transformedData?.PlayerData?.Count > 0)
            {
                var res = await playerLoader.LoadData(transformedData.PlayerData);
                dataCount = (res.UpdateCount ?? 0, res.AddCount ?? 0, transformedData.IncompletePlayerData!.Count);
            }

            // external ids loader
            if (transformedData?.ExternalIdPlayerData?.Count > 0)
            {
                var res = await idloader.LoadData(transformedData.ExternalIdPlayerData);
                dataCount = (0, res.AddCount ?? 0, 0);
            }

            // player values loader
            if (transformedData?.PlayerValueData?.Count > 0)
            {
                // var res = PlayerValuesLoader.LoadData(transformedData.PlayerValueData);
                // dataCount = (0, res.AddCount, 0);

            }

            logger.LogInformation("{r} success", runner);
            Console.WriteLine("==================METRICS========================");
            logger.LogInformation("extract count: {x}", dataExtract.Count);
            logger.LogInformation("updated: {a}, added: {b}, incomplete: {c}", dataCount.Item1, dataCount.Item2, dataCount.Item3);
            Console.WriteLine("=================================================");


        }
        catch (Exception e)
        {
            logger.LogError("{r} failed: {e}", runner, e);
            throw;
        }
    }

}