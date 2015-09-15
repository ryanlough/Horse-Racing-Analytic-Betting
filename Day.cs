using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace HorseRacing
{
  [ProtoContract]
  class Day
  {
    [ProtoMember(1)]
    private DateTime date { get; set; }
    [ProtoMember(2)]
    private List<Race> races { get; set; }

    //Constructor for Day
    public Day(DateTime date, List<Race> races)
    {
      this.date = date;
      this.races = races;
    }

    //Needed to allow protobuf default model binder to desereialize
    public Day()
    {

    }

    public DateTime getDate()
    {
      return date;
    }

    /**
     * Returns the SQLite compatible string representation of the DateTime.
     */
    public string getSqlDate()
    {
      string zeroMonth = date.Month < 10 ? "0" : "";
      string zeroDay = date.Day < 10 ? "0" : "";
      return date.Year + "-" + zeroMonth + date.Month + "-" + zeroDay + date.Day;
    }

    /**
     * Returns the list of races.
     */
    public List<Race> getRaces()
    {
      return races;
    }

    /**
     * Sets how all horses rank compared to each other via their odds (0 = best odds, 10 = worst, etc)
     */
    public void setAllHorseRanks()
    {
      if (races != null)
      foreach(Race race in races) {
        if(race != null) {
          race.setAllOddRanks();
        }
      }
    }

    /**
     * Returns true if no essential info is null.
     */
    public bool isValid()
    {
      bool result = true;

      if (races == null)
      {
        return false;
      }

      foreach (Race race in races)
      {
        result = race != null && race.isValid();
      }

      return result;
    }

    public override string ToString()
    {
      string result = "";
      if (races == null) { Console.WriteLine("UHOHHHHH" + date); }
      else
      {
        races.ForEach((h) => result += "\n\n\t" + ((h == null) ? "" : h.ToString()));
      }
      return "Date: " + date.ToShortDateString() + result;
    }
  }
}
