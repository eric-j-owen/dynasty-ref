namespace DataPipeline.Helpers;

public static class PlayerMatcher
{
    public static bool MatchNames(string? name1, string? name2)
    {
        if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2))
        {
            return false;
        }

        if (name1 == name2)
        {
            return true;
        }

        // account for nicknames being used in different data sources
        var nameMappings = new Dictionary<string, string>
        {
            {"zonovanknight", "bamknight" },
            {"marquisebrown", "hollywoodbrown" },
            {"chigoziemokonkwo", "chigokonkwo" },
            {"gabrieldavis", "gabedavis" },
        };

        string mappedName1 = nameMappings.ContainsKey(name1) ? nameMappings[name1] : name1;
        string mappedName2 = nameMappings.ContainsKey(name2) ? nameMappings[name2] : name2;

        if (mappedName1 == mappedName2)
        {
            return true;
        }

        //check without suffixes
        if (RemoveSuffix(name1) == RemoveSuffix(name2))
        {
            return true;
        }

        return false;
    }

    //normalize inconsistent suffixes being included/excluded between data sources
    public static string RemoveSuffix(string name)
    {
        string[] suffixes = ["jr", "sr", "iii", "ii", "iv", "v"];

        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix))
            {
                return name.Substring(0, name.Length - suffix.Length);
            }
        }

        return name;
    }
}