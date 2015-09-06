using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using ProtoBuf;

namespace HorseRacing
{
  enum Track
  {
    None,
    Dirt,
    Turf
  }
  [ProtoContract]
  class Race
  {
    [ProtoMember(1)]
    private byte number { get; set; }
    [ProtoMember(2)]
    private int purse { get; set; }
    [ProtoMember(3)]
    private Horse[] horses { get; set; }
    [ProtoMember(4)]
    private string weather { get; set; }
    [ProtoMember(5)]
    private Track track { get; set; }
    [ProtoMember(6)]
    private string length { get; set; }

    //Constructor for Race
    public Race(byte number, int purse, Horse[] horses, string weather, Track track, string length)
    {
      this.number = number;
      this.purse = purse;
      this.horses = horses;
      this.weather = weather;
      this.track = track;
      this.length = length;
    }

    //Needed to allow protobuf default model binder to desereialize
    public Race()
    {

    }

    /**
     * Gets the purse amount for the given race
     */
    public static int getPurse(string s)
    {
      string pattern = @"\$\S+\s";
      return int.Parse(Regex.Match(s.Substring(s.IndexOf("Purse:")), pattern,
        RegexOptions.ExplicitCapture).Value, NumberStyles.Currency);
    }
    
    /**
     * Gets the weather conditions for the given race
     */
    public static string getWeather(string s)
    {
      string pattern = @"(?<weather>\S+)\sTrack:";
      return Regex.Match(s.Substring(s.IndexOf("Weather:")), pattern,
        RegexOptions.ExplicitCapture).Groups["weather"].Value;
    }

    /**
     * Gets what type of track was used
     * @Return: Track Enum
     */
    public static Track getTrack(string s)
    {
      string pattern = @"(?<track>\S+)\sTrack\sRecord:";
      Match match = Regex.Match(s, pattern, RegexOptions.ExplicitCapture);
      Track track;
      return Enum.TryParse(match.Groups["track"].Value.Trim().ToLower(), true, out track) ? track : Track.None;
    }

    /**
     * Gets the distance run in the given race.
     * @Return: string distance (TODO: Look into better ways to represent this data)
     */
    public static string getLength(string s)
    {
      string pattern =
        @"(?<length>\S+(\sAnd\sOne\s\S+)?\s(Furlongs)?(Mile(s)?)?)\sOn\sThe\s(\S+\s)?\S+\sTrack\sRecord:";
      return Regex.Match(s, pattern, RegexOptions.ExplicitCapture).Groups["length"].Value.Trim();
    }

    public override string ToString()
    {
      string result = "";
      try
      {
        Array.ForEach<Horse>(horses, (h) => result += "\n\t" + h.ToString());
      }
      catch (Exception e)
      {

      }
      return "Race: " + number + " Purse: " + purse + " Weather: " + weather +
             " Track: " + track + " Length: " + length + result;
    }
  }
}
