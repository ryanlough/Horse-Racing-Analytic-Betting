using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRacing
{
  class Analytics
  {
    public delegate Horse Integrand(Race r);

    static void Main(string[] args)
    {
      SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=Saratoga.sqlite;Version=3;");
      m_dbConnection.Open();

      List<Day> days = DataAccessObject.retrieve(m_dbConnection);
      findBestBet(days);
    }

    public static void findBestBet(List<Day> days)
    {
      Console.WriteLine("Horses to win:");
      retrieveListForPosn(days, getWin);
      Console.WriteLine("\nHorses to place:");
      retrieveListForPosn(days, getPlace);
      Console.WriteLine("\nHorses to show:");
      retrieveListForPosn(days, getShow);
      Console.WriteLine("\nHorses to fourth:");
      retrieveListForPosn(days, getFourth);

      Console.ReadKey();
    }

    public static void retrieveListForPosn(List<Day> days, Integrand f) {
      Dictionary<byte, int> result = new Dictionary<byte, int>();
      foreach(Day day in days) {
        if(day != null)
        day.setAllHorseRanks();
      }

      foreach (Day day in days)
      {
        if (day != null && day.getRaces() != null) {
          foreach (Race race in day.getRaces())
          {
            byte rank = f(race).getOddRank();
            if (!result.ContainsKey(rank))
            {
              result.Add(rank, 0);
            }
            else
            {
              result[rank] += 1;
            }
          }
        }
      }

      foreach (KeyValuePair<byte, int> kvp in from entry in result orderby entry.Value descending select entry)
      {
        Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
      }
    }

    private static Horse getWin(Race r)
    {
      return r.getWin();
    }

    private static Horse getPlace(Race r)
    {
      return r.getPlace();
    }

    private static Horse getShow(Race r)
    {
      return r.getShow();
    }

    private static Horse getFourth(Race r)
    {
      return r.getFourth();
    }
  }
}
