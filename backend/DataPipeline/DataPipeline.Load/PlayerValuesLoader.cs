using DataPipeline.Interfaces;
using Db;
using Db.Models;

namespace DataPipeline.DataPipeline.Load;

public class PlayerValuesLoader(AppDbContext context) : IDataLoader<PlayerValueModel>
{
    private readonly AppDbContext _context = context;
    public Task<LoadResult> LoadData(List<PlayerModel> data)
    {
        return Task.FromResult(new LoadResult(1));
    }
}