using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Db.Models;

namespace DataPipeline.Datapipeline.Transform;

public class ExternalIdTransform : IDataTransformer<DynastyProcessIdsDto>
{
    public TransformResult Transform(List<DynastyProcessIdsDto> data)
    {
        Console.WriteLine("transform");
        List<ExternalIdModel> externalIdsData = [];
        return new TransformResult(null, externalIdsData);
    }
}