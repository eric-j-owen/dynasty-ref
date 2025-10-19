using System.Text.Json;

namespace Shared.Services;

public class FileService(string basePath = "../Data/json-data")
{
    private readonly string _basePath = basePath;

    public void WriteToFileJson<T>(string fileName, T data)
    {
        string filePath = CreateFilePath(fileName);
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(filePath, json);

        Console.WriteLine($"success: saved to {filePath}");
    }

    public T? ReadFromFileJson<T>(string fileName)
    {
        string filePath = CreateFilePath(fileName);

        if (!File.Exists(filePath))
        {
            throw new Exception($"{filePath} not found");
        }

        string json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<T>(json);
    }

    private string CreateFilePath(string fileName)
    {
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
        
        return Path.Combine(_basePath, $"{fileName}.json");
    }
}