using DataPipeline.DataTransformers;
using DataPipeline.DTOs;
using Db.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Shared.Consts;

namespace Tests.DataPipeline;

public class SleeperTransformerTests
{
    [Fact]
    public void Transform_Returns_PlayerObjectWithExternalId()
    {
        var transformer = new SleeperPlayerTransformer(NullLogger<SleeperPlayerTransformer>.Instance);
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
        var transformer = new SleeperPlayerTransformer(NullLogger<SleeperPlayerTransformer>.Instance);
        var data = new List<SleeperPlayer>
        {
            new()
            {
                SleeperId = "1",
                FirstName = "Josh",
                LastName = "Allen",
                Positions=["QB", "WR", "TE"],
                SearchFullName = "joshallen"
            },
            new()
            {
                SleeperId = "1",
                FirstName = "Josh",
                LastName = "Allen",
                Positions=["QB", "DL"],
                SearchFullName = "joshallen"
            },
            new()
            {
                SleeperId = "1",
                FirstName = "Josh",
                LastName = "Allen",
                Positions=["RB", "LB", "K", ""],
                SearchFullName = "joshallen"
            },
        };

        var result = transformer.Transform(data);

        Assert.NotNull(result.PlayerData);

        foreach (var player in result.PlayerData)
        {
            Assert.All(player.Positions, p =>
            {
                Assert.IsType<IncludedPosition>(p);
            });
        }

    }
    [Fact]
    public void Transform_Filters_IncompleteData()
    {

        var transformer = new SleeperPlayerTransformer(NullLogger<SleeperPlayerTransformer>.Instance);
        var data = new List<SleeperPlayer>
        {
            new() {SleeperId="1", Positions=["QB"], FirstName="", LastName="b", SearchFullName=" b" },
            new() {SleeperId="1", Positions=["QB"], FirstName="a", LastName="", SearchFullName="a " },
            new() {SleeperId="1", Positions=["QB"], FirstName="a", LastName="b", SearchFullName=""}
        };

        var result = transformer.Transform(data);
        Assert.NotNull(result.PlayerData);
        Assert.NotNull(result.IncompletePlayerData);

        var actual = result.IncompletePlayerData;
        Assert.Equal(3, actual.Count);
        foreach (var player in actual)
        {
            Assert.Equal(IncompleteDataReason.MissingName, player.Reason);
        }
    }

    [Fact]
    public void Transform_Filters_NonPlayerRecords()
    {
        var transformer = new SleeperPlayerTransformer(NullLogger<SleeperPlayerTransformer>.Instance);
        var data = new List<SleeperPlayer>
        {
            new() {Positions = null},
            new() {SleeperId = null},
            new() {Positions = [""]},
            new() {SleeperId = ""}
        };

        var result = transformer.Transform(data);
        Assert.NotNull(result.PlayerData);
        Assert.Empty(result.PlayerData);
    }
}