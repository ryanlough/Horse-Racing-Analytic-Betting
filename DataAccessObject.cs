using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HorseRacing
{
  class DataAccessObject
  {
    /**
     * Saves the given Day into the given SQLite DB
     */
    public void save(SQLiteConnection m_dbConnection, Day day)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        Serializer.Serialize<Day>(stream, day);
        string s64 = Convert.ToBase64String(stream.ToArray());

        string sql = "INSERT INTO saratoga (date, data) VALUES (@date, @data)";
        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
        command.CommandType = System.Data.CommandType.Text;
        command.Parameters.Add(new SQLiteParameter("@date", day.getSqlDate()));
        command.Parameters.Add(new SQLiteParameter("@data", s64));
        command.ExecuteNonQuery();
      }
    }
    
    /**
     * Returns a list of all Days in the given DB
     */
    public List<Day> retrieve(SQLiteConnection m_dbConnection)
    {
      List<Day> days = new List<Day>();
      string sql = "SELECT * FROM saratoga ORDER BY date desc";
      SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
      SQLiteDataReader reader = command.ExecuteReader();
      
      while (reader.Read())
      {
        using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(reader["data"].ToString())))
        {
          days.Add(Serializer.Deserialize<Day>(stream));
        }
      }
      return days;
    }
  }
}
