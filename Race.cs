using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace HorseRacing
{
  enum Track
  {
    None,
    Dirt,
    Turf
  }
  class Race
  {
    private int number;
    private int purse;
    private Horse[] horses;
    private string weather;
    private Track track;
    private string length;

    //Constructor for Race
    public Race(int number, int purse, Horse[] horses, string weather, Track track, string length)
    {
      this.number = number;
      this.purse = purse;
      this.horses = horses;
      this.weather = weather;
      this.track = track;
      this.length = length;
    }

    /**
     * Gets the purse amount for the given race
     */
    public static int getPurse(string s)
    {
      string pattern = @"\$\S+\s";
      foreach (Match match in Regex.Matches(s.Substring(s.IndexOf("Purse:")), pattern, RegexOptions.ExplicitCapture))
      {
        return int.Parse(match.Value, NumberStyles.Currency);
      }
      return -1;
    }
    
    /**
     * Gets the weather conditions for the given race
     */
    public static string getWeather(string s)
    {
      string pattern = @"(?<weather>\S+)\sTrack:";
      foreach (Match match in Regex.Matches(s.Substring(s.IndexOf("Weather:")), pattern, RegexOptions.ExplicitCapture))
      {
        return match.Groups["weather"].Value.Trim();
      }
      return "";
    }

    /**
     * Gets what type of track was used
     * @Return: Track Enum
     */
    public static Track getTrack(string s)
    {
      string pattern = @"(?<track>\S+)\sTrack\sRecord:";
      foreach (Match match in Regex.Matches(s, pattern, RegexOptions.ExplicitCapture))
      {
        Track track = Track.None;
        Enum.TryParse(match.Groups["track"].Value.Trim().ToLower(), true, out track);
        return track;
      }
      return Track.None;
    }

    /**
     * Gets the distance run in the given race.
     * @Return: string distance (TODO: Look into better ways to represent this data)
     */
    public static string getLength(string s)
    {
      string pattern = @"(?<length>\S+(\sAnd\sOne\s\S+)?\s(Furlongs)?(Mile(s)?)?)\sOn\sThe\s(\S+\s)?\S+\sTrack\sRecord:";
      foreach (Match match in Regex.Matches(s, pattern, RegexOptions.ExplicitCapture))
      {
        return match.Groups["length"].Value.Trim();
      }
      return "";
    }

    public override string ToString()
    {
      string result = "";
      Array.ForEach<Horse>(horses, (h) => result += "\n\t" + h.ToString());
      return "Race: " + number + " Purse: " + purse + " Weather: " + weather + " Track: " + track + " Length: " + length + result;
    }
  }
}
