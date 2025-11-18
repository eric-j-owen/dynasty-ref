using System.Net.Http.Json;
using DataPipeline.DTOs;
using DataPipeline.Helpers;
using DataPipeline.Interfaces;
using Shared.Consts;

namespace DataPipeline.DataPipeline.Extract.PlayerValueSources;

public class FcValuesExtract(HttpClient client) : IDataProvider<PlayerValueWithLookupDto>
{
    private readonly HttpClient _client = client;


    public async Task<List<PlayerValueWithLookupDto>> ExtractDataAsync()
    {
        // shared params
        var isDynasty = true;
        var numTeams = 10;
        var ppr = .5;

        //urls
        string getOneQbUrl = $"{ApiBaseUrl.Fc}/values/current?isDynasty={isDynasty}&numQbs={1}&numTeams={numTeams}&ppr={ppr}&includeAdp=false";
        string getSuperFlexUrl = $"{ApiBaseUrl.Fc}/values/current?isDynasty={isDynasty}&numQbs={2}&numTeams={numTeams}&ppr={ppr}&includeAdp=false";

        //tasks
        var oneQbRes = await _client.GetFromJsonAsync<List<FcPlayerApiResDto>>(getOneQbUrl);
        var superFlexRes = await _client.GetFromJsonAsync<List<FcPlayerApiResDto>>(getSuperFlexUrl);

        if (oneQbRes == null || superFlexRes == null)
        {
            throw new Exception("Fc extract failed");
        }

        //data process 
        List<PlayerValueWithLookupDto> merged = [];
        ConvertAndAddValues(oneQbRes, merged, false);
        ConvertAndAddValues(superFlexRes, merged, true);

        return merged;
    }

    private static void ConvertAndAddValues(List<FcPlayerApiResDto> records, List<PlayerValueWithLookupDto> result, bool isSuperflex)
    {
        foreach (var record in records)
        {
            var dto = new PlayerValueWithLookupDto
            {
                LookupIds = new()
                {
                    [DataSource.Sleeper] = record.Player.SleeperId
                },
                ValueSource = DataSource.FantasyCalc,
            };

            if (isSuperflex)
            {
                dto.SuperFlexValue = record.Value;
            }
            else
            {
                dto.OneQbValue = record.Value;
            }

            result.Add(dto);
        }
    }
}
