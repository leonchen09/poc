using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
using Oracle.DataAccess;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Oracle.DataAccess.Types.OracleClob c = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            word.Application app = new word.Application();
            app.Visible = true;

            CommandBar pdbar = app.CommandBars.Add("prontodoc link");


            CommandBarButton ctrl = pdbar.Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlButton, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarButton;
            ctrl.Tag = "test";
            ctrl.Caption = "Add ProntoDoc link";
            //ctrl.Style = MsoButtonStyle.msoButtonIconAndCaption;
            ctrl.Visible = true;
            //ctrl.Click += new _CommandBarButtonEvents_ClickEventHandler(ctrl_Click);
           // ctrl.OnAction 
            pdbar.Visible = true;


            word.Document doc = app.Documents.Open(@"e:\Doc1.docx");
            word.InlineShape img = doc.InlineShapes[1];

            
            string link = img.Hyperlink.Address;
            MessageBox.Show("old link: " + link);
            img.Hyperlink.Address = "http://localhost:81/";
            doc.Save();
            doc.Close();
            app.Quit();
        }
    }
}
