namespace DataPipeline.Interfaces;

interface IDataProvider<T>
{
    string DataSource { get; }
    Task<List<T>> ExtractDataAsync(string? path);
}