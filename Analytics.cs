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
    static void Main(string[] args)
    {
      SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=Saratoga.sqlite;Version=3;");
      m_dbConnection.Open();

      findBestBet(DataAccessObject.retrieve(m_dbConnection));
    }

    public static void findBestBet(List<Day> days) {
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
            byte rank = race.getWin().getOddRank();
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
      Console.ReadKey();
    }
  }
}
