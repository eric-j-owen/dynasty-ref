namespace DataPipeline.Interfaces;

public interface IDataLoader<T>
{
    Task<LoadResult> LoadData(List<T> data);
}

public record LoadResult(int? AddCount = null, int? UpdateCount = null);