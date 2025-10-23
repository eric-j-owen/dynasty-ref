namespace DataPipeline.DataProviders;

interface IDataProvider<T>
{
    string DataSource { get; }
    List<T> ExtractData();
}