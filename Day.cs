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
  }
}
