using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading;
using System.Data.SQLite;
using ProtoBuf;
using System.IO;


namespace HorseRacing
{
  /**
   * Main class
   */
  class Program
  {
    public static List<string> pages = new List<string>();

    static void Main(string[] args)
    {/*
      //SQLiteConnection.CreateFile("Saratoga.sqlite");
      SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=Saratoga.sqlite;Version=3;");
      m_dbConnection.Open();

      //string sql = "CREATE TABLE saratoga (date TEXT, data TEXT)";
      //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
      //command.ExecuteNonQuery();

      DateTime startDate = new DateTime(2000, 7, 25);
      DateTime endDate = new DateTime(2015, 9, 15);
      while (startDate.CompareTo(endDate) != 0)
      {
        Console.WriteLine("Trying: " + startDate.ToShortDateString());
        collectDataforDay(startDate, m_dbConnection);
        Thread.Sleep(2100);
        startDate = startDate.AddDays(1);
        if (startDate.Month > 8 && startDate.Day > 10)
        {
          startDate = new DateTime(startDate.Year + 1, 7, 25);
        }
      }
      */
      Console.WriteLine("PROCESS COMPLETE!");
    }

    /**
     * Put all data from given day into given table
     */
    public static void collectDataforDay(DateTime dateTime, SQLiteConnection m_dbConnection)
    {
      string zeroMonth = dateTime.Month < 10 ? "0" : "";
      string zeroDay = dateTime.Day < 10 ? "0" : "";
      string date = zeroMonth + dateTime.Month + "/" + zeroDay + dateTime.Day + "/" + dateTime.Year;

      PdfReader reader;
      try
      {
        reader = new PdfReader(@"http://web.archive.org/web/20150827194015/" +
                                "http://www.equibase.com/premium/eqbPDFChartPlus.cfm?RACE=A&BorP=P&TID=SAR&CTRY=USA&DT=" +
                                 date + "&DAY=D&STYLE=EQB");
      }
      catch (Exception e)
      {
        //Gross hack to get around captcha on equibase. This was more useful before I realized all the pdfs were cached..
        Console.WriteLine("CAPSHA TIME!!");
        Console.Beep();
        Console.ReadKey();
        reader = new PdfReader(@"http://web.archive.org/web/20150827194015/" +
                                "http://www.equibase.com/premium/eqbPDFChartPlus.cfm?RACE=A&BorP=P&TID=SAR&CTRY=USA&DT=" +
                                date + "&DAY=D&STYLE=EQB");
      }
      StringBuilder builder = new StringBuilder();

      for (int x = 1; x <= reader.NumberOfPages; x++)
      {
        PdfDictionary page = reader.GetPageN(x);
        IRenderListener listener = new SBTextRenderer(builder);
        PdfContentStreamProcessor processor = new PdfContentStreamProcessor(listener);
        PdfDictionary pageDic = reader.GetPageN(x);
        PdfDictionary resourcesDic = pageDic.GetAsDict(PdfName.RESOURCES);
        processor.ProcessContent(ContentByteUtils.GetContentBytesForPage(reader, x), resourcesDic);
      }

      if (pages.Count != 0)
      {

        DataHandler handler = new DataHandler(dateTime, pages, m_dbConnection);
        Thread thread = new Thread(new ThreadStart(handler.extractPdfData));

        thread.Start();
        thread.Join();
        reader.Dispose();
        pages.Clear();
      }
      else
      {
        // If there were no races on this particular day, simply skip it! :D
        Console.WriteLine("Invalid Date: " + date);
      }
    }

    /**
     * Class to convert a PDF into usable text.
     */
    public class SBTextRenderer : IRenderListener
    {

      private StringBuilder _builder;
      public SBTextRenderer(StringBuilder builder)
      {
        _builder = builder;
      }
      #region IRenderListener Members

      public void BeginTextBlock()
      {
      }

      public void EndTextBlock()
      {
      }

      public void RenderImage(ImageRenderInfo renderInfo)
      {
      }

      public void RenderText(TextRenderInfo renderInfo)
      {
        _builder.Append(renderInfo.GetText() + " ");
        // New page detected at copywrite statement.
        if (renderInfo.GetText().Equals("Reserved."))
        {
          pages.Add(_builder.ToString());
          _builder.Clear();
        }
      }

      #endregion
    }

    /**
     * Class to handle reading and storing all the extracted PDF data.
     */
    public class DataHandler
    {
      private DateTime date;
      private List<string> pages;
      private SQLiteConnection m_dbConnection;

      public DataHandler(DateTime date, List<string> pages, SQLiteConnection m_dbConnection)
      {
        this.date = date;
        this.pages = pages;
        this.m_dbConnection = m_dbConnection;
      }

      /**
       * Extract the current day's race data from the pdf
       */
      public void extractPdfData()
      {
        List<Race> races = new List<Race>();

        byte b = 0;
        foreach (String s in pages)
        {
          //Abort if the race has too few characters.
          //Workaround due to equibase putting a ton of redundant text in their pdfs for no reason...
          if (s == null || s.Length < 150)
          {
            break;
          }
          races.Add(new Race(++b, Race.extractPurse(s), Horse.extractHorses(s),
                        Race.extractWeather(s), Race.extractTrack(s), Race.extractLength(s)));
        }
        b = 0;

        saveToDatabase(races, m_dbConnection);
      }

      public void saveToDatabase(List<Race> races, SQLiteConnection m_dbConnection)
      {
        Day d = new Day(date, races);

        DataAccessObject.save(m_dbConnection, d, true);
        Console.WriteLine("Finished saving: " + d.getSqlDate());
      }
    }
  }
}