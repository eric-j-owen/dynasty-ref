using System.Text.Json;

namespace DataPipeline.Helpers;

public static class FileService
{
    static public void WriteToFileJson<T>(string fileName, T data)
    {
        string filePath = CreateFilePath(fileName);
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(filePath, json);
    }

    static public T? ReadFromFileJson<T>(string fileName)
    {
        string filePath = CreateFilePath(fileName);

        if (!File.Exists(filePath))
        {
            throw new Exception($"{filePath} not found");
        }

        string json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<T>(json);
    }

    static private string CreateFilePath(string fileName)
    {
        var _basePath = "../dataexplore";
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        return Path.Combine(_basePath, $"{fileName}.json");
    }
}