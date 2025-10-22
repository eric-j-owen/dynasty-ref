using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Data.Models;
using Shared.DTOs;
using Data.Helpers;

namespace Data.Services;

public class PlayerValueService
{
    private readonly AppDbContext _context;
    
    public PlayerValueService(AppDbContext context)
    {
        _context = context;
    }
   
    private void ProcessPlayerValueData(List<ScrapedPlayer> scraped)
    {
        /*todo
            []check for existing player values for the source/player
            []if existing calculate delta and update value,delta,date
            []if not existing insert new record
            [x]relate player values to player table
            [x]if sleeperid, use that
            [x]else use team, position, and searchfullname to match
            []refactor
        */

        var players = _context.Players.ToList();
        IEnumerable<Player> matched = Enumerable.Empty<Player>();

        foreach (var record in scraped)
        {
            //match with sleeperid if available
            if (!string.IsNullOrEmpty(record.SleeperId))
            {
                matched = players.Where(p => p.PlayerId == record.SleeperId);
            }

            //no sleeperid, try to match with name and position
            else if (!string.IsNullOrEmpty(record.Position))
            {
                matched = players
                    .Where(p =>
                        PlayerMatcher.MatchNames(p.SearchFullName, record.SearchFullName) &&
                        p.Position == record.Position);


                //if multiple matches try to match teams
                if (matched.Count() > 1)
                {
                    var teamMatched = matched.Where(p => p.Team == record.Team);

                    //match found by team
                    if (teamMatched.Count() == 1)
                    {
                        matched = teamMatched;
                        Console.WriteLine($"resolved dup for {record.SearchFullName}");
                    }

                    //unable to resolve duplicates
                    else
                    {
                        Console.WriteLine($"unresolved duplicates for: {record.SearchFullName}");
                    }
                }
            }

            else
            {
                Console.WriteLine($"incomplete data for {record.SearchFullName}");
            }

            if (!matched.Any())
            {
                Console.WriteLine($"no match found for {record.SearchFullName}");
                Console.WriteLine(record.DataSource);
            }

        }

    }
}

