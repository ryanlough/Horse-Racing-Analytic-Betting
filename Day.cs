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
    private Race[] races { get; set; }

    //Constructor for Day
    public Day(DateTime date, Race[] races)
    {
      this.date = date;
      this.races = races;
    }

    //Needed to allow protobuf default model binder to desereialize
    public Day()
    {

    }

    public string getSqlDate()
    {
      string zeroMonth = date.Month < 10 ? "0" : "";
      string zeroDay = date.Day < 10 ? "0" : "";

      return zeroMonth + date.Month + "/" + zeroDay + date.Day + "/" + date.Year;
    }

    public override string ToString()
    {
      string result = "";
      Array.ForEach<Race>(races, (h) => result += "\n\n\t" + ((h == null) ? "" : h.ToString()));
      return "Date: " + date.ToShortDateString() + result;
    }
  }
}
