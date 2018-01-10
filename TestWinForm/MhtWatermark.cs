using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;

namespace TestWinForm
{
    public partial class MhtWatermark : Form
    {
        public MhtWatermark()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. initialize word application
            Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = false;
            int WaterMarkType = 0;
            WordApp.Documents.Open(textBox1.Text);
            System.Collections.IEnumerator shapes = WordApp.ActiveDocument.Shapes.GetEnumerator();
            int num = 9;//for compile only.
            while (shapes.MoveNext())
            {
                Shape s1 = (Shape)shapes.Current;
               if (s1.Name.StartsWith("PowerPlusWaterMark"))
                {
	                    s1.Select();
                   s1.IncrementTop(WordApp.ActiveDocument.PageSetup.TopMargin - WordApp.ActiveDocument.PageSetup.HeaderDistance);
	                    WordApp.Selection.Copy();
	                    WaterMarkType = 1;//text
               }
	                if (s1.Name.StartsWith("WordPictureWatermark"))
                {
                   s1.Select();
                    s1.IncrementTop(WordApp.ActiveDocument.PageSetup.HeaderDistance);
                    WordApp.Selection.Copy();
                    WaterMarkType = 2;//picture
              }
            }
           if (WaterMarkType > 0)
            {
                for (int i = 1; i < num; i++)
               {
	                    WordApp.Selection.Paste();
                    //System.Console.WriteLine("select:" + WordApp.Selection.GetHashCode());
                    if (WaterMarkType == 1)
                   {
                        WordApp.Selection.ShapeRange.IncrementTop(WordApp.ActiveDocument.PageSetup.PageHeight * i);
                    }
	                    else
                    {
                        //WordApp.Selection.Select();
	                        WordApp.Selection.MoveDown(WdUnits.wdLine, i*WordApp.ActiveDocument.PageSetup.LinesPage);
                  }
                }
           }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = false;
            Document doc = WordApp.Documents.Add();
            //doc.InlineShapes.AddChart();
            doc.SaveAs("e:\\11.pdwr");
            doc.Close();
            WordApp.Quit();
        }
    }
}
