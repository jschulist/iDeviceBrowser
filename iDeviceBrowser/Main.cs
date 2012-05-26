using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

        // TODO: CONSIDER ADDING PREVIEW BACK IN, TEXT, IMAGE, PLISTS, ETC.

        // TODO: ADD SUPPORT TO CONTEXT MENUS SO THAT ANY FOLDER CAN BE ADDED AS A FAVORITE

        // TODO: CONSIDER CHANING MANZANA GENERIC EXCEPTIONS TO TYPED EXCEPTIONS SO THEY CAN BE HANDLED GRACEFULLY

        // TODO: CONSIDER ADDING AUTOCOMPLETE TO THE PATH TEXTBOX

        private readonly iPhone _iDeviceInterface = new iPhone();
        private bool _isConnected;
        private TreeNode _selectedNode;
        private UserSettings _userSettings = new UserSettings();

        private const string ROOT_NODE_NAME = "/ [root]";
        private const string ROOT_PATH = "/";

        public delegate void Callback();

        public Main()
        {
            InitializeComponent();
        }

        private void PopulateFavoritesDropDown()
        {
            FavoritesToolStripMenuItem.DropDownItems.Clear();

            FavoritesToolStripMenuItem.DropDownItems.Add(FavoritesToolStripMenuItem_EditToolStripMenuItem);
            FavoritesToolStripMenuItem.DropDownItems.Add(toolStripSeparator2);

            foreach (Favorite favorite in _userSettings.Favorites)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(favorite.Name);
                tsmi.Tag = favorite.Path;
                tsmi.Click += new EventHandler(FavoritesToolStripMenuItem_Click);
                FavoritesToolStripMenuItem.DropDownItems.Add(tsmi);
            }
        }

        private void SetConnectionChanged()
        {
            _isConnected = !_isConnected;

            if (_isConnected)
            {
                MainStatusStrip_StatusToolStripStatusLabel.Text = "Connected";

                PopulateInitialFolders();
            }
            else
            {
                MainStatusStrip_StatusToolStripStatusLabel.Text = "Disconnected";

                ResetUserInterface();
            }

            ShiftToUiThread(UpdateTitle);
        }

        private void ResetUserInterface()
        {
            ShiftToUiThread(
                delegate
                {
                    FolderTreeView.Nodes.Clear();
                    FolderAndFileListView.Items.Clear();
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
                () =>
                {
                    FolderTreeView.BeginUpdate();

                    FolderTreeView.Nodes.Add(rootNode);
                    FolderTreeView.ContextMenuStrip = FolderContextMenu;

                    rootNode.Expand();
                    rootNode.Tag = true;
                    FolderTreeView.SelectedNode = rootNode;

                    FolderTreeView.EndUpdate();
                }
                );

            RefreshChildFoldersAsync(rootNode, true, rootNode.Expand);
        }

        // TODO: CONSIDER DEFERING EAGER FOLDER FETCHING, OR PERFORM IT IN THE BACKGROUND, WHICH WOULD PREVENT US FROM NEEDING TO DISABLE THE CONTROLS WHILE THIS IS TAKING PLACE
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
                    MainStatusStrip_StatusToolStripStatusLabel.Text = message;
                }
            );
        }

        private void RemoveNodeFromNode(TreeNode parent, TreeNode child)
        {
            ShiftToUiThread(
                delegate
                {
                    if (parent != null)
                    {
                        FolderTreeView.BeginUpdate();
                        parent.Nodes.Remove(child);
                        FolderTreeView.EndUpdate();
                    }
                }
            );
        }

        private void UpdateChildFolders(IEnumerable<NodeAndFolders> nodeAndFolders)
        {
            if (Enumerable.Count(nodeAndFolders) > 0)
            {
                ShiftToUiThread(
                    delegate
                    {
                        FolderTreeView.BeginUpdate();

                        foreach (NodeAndFolders naf in nodeAndFolders)
                        {
                            naf.Node.Nodes.Clear();
                            AddFoldersToNode(naf.Node, naf.Folders);
                        }

                        FolderTreeView.EndUpdate();
                    }
                );
            }
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

        private void RefreshListViewAsync(Callback callback)
        {
            Async(
                DisableInterface,
                RefreshListView,
                () =>
                {
                    EnableInterface();
                    if (callback != null)
                    {
                        callback();
                    }
                });
        }

        private void RefreshListView()
        {
            string path = GetPathFromNode(_selectedNode);

            IEnumerable<string> folders = null;
            IEnumerable<string> files = null;
            try
            {
                folders = GetFoldersAndFiles(path, out files);
            }
            catch (Exception ex) // most likely someone removed the folder
            {
                RemoveNodeFromNode(_selectedNode.Parent, _selectedNode);
            }

            UpdateListView(path, folders, files);
        }

        private IEnumerable<string> GetFoldersAndFiles(string path, out IEnumerable<string> files)
        {
            IEnumerable<string> folders = _iDeviceInterface.GetDirectories(path);
            files = _iDeviceInterface.GetFiles(path);

            return folders;
        }

        private void UpdateListView(string path, IEnumerable<string> folders, IEnumerable<string> files)
        {
            ShiftToUiThread(
                () =>
                {
                    if (files != null)
                    {
                        FolderAndFileListView.BeginUpdate();

                        FolderAndFileListView.Items.Clear();

                        foreach (string folder in folders)
                        {
                            AddFolderToListView(folder);
                        }

                        foreach (string file in files)
                        {
                            string filePath = Utilities.PathCombine(path, file);

                            ListViewItem listViewItemTemp = new ListViewItem(file);
                            listViewItemTemp.ImageIndex = 1;

                            UInt64 fileSize = _iDeviceInterface.FileSize(filePath);
                            listViewItemTemp.SubItems.Add(Utilities.GetFileSize(fileSize));
                            //listViewItemTemp.SubItems.Add(GetFileType(lstTemp.ImageIndex));

                            FolderAndFileListView.Items.Add(listViewItemTemp);
                        }

                        FolderAndFileListView.EndUpdate();
                    }
                });
        }

        private void AddFolderToListView(string folder)
        {
            ListViewItem listViewItemTemp = new ListViewItem(folder);
            listViewItemTemp.ImageIndex = 0;

            FolderAndFileListView.Items.Add(listViewItemTemp);
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
            Async(
                DisableInterface,
                () => RefreshChildFolders(node, forceRefresh),
                () =>
                {
                    EnableInterface();
                    if (callback != null)
                    {
                        callback();
                    }
                });
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
                this.Invoke(new MethodInvoker(() => ShiftToUiThread(callback)));
                return;
            }

            callback();
        }

        private void DisableInterface()
        {
            ShiftToUiThread(
                delegate
                {
                    FolderTreeView.Enabled = false;
                    FolderAndFileListView.Enabled = false;
                }
            );
        }

        private void EnableInterface()
        {
            ShiftToUiThread(
                delegate
                {
                    FolderTreeView.Enabled = true;
                    FolderAndFileListView.Enabled = true;
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
            // TODO: VALIDATE RESULT TO ENSURE IT'S A VALID FOLDER
            DialogResult dialogResult = InputBox.Show(string.Format("Create New Folder Under: {0}", existingPath), "Enter the name of the new folder:", ref folderResult);
            if (dialogResult == DialogResult.OK)
            {
                _iDeviceInterface.CreateDirectory(existingPath + folderResult);
                // update our tree and listview
                // TODO: CONSIDER ADDING THIS NODE IN THE CORRECT SPOT OR SORTING AFTER ADDING IT
                AddFoldersToNode(_selectedNode, new List<Folder>() { new Folder(folderResult, null) });
                AddFolderToListView(folderResult);
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
                Async(
                    null,
                    () => Delete(new string[] { GetPathFromNode(node) }),
                    () =>
                    {
                        // no need to refresh as we know the folder is being removed, so just remove the node from the tree
                        RemoveNodeFromNode(node.Parent, node);
                        UpdateStatus("Directory deleted");
                    }
                );
            }
        }

        private void DeleteFiles(string path, ListView.SelectedListViewItemCollection items)
        {
            string message = items.Count > 1 ? "Are you sure you want to delete the selected files?" : string.Format("Are you sure you want to delete file '{0}'?", items[0].Text);
            DialogResult dialogResult = MessageBox.Show(message, "Delete File(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                List<string> foldersOrFiles = new List<string>();
                for (int i = 0; i < items.Count; i++)
                {
                    foldersOrFiles.Add(Utilities.PathCombine(path, items[i].Text));
                }

                Async(
                    null,
                    () => Delete(foldersOrFiles),
                    () =>
                    {
                        UpdateStatus("File(s) deleted");
                        // TODO: REMOVE FOLDERS FROM TREEVIEW THAT WERE DELETED
                        RefreshListViewAsync(null);
                    }
                );
            }
        }

        private void Delete(IEnumerable<string> foldersAndFiles)
        {
            foreach (string folderOrFile in foldersAndFiles)
            {
                if (_iDeviceInterface.IsDirectory(folderOrFile))
                {
                    _iDeviceInterface.DeleteDirectory(folderOrFile, true);
                }
                else
                {
                    _iDeviceInterface.DeleteFile(folderOrFile);
                }
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
            if (items.Count == 1 && _iDeviceInterface.IsFile(Utilities.PathCombine(path, FolderAndFileListView.SelectedItems[0].Text)))
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
            TreeNode[] nodes = FolderTreeView.Nodes.Find(ROOT_PATH, false);

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
                        FolderTreeView.SelectedNode = nodes[0];
                        nodes[0].Expand();
                        FolderTreeView.Focus();
                    }
                }
            );
        }

        private void SelectNodeAsync(string path)
        {
            Async(DisableInterface, delegate { SelectNode(path); }, EnableInterface);
        }

        private void UpdateSelectedPath()
        {
            PathTextBox.Text = GetPathFromNode(_selectedNode);
        }

        private void ShowExceptionDialog(Exception exception)
        {
            this.Hide();
            ExceptionDialog exceptionDialog = new ExceptionDialog();
            exceptionDialog.Exception = exception;
            exceptionDialog.ShowDialog();
            this.Close();
        }

        #region Events
        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowExceptionDialog(e.Exception);
        }

        private void Connection_Changed(object sender, ConnectEventArgs args)
        {
            SetConnectionChanged();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowExceptionDialog(e.ExceptionObject as Exception);
        }

        private void FavoritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            string path = tsmi.Tag.ToString();

            SelectNodeAsync(path);
        }

        private void FavoritesToolStripMenuItem_EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoritesDialog fd = new FavoritesDialog();
            fd.Favorites = _userSettings.Favorites;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                _userSettings.Favorites = fd.Favorites;
                _userSettings.Save();
                PopulateFavoritesDropDown();
            }
        }

        private void FileToolStripMenuItem_ExitExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FolderAndFileContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;
            if (cms != null)
            {
                cms.Close();
            }

            TreeNode node = _selectedNode;
            string path = GetPathFromNodeForDisplay(node);

            if (e.ClickedItem == FolderAndFileContextMenu_RefreshToolStripMenuItem)
            {
                RefreshListViewAsync(() => UpdateStatus("Refresh complete"));
            }
            else if (e.ClickedItem == FolderAndFileContextMenu_NewFolderToolStripMenuItem)
            {
                CreateDirectory(node);
            }
            else if (e.ClickedItem == FolderAndFileContextMenu_SaveToolStripMenuItem)
            {
                SaveFiles(path, FolderAndFileListView.SelectedItems);
            }
            else if (e.ClickedItem == FolderAndFileContextMenu_SaveAsToolStripMenuItem)
            {
                SaveFiles(path, FolderAndFileListView.SelectedItems);
            }
            else if (e.ClickedItem == FolderAndFileContextMenu_RenameToolStripMenuItem)
            {
                RenameFile(path, FolderAndFileListView.SelectedItems[0]);
            }
            else if (e.ClickedItem == FolderAndFileContextMenu_ReplaceToolStripMenuItem)
            {
                throw new NotImplementedException(string.Format("Context menu '{0}' not implemented", e.ClickedItem.Name));
            }
            else if (e.ClickedItem == FolderAndFileContextMenu_DeleteToolStripMenuItem)
            {
                DeleteFiles(path, FolderAndFileListView.SelectedItems);
            }
            else // default
            {
                if (!(e.ClickedItem is ToolStripSeparator))
                {
                    throw new NotImplementedException(string.Format("Context menu '{0}' not implemented", e.ClickedItem.Name));
                }
            }
        }

        private void FolderAndFileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            int count = FolderAndFileListView.SelectedItems.Count;

            FolderAndFileContextMenu_SaveToolStripMenuItem.Enabled = count > 0;
            FolderAndFileContextMenu_SaveAsToolStripMenuItem.Enabled = count == 1 && _iDeviceInterface.IsFile(Utilities.PathCombine(GetPathFromNodeForDisplay(_selectedNode), FolderAndFileListView.SelectedItems[0].Text));
            FolderAndFileContextMenu_RenameToolStripMenuItem.Enabled = count == 1;
            FolderAndFileContextMenu_ReplaceToolStripMenuItem.Enabled = count == 1;
            FolderAndFileContextMenu_DeleteToolStripMenuItem.Enabled = count > 0;
        }

        private void FolderAndFileListView_DragDrop(object sender, DragEventArgs e)
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

        private void FolderAndFileListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        // TODO: ADD SUPPORT FOR TREEVIEW DRAGGING, EXTRACT THE BULK OF THIS LOGIC INTO A SEPARATE METHOD THAT TAKES A LIST OF FILES
        private void FolderAndFileListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            int count = FolderAndFileListView.SelectedItems.Count;

            string path = GetPathFromNode(_selectedNode);

            //FileDialog fd = new FileDialog(_iDeviceInterface);
            //fd.SyncContext = SynchronizationContext.Current;

            VirtualFileDataObject.FileDescriptor[] files = new VirtualFileDataObject.FileDescriptor[count];
            for (int i = 0; i < count; i++)
            {
                string source = Utilities.PathCombine(path, FolderAndFileListView.SelectedItems[i].Text);

                files[i] = new VirtualFileDataObject.FileDescriptor
                {
                    Name = FolderAndFileListView.SelectedItems[i].Text,
                    Length = (long)_iDeviceInterface.FileSize(source),
                    StreamContents = destinationStream =>
                    {
                        //ShiftToUiThread(fd.Show);
                        //fd.ShowAsync();
                        using (Stream sourceStream = iPhoneFile.OpenRead(_iDeviceInterface, source))
                        {
                            Utilities.Copy(sourceStream, destinationStream);
                        }
                        //fd.Hide();
                    }
                };
            }

            VirtualFileDataObject virtualFileDataObject = new VirtualFileDataObject(
                vdo =>
                {
                    //DisableInterface();
                    //UpdateStatus("Copying files to local machine...");
                },
                vdo =>
                {
                    //EnableInterface();
                    //UpdateStatus("Done");
                }
            );
            virtualFileDataObject.SetData(files);

            VirtualFileDataObject.DoDragDrop(virtualFileDataObject, DragDropEffects.Copy);
        }

        private void FolderAndFileListView_KeyDown(object sender, KeyEventArgs e)
        {
            // select all
            if (e.Control && e.KeyCode == Keys.A)
            {
                //listView1.MultiSelect = true;
                foreach (ListViewItem item in FolderAndFileListView.Items)
                {
                    item.Selected = true;
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                string path = GetPathFromNodeForDisplay(_selectedNode);
                DeleteFiles(path, FolderAndFileListView.SelectedItems);
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                SelectNodeFromFolderAndFileListView();
            }
        }

        private void FolderAndFileListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectNodeFromFolderAndFileListView();
        }

        private void SelectNodeFromFolderAndFileListView()
        {
            if (FolderAndFileListView.SelectedItems.Count == 1 && _iDeviceInterface.IsDirectory(Utilities.PathCombine(GetPathFromNodeForDisplay(_selectedNode), FolderAndFileListView.SelectedItems[0].Text)))
            {
                SelectNodeAsync(Utilities.PathCombine(GetPathFromNodeForDisplay(_selectedNode), FolderAndFileListView.SelectedItems[0].Text));
            }
        }

        private void FolderContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;
            if (cms != null)
            {
                cms.Close();
            }

            TreeNode node = _selectedNode;

            if (e.ClickedItem == FolderContextMenu_RefreshToolStripMenuItem)
            {
                RefreshChildFoldersAsync(node, true);
            }
            else if (e.ClickedItem == FolderContextMenu_NewFolderToolStripMenuItem)
            {
                CreateDirectory(node);
            }
            else if (e.ClickedItem == FolderContextMenu_SaveToolStripMenuItem)
            {
                SaveDirectory(node);
            }
            else if (e.ClickedItem == FolderContextMenu_RenameToolStripMenuItem)
            {
                RenameDirectory(node);
            }
            else if (e.ClickedItem == FolderContextMenu_DeleteToolStripMenuItem)
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

        private void FolderContextMenu_Opening(object sender, CancelEventArgs e)
        {
            bool isRoot = GetPathFromNode(_selectedNode).Equals(ROOT_PATH);

            FolderContextMenu_RenameToolStripMenuItem.Enabled = !isRoot;
            FolderContextMenu_DeleteToolStripMenuItem.Enabled = !isRoot;
        }

        private void FolderTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            RefreshChildFoldersAsync(e.Node, false);
        }

        private void FolderTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _selectedNode = e.Node;
            RefreshListViewAsync(delegate { FolderTreeView.SelectedNode = _selectedNode; FolderTreeView.Focus(); });
            UpdateSelectedPath();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            _userSettings.Setup();

            UpdateTitle();

            _iDeviceInterface.Connect += new ConnectEventHandler(Connection_Changed);
            _iDeviceInterface.Disconnect += new ConnectEventHandler(Connection_Changed);

            PopulateFavoritesDropDown();
        }

        private void MainMenuStrip_ViewToolStripMenuItem_PreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewToolStripMenuItem_PreviewToolStripMenuItem.Checked = !ViewToolStripMenuItem_PreviewToolStripMenuItem.Checked;
            PreviewSplitContainer.Panel2Collapsed = !ViewToolStripMenuItem_PreviewToolStripMenuItem.Checked;
        }

        private void PathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                SelectNodeAsync(PathTextBox.Text);
            }
        }

        private void PathTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
            }
        }
        #endregion Events
    }
}
