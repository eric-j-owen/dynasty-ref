using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Shared.Consts;

namespace DataPipeline.DataPipeline.Transform;

public class ExternalIdTransform : IDataTransformer<DynastyProcessIdsDto>
{
    public TransformResult Transform(List<DynastyProcessIdsDto> data)
    {
        List<ExternalIdWithLookupDto> externalIdsData = [];

        // filters out records without sleeper ids
        var filteredIds = from player in data
                          where !string.IsNullOrEmpty(player.SleeperId)
                          where player.SleeperId != "NA"
                          select player;

        foreach (var player in filteredIds)
        {
            // array of data sources, expandable as needed
            var sources = new[]
            {
                (DataSource: DataSource.KeepTradeCut, SourceId: player.KtcId),
                (DataSource: DataSource.Mfl, SourceId: player.MflId)
            };

            //per datasource with non null id, create entity and add to master list
            var externalId = sources
                .Where(x => !string.IsNullOrEmpty(x.SourceId))
                .Where(x => x.SourceId != "NA")
                .Select(x => new ExternalIdWithLookupDto
                {
                    SleeperId = player.SleeperId!,
                    DataSource = x.DataSource,
                    SourceId = x.SourceId!
                });

            externalIdsData.AddRange(externalId);
        }
        return new TransformResult(null, externalIdsData);
    }
}