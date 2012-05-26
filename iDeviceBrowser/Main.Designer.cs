namespace iDeviceBrowser
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.FolderTreeView = new System.Windows.Forms.TreeView();
            this.FolderAndFileImageList = new System.Windows.Forms.ImageList(this.components);
            this.PreviewSplitContainer = new System.Windows.Forms.SplitContainer();
            this.FolderAndFileListView = new System.Windows.Forms.ListView();
            this.NameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SizeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FolderAndFileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FolderAndFileContextMenu_RefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.FolderAndFileContextMenu_NewFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.FolderAndFileContextMenu_SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderAndFileContextMenu_SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.FolderAndFileContextMenu_RenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderAndFileContextMenu_ReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderAndFileContextMenu_DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PreviewGroupBox = new System.Windows.Forms.GroupBox();
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.MainStatusStrip_StatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainStatusStrip_ProgressToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.MainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileToolStripMenuItem_ExitExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FavoritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FavoritesToolStripMenuItem_EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.PathTextBox = new System.Windows.Forms.TextBox();
            this.FolderContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FolderContextMenu_RefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.FolderContextMenu_NewFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.FolderContextMenu_SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.FolderContextMenu_RenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderContextMenu_DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderContextMenu_AddAsFavoriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.PreviewSplitContainer.Panel1.SuspendLayout();
            this.PreviewSplitContainer.Panel2.SuspendLayout();
            this.PreviewSplitContainer.SuspendLayout();
            this.FolderAndFileContextMenu.SuspendLayout();
            this.MainStatusStrip.SuspendLayout();
            this.MainMenuStrip.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.FolderContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainSplitContainer.Location = new System.Drawing.Point(0, 21);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.FolderTreeView);
            this.MainSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.PreviewSplitContainer);
            this.MainSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.MainSplitContainer.Size = new System.Drawing.Size(701, 390);
            this.MainSplitContainer.SplitterDistance = 231;
            this.MainSplitContainer.SplitterWidth = 6;
            this.MainSplitContainer.TabIndex = 1;
            // 
            // FolderTreeView
            // 
            this.FolderTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FolderTreeView.ImageIndex = 0;
            this.FolderTreeView.ImageList = this.FolderAndFileImageList;
            this.FolderTreeView.Location = new System.Drawing.Point(3, 0);
            this.FolderTreeView.Name = "FolderTreeView";
            this.FolderTreeView.PathSeparator = "/";
            this.FolderTreeView.SelectedImageIndex = 0;
            this.FolderTreeView.ShowRootLines = false;
            this.FolderTreeView.Size = new System.Drawing.Size(228, 390);
            this.FolderTreeView.TabIndex = 0;
            this.FolderTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.FolderTreeView_AfterExpand);
            this.FolderTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.FolderTreeView_AfterSelect);
            // 
            // FolderAndFileImageList
            // 
            this.FolderAndFileImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("FolderAndFileImageList.ImageStream")));
            this.FolderAndFileImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.FolderAndFileImageList.Images.SetKeyName(0, "folder.ico");
            this.FolderAndFileImageList.Images.SetKeyName(1, "file.ico");
            // 
            // PreviewSplitContainer
            // 
            this.PreviewSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.PreviewSplitContainer.Name = "PreviewSplitContainer";
            this.PreviewSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // PreviewSplitContainer.Panel1
            // 
            this.PreviewSplitContainer.Panel1.Controls.Add(this.FolderAndFileListView);
            // 
            // PreviewSplitContainer.Panel2
            // 
            this.PreviewSplitContainer.Panel2.Controls.Add(this.PreviewGroupBox);
            this.PreviewSplitContainer.Size = new System.Drawing.Size(461, 390);
            this.PreviewSplitContainer.SplitterDistance = 188;
            this.PreviewSplitContainer.TabIndex = 0;
            // 
            // FolderAndFileListView
            // 
            this.FolderAndFileListView.AllowColumnReorder = true;
            this.FolderAndFileListView.AllowDrop = true;
            this.FolderAndFileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumnHeader,
            this.SizeColumnHeader,
            this.TypeColumnHeader});
            this.FolderAndFileListView.ContextMenuStrip = this.FolderAndFileContextMenu;
            this.FolderAndFileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FolderAndFileListView.Location = new System.Drawing.Point(0, 0);
            this.FolderAndFileListView.Name = "FolderAndFileListView";
            this.FolderAndFileListView.Size = new System.Drawing.Size(461, 188);
            this.FolderAndFileListView.SmallImageList = this.FolderAndFileImageList;
            this.FolderAndFileListView.TabIndex = 0;
            this.FolderAndFileListView.UseCompatibleStateImageBehavior = false;
            this.FolderAndFileListView.View = System.Windows.Forms.View.Details;
            this.FolderAndFileListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.FolderAndFileListView_ItemDrag);
            this.FolderAndFileListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.FolderAndFileListView_DragDrop);
            this.FolderAndFileListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.FolderAndFileListView_DragEnter);
            this.FolderAndFileListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FolderAndFileListView_KeyDown);
            this.FolderAndFileListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FolderAndFileListView_MouseDoubleClick);
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Name";
            this.NameColumnHeader.Width = 250;
            // 
            // SizeColumnHeader
            // 
            this.SizeColumnHeader.Text = "Size";
            this.SizeColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.SizeColumnHeader.Width = 80;
            // 
            // TypeColumnHeader
            // 
            this.TypeColumnHeader.Text = "Type";
            this.TypeColumnHeader.Width = 80;
            // 
            // FolderAndFileContextMenu
            // 
            this.FolderAndFileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FolderAndFileContextMenu_RefreshToolStripMenuItem,
            this.FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem,
            this.toolStripSeparator12,
            this.FolderAndFileContextMenu_NewFolderToolStripMenuItem,
            this.toolStripSeparator1,
            this.FolderAndFileContextMenu_SaveToolStripMenuItem,
            this.FolderAndFileContextMenu_SaveAsToolStripMenuItem,
            this.toolStripSeparator13,
            this.FolderAndFileContextMenu_RenameToolStripMenuItem,
            this.FolderAndFileContextMenu_ReplaceToolStripMenuItem,
            this.FolderAndFileContextMenu_DeleteToolStripMenuItem});
            this.FolderAndFileContextMenu.Name = "fileContextMenu";
            this.FolderAndFileContextMenu.Size = new System.Drawing.Size(156, 220);
            this.FolderAndFileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FolderAndFileContextMenu_Opening);
            this.FolderAndFileContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.FolderAndFileContextMenu_ItemClicked);
            // 
            // FolderAndFileContextMenu_RefreshToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_RefreshToolStripMenuItem.Name = "FolderAndFileContextMenu_RefreshToolStripMenuItem";
            this.FolderAndFileContextMenu_RefreshToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_RefreshToolStripMenuItem.Text = "Refresh";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(152, 6);
            // 
            // FolderAndFileContextMenu_NewFolderToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_NewFolderToolStripMenuItem.Name = "FolderAndFileContextMenu_NewFolderToolStripMenuItem";
            this.FolderAndFileContextMenu_NewFolderToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_NewFolderToolStripMenuItem.Text = "New Folder";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // FolderAndFileContextMenu_SaveToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_SaveToolStripMenuItem.Name = "FolderAndFileContextMenu_SaveToolStripMenuItem";
            this.FolderAndFileContextMenu_SaveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_SaveToolStripMenuItem.Text = "Save...";
            // 
            // FolderAndFileContextMenu_SaveAsToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_SaveAsToolStripMenuItem.Name = "FolderAndFileContextMenu_SaveAsToolStripMenuItem";
            this.FolderAndFileContextMenu_SaveAsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_SaveAsToolStripMenuItem.Text = "Save As...";
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(152, 6);
            // 
            // FolderAndFileContextMenu_RenameToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_RenameToolStripMenuItem.Name = "FolderAndFileContextMenu_RenameToolStripMenuItem";
            this.FolderAndFileContextMenu_RenameToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_RenameToolStripMenuItem.Text = "Rename";
            // 
            // FolderAndFileContextMenu_ReplaceToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_ReplaceToolStripMenuItem.Name = "FolderAndFileContextMenu_ReplaceToolStripMenuItem";
            this.FolderAndFileContextMenu_ReplaceToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_ReplaceToolStripMenuItem.Text = "Replace...";
            // 
            // FolderAndFileContextMenu_DeleteToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_DeleteToolStripMenuItem.Name = "FolderAndFileContextMenu_DeleteToolStripMenuItem";
            this.FolderAndFileContextMenu_DeleteToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_DeleteToolStripMenuItem.Text = "Delete";
            // 
            // PreviewGroupBox
            // 
            this.PreviewGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewGroupBox.Location = new System.Drawing.Point(0, 0);
            this.PreviewGroupBox.Name = "PreviewGroupBox";
            this.PreviewGroupBox.Size = new System.Drawing.Size(461, 198);
            this.PreviewGroupBox.TabIndex = 0;
            this.PreviewGroupBox.TabStop = false;
            this.PreviewGroupBox.Text = "Preview";
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainStatusStrip_StatusToolStripStatusLabel,
            this.MainStatusStrip_ProgressToolStripProgressBar});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 436);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(701, 22);
            this.MainStatusStrip.TabIndex = 2;
            this.MainStatusStrip.Text = "statusStrip1";
            // 
            // MainStatusStrip_StatusToolStripStatusLabel
            // 
            this.MainStatusStrip_StatusToolStripStatusLabel.Name = "MainStatusStrip_StatusToolStripStatusLabel";
            this.MainStatusStrip_StatusToolStripStatusLabel.Size = new System.Drawing.Size(584, 17);
            this.MainStatusStrip_StatusToolStripStatusLabel.Spring = true;
            this.MainStatusStrip_StatusToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainStatusStrip_ProgressToolStripProgressBar
            // 
            this.MainStatusStrip_ProgressToolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MainStatusStrip_ProgressToolStripProgressBar.Name = "MainStatusStrip_ProgressToolStripProgressBar";
            this.MainStatusStrip_ProgressToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            this.MainStatusStrip_ProgressToolStripProgressBar.Step = 1;
            // 
            // MainMenuStrip
            // 
            this.MainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.ViewToolStripMenuItem,
            this.FavoritesToolStripMenuItem});
            this.MainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip.Name = "MainMenuStrip";
            this.MainMenuStrip.Size = new System.Drawing.Size(701, 24);
            this.MainMenuStrip.TabIndex = 0;
            this.MainMenuStrip.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem_ExitExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "&File";
            // 
            // FileToolStripMenuItem_ExitExitToolStripMenuItem
            // 
            this.FileToolStripMenuItem_ExitExitToolStripMenuItem.Name = "FileToolStripMenuItem_ExitExitToolStripMenuItem";
            this.FileToolStripMenuItem_ExitExitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.FileToolStripMenuItem_ExitExitToolStripMenuItem.Text = "Exit";
            this.FileToolStripMenuItem_ExitExitToolStripMenuItem.Click += new System.EventHandler(this.FileToolStripMenuItem_ExitExitToolStripMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.EditToolStripMenuItem.Text = "&Edit";
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "&View";
            // 
            // ViewToolStripMenuItem_PreviewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem.Checked = true;
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem.Name = "ViewToolStripMenuItem_PreviewToolStripMenuItem";
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem.Text = "Preview";
            this.ViewToolStripMenuItem_PreviewToolStripMenuItem.Click += new System.EventHandler(this.MainMenuStrip_ViewToolStripMenuItem_PreviewToolStripMenuItem_Click);
            // 
            // FavoritesToolStripMenuItem
            // 
            this.FavoritesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FavoritesToolStripMenuItem_EditToolStripMenuItem,
            this.toolStripSeparator2});
            this.FavoritesToolStripMenuItem.Name = "FavoritesToolStripMenuItem";
            this.FavoritesToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.FavoritesToolStripMenuItem.Text = "&Favorites";
            // 
            // FavoritesToolStripMenuItem_EditToolStripMenuItem
            // 
            this.FavoritesToolStripMenuItem_EditToolStripMenuItem.Name = "FavoritesToolStripMenuItem_EditToolStripMenuItem";
            this.FavoritesToolStripMenuItem_EditToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.FavoritesToolStripMenuItem_EditToolStripMenuItem.Text = "Edit";
            this.FavoritesToolStripMenuItem_EditToolStripMenuItem.Click += new System.EventHandler(this.FavoritesToolStripMenuItem_EditToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(91, 6);
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.PathTextBox);
            this.MainPanel.Controls.Add(this.MainSplitContainer);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 24);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(701, 412);
            this.MainPanel.TabIndex = 1;
            // 
            // PathTextBox
            // 
            this.PathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PathTextBox.Location = new System.Drawing.Point(3, 0);
            this.PathTextBox.Name = "PathTextBox";
            this.PathTextBox.Size = new System.Drawing.Size(695, 20);
            this.PathTextBox.TabIndex = 0;
            this.PathTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PathTextBox_KeyDown);
            this.PathTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PathTextBox_KeyPress);
            // 
            // FolderContextMenu
            // 
            this.FolderContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FolderContextMenu_RefreshToolStripMenuItem,
            this.FolderContextMenu_AddAsFavoriteToolStripMenuItem,
            this.toolStripSeparator11,
            this.FolderContextMenu_NewFolderToolStripMenuItem,
            this.toolStripSeparator10,
            this.FolderContextMenu_SaveToolStripMenuItem,
            this.toolStripSeparator9,
            this.FolderContextMenu_RenameToolStripMenuItem,
            this.FolderContextMenu_DeleteToolStripMenuItem});
            this.FolderContextMenu.Name = "contextMenuStrip1";
            this.FolderContextMenu.Size = new System.Drawing.Size(156, 154);
            this.FolderContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FolderContextMenu_Opening);
            this.FolderContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.FolderContextMenu_ItemClicked);
            // 
            // FolderContextMenu_RefreshToolStripMenuItem
            // 
            this.FolderContextMenu_RefreshToolStripMenuItem.Name = "FolderContextMenu_RefreshToolStripMenuItem";
            this.FolderContextMenu_RefreshToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderContextMenu_RefreshToolStripMenuItem.Text = "Refresh";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(152, 6);
            // 
            // FolderContextMenu_NewFolderToolStripMenuItem
            // 
            this.FolderContextMenu_NewFolderToolStripMenuItem.Name = "FolderContextMenu_NewFolderToolStripMenuItem";
            this.FolderContextMenu_NewFolderToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderContextMenu_NewFolderToolStripMenuItem.Text = "New Folder";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(152, 6);
            // 
            // FolderContextMenu_SaveToolStripMenuItem
            // 
            this.FolderContextMenu_SaveToolStripMenuItem.Name = "FolderContextMenu_SaveToolStripMenuItem";
            this.FolderContextMenu_SaveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderContextMenu_SaveToolStripMenuItem.Text = "Save...";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(152, 6);
            // 
            // FolderContextMenu_RenameToolStripMenuItem
            // 
            this.FolderContextMenu_RenameToolStripMenuItem.Name = "FolderContextMenu_RenameToolStripMenuItem";
            this.FolderContextMenu_RenameToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderContextMenu_RenameToolStripMenuItem.Text = "Rename";
            // 
            // FolderContextMenu_DeleteToolStripMenuItem
            // 
            this.FolderContextMenu_DeleteToolStripMenuItem.Name = "FolderContextMenu_DeleteToolStripMenuItem";
            this.FolderContextMenu_DeleteToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderContextMenu_DeleteToolStripMenuItem.Text = "Delete";
            // 
            // FolderContextMenu_AddAsFavoriteToolStripMenuItem
            // 
            this.FolderContextMenu_AddAsFavoriteToolStripMenuItem.Name = "FolderContextMenu_AddAsFavoriteToolStripMenuItem";
            this.FolderContextMenu_AddAsFavoriteToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderContextMenu_AddAsFavoriteToolStripMenuItem.Text = "Add as Favorite";
            // 
            // FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem
            // 
            this.FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem.Name = "FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem";
            this.FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem.Text = "Add as Favorite";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 458);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.MainStatusStrip);
            this.Controls.Add(this.MainMenuStrip);
            this.Name = "Main";
            this.Text = "iDeviceBrowser";
            this.Load += new System.EventHandler(this.Main_Load);
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            this.MainSplitContainer.ResumeLayout(false);
            this.PreviewSplitContainer.Panel1.ResumeLayout(false);
            this.PreviewSplitContainer.Panel2.ResumeLayout(false);
            this.PreviewSplitContainer.ResumeLayout(false);
            this.FolderAndFileContextMenu.ResumeLayout(false);
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            this.MainMenuStrip.ResumeLayout(false);
            this.MainMenuStrip.PerformLayout();
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.FolderContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainSplitContainer;
        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.MenuStrip MainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FavoritesToolStripMenuItem;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.SplitContainer PreviewSplitContainer;
        private System.Windows.Forms.GroupBox PreviewGroupBox;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem_ExitExitToolStripMenuItem;
        private System.Windows.Forms.TreeView FolderTreeView;
        private System.Windows.Forms.ListView FolderAndFileListView;
        private System.Windows.Forms.ColumnHeader NameColumnHeader;
        private System.Windows.Forms.ColumnHeader SizeColumnHeader;
        private System.Windows.Forms.ColumnHeader TypeColumnHeader;
        private System.Windows.Forms.ToolStripStatusLabel MainStatusStrip_StatusToolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar MainStatusStrip_ProgressToolStripProgressBar;
        private System.Windows.Forms.ContextMenuStrip FolderContextMenu;
        private System.Windows.Forms.ToolStripMenuItem FolderContextMenu_RefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderContextMenu_DeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderContextMenu_SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderContextMenu_RenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem FolderContextMenu_NewFolderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip FolderAndFileContextMenu;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_RefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_SaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_RenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_ReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_DeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_SaveToolStripMenuItem;
        private System.Windows.Forms.ImageList FolderAndFileImageList;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem_PreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_NewFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem FavoritesToolStripMenuItem_EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TextBox PathTextBox;
        private System.Windows.Forms.ToolStripMenuItem FolderContextMenu_AddAsFavoriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderAndFileContextMenu_AddAsFavoriteToolStripMenuItem;
    }
}

