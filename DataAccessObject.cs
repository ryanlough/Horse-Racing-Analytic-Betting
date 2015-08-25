using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRacing
{
  class DataAccessObject
  {
    public void save(Day day)
    {
      SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=Saratoga.sqlite;");
      m_dbConnection.Open();

      using (MemoryStream stream = new MemoryStream())
      {
        Serializer.Serialize<Day>(stream, day);


        stream.Position = 0;
        StreamReader sr = new StreamReader(stream);
        string data = sr.ReadToEnd();

        string sql = "INSERT INTO saratoga (day, data) values (TO_DATE('" + day.getSqlDate() +
                     "', 'YYYYMMDD'), '" + data.Replace("'", "''") + "')";
        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
        command.ExecuteNonQuery();
      }
    }

    public Day retrieve()
    {
      using (var file = System.IO.File.OpenRead(@"C:\Users\Ryan\person.bin"))
      {
        //Console.WriteLine(Serializer.Deserialize<Day>(stream));
      }

      return null;
    }
  }
}
