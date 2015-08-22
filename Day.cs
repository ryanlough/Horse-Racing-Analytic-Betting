using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRacing
{
  class Day
  {
    private DateTime date;
    private Race[] races;

    //Constructor for Day
    public Day(DateTime date, Race[] races)
    {
      this.date = date;
      this.races = races;
    }

    public override string ToString()
    {
      string result = "";
      Array.ForEach<Race>(races, (h) => result += "\n\n\t" + ((h == null) ? "" : h.ToString()));
      return "Date: " + date.ToShortDateString() + result;
    }
  }
}
