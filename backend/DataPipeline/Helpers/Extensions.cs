using Microsoft.Extensions.Logging;

namespace DataPipeline.Helpers;

public static class Extensions
{
    public static void LogSuccessPipeline(this ILogger logger, string pipeline, int? extracted, int? updated, int? added, int? incomplete, int? deleted)
    {

        Console.WriteLine("================================================");
        logger.LogInformation("{x}: success", pipeline);

        if (extracted.HasValue) logger.LogInformation("extract count: {x}", extracted);
        if (updated.HasValue) logger.LogInformation("updated: {x}", updated);
        if (added.HasValue) logger.LogInformation("added: {x}", added);
        if (incomplete.HasValue) logger.LogInformation("incomplete: {x}", incomplete);
        if (incomplete.HasValue) logger.LogInformation("deleted: {x}", deleted);

        Console.WriteLine("=================================================");
    }

}