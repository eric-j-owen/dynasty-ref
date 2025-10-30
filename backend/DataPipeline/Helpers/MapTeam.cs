namespace DataPipeline.Helpers;

public static class TeamMapper
{
    private static readonly Dictionary<string, string?> _teamMappings = new()  // <ktc, sleeper>
        {
            {"SFO", "SF"},
            {"NOS", "NO"},
            {"JAC", "JAX"},
            {"GBP", "GB"},
            {"KCC", "KC"},
            {"NEP", "NE"},
            {"TBB", "TB"},
            {"LVR", "LV"},
            {"FA", null }
        };

    public static string? MapTeam(string team)
    {


        if (_teamMappings.TryGetValue(team, out string? value))
        {
            return value;
        }

        return team;

    }

}