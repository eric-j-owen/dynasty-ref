using System.Globalization;
using CsvHelper;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;

namespace DataPipeline.DataPipeline.Extract;

public class GetDynastyProcessIds(HttpClient client) : IDataProvider<DynastyProcessIdsDto>
{
    private readonly HttpClient _client = client;
    private readonly string _endpoint = "/repos/dynastyprocess/data/contents/files/db_playerids.csv";
    public async Task<List<DynastyProcessIdsDto>> ExtractDataAsync()
    {
        try
        {


            var stream = await _client.GetStreamAsync(_endpoint);
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<DynastyProcessIdsDto>().ToList();

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

