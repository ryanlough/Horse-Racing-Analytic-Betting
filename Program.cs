using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

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
  }
  class Race
  {
    private int number;
    private Horse[] horses;
  }
  class Horse
  {
    private String name;
    private int number;
    private Position pos; 
  }
  class Program
  {
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

      Console.WriteLine(builder.ToString());
      while (true) ;
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
        Console.WriteLine("\n\n\n\n\n\n\n\n\n");
      }

      public void EndTextBlock()
      {
      }

      public void RenderImage(ImageRenderInfo renderInfo)
      {
      }

      public void RenderText(TextRenderInfo renderInfo)
      {
        _builder.Append(renderInfo.GetText());
      }

      #endregion
    }
  }
}
