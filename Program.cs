using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ConsoleApplication2
{
  enum Position {
    None,
    Win,
    Place,
    Show,
    Fourth
  }
  class Day
  {
    private DateTime date;
    private Race[] races;

    public Day(DateTime date)
    {
      this.date = date;
    }
  }
  class Race
  {
    private int purse; //TODO need condition, weather, type
    private Horse[] horses;

    public Race(int number, int purse, Horse[] horses)
    {
      this.purse = purse;
      this.horses = horses;
    }
  }
  class Horse
  {
    private int number;
    private String name;
    private String jockey;
    private double odds;

    public Horse(string number, String name, String jockey, string odds)
    {
      this.number = Convert.ToInt32(number.Trim());
      this.name = name;
      this.jockey = jockey;
      this.odds = Convert.ToDouble(odds.Trim());
    }

    public override string ToString()
    {
      return "Number: " + number + " Name: " + name + " Jockey: " + jockey + " Odds: " + odds;
    }
  }
  class Program
  {
    public static int currentRace = 0;
    public static string[] sbArray = new string[10];
    static void Main(string[] args)
    {
      PdfReader reader = new PdfReader(@"c:\users\ryan\test.pdf");

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

      Race[] r = new Race[10];
      int i = 0;
      Array.ForEach<string>(sbArray, (s) => r[i] = new Race(i, getPurse(s), getHorses(s)));
    }

    public static int getPurse(string s)
    {
      string pattern = @"\$\S+\s";
      foreach (Match match in Regex.Matches(s.Substring(s.IndexOf("Purse:")), pattern, RegexOptions.ExplicitCapture))
      {
        return int.Parse(match.Value, NumberStyles.Currency);
      }
      return -1;
    }

    public static Horse[] getHorses(string s)
    {
      MatchCollection matches = getHorseData(s);
      if (matches.Count > 0)
      {
        Horse[] h;
        h = new Horse[matches.Count];

        int i = 0;
        foreach (Match match in matches)
        {
          h[i++] = new Horse(match.Groups["num"].Value, match.Groups["name"].Value,
                             match.Groups["jockey"].Value, match.Groups["odds"].Value);
        }
        return h;
      }
      else
      {
        throw new Exception("Something went wrong: there are no horses.");
      }
    }

    /**
     * Finds all horse numbers, names, jockeys, and odds
     * returns MatchCollection of above for each horse
     */
    public static MatchCollection getHorseData(string s)
    {
      string pattern = @"\s(?<num>\d)\s(?<name>(\D+\s)?(\S+\s))\((?<jockey>(\S+,\s\S+)+)\).+?(?<odds>\d+\.\d+)";
      return Regex.Matches(s.Substring(s.IndexOf("Last Raced")), pattern, RegexOptions.ExplicitCapture);
    }

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
        if (renderInfo.GetText().Equals("Reserved."))
        {
          sbArray[currentRace] = _builder.ToString();
          _builder.Clear();
          currentRace++;
        }
      }

      #endregion
    }
  }
}
