using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Manzana;
using System.Threading;
using System.IO;

namespace iDeviceBrowser
{
    public partial class Main : Form
    {
        // TODO: SET TAB ORDER

        // TODO: ADD SUPPORT FOR DRAGGING FOLDERS FROM THE TREEVIEW TO THE DESKTOP

        private readonly iPhone _iDeviceInterface = new iPhone();
        private bool _isConnected;
        private ShellDataObject _dataObj;
        private TreeNode _selectedNode;
        private UserSettings _userSettings = new UserSettings();

        private const string ROOT_NODE_NAME = "/ [root]";
        private const string ROOT_PATH = "/";

        public delegate void Callback();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            _userSettings.Setup();

            UpdateTitle();

            _iDeviceInterface.Connect += new ConnectEventHandler(ConnectionChanged);
            _iDeviceInterface.Disconnect += new ConnectEventHandler(ConnectionChanged);

            PopulateFavorites();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PopulateFavorites()
        {
            foreach (Favorite favorite in _userSettings.Favorites)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(favorite.Name);
                tsmi.Tag = favorite.Path;
                tsmi.Click += new EventHandler(Favorite_Click);
                favoritesToolStripMenuItem.DropDownItems.Add(tsmi);
            }
        }

        private void Favorite_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            string path = tsmi.Tag.ToString();

            Async(DisableInterface, delegate { SelectNode(path); }, EnableInterface);
        }

        private void ConnectionChanged(object sender, ConnectEventArgs args)
        {
            SetConnectionChanged();
        }

        private void SetConnectionChanged()
        {
            _isConnected = !_isConnected;

            if (_isConnected)
            {
                toolStripStatusLabel1.Text = "Connected";

                PopulateInitialFolders();
            }
            else
            {
                toolStripStatusLabel1.Text = "Disconnected";

                Cleanup();
            }

            ShiftToUiThread(UpdateTitle);
        }

        private void Cleanup()
        {
            ShiftToUiThread(
                delegate
                {
                    treeView1.Nodes.Clear();
                    listView1.Items.Clear();
                }
            );
        }

        private void UpdateTitle()
        {
            string status;
            if (_isConnected)
            {
                status = string.Format("Connected, {0} {1} ({2}), " + (_iDeviceInterface.IsJailbreak ? "jailbroken" : "not jailbroken (no afc2 service found)"), _iDeviceInterface.DeviceType, _iDeviceInterface.DeviceVersion, _iDeviceInterface.DeviceName.Replace("\0", ""));
            }
            else
            {
                status = "Disconnected";
            }

            this.Text = Constants.TITLE + " - " + status;
        }

        private void PopulateInitialFolders()
        {
            TreeNode rootNode = new TreeNode(ROOT_NODE_NAME);
            rootNode.Name = ROOT_PATH;

            ShiftToUiThread(
                delegate
                {
                    treeView1.BeginUpdate();

                    treeView1.Nodes.Add(rootNode);
                    treeView1.ContextMenuStrip = folderContextMenu;

                    rootNode.Expand();
                    rootNode.Tag = true;
                    treeView1.SelectedNode = rootNode;

                    treeView1.EndUpdate();
                }
            );

            RefreshChildFoldersAsync(rootNode, true, delegate { rootNode.Expand(); });
        }

        private void RefreshChildFolders(TreeNode node, bool forceRefresh)
        {
            List<NodeAndFolders> nodeAndFolders = new List<NodeAndFolders>();

            if (node.Tag == null || forceRefresh)
            {
                string path = GetPathFromNode(node);

                List<Folder> folders = GetFolders(path, 0);

                nodeAndFolders.Add(new NodeAndFolders(node, folders));
            }

            UpdateChildFolders(nodeAndFolders);

            node.Tag = true;

            UpdateStatus("Refresh complete");
        }

        private void UpdateStatus(string message)
        {
            ShiftToUiThread(
                delegate
                {
                    toolStripStatusLabel1.Text = message;
                }
            );
        }

        private void RemoveNodeFromNode(TreeNode parent, TreeNode child)
        {
            ShiftToUiThread(
                delegate
                {
                    treeView1.BeginUpdate();
                    parent.Nodes.Remove(child);
                    treeView1.EndUpdate();
                }
            );
        }

        private void UpdateChildFolders(List<NodeAndFolders> nodeAndFolders)
        {
            ShiftToUiThread(
                delegate
                {
                    treeView1.BeginUpdate();

                    foreach (NodeAndFolders naf in nodeAndFolders)
                    {
                        naf.Node.Nodes.Clear();
                        AddFoldersToNode(naf.Node, naf.Folders);
                    }

                    treeView1.EndUpdate();
                }
            );
        }

        private void AddFoldersToNode(TreeNode node, List<Folder> folders)
        {
            ShiftToUiThread(
               delegate
               {
                   foreach (Folder folder in folders)
                   {
                       TreeNode newNode = new TreeNode(folder.Name);
                       newNode.Name = folder.Name;
                       node.Nodes.Add(newNode);

                       if (folder.SubFolders != null && folder.SubFolders.Count > 0)
                       {
                           AddFoldersToNode(newNode, folder.SubFolders);
                       }
                   }
               }
           );
        }

        private List<Folder> GetFolders(string path, int depth)
        {
            List<Folder> folders = new List<Folder>();

            UpdateStatus("Retrieving folders under: " + path);

            string[] directories = _iDeviceInterface.GetDirectories(path);
            Array.Sort(directories);

            foreach (string directory in directories)
            {
                List<Folder> subFolders = null;

                if (depth < 1)
                {
                    subFolders = GetFolders(Utilities.PathCombine(path, directory), depth + 1);
                }

                folders.Add(new Folder(directory, subFolders));
            }

            return folders;
        }

        private void RefreshFilesAsync(Callback callback)
        {
            Async(DisableInterface, RefreshFiles, delegate { EnableInterface(); if (callback != null) callback(); });
        }

        private void RefreshFilesAsync()
        {
            RefreshFilesAsync(null);
        }

        private void RefreshFiles()
        {
            string path = GetPathFromNode(_selectedNode);

            string[] folders = null;
            string[] files = null;
            try
            {
                folders = _iDeviceInterface.GetDirectories(path);
                files = _iDeviceInterface.GetFiles(path);
            }
            catch (Exception ex) // most likely someone removed the folder
            {
                RemoveNodeFromNode(_selectedNode.Parent, _selectedNode);
            }

            UpdateFiles(path, folders, files);
        }

        private void UpdateFiles(string path, IEnumerable<string> folders, IEnumerable<string> files)
        {
            ShiftToUiThread(
               delegate
               {
                   if (files != null)
                   {
                       listView1.BeginUpdate();

                       listView1.Items.Clear();

                       foreach (string folder in folders)
                       {
                           ListViewItem listViewItemTemp = new ListViewItem(folder);
                           listViewItemTemp.ImageIndex = 0;

                           listView1.Items.Add(listViewItemTemp);
                       }

                       foreach (string file in files)
                       {
                           string filePath = Utilities.PathCombine(path, file);

                           ListViewItem listViewItemTemp = new ListViewItem(file);
                           listViewItemTemp.ImageIndex = 1;

                           UInt64 fileSize = _iDeviceInterface.FileSize(filePath);
                           listViewItemTemp.SubItems.Add(Utilities.GetFileSize(fileSize));
                           //listViewItemTemp.SubItems.Add(GetFileType(lstTemp.ImageIndex));

                           listView1.Items.Add(listViewItemTemp);
                       }

                       listView1.EndUpdate();
                   }
               }
            );
        }

        private string GetPathFromNodeForDisplay(TreeNode node)
        {
            string result;

            if (node.FullPath.Equals(ROOT_NODE_NAME))
            {
                result = ROOT_PATH;
            }
            else
            {
                result = node.FullPath.Replace("\\", Constants.PATH_SEPARATOR.ToString()).Replace(ROOT_NODE_NAME, "");
            }

            return result;
        }

        private string GetPathFromNode(TreeNode node)
        {
            return node.FullPath.Replace("\\", "/").Replace(ROOT_NODE_NAME, "") + "/";
        }

        private void RefreshChildFoldersAsync(TreeNode node, bool forceRefresh, Callback callback)
        {
            Async(delegate { DisableInterface(); }, delegate { RefreshChildFolders(node, forceRefresh); }, delegate { EnableInterface(); if (callback != null) callback(); });
        }

        private void RefreshChildFoldersAsync(TreeNode node, bool forceRefresh)
        {
            RefreshChildFoldersAsync(node, forceRefresh, null);
        }

        private void Async(Callback before, Callback async, Callback after)
        {
            if (before != null)
            {
                before();
            }
            ThreadPool.QueueUserWorkItem(
                delegate(object o)
                {
                    async();

                    if (after != null)
                    {
                        ShiftToUiThread(after);
                    }
                }
            );
        }

        private void ShiftToUiThread(Callback callback)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { ShiftToUiThread(callback); }));
                return;
            }

            callback();
        }

        private void DisableInterface()
        {
            ShiftToUiThread(
                delegate
                {
                    treeView1.Enabled = false;
                    listView1.Enabled = false;
                }
            );
        }

        private void EnableInterface()
        {
            ShiftToUiThread(
                delegate
                {
                    treeView1.Enabled = true;
                    listView1.Enabled = true;
                }
            );
        }

        // TODO: IS THERE ANY REASON TO CONTINUE DOING THIS?  WHY NOT JUST REMOVE THE INVALID CHARACTERS?
        private string FixFilename(string filename)
        {
            string result = filename;
            char[] chars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < chars.Length; i++)
            {
                result = result.Replace(chars[i].ToString(), "%" + ((int)chars[i]).ToString("X"));
            }

            return result;
        }

        private string UnfixFilename(string filename)
        {
            string result = filename;
            char[] chars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < chars.Length; i++)
            {
                result = result.Replace("%" + ((int)chars[i]).ToString("X"), chars[i].ToString());
            }

            return result;
        }

        private void SaveDirectory(TreeNode node)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the destination folder";
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string sourcePath = GetPathFromNodeForDisplay(node);

                FileDialog fd = new FileDialog(_iDeviceInterface);
                fd.Show();
                fd.CopyRemoteSources(new string[] { sourcePath }, folderBrowserDialog.SelectedPath);
            }
        }

        private void CreateDirectory(TreeNode node)
        {
            string existingPath = GetPathFromNode(node);
            string folderResult = null;
            DialogResult dialogResult = InputBox.Show(string.Format("Create New Folder Under: {0}", existingPath), "Enter the name of the new folder:", ref folderResult);
            if (dialogResult == DialogResult.OK)
            {
                _iDeviceInterface.CreateDirectory(existingPath + folderResult);
                // update our tree
                AddFoldersToNode(_selectedNode, new List<Folder>() { new Folder(folderResult, null) });
                // TODO: CONSIDER ADDING THIS NODE IN THE CORRECT SPOT OR SORTING AFTER ADDING IT
                UpdateStatus("Directory created");
            }
        }

        private void RenameDirectory(TreeNode node)
        {
            string existingPath = GetPathFromNodeForDisplay(node);
            string existingfolderPath = existingPath.Substring(0, existingPath.LastIndexOf(Constants.PATH_SEPARATOR) + 1);
            string existingFolder = existingPath.Substring(existingPath.LastIndexOf(Constants.PATH_SEPARATOR) + 1, existingPath.Length - existingPath.LastIndexOf(Constants.PATH_SEPARATOR) - 1);
            string folderResult = null;
            DialogResult dialogResult = InputBox.Show(string.Format("Rename Folder: {0}", existingFolder), "Enter the new folder name:", ref folderResult);
            if (dialogResult == DialogResult.OK)
            {
                _iDeviceInterface.Rename(existingPath, existingfolderPath + folderResult);
                // update our tree
                RenameNode(node, folderResult);
                UpdateSelectedPath();
                UpdateStatus("Directory renamed");
            }
        }

        private void RenameNode(TreeNode node, string newName)
        {
            node.Text = newName;
        }

        private void DeleteDirectory(TreeNode node)
        {
            // TODO: USE the FILEDIALOG AND MAKE THIS ASYNC
            DialogResult dialogResult = MessageBox.Show(string.Format("Are you sure you want to delete folder '{0}'?", GetPathFromNodeForDisplay(node)), "Delete Folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                _iDeviceInterface.DeleteDirectory(GetPathFromNode(node), true);
                // no need to refresh as we know the folder is being removed, so just remove the node from the tree
                RemoveNodeFromNode(node.Parent, node);
                UpdateStatus("Directory deleted");
            }
        }

        private void DeleteFiles(string path, ListView.SelectedListViewItemCollection items)
        {
            string message = items.Count > 0 ? "Are you sure you want to delete the selected files?" : string.Format("Are you sure you want to delete file '{0}'?", items[0].Text);
            DialogResult dialogResult = MessageBox.Show(message, "Delete File(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    _iDeviceInterface.DeleteFile(Utilities.PathCombine(path, items[i].Text));
                }

                UpdateStatus("File(s) deleted");

                RefreshFilesAsync();
            }
        }

        private void RenameFile(string path, ListViewItem item)
        {
            string existingFilename = item.Text;
            string existingPath = Utilities.PathCombine(path, existingFilename);
            string fileResult = null;
            DialogResult dialogResult = InputBox.Show(string.Format("Rename File: {0}", existingFilename), "Enter the new file name:", ref fileResult);
            if (dialogResult == DialogResult.OK)
            {
                _iDeviceInterface.Rename(existingPath, Utilities.PathCombine(path, fileResult));
                // update our listview
                RenameItem(item, fileResult);
                UpdateStatus("File renamed");
            }
        }

        private void RenameItem(ListViewItem item, string newName)
        {
            item.Text = newName;
        }

        private void SaveFiles(string path, ListView.SelectedListViewItemCollection items)
        {
            if (items.Count == 1 && _iDeviceInterface.IsFile(Utilities.PathCombine(path, listView1.SelectedItems[0].Text)))
            {
                ListViewItem item = items[0];

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = string.Format("Save '{0}' As", item.Text);
                saveFileDialog.FileName = item.Text;
                DialogResult dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string source = Utilities.PathCombine(path, item.Text);
                    string destination = saveFileDialog.FileName;

                    FileDialog fd = new FileDialog(_iDeviceInterface);
                    fd.Show();
                    fd.CopyRemoteSources(new string[] { source }, destination, true);
                }
            }
            else // multiple items selected
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Select the destination folder";
                DialogResult dialogResult = folderBrowserDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string[] sources = new string[items.Count];

                    for (int i = 0; i < items.Count; i++)
                    {
                        sources[i] = Utilities.PathCombine(path, items[i].Text);
                    }

                    FileDialog fd = new FileDialog(_iDeviceInterface);
                    fd.Show();
                    fd.CopyRemoteSources(sources, folderBrowserDialog.SelectedPath);
                }
            }
        }

        private void SelectNode(string path)
        {
            TreeNode[] nodes = treeView1.Nodes.Find(ROOT_PATH, false);

            string[] pieces = path.Split(new char[] { Constants.PATH_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string piece in pieces)
            {
                if (nodes.Length > 0)
                {
                    TreeNode[] tempNodes = nodes[0].Nodes.Find(piece, false);
                    if (tempNodes.Length > 0)
                    {
                        nodes = tempNodes;
                        RefreshChildFolders(nodes[0], false);
                    }
                }
            }

            ShiftToUiThread(
                delegate
                {
                    if (nodes.Length > 0)
                    {
                        treeView1.SelectedNode = nodes[0];
                        nodes[0].Expand();
                        treeView1.Focus();
                    }
                }
            );
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _selectedNode = e.Node;
            RefreshFilesAsync(delegate { treeView1.SelectedNode = _selectedNode; treeView1.Focus(); });
            UpdateSelectedPath();
        }

        private void UpdateSelectedPath()
        {
            groupBox2.Text = "Files in: " + GetPathFromNodeForDisplay(_selectedNode);
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            RefreshChildFoldersAsync(e.Node, false);
        }

        private void listView1_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //ESC pressed
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;

                return;
            }

            //Drop!
            if (e.KeyState == 0)
            {
                _dataObj.SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 0);

                e.Action = DragAction.Drop;

                return;
            }

            e.Action = DragAction.Continue;
        }

        //private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        //{
        //    // create an array of strings to hold the filename(s)
        //    string[] fileNames = new string[listView1.SelectedItems.Count];
        //    Dictionary<string, string> mapping = new Dictionary<string, string>();

        //    string path = GetPathFromNode(treeView1.SelectedNode);

        //    // create temporary files that the user can drag.
        //    for (int i = 0; i < listView1.SelectedItems.Count; i++)
        //    {
        //        fileNames[i] = System.IO.Path.Combine(System.IO.Path.GetTempPath(), listView1.SelectedItems[i].Text);
        //        mapping.Add(fileNames[i], PathCombine(path, listView1.SelectedItems[i].Text));
        //    }

        //    // create the data object used for the drag drop operation
        //    _dataObj = new ShellDataObject(_iDeviceInterface, mapping);
        //    _dataObj.StatusUpdate += delegate(string s) { Async(null, delegate { UpdateStatus(s); }, null); };

        //    // add the list of files to the data object
        //    _dataObj.SetData(DataFormats.FileDrop, fileNames);

        //    // set the preferred drop effect
        //    _dataObj.SetData(ShellClipboardFormats.CFSTR_PREFERREDDROPEFFECT, DragDropEffects.Move);

        //    // indicate that we are in a drag loop
        //    _dataObj.SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 1);

        //    // initiate the drag operation
        //    listView1.DoDragDrop(_dataObj, DragDropEffects.Move | DragDropEffects.Copy);

        //    // free the data object
        //    _dataObj = null;
        //}

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            int count = listView1.SelectedItems.Count;

            string path = GetPathFromNode(_selectedNode);

            VirtualFileDataObject.FileDescriptor[] files = new VirtualFileDataObject.FileDescriptor[count];
            for (int i = 0; i < count; i++)
            {
                string source = Utilities.PathCombine(path, listView1.SelectedItems[i].Text);

                files[i] = new VirtualFileDataObject.FileDescriptor
                {
                    Name = listView1.SelectedItems[i].Text,
                    //Length = 26,
                    //ChangeTimeUtc = DateTime.Now.AddDays(-1),
                    StreamContents = stream =>
                    {
                        Utilities.Copy(iPhoneFile.OpenRead(_iDeviceInterface, source), stream, (bytes) => { });
                    }
                };
            }

            VirtualFileDataObject virtualFileDataObject = new VirtualFileDataObject(
                delegate(VirtualFileDataObject vdo) { DisableInterface(); UpdateStatus("Copying files to local machine..."); },
                delegate(VirtualFileDataObject vdo) { EnableInterface(); UpdateStatus("Done"); }
            );
            virtualFileDataObject.SetData(files);

            VirtualFileDataObject.DoDragDrop(virtualFileDataObject, DragDropEffects.Move | DragDropEffects.Copy);
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            // TODO: LOCK DOWN THE INTERFACE WHILE A FILE COPY IS IN PROGRESS
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string destination = GetPathFromNode(_selectedNode);
                string[] sources = e.Data.GetData(DataFormats.FileDrop) as string[];

                if (sources != null)
                {
                    FileDialog fd = new FileDialog(_iDeviceInterface);
                    fd.Show();
                    fd.CopyLocalSources(sources, destination);
                }
            }
        }

        private void folderContextMenu_Opening(object sender, CancelEventArgs e)
        {
            bool isRoot = GetPathFromNode(_selectedNode).Equals(ROOT_PATH);

            renameFolder.Enabled = !isRoot;
            deleteFolder.Enabled = !isRoot;
        }

        private void folderContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;
            if (cms != null)
            {
                cms.Close();
            }

            TreeNode node = _selectedNode;

            if (e.ClickedItem == refreshFolder)
            {
                RefreshChildFoldersAsync(node, true);
            }
            else if (e.ClickedItem == newFolder)
            {
                CreateDirectory(node);
            }
            else if (e.ClickedItem == saveFolder)
            {
                SaveDirectory(node);
            }
            else if (e.ClickedItem == renameFolder)
            {
                RenameDirectory(node);
            }
            else if (e.ClickedItem == deleteFolder)
            {
                DeleteDirectory(node);
            }
            else // default
            {
                if (!(e.ClickedItem is ToolStripSeparator))
                {
                    throw new NotImplementedException(string.Format("Context menu '{0}' not implemented", e.ClickedItem.Name));
                }
            }
        }

        private void fileContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;
            if (cms != null)
            {
                cms.Close();
            }

            TreeNode node = _selectedNode;
            string path = GetPathFromNodeForDisplay(node);

            if (e.ClickedItem == refreshFile)
            {
                RefreshFilesAsync(delegate { UpdateStatus("Refresh complete"); });
            }
            else if (e.ClickedItem == saveFile)
            {
                SaveFiles(path, listView1.SelectedItems);
            }
            else if (e.ClickedItem == saveAsFile)
            {
                SaveFiles(path, listView1.SelectedItems);
            }
            else if (e.ClickedItem == renameFile)
            {
                RenameFile(path, listView1.SelectedItems[0]);
            }
            else if (e.ClickedItem == replaceFile)
            {
                throw new NotImplementedException(string.Format("Context menu '{0}' not implemented", e.ClickedItem.Name));
            }
            else if (e.ClickedItem == deleteFile)
            {
                DeleteFiles(path, listView1.SelectedItems);
            }
            else // default
            {
                if (!(e.ClickedItem is ToolStripSeparator))
                {
                    throw new NotImplementedException(string.Format("Context menu '{0}' not implemented", e.ClickedItem.Name));
                }
            }
        }

        private void fileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            int count = listView1.SelectedItems.Count;

            saveFile.Enabled = count > 0;
            saveAsFile.Enabled = count == 1 && _iDeviceInterface.IsFile(Utilities.PathCombine(GetPathFromNodeForDisplay(_selectedNode), listView1.SelectedItems[0].Text));
            renameFile.Enabled = count == 1;
            replaceFile.Enabled = count == 1;
            deleteFile.Enabled = count > 0;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1 && _iDeviceInterface.IsDirectory(Utilities.PathCombine(GetPathFromNodeForDisplay(_selectedNode), listView1.SelectedItems[0].Text)))
            {
                SelectNode(Utilities.PathCombine(GetPathFromNodeForDisplay(_selectedNode), listView1.SelectedItems[0].Text));
            }
        }
    }
}
