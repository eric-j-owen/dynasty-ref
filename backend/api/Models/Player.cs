/* 
GET https://api.sleeper.app/v1/players/nfl
{
  "3086": {
    "hashtag": "#TomBrady-NFL-NE-12",
    "depth_chart_position": 1,
    "status": "Active",
    "sport": "nfl",
    "fantasy_positions": ["QB"],
    "number": 12,
    "search_last_name": "brady",
    "injury_start_date": null,
    "weight": "220",
    "position": "QB",
    "practice_participation": null,
    "sportradar_id": "",
    "team": "NE",
    "last_name": "Brady",
    "college": "Michigan",
    "fantasy_data_id":17836,
    "injury_status":null,
    "player_id":"3086",
    "height": "6'4\"",
    "search_full_name": "tombrady",
    "age": 40,
    "stats_id": "",
    "birth_country": "United States",
    "espn_id": "",
    "search_rank": 24,
    "first_name": "Tom",
    "depth_chart_order": 1,
    "years_exp": 14,
    "rotowire_id": null,
    "rotoworld_id": 8356,
    "search_first_name": "tom",
    "yahoo_id": null
  },
  ...
}
*/


public class Player
{
    public required string PlayerId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public required string FantasyPositions { get; set; }
}