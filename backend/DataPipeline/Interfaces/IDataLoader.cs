namespace DataPipeline.Interfaces;

public interface IDataLoader<T>
{
    Task<int> LoadData(List<T> data);
}