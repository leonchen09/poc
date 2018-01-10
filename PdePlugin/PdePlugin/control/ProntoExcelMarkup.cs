using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using PdePlugin;
using Microsoft.Office.Interop.Excel;
using word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;

namespace Pdw.Managers.Hcl
{
    public partial class ProntoExcelMarkup : UserControl
    {
        private ToolTip _domainToolTip;
        private const int DefaultWidth = 250;

        public TreeNode selectedNode { get; set; }
        public string LastCurrentDomain { get; set; }

        public ProntoExcelMarkup()
        {
            InitializeComponent();


            _domainToolTip = new ToolTip();

            treeView1.ExpandAll();

        }

        /// <summary>
        /// To Prevent Flicker Control
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }


        #region Resizable events (Collapse/Expand)

        private void cplDomain_CollapseExpand(EventArgs e)
        {
            MakePosition();
        }

        #endregion


        private void cboDomain_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        

        private void txtCondition_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.Text))
                {
                    txtUSCValue.Text +="";
                }
            }
            catch { }
        }

        private void txtCondition_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void trvDomain_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode dragNode = e.Item as TreeNode;
            if (dragNode != null)
            {
                DoDragDrop(dragNode.Text, DragDropEffects.Copy);
            }
        }

        /// <summary>
        /// enable or disable add conditon button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCondition_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUSCValue.Text.Trim()))
                btnAdd.Enabled = false;
            else
                btnAdd.Enabled = true;
        }


        /// <summary>
        /// Set default controls what are displayed
        /// </summary>
        private void SetDefaultControl()
        {

            MakePosition();
        }

        /// <summary>
        /// Set position for Domain, Exception, Condition section.
        /// </summary>
        private void MakePosition()
        {
            int headerHeight = pnlConditionHeader.Height;
            int splitterHeight = 2;
            int minimizeConditionHeight = 130;

          

            // make sure lstUSC.Height = int * lstUSC.ItemHeight
            lstUSC.Height = pnlListUSC.Height + (lstUSC.ItemHeight - pnlListUSC.Height % lstUSC.ItemHeight);
        }

        /// <summary>
        /// Set Height for this control when first load.
        /// </summary>
        /// <param name="height"></param>
        public void SetHeightControl(int height)
        {
            this.Height = height - 23;
            SetDefaultControl();
        }

       
        private void splitter2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            try
            {
                lstUSC.Height = pnlListUSC.Height + (lstUSC.ItemHeight - pnlListUSC.Height % lstUSC.ItemHeight);
            }
            catch { }
        }

        private void splitterInsideConditionTop_SplitterMoved(object sender, SplitterEventArgs e)
        {
            try
            {
                int heightChanged = pnlName.Height - 20;

                pnlName.Height = 20;

                if (heightChanged > 0) // decrease height of list condition
                {
                    int temp = pnlListUSC.Height - heightChanged;

                    if (temp > lstUSC.ItemHeight * 2)
                        pnlConditionExpression.Height += heightChanged;
                }
                else
                {
                    int temp = pnlConditionExpression.Height + heightChanged;
                    if (temp >= lstUSC.ItemHeight * 2)
                        pnlConditionExpression.Height = temp;
                }

                lstUSC.Height = pnlListUSC.Height + (lstUSC.ItemHeight - pnlListUSC.Height % lstUSC.ItemHeight);
            }
            catch { }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedNode = treeView1.SelectedNode;
            /**
            Microsoft.Office.Interop.Excel.Application app = Globals.ThisAddIn.Application;
            //app.ActiveWorkbook.XmlMaps.Add(app.ActiveWorkbook.Path + "\\PDEpocplugin.xsd");
            app.ActiveWorkbook.XmlMaps.Add("E:\\ProntoDir\\pde\\PDEpocplugin.xsd");
            XmlMap InvMap = app.ActiveWorkbook.XmlMaps.get_Item(1);
            InvMap.Name = "pdepoc_map";
            Worksheet sheet = app.ActiveWorkbook.Sheets.get_Item(1);
            ListObject lst =  sheet.ListObjects.AddEx();
            lst.ListColumns.get_Item(1).Name = "中文名字";
            lst.ShowAutoFilter = false;
            string strXPath = "/Root/ExpenseItem/Description";
            lst.ListColumns.get_Item(1).XPath.SetValue(InvMap, strXPath);
            Range cl = sheet.Cells[5,5];
            cl.XPath.SetValue(InvMap, "/Root/Meta/Email");
            Range c2 = sheet.Cells[5, 4];
            c2.set_Value(XlRangeValueDataType.xlRangeValueDefault, "email:");

            Range c5 = sheet.Cells[8, 3];
            c5.Select();
           // lst.Range.Copy();
            //sheet.Paste();
            ListObject lst2 = sheet.ListObjects.AddEx(XlListObjectSourceType.xlSrcRange, lst.Range);
            //lst2.DataBodyRange.DataSeries
            //lst2.Range = lst.DataBodyRange; . .get_Item(1).v = lst.ListColumns.get_Item(1);
             */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application app = Globals.ThisAddIn.Application;
            //app.ActiveWorkbook.XmlMaps.get_Item(1).Import(app.ActiveWorkbook.Path + "\\PDEpocXmlData.xml");
            app.ActiveWorkbook.XmlMaps.get_Item(1).Import("E:\\ProntoDir\\pde\\PDEpocXmlData.xml");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Microsoft.Office.Interop.Excel.Application app = Globals.ThisAddIn.Application;
            //word.Application Word;
            //Object creator = app.Parent;
            //if (creator != null)
            //{
            //    Word = (word.Application)creator;

            //}
            //else
            //{
            //    Word = new word.Application();
            //}
            word.Application Word = Globals.ThisAddIn.wordApp;
            if (Word == null || Word.Visible == false)
            {
                Word = null;
                MessageBox.Show("This excel do not link to word or word has been closed.");
                return;
            }
            refershWordAddIn(Word);
        }

        private void refershWordAddIn(word.Application Word)
        {
            COMAddIns addins = Word.COMAddIns;
            foreach (COMAddIn addin in addins)
            {
                if (addin.Description.Equals("Pdw"))
                {
                    //IAddinUtilities util = (IAddinUtilities)addin.Object;
                    //string result = util.DoSomething("from1");
                    //MessageBox.Show(result);

                    Object pdwApp = addin.Object;
                    object[] invokeArgs = { " name in excel" };
                    object retVal = pdwApp.GetType().InvokeMember("DoSomething",
                        System.Reflection.BindingFlags.InvokeMethod, null, pdwApp, invokeArgs);
                    MessageBox.Show(retVal.ToString());
                }
            }
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBox1.Text = treeView2.SelectedNode.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string oldNodePath = treeView2.SelectedNode.FullPath;
            string newName = textBox1.Text;
            treeView2.SelectedNode.Text = newName;
            string newNodePath = treeView2.SelectedNode.FullPath;
            Globals.ThisAddIn.ribbon2.export.updateName(oldNodePath, newName, newNodePath);
        }

    }
}