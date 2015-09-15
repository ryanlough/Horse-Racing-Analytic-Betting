using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRacing
{
  class Analytics
  {
    public delegate Horse RetrievePosn(Race r);
    public delegate byte[] RetrieveHorses(Race r);

    public struct WinnerData
    {
      public int count;
      public double payoff;
    }

    static void Main(string[] args)
    {
      SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=Saratoga.sqlite;Version=3;");
      m_dbConnection.Open();

      List<Day> days = DataAccessObject.retrieve(m_dbConnection);


      foreach (Day day in days)
      {
        if (day != null && day.isValid())
          day.setAllHorseRanks();
      }

      findBestBet(days);
    }

    /**
     * Prints out a list of the odds for various bets and their respective payouts across ten years of data.
     * returns void
     */
    public static void findBestBet(List<Day> days)
    {
      Console.WriteLine("Horses to win:");
      printSingleHorseDictionary(retrieveListForPosn(days, getWin));
      Console.WriteLine("\nHorses to place:");
      printSingleHorseDictionary(retrieveListForPosn(days, getPlace));
      Console.WriteLine("\nHorses to show:");
      printSingleHorseDictionary(retrieveListForPosn(days, getShow));
      Console.WriteLine("\nHorses to fourth:");
      printSingleHorseDictionary(retrieveListForPosn(days, getFourth));

      Console.WriteLine("\nExacta:");
      printExactaHorseDictionary(bestSpecialBet(days, exacta));

      Console.WriteLine("\nTrifecta:");
      printTrifectaHorseDictionary(bestSpecialBet(days, trifecta));

      Console.ReadKey();
    }

    /**
     * Prints out the given dictionary sorted from most likely bet to least likely.
     * returns void
     */
    public static void printSingleHorseDictionary(Dictionary<byte, WinnerData> d)
    {
      int total = 0;
      foreach (KeyValuePair<byte, WinnerData> kvp in from entry in d orderby entry.Value.count descending select entry)
      {
        total += kvp.Value.count;
      }

      foreach (KeyValuePair<byte, WinnerData> kvp in from entry in d orderby entry.Value.count descending select entry)
      {
        double percent = (double)kvp.Value.count / (double)total;
        if (percent > .01)
        {
          Console.WriteLine("Key = {0}, Value = {1}, $2 Payout = {2}", kvp.Key, percent, kvp.Value.payoff);
        }
        else
        {
          break;
        }
      }
    }

    /**
     * Prints out the given dictionary sorted from most likely bet to least likely. For exactas.
     * returns void
     */
    public static void printExactaHorseDictionary(Dictionary<byte[], WinnerData> d)
    {
      int total = 0;
      foreach (KeyValuePair<byte[], WinnerData> kvp in from entry in d orderby entry.Value.count descending select entry)
      {
        total += kvp.Value.count;
      }

      foreach (KeyValuePair<byte[], WinnerData> kvp in from entry in d orderby entry.Value.count descending select entry)
      {
        double percent = (double)kvp.Value.count / (double)total;
        if (percent > .01)
        {
          Console.WriteLine("Key = {0},{1}, Value = {2}, $2 Payout = {3}", kvp.Key[0], kvp.Key[1], percent, kvp.Value.payoff);
        }
        else
        {
          break;
        }
      }
    }

    /**
     * Prints out the given dictionary sorted from most likely bet to least likely. For trifectas.
     * returns void
     */
    public static void printTrifectaHorseDictionary(Dictionary<byte[], WinnerData> d)
    {
      int total = 0;
      foreach (KeyValuePair<byte[], WinnerData> kvp in from entry in d orderby entry.Value.count descending select entry)
      {
        total += kvp.Value.count;
      }

      foreach (KeyValuePair<byte[], WinnerData> kvp in from entry in d orderby entry.Value.count descending select entry)
      {
        double percent = (double)kvp.Value.count / (double)total;
        if (percent > .001)
        {
          Console.WriteLine("Key = {0},{1},{2}, Value = {3}, $2 Payout = {4}", kvp.Key[0], kvp.Key[1], kvp.Key[2], percent, kvp.Value.payoff);
        }
        else
        {
          break;
        }
      }
    }

    /**
     * Returns a dictionary containing the odds that each horse came in for the given Position over the given days.
     */
    public static Dictionary<byte, WinnerData> retrieveListForPosn(List<Day> days, RetrievePosn f)
    {
      int finalCount = 0;
      Dictionary<byte, WinnerData> result = new Dictionary<byte, WinnerData>();
      

      foreach (Day day in days)
      {
        if (day != null && day.getRaces() != null) {
          foreach (Race race in day.getRaces())
          {
            if (race.getWinPayoff() > 0)
            {
              byte rank = f(race).getOddRank();
              if (!result.ContainsKey(rank))
              {
                result.Add(rank, new WinnerData() { count = 0, payoff = race.getWinPayoff() });
              }
              else
              {
                WinnerData old = result[rank];
                old.count++;
                old.payoff += race.getWinPayoff();
                result[rank] = old;
              }
              finalCount++;
            }
          }
        }
      }
      Console.WriteLine("Total number of races: " + finalCount);
      return result;
    }

    /**
     * Retrieves the payout odds for the given type of bet.
     * return dictionary of the data
     */
    public static Dictionary<byte[], WinnerData> bestSpecialBet(List<Day> days, RetrieveHorses f)
    {
      int finalCount = 0;
      Dictionary<byte[], WinnerData> result = new Dictionary<byte[], WinnerData>(new ByteArrayComparer());
      foreach (Day day in days)
      {
        if (day != null && day.getRaces() != null)
        {
          foreach (Race race in day.getRaces())
          {
            if (race.getTrifectaPayoff() > 0)
            {
              byte[] winners = f(race);
              if (!result.ContainsKey(winners))
              {
                result.Add(winners, new WinnerData()
                {
                  count = 0,
                  payoff = race.getTrifectaPayoff()
                });
              }
              else
              {
                WinnerData old = result[winners];
                old.count++;
                old.payoff += race.getTrifectaPayoff();
                result[winners] = old;
              }
              finalCount++;
            }
          }
        }
      }
      Console.WriteLine("THIS IS THE NUMBER OF HORSES: " + finalCount);
      return result;
    }

    /**
     * Returns a byte array of the rank of the win horse and place horse.
     */
    private static byte[] exacta(Race r)
    {
      return new byte[] { getWin(r).getOddRank(), getPlace(r).getOddRank() };
    }

    /**
     * Returns a byte array of the rank of the win horse, place horse, and show horse.
     */
    private static byte[] trifecta(Race r)
    {
      return new byte[] { getWin(r).getOddRank(), getPlace(r).getOddRank(), getShow(r).getOddRank() };
    }

    /**
     * Returns the winning horse for the given race.
     */
    private static Horse getWin(Race r)
    {
      return r.getWin();
    }

    /**
     * Returns the placing horse for the given race.
     */
    private static Horse getPlace(Race r)
    {
      return r.getPlace();
    }

    /**
     * Returns the showing horse for the given race.
     */
    private static Horse getShow(Race r)
    {
      return r.getShow();
    }

    /**
     * Returns the fourth placing horse for the given race.
     */
    private static Horse getFourth(Race r)
    {
      return r.getFourth();
    }

    private class ByteArrayComparer : IEqualityComparer<byte[]>
    {
      public int GetHashCode(byte[] obj)
      {
        byte[] arr = obj as byte[];
        int hash = 0;
        foreach (byte b in arr) hash ^= b;
        return hash;
      }
      public new bool Equals(byte[] x, byte[] y)
      {
        byte[] arr1 = x as byte[];
        byte[] arr2 = y as byte[];
        if (arr1.Length != arr2.Length) return false;
        for (int ix = 0; ix < arr1.Length; ++ix)
          if (arr1[ix] != arr2[ix]) return false;
        return true;
      }
    }
  }
}
