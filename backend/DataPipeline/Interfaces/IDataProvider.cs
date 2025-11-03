namespace DataPipeline.Interfaces;

public interface IDataProvider<T>
{
    Task<List<T>> ExtractDataAsync();
}