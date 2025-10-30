using System.Globalization;
using CsvHelper;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;

namespace DataPipeline.DataProviders;

public class GetDynastyProcessPlayers(HttpClient client) : IDataProvider<DynastyProcessPlayer>
{
    private readonly HttpClient _client = client;
    private readonly string _endpoint = "db_playerids.csv";
    public async Task<List<DynastyProcessPlayer>> ExtractDataAsync()
    {
        try
        {
            var stream = await _client.GetStreamAsync(_endpoint);
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<DynastyProcessPlayer>().ToList();

                if (records == null || records.Count == 0)
                {
                    throw new Exception("dynastyProcess: returned null or empty");
                }

                return records;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

