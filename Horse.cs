using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HorseRacing
{
  class Horse
  {
    private int number;
    private String name;
    private String jockey;
    private double odds;

    //Private constructor for Horse
    private Horse(string number, String name, String jockey, string odds)
    {
      this.number = Convert.ToInt32(number.Trim());
      this.name = name;
      this.jockey = jockey;
      this.odds = Convert.ToDouble(odds.Trim());
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
          horses[i++] = new Horse(match.Groups["num"].Value, match.Groups["name"].Value,
                             match.Groups["jockey"].Value, match.Groups["odds"].Value);
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
      string pattern = @"\s(?<num>\d)\s(?<name>(\D+\s)?(\S+\s))\((?<jockey>(\S+,\s\S+)+)\).+?(?<odds>\d+\.\d+)";
      return Regex.Matches(s.Substring(s.IndexOf("Last Raced")), pattern, RegexOptions.ExplicitCapture);
    }

    public override string ToString()
    {
      return "Number: " + number + " Name: " + name + " Jockey: " + jockey + " Odds: " + odds;
    }


  }
}
