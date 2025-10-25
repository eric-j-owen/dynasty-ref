namespace DataPipeline.Helpers;

public static class PlayerMatcher
{
    public static bool MatchNames(string? name1, string? name2)
    {
        if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2))
        {
            return false;
        }

        if (CheckAltNameEquality(name1, name2) && RemoveSuffixEquality(name1, name2))
        {
            return true;
        }

        return false;
    }

    public static bool RemoveSuffixEquality(string name1, string name2)
    {
        string[] suffixes = ["jr", "sr", "iii", "ii", "iv", "v"];

        foreach (var suffix in suffixes)
        {
            if (name1.EndsWith(suffix))
            {
                name1 = name1[..^suffix.Length];
            }

            if (name2.EndsWith(suffix))
            {
                name2 = name2[..^suffix.Length];
            }
        }

        return name1 == name2;
    }
    public static bool CheckAltNameEquality(string name1, string name2)
    {
        var nameMappings = new Dictionary<string, string>
        {
            {"zonovanknight", "bamknight" },
            {"marquisebrown", "hollywoodbrown" },
            {"chigoziemokonkwo", "chigokonkwo" },
            {"gabrieldavis", "gabedavis" },
        };

        string mappedName1 = nameMappings.ContainsKey(name1) ? nameMappings[name1] : name1;
        string mappedName2 = nameMappings.ContainsKey(name2) ? nameMappings[name2] : name2;

        return mappedName1 == mappedName2;
    }
}