using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProtoBuf;

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
  [ProtoContract]
  class Horse
  {
    [ProtoMember(1)]
    private byte number { get; set; }
    [ProtoMember(2)]
    private Position position { get; set; }
    [ProtoMember(3)]
    private string name { get; set; }
    [ProtoMember(4)]
    private string jockey { get; set; }
    [ProtoMember(5)]
    private double odds { get; set; }
    [ProtoMember(6)]
    private string trainer { get; set; }
    [ProtoMember(7)]
    private string owner { get; set; }

    //Private constructor for Horse
    private Horse(string number, Position position, string name,
                  string jockey, string odds, string trainer, string owner)
    {
      this.number = Convert.ToByte(number.Trim());
      this.position = position;
      this.name = name;
      this.jockey = jockey;
      this.odds = Convert.ToDouble(odds.Trim());
      this.trainer = trainer.Trim();
      this.owner = owner.Trim();
    }

    public Horse()
    {

    }

    /**
     * Public method to retrieve an array of all horses found on the given page.
     */
    public static Horse[] getHorses(string page)
    {
      MatchCollection matches = getHorseData(page);
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
                             getTrainer(page, match.Groups["num"].Value),
                             getOwner(page, match.Groups["num"].Value));
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

    /**
     * Returns the trainer for the given number horse.
     */
    private static string getTrainer(string s, string number)
    {
      string pattern = @"\s" + number.Trim() + @"\w?\s-\s(?<trainer>(\S+,(\s\S+,)?\s\S+)+)\s";
      return Regex.Match(s.Substring(s.IndexOf("Trainers:")), pattern,
        RegexOptions.ExplicitCapture).Groups["trainer"].Value.Replace(';', ' ');
    }

    /**
     * Returns the owner(s) for the given number horse.
     */
    private static string getOwner(string s, string number)
    {
      string pattern = @"\s?" + number.Trim() + @"\w?\s?-\s?(?<owner>\D+)\d";
      //Hack to fix retrival last owner: Replace Footnotes with 0
      return Regex.Match(s.Substring(s.IndexOf("Owners:")).Replace("Footnotes", "0"), pattern,
        RegexOptions.ExplicitCapture).Groups["owner"].Value.Replace(';', ' ');
    }

    public override string ToString()
    {
      return "Number: " + number + " Position: " + position + " Name: " + name +
             " Jockey: " + jockey + " Odds: " + odds + " Trainer: " + trainer + " Owner: " + owner;
    }


  }
}
