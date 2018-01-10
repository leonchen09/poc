
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pdw.Core
{
    public partial class TreeViewSpliter : UserControl
    {
        private const int NoneIndex = -1;

        #region delegates
        public event ItemDragEventHandler ItemDrag;
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick;
        #endregion

        #region pubpic properties
        public int ImageIndex
        {
            set
            {
                trvTop.ImageIndex = value;
                trvBottom.ImageIndex = value;
            }
        }
        public ImageList ImageList
        {
            set
            {
                trvTop.ImageList = value;
                trvBottom.ImageList = value;
            }
        }
        public int SelectedImageIndex
        {
            set
            {
                trvTop.SelectedImageIndex = value;
                trvBottom.SelectedImageIndex = value;
            }
        }

        public TreeView TreeTop
        {
            get { return trvTop; }
        }

        public TreeView TreeBottom
        {
            get { return trvBottom; }
        }

        public System.Collections.Generic.List<TreeNode> AllNodes { get; private set; }
        #endregion

        public TreeViewSpliter()
        {
            InitializeComponent();

            trvTop.ItemDrag += new ItemDragEventHandler(trvData_ItemDrag);
            trvTop.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(trvData_NodeMouseDoubleClick);
            trvBottom.ItemDrag += new ItemDragEventHandler(trvData_ItemDrag);
            trvBottom.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(trvData_NodeMouseDoubleClick);

            spcTreeView.SplitterDistance = spcTreeView.Height;
        }

        #region public methods
        /// <summary>
        /// add node into tree
        /// </summary>
        /// <param name="text"></param>
        /// <param name="imageIndex"></param>
        /// <param name="selectedImageIndex"></param>
        /// <param name="tooltip"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public TreeViewSpliterItem AddNode(string text,
            int imageIndex = NoneIndex, int selectedImageIndex = NoneIndex, string tooltip = "", object tag = null)
        {
            TreeViewSpliterItem tsiItem = new TreeViewSpliterItem();
            string nodeID = GenNodeID();
            tsiItem.TreeNodeTop = AddNode(trvTop.Nodes, nodeID, text, imageIndex, selectedImageIndex, tooltip, tag);
            tsiItem.TreeNodeBottom = AddNode(trvBottom.Nodes, nodeID, text, imageIndex, selectedImageIndex, tooltip, tag);

            return tsiItem;
        }

        /// <summary>
        /// Add a node into tree
        /// </summary>
        /// <param name="topNodes"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="imageIndex"></param>
        /// <param name="selectedImageIndex"></param>
        /// <param name="tooltip"></param>
        /// <param name="tag"></param>
        public TreeViewSpliterItem AddNode(TreeViewSpliterItem parentItem, string text,
            int imageIndex = NoneIndex, int selectedImageIndex = NoneIndex, string tooltip = "", object tag = null)
        {
            TreeViewSpliterItem tsi = new TreeViewSpliterItem();

            string nodeID = GenNodeID();
            if (parentItem != null && parentItem.TreeNodeTop != null)
                tsi.TreeNodeTop = AddNode(parentItem.TreeNodeTop, nodeID, text, imageIndex, selectedImageIndex, tooltip, tag);
            if (parentItem != null && parentItem.TreeNodeBottom != null)
                tsi.TreeNodeBottom = AddNode(parentItem.TreeNodeBottom, nodeID, text, imageIndex, selectedImageIndex, tooltip, tag);

            return tsi;
        }

        /// <summary>
        /// clear data of tree
        /// </summary>
        /// <param name="type"></param>
        public void Clear()
        {
            trvTop.Nodes.Clear();
            trvBottom.Nodes.Clear();
        }

        public void SetSelectedNode(int index)
        {
            if (trvTop.Nodes != null && trvTop.Nodes.Count > index)
                trvTop.SelectedNode = trvTop.Nodes[index];

            if (trvBottom.Nodes != null && trvBottom.Nodes.Count > index)
                trvBottom.SelectedNode = trvBottom.Nodes[index];
        }

        /// <summary>
        /// expand all the tree nodes
        /// </summary>
        public void ExpandAll()
        {
            trvTop.ExpandAll();
            trvBottom.ExpandAll();
        }

        public void ShowNodeToolTips(bool isShow)
        {
            trvTop.ShowNodeToolTips = isShow;
            trvBottom.ShowNodeToolTips = isShow;
        }

        public TreeNodeCollection Nodes
        {
            get
            {
                return trvTop.Nodes;
            }
        }
        #endregion

        #region helper methods
        private string GenNodeID()
        {
            return "trn" + Guid.NewGuid().ToString().Replace('-', '_');
        }

        private TreeNode AddNode(TreeNode parent, string nodeID, string text,
            int imageIndex = NoneIndex, int selectedImageIndex = NoneIndex, string tooltip = "", object tag = null)
        {
            if (parent != null && parent.Nodes != null)
            {
                TreeNode trnNode = parent.Nodes.Add(nodeID, text);
                if (imageIndex > NoneIndex)
                    trnNode.ImageIndex = imageIndex;
                if (selectedImageIndex > NoneIndex)
                    trnNode.SelectedImageIndex = selectedImageIndex;
                if (!string.IsNullOrWhiteSpace(tooltip))
                    trnNode.ToolTipText = tooltip;
                if (tag != null)
                    trnNode.Tag = tag;

                // using for decorate
                if (AllNodes == null)
                    AllNodes = new System.Collections.Generic.List<TreeNode>();
                AllNodes.Add(trnNode);

                return trnNode;
            }

            return null;
        }

        private TreeNode AddNode(TreeNodeCollection nodes, string nodeID, string text,
            int imageIndex = NoneIndex, int selectedImageIndex = NoneIndex, string tooltip = "", object tag = null)
        {
            if (nodes != null)
            {
                TreeNode trnNode = nodes.Add(nodeID, text);
                if (imageIndex > NoneIndex)
                    trnNode.ImageIndex = imageIndex;
                if (selectedImageIndex > NoneIndex)
                    trnNode.SelectedImageIndex = selectedImageIndex;
                if (!string.IsNullOrWhiteSpace(tooltip))
                    trnNode.ToolTipText = tooltip;
                if (tag != null)
                    trnNode.Tag = tag;

                return trnNode;
            }

            return null;
        }
        #endregion

        #region events
        void trvData_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (NodeMouseDoubleClick != null)
                NodeMouseDoubleClick(sender, e);
        }

        void trvData_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (ItemDrag != null)
                ItemDrag(sender, e);
        }

        private void spcTreeView_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            Cursor.Current = System.Windows.Forms.Cursors.NoMoveVert;
        }

        private void spcTreeView_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Cursor.Current = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region helper class
        public class TreeViewSpliterItem
        {
            public TreeNode TreeNodeTop { get; set; }
            public TreeNode TreeNodeBottom { get; set; }
        }
        #endregion
    }
}