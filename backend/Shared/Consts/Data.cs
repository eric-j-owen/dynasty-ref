namespace Shared.Consts;


public static class ApiConfig
{
    public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
}

public static class ApiBaseUrl
{
    public const string Ktc = "https://keeptradecut.com";
    public const string Fc = "https://api.fantasycalc.com";
    public const string Sleeper = "https://api.sleeper.app/v1/";
    public const string Github = "https://api.github.com";
    public const string Espn = "https://site.api.espn.com/apis";
}

public enum DataSource
{
    KeepTradeCut,
    FantasyCalc,
    Sleeper,
    DynastyProcess,
    Mfl,
    Espn
}