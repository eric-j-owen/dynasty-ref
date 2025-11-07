using DataPipeline.Interfaces;
using Db.Models;

namespace DataPipeline.DataPipeline.Load;


public class ExternalIdsLoader : IDataLoader<ExternalIdModel>
{
    public async Task<int> LoadData(List<ExternalIdModel> data)
    {
        Console.WriteLine("load)");
        return await Task.FromResult(1);
    }
}