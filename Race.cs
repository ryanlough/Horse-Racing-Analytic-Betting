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
    [ProtoMember(7)]

    private double payoffExacta { get; set; }
    [ProtoMember(8)]
    private double payoffTrifecta { get; set; }
    [ProtoMember(9)]
    private double payoffSuperfecta { get; set; }
    [ProtoMember(10)]
    private double payoffDailyDouble { get; set; }
    [ProtoMember(11)]
    private double payoffQuinella { get; set; }
    [ProtoMember(12)]
    private double payoffPick3 { get; set; }
    [ProtoMember(13)]
    private double payoffPick4 { get; set; }
    [ProtoMember(14)]
    private double[] payoffWin { get; set; }
    [ProtoMember(15)]
    private double[] payoffPlace { get; set; }
    [ProtoMember(16)]
    private double payoffShow { get; set; }

    //Constructor for Race
    public Race(byte number, string s)
    {
      this.number = number;
      this.purse = extractPurse(s);
      this.horses = Horse.extractHorses(s);
      this.weather = extractWeather(s);
      this.track = extractTrack(s);
      this.length = extractLength(s);

      double[] payoffs = extractPayoffs(s);
      this.payoffExacta = payoffs[0];
      this.payoffTrifecta = payoffs[1];
      this.payoffSuperfecta = payoffs[2];
      this.payoffDailyDouble = payoffs[3];
      this.payoffQuinella = payoffs[4];
      this.payoffPick3 = payoffs[5];
      this.payoffPick4 = payoffs[6];

      this.payoffWin = extractWinPayoff(s);
      this.payoffPlace = extractPlacePayoff(s);
      this.payoffShow = extractShowPayoff(s);
    }

    //Needed to allow protobuf default model binder to desereialize
    public Race()
    {

    }

    /**
     * Returns an array of all the payoff amounts for the given race.
     */
    public static double[] extractPayoffs(string s)
    {
      double[] result = new double[10];

      result[0] = extractGivenPayoff(s, @"Exacta");
      result[1] = extractGivenPayoff(s, @"Trifecta");
      result[2] = extractGivenPayoff(s, @"Superfecta");
      result[3] = extractGivenPayoff(s, @"Daily\sDouble");
      result[4] = extractGivenPayoff(s, @"Quinella");
      result[5] = extractGivenPayoff(s, @"Pick\s3");
      result[6] = extractGivenPayoff(s, @"Pick\s4");

      return result;
    }

    private double[] extractWinPayoff(string s)
    {
      string pattern = @"\s(?<win>\d+\.\d+)\s(?<place>\d+\.\d+)\s(?<show>\d+\.\d+)\s";
      try
      {
        Match match = Regex.Match(s.Substring(s.IndexOf("Show " + getWin().getNumber() + " " + getWin().getName())), pattern, RegexOptions.ExplicitCapture);
        return new double[] { Convert.ToDouble(match.Groups["win"].Value),
                              Convert.ToDouble(match.Groups["place"].Value),
                              Convert.ToDouble(match.Groups["show"].Value)};
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
        return null;
      }
    }

    private double[] extractPlacePayoff(string s)
    {
      string pattern = @"\s(?<place>\d+\.\d+)\s(?<show>\d+\.\d+)\s";
      try
      {
        Match match = Regex.Match(s.Substring(s.IndexOf("Show " + getWin().getNumber() + " " + getWin().getName())), pattern, RegexOptions.ExplicitCapture);
        return new double[] { Convert.ToDouble(match.Groups["place"].Value),
                              Convert.ToDouble(match.Groups["show"].Value)};
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
        return null;
      }
    }

    private double extractShowPayoff(string s)
    {
      string pattern = @"\s(?<show>\d+\.\d+)\s";
      try
      {
        Match match = Regex.Match(s.Substring(s.IndexOf("Show " + getWin().getNumber() + " " + getWin().getName())), pattern, RegexOptions.ExplicitCapture);
        return Convert.ToDouble(match.Groups["show"].Value);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
        return -1;
      }
    }

    private static double extractGivenPayoff(string s, string type)
    {
      string pattern = type + @"\s\d((-|\/)\d)+\s(\(\d\scorrect\)\s)?(?<payoff>(\d+,)?\d+\.\d+)\s";
      try
      {
        return Convert.ToDouble(Regex.Match(s.Substring(s.IndexOf("Payoff")), pattern,
          RegexOptions.ExplicitCapture).Groups["payoff"].Value);
      }
      catch (Exception e)
      {
        return -1;
      }
    }

    /**
     * Gets the purse amount for the given race
     */
    public static int extractPurse(string s)
    {
      string pattern = @"\$\S+\s";
      return int.Parse(Regex.Match(s.Substring(s.IndexOf("Purse:")), pattern,
        RegexOptions.ExplicitCapture).Value, NumberStyles.Currency);
    }
    
    /**
     * Gets the weather conditions for the given race
     */
    public static string extractWeather(string s)
    {
      string pattern = @"(?<weather>\S+)\sTrack:";
      return Regex.Match(s.Substring(s.IndexOf("Weather:")), pattern,
        RegexOptions.ExplicitCapture).Groups["weather"].Value;
    }

    /**
     * Gets what type of track was used
     * @Return: Track Enum
     */
    public static Track extractTrack(string s)
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
    public static string extractLength(string s)
    {
      string pattern =
        @"(?<length>\S+(\sAnd\sOne\s\S+)?\s(Furlongs)?(Mile(s)?)?)\sOn\sThe\s(\S+\s)?\S+\sTrack\sRecord:";
      return Regex.Match(s, pattern, RegexOptions.ExplicitCapture).Groups["length"].Value.Trim();
    }

    /**
     * Sets how all horses rank compared to each other via their odds (0 = best odds, 10 = worst, etc)
     */
    public void setAllOddRanks()
    {
      if (horses == null)
        return;
      Horse[] temp = (Horse[])horses.Clone();
      Array.Sort(temp, Comparer<Horse>.Create((x, y) => (x.getOdds()< y.getOdds()) ? -1 : ((x.getOdds() > y.getOdds()) ? 1 : 0)));
      for (byte i = 0; i < temp.Length; i++)
      {
        horses[Array.IndexOf(horses, temp[i])].setOddRank(i);
      }
    }

    /**
     * Returns the winning horse
     */
    public Horse getWin()
    {
      return positionHelper(Position.Win);
    }

    /**
     * Returns the placing horse
     */
    public Horse getPlace()
    {
      return positionHelper(Position.Place);
    }

    /**
     * Returns the show horse
     */
    public Horse getShow()
    {
      return positionHelper(Position.Show);
    }

    /**
     * Returns the fourth horse
     */
    public Horse getFourth()
    {
      return positionHelper(Position.Fourth);
    }

    private Horse positionHelper(Position pos)
    {
      if (horses != null)
      foreach (Horse horse in horses)
      {
        if (horse.getPosition() == pos)
        {
          return horse;
        }
      }
      return new Horse();
    }

    /**
     * Returns true if the race is valid
     */
    public bool isValid()
    {
      if (horses == null)
      {
        return false;
      }

      foreach (Horse horse in horses)
      {
        if (horse == null)
        {
          return false;
        }
      }
      return true;
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
