using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

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
    private int number;
    private int purse;
    private Horse[] horses;

    public Race(int number, int purse, Horse[] horses)
    {
      this.number = number;
      this.purse = purse;
      this.horses = horses;
    }
  }
  class Horse
  {
    private String name;
    private int number;
    private Position pos;
    private double odds;

    public Horse(String name, int number, Position pos, double odds)
    {
      this.name = name;
      this.number = number;
      this.pos = pos;
      this.odds = odds;
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

      foreach (string s in sbArray)
      {
        getHorses(s);
        System.IO.File.WriteAllLines(@"c:\users\ryan\FOOOOOOOOOOOK.txt", s.Split(new Char[] {' '}));
        break;
        s.ToString().Split(new Char[] {' '});
      }
    }

    public static Horse[] getHorses(string s)
    {


      string pattern = @"\d\s(\S+\s)?(\S+\s)\((\S+,\s\S+)+\)";
      Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
      MatchCollection matches = rgx.Matches(s.Substring(s.IndexOf("Last Raced")));
      if (matches.Count > 0)
      {
        Console.WriteLine("({0} matches):\n", matches.Count);
        foreach (Match match in matches)
          Console.WriteLine("   " + match.Value);
      }
      //Console.WriteLine(s);
      while (true) ;

      return null;
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
