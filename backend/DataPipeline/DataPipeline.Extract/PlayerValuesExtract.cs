using DataPipeline.DataPipeline.Extract.PlayerValueSources;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;

namespace DataPipeline.DataPipeline.Extract;

public class PlayerValuesExtract(FcValuesExtract fc, KtcValuesExtract ktc) : IDataProvider<PlayerValueWithLookupDto>
{
    private readonly KtcValuesExtract _ktc = ktc;
    private readonly FcValuesExtract _fc = fc;

    public async Task<List<PlayerValueWithLookupDto>> ExtractDataAsync()
    {
        var ktcData = await _ktc.ExtractDataAsync();
        var fcData = await _fc.ExtractDataAsync();

        return [.. ktcData, .. fcData];
    }

}