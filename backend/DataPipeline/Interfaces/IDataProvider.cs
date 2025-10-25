namespace DataPipeline.Interfaces;

interface IDataProvider<T>
{
    Task<List<T>> ExtractDataAsync();
}