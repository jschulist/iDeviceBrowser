using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace iDeviceBrowser
{
    public class NodeAndFolders
    {
        public TreeNode Node { get; set; }
        public List<Folder> Folders { get; set; }

        public NodeAndFolders(TreeNode node, List<Folder> folders)
        {
            this.Node = node;
            this.Folders = folders;
        }
    }
}
