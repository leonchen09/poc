
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Pdw.Managers.Hcl
{
    public partial class ChooseDomain : Form
    {
        public List<Core.DomainMatch> DomainMatches { get; private set; }

        #region constrcutor
        /// <summary>
        /// initialize choose domain dialog
        /// </summary>
        /// <param name="domainMatches"></param>
        public ChooseDomain(List<Pdw.Core.DomainMatch> domainMatches)
        {
            InitializeComponent();
            DomainMatches = domainMatches;

            foreach (Core.DomainMatch domainMatch in domainMatches)
                BuildTabItemDomain(domainMatch);
        }
        #endregion

        #region events handler (OK button, check to radio button)
        /// <summary>
        /// user confirm choosen a domain or no
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Button btnOK = sender as Button;
            TabPage tabItem = btnOK.Parent as TabPage;
            Core.DomainMatch domainMatch = tabItem.Tag as Core.DomainMatch;
            Core.DomainMatch returnDomainMatch = DomainMatches.FirstOrDefault(d => d.DomainName == domainMatch.DomainName);
            returnDomainMatch.CandidateDomainName = returnDomainMatch.DomainName;
            returnDomainMatch.NewDomainName = returnDomainMatch.CandidateDomainName;
        }

        /// <summary>
        /// load not match field when user select difference domain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbdDomain_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbnDomain = sender as RadioButton;
            Panel pnlDomain = rbnDomain.Parent as Panel;
            TabPage tabDomain = pnlDomain.Parent as TabPage;
            Core.DomainMatch domainMatch = tabDomain.Tag as Core.DomainMatch;
            Core.DomainMatchItem domainMatchItem = rbnDomain.Tag as Core.DomainMatchItem;

            Core.DomainMatch returnDomainMatch = DomainMatches.FirstOrDefault(d => d.DomainName == domainMatch.DomainName);
            returnDomainMatch.CandidateDomainName = domainMatchItem.DomainName;

            
            BuildListboxMatchedField(domainMatchItem, tabDomain);
        }
        #endregion

        #region UI helpers
        private void BuildTabItemDomain(Core.DomainMatch domainMatch)
        {
            string guid = Guid.NewGuid().ToString().Replace('-', '_');
            TabPage tabItem = new TabPage();
            tabItem.Location = new System.Drawing.Point(4, 22);
            tabItem.Name = GenControlID("tabItem", guid);
            tabItem.Padding = new System.Windows.Forms.Padding(3);
            tabItem.Size = new System.Drawing.Size(454, 290);
            tabItem.TabIndex = 0;
            tabItem.Text = domainMatch.DomainName;
            tabItem.UseVisualStyleBackColor = true;
            tabItem.Tag = domainMatch;

            // prepare data
            List<Core.DomainMatchItem> sortedDomainMatchItems = domainMatch.DomainMatchItems.Values.ToList();
            Sort(sortedDomainMatchItems);

            // lstFields
            ListBox lstFields = BuildListboxMatchedField(sortedDomainMatchItems[0], tabItem);
            tabItem.Controls.Add(lstFields);

            // pnlDomain
            Panel pnlDomain = BuildPanelCandidateDomains(sortedDomainMatchItems, guid);
            tabItem.Controls.Add(pnlDomain);

            // btnOK
            Button btnOK = BuildButtonAccept(guid);
            tabItem.Controls.Add(btnOK);

            tabDomains.TabPages.Add(tabItem);
        }

        private ListBox BuildListboxMatchedField(Core.DomainMatchItem domainMatchItem, TabPage tabItem)
        {
            string lstFieldsID = tabItem.Name.Replace("tabItem", "lstFields");
            ListBox lstFields = null;

            // set properties
            if (tabItem.Controls.ContainsKey(lstFieldsID))
                lstFields = tabItem.Controls[lstFieldsID] as ListBox;
            else
            {
                lstFields = new ListBox();
                lstFields.FormattingEnabled = true;
                lstFields.Location = new System.Drawing.Point(7, 8);
                lstFields.Name = lstFieldsID;
                lstFields.Size = new System.Drawing.Size(205, 251);
                lstFields.TabIndex = 12;
            }

            // bind data
            lstFields.DataSource = null;
            if (domainMatchItem != null && domainMatchItem.MatchedFields != null)
                lstFields.DataSource = domainMatchItem.MatchedFields;

            return lstFields;
        }

        private Panel BuildPanelCandidateDomains(List<Core.DomainMatchItem> domainMatchItems, string guid)
        {
            Panel pnlDomain = new Panel();
            pnlDomain.AutoScroll = true;
            pnlDomain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlDomain.Location = new System.Drawing.Point(220, 8);
            pnlDomain.Name = GenControlID("pnlDomain", guid);
            pnlDomain.Size = new System.Drawing.Size(226, 251);
            pnlDomain.TabIndex = 13;

            bool isChecked = true;
            int paddingLeft = 5;
            int paddingTop = 5;
            int itemHeight = 17;
            int startY = paddingTop;
            foreach (Core.DomainMatchItem domainMatchItem in domainMatchItems)
            {
                RadioButton rbnDomain = new RadioButton();
                rbnDomain.AutoSize = true;
                rbnDomain.Location = new System.Drawing.Point(paddingLeft, startY);
                rbnDomain.Height = itemHeight;
                rbnDomain.Text = domainMatchItem.Description;
                rbnDomain.UseVisualStyleBackColor = true;
                rbnDomain.Checked = isChecked;
                rbnDomain.CheckedChanged += new EventHandler(rbdDomain_CheckedChanged);
                rbnDomain.Tag = domainMatchItem;

                pnlDomain.Controls.Add(rbnDomain);

                startY += itemHeight + paddingTop;
                isChecked = false;
            }

            return pnlDomain;
        }

        private Button BuildButtonAccept(string guid)
        {
            Button btnOK = new Button();
            btnOK.Location = new System.Drawing.Point(371, 264);
            btnOK.Name = GenControlID("btnOK", guid);
            btnOK.Size = new System.Drawing.Size(75, 23);
            btnOK.TabIndex = 11;
            btnOK.Text = "Accept";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += new EventHandler(btnOK_Click);

            return btnOK;
        }

        /// <summary>
        /// sort input data (match fields is desc, domain name is asc)
        /// </summary>
        /// <param name="domainMatchItems"></param>\
        private void Sort(List<Pdw.Core.DomainMatchItem> domainMatchItems)
        {
            domainMatchItems.Sort(delegate(Core.DomainMatchItem item1, Core.DomainMatchItem item2)
            {
                // 1. compare not matched item (desc)
                // 2. compare domain name (asc)
                if (item1.MatchedFields.Count == item2.MatchedFields.Count) // compare domain name
                    return string.Compare(item1.DomainName, item2.DomainName);

                return (item1.MatchedFields.Count < item2.MatchedFields.Count) ? 1 : -1;
            });
        }

        private string GenControlID(string prefix, string guid, string suffix = "")
        {
            return prefix + guid;
        }
        #endregion
    }
}