namespace DataPipeline.Interfaces;

interface IDataLoader<T>
{
    Task<int> LoadData(List<T> data);
}