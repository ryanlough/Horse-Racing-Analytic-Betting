using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace HorseRacing
{
  class Race
  {
    private int number;
    private int purse; //TODO need condition, weather, type
    private Horse[] horses;

    //Constructor for Race
    public Race(int number, int purse, Horse[] horses)
    {
      this.number = number;
      this.purse = purse;
      this.horses = horses;
    }

    public static int getPurse(string s)
    {
      string pattern = @"\$\S+\s";
      foreach (Match match in Regex.Matches(s.Substring(s.IndexOf("Purse:")), pattern, RegexOptions.ExplicitCapture))
      {
        return int.Parse(match.Value, NumberStyles.Currency);
      }
      return -1;
    }

    public override string ToString()
    {
      string result = "";
      Array.ForEach<Horse>(horses, (h) => result += "\n\t" + h.ToString());
      return "Number: " + number + " Purse: " + purse + result;
    }
  }
}
