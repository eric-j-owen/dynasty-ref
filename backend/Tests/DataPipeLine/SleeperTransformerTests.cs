using DataPipeline.DataTransformers;
using DataPipeline.DTOs;
using Db.Models;
using Shared.Consts;

namespace Tests.DataPipeline;

public class SleeperTransformerTests
{
    [Fact]
    public void Transform_Returns_PlayerObjectWithExternalId()
    {
        var transformer = new SleeperPlayerTransformer();
        var data = new List<SleeperPlayer>
        {
            new()
            {
                SleeperId = "1",
                FirstName = "Josh",
                LastName = "Allen",
                Positions = ["QB"],
                SearchFullName = "joshallen"
            }
        };

        var result = transformer.Transform(data);

        Assert.NotNull(result.PlayerData);

        var expected = data[0];
        var actual = result.PlayerData[0];
        var actualExternalId = actual.ExternalIds.FirstOrDefault();

        Assert.NotNull(actualExternalId);

        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
        Assert.Equal(expected.SleeperId, actualExternalId.SourceId);
        Assert.Equal(DataSource.Sleeper, actualExternalId.DataSource);

    }

    [Fact]
    public void Transform_ParsesAndFilters_Positions()
    {
        Assert.Fail();
    }
    [Fact]
    public void Transform_Filters_IncompleteData()
    {
        Assert.Fail();
    }

    [Fact]
    public void Transform_Filters_NullData()
    {
        Assert.Fail();
    }
}