using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Db.Models;

namespace DataPipeline.DataPipeline.Load;


public class ExternalIdsLoader : IDataLoader<ExternalIdWithLookupDto>
{
    public async Task<int> LoadData(List<ExternalIdWithLookupDto> data)
    {
        Console.WriteLine("load)");
        return await Task.FromResult(1);
    }
}