using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HorseRacing
{
  enum Position
  {
    None,
    Win,
    Place,
    Show,
    Fourth
  }
  class Horse
  {
    private int number;
    private Position position;
    private string name;
    private string jockey;
    private double odds;
    private string trainer;
    private string owner;

    //Private constructor for Horse
    private Horse(string number, Position position, string name, string jockey, string odds, string trainer, string owner)
    {
      this.number = Convert.ToInt32(number.Trim());
      this.position = position;
      this.name = name;
      this.jockey = jockey;
      this.odds = Convert.ToDouble(odds.Trim());
      this.trainer = trainer.Trim();
      this.owner = owner.Trim();
    }

    public static Horse[] getHorses(string s)
    {
      MatchCollection matches = getHorseData(s);
      if (matches.Count > 0)
      {
        Horse[] horses;
        horses = new Horse[matches.Count];

        int i = 0;
        foreach (Match match in matches)
        {
          Position pos;
          switch (i) {
            case 0: pos = Position.Win;
              break;
            case 1: pos = Position.Place;
              break;
            case 2: pos = Position.Show;
              break;
            case 3: pos = Position.Fourth;
              break;
            default: pos = Position.None;
              break;
          }
          horses[i++] = new Horse(match.Groups["num"].Value, pos, match.Groups["name"].Value,
                             match.Groups["jockey"].Value, match.Groups["odds"].Value, 
                             getTrainer(s, match.Groups["num"].Value), getOwner(s, match.Groups["num"].Value));
        }
        return horses;
      }
      else
      {
        throw new Exception("Something went wrong: there are no horses.");
      }
    }

    /**
     * Finds all horse numbers, names, jockeys, and odds
     * returns MatchCollection of above for each horse
     */
    private static MatchCollection getHorseData(string s)
    {
      string pattern = @"\s(?<num>\d+)\w?\s(?<name>(\D+\s)?(\S+\s))\((?<jockey>(\S+,\s\S+)+)\).+?(?<odds>\d+\.\d+)";
      return Regex.Matches(s.Substring(s.IndexOf("Last Raced")), pattern, RegexOptions.ExplicitCapture);
    }

    private static string getTrainer(string s, string number)
    {
      string pattern = @"\s" + number.Trim() + @"\w?\s-\s(?<trainer>(\S+,(\s\S+,)?\s\S+)+)\s";
      foreach (Match match in Regex.Matches(s.Substring(s.IndexOf("Trainers:")), pattern, RegexOptions.ExplicitCapture))
      {
        return match.Groups["trainer"].Value.Replace(';', ' ');
      }
      return "";
    }

    private static string getOwner(string s, string number)
    {
      string pattern = @"\s?" + number.Trim() + @"\w?\s?-\s?(?<owner>\D+)\d";
      //Hack to fix retrival last owner: Replace Footnotes with 0
      foreach (Match match in Regex.Matches(s.Substring(s.IndexOf("Owners:")).Replace("Footnotes", "0"), pattern, RegexOptions.ExplicitCapture))
      {
        return match.Groups["owner"].Value.Replace(';', ' ');
      }
      return "";
    }

    public override string ToString()
    {
      return "Number: " + number + " Position: " + position + " Name: " + name +
             " Jockey: " + jockey + " Odds: " + odds + " Trainer: " + trainer + " Owner: " + owner;
    }


  }
}
