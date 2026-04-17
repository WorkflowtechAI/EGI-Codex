<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        TreeView1 = New TreeView()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        CopyToolStripMenuItem = New ToolStripMenuItem()
        GoToToolStripMenuItem = New ToolStripMenuItem()
        ImageList1 = New ImageList(components)
        MenuStrip1 = New MenuStrip()
        FileToolStripMenuItem = New ToolStripMenuItem()
        LoadFoldersToolStripMenuItem = New ToolStripMenuItem()
        ToolStripMenuItem1 = New ToolStripSeparator()
        ExitToolStripMenuItem = New ToolStripMenuItem()
        ViewToolStripMenuItem = New ToolStripMenuItem()
        TagListToolStripMenuItem = New ToolStripMenuItem()
        SearchToolStripMenuItem = New ToolStripMenuItem()
        ToolStripMenuItem2 = New ToolStripSeparator()
        CollapseAllToolStripMenuItem = New ToolStripMenuItem()
        ExpandAllToolStripMenuItem = New ToolStripMenuItem()
        OptionsToolStripMenuItem = New ToolStripMenuItem()
        SettingsToolStripMenuItem = New ToolStripMenuItem()
        AboutToolStripMenuItem = New ToolStripMenuItem()
        StatusStrip1 = New StatusStrip()
        StatusLabel = New ToolStripStatusLabel()
        SplitContainer1 = New SplitContainer()
        TableLayoutPanel1 = New TableLayoutPanel()
        Label1 = New Label()
        SearchTextBox = New TextBox()
        Button1 = New Button()
        ContentSplitContainer = New SplitContainer()
        TagsListBox = New ListBox()
        Label2 = New Label()
        SplitContainer2 = New SplitContainer()
        PathLabel = New TextBox()
        TableLayoutPanel2 = New TableLayoutPanel()
        CloseResultsButton = New Button()
        ClearResultsButton = New Button()
        ResultsListBox = New ListBox()
        ContextMenuStrip1.SuspendLayout()
        MenuStrip1.SuspendLayout()
        StatusStrip1.SuspendLayout()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        TableLayoutPanel1.SuspendLayout()
        CType(ContentSplitContainer, ComponentModel.ISupportInitialize).BeginInit()
        ContentSplitContainer.Panel1.SuspendLayout()
        ContentSplitContainer.Panel2.SuspendLayout()
        ContentSplitContainer.SuspendLayout()
        CType(SplitContainer2, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer2.Panel1.SuspendLayout()
        SplitContainer2.Panel2.SuspendLayout()
        SplitContainer2.SuspendLayout()
        TableLayoutPanel2.SuspendLayout()
        SuspendLayout()
        ' 
        ' TreeView1
        ' 
        TreeView1.ContextMenuStrip = ContextMenuStrip1
        TreeView1.Dock = DockStyle.Fill
        TreeView1.HideSelection = False
        TreeView1.ImageIndex = 0
        TreeView1.ImageList = ImageList1
        TreeView1.Location = New Point(0, 0)
        TreeView1.Name = "TreeView1"
        TreeView1.SelectedImageIndex = 0
        TreeView1.Size = New Size(740, 460)
        TreeView1.TabIndex = 0
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.ImageScalingSize = New Size(24, 24)
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {CopyToolStripMenuItem, GoToToolStripMenuItem})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(143, 68)
        ' 
        ' CopyToolStripMenuItem
        ' 
        CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        CopyToolStripMenuItem.Size = New Size(142, 32)
        CopyToolStripMenuItem.Text = "Copy"
        ' 
        ' GoToToolStripMenuItem
        ' 
        GoToToolStripMenuItem.Name = "GoToToolStripMenuItem"
        GoToToolStripMenuItem.Size = New Size(142, 32)
        GoToToolStripMenuItem.Text = "Go To..."
        ' 
        ' ImageList1
        ' 
        ImageList1.ColorDepth = ColorDepth.Depth32Bit
        ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), ImageListStreamer)
        ImageList1.TransparentColor = Color.Transparent
        ImageList1.Images.SetKeyName(0, "Folder_16x.png")
        ImageList1.Images.SetKeyName(1, "FolderOpen_16x.png")
        ImageList1.Images.SetKeyName(2, "WorkflowActivity_16x.png")
        ImageList1.Images.SetKeyName(3, "FormPostBodyStringnode_16x.png")
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.ImageScalingSize = New Size(24, 24)
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem, ViewToolStripMenuItem, OptionsToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(740, 33)
        MenuStrip1.TabIndex = 4
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {LoadFoldersToolStripMenuItem, ToolStripMenuItem1, ExitToolStripMenuItem})
        FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        FileToolStripMenuItem.Size = New Size(54, 29)
        FileToolStripMenuItem.Text = "File"
        ' 
        ' LoadFoldersToolStripMenuItem
        ' 
        LoadFoldersToolStripMenuItem.Image = CType(resources.GetObject("LoadFoldersToolStripMenuItem.Image"), Image)
        LoadFoldersToolStripMenuItem.Name = "LoadFoldersToolStripMenuItem"
        LoadFoldersToolStripMenuItem.Size = New Size(216, 34)
        LoadFoldersToolStripMenuItem.Text = "Load Folders"
        ' 
        ' ToolStripMenuItem1
        ' 
        ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        ToolStripMenuItem1.Size = New Size(213, 6)
        ' 
        ' ExitToolStripMenuItem
        ' 
        ExitToolStripMenuItem.Image = CType(resources.GetObject("ExitToolStripMenuItem.Image"), Image)
        ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        ExitToolStripMenuItem.Size = New Size(216, 34)
        ExitToolStripMenuItem.Text = "Exit"
        ' 
        ' ViewToolStripMenuItem
        ' 
        ViewToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {TagListToolStripMenuItem, SearchToolStripMenuItem, ToolStripMenuItem2, CollapseAllToolStripMenuItem, ExpandAllToolStripMenuItem})
        ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        ViewToolStripMenuItem.Size = New Size(65, 29)
        ViewToolStripMenuItem.Text = "View"
        ' 
        ' TagListToolStripMenuItem
        ' 
        TagListToolStripMenuItem.CheckOnClick = True
        TagListToolStripMenuItem.Image = My.Resources.Resources.Tag_16x
        TagListToolStripMenuItem.Name = "TagListToolStripMenuItem"
        TagListToolStripMenuItem.Size = New Size(206, 34)
        TagListToolStripMenuItem.Text = "Tag List"
        ' 
        ' SearchToolStripMenuItem
        ' 
        SearchToolStripMenuItem.CheckOnClick = True
        SearchToolStripMenuItem.Image = My.Resources.Resources.Search_noHalo_16x
        SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
        SearchToolStripMenuItem.Size = New Size(206, 34)
        SearchToolStripMenuItem.Text = "Search"
        ' 
        ' ToolStripMenuItem2
        ' 
        ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        ToolStripMenuItem2.Size = New Size(203, 6)
        ' 
        ' CollapseAllToolStripMenuItem
        ' 
        CollapseAllToolStripMenuItem.Image = My.Resources.Resources.CollapseAll_16x
        CollapseAllToolStripMenuItem.Name = "CollapseAllToolStripMenuItem"
        CollapseAllToolStripMenuItem.Size = New Size(206, 34)
        CollapseAllToolStripMenuItem.Text = "Collapse All"
        ' 
        ' ExpandAllToolStripMenuItem
        ' 
        ExpandAllToolStripMenuItem.Image = My.Resources.Resources.ExpandAll_16x
        ExpandAllToolStripMenuItem.Name = "ExpandAllToolStripMenuItem"
        ExpandAllToolStripMenuItem.Size = New Size(206, 34)
        ExpandAllToolStripMenuItem.Text = "Expand All"
        ' 
        ' OptionsToolStripMenuItem
        ' 
        OptionsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {SettingsToolStripMenuItem, AboutToolStripMenuItem})
        OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        OptionsToolStripMenuItem.Size = New Size(92, 29)
        OptionsToolStripMenuItem.Text = "Options"
        ' 
        ' SettingsToolStripMenuItem
        ' 
        SettingsToolStripMenuItem.Image = CType(resources.GetObject("SettingsToolStripMenuItem.Image"), Image)
        SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        SettingsToolStripMenuItem.Size = New Size(190, 34)
        SettingsToolStripMenuItem.Text = "Settings..."
        ' 
        ' AboutToolStripMenuItem
        ' 
        AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        AboutToolStripMenuItem.Size = New Size(190, 34)
        AboutToolStripMenuItem.Text = "About..."
        ' 
        ' StatusStrip1
        ' 
        StatusStrip1.ImageScalingSize = New Size(24, 24)
        StatusStrip1.Items.AddRange(New ToolStripItem() {StatusLabel})
        StatusStrip1.Location = New Point(0, 524)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Size = New Size(740, 32)
        StatusStrip1.TabIndex = 5
        StatusStrip1.Text = "StatusStrip1"
        ' 
        ' StatusLabel
        ' 
        StatusLabel.Name = "StatusLabel"
        StatusLabel.Size = New Size(60, 25)
        StatusLabel.Text = "Ready"
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.Dock = DockStyle.Fill
        SplitContainer1.FixedPanel = FixedPanel.Panel1
        SplitContainer1.IsSplitterFixed = True
        SplitContainer1.Location = New Point(0, 33)
        SplitContainer1.Name = "SplitContainer1"
        SplitContainer1.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(TableLayoutPanel1)
        SplitContainer1.Panel1Collapsed = True
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(ContentSplitContainer)
        SplitContainer1.Size = New Size(740, 491)
        SplitContainer1.SplitterDistance = 43
        SplitContainer1.TabIndex = 6
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.ColumnCount = 3
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle())
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle())
        TableLayoutPanel1.Controls.Add(Label1, 0, 0)
        TableLayoutPanel1.Controls.Add(SearchTextBox, 1, 0)
        TableLayoutPanel1.Controls.Add(Button1, 2, 0)
        TableLayoutPanel1.Dock = DockStyle.Fill
        TableLayoutPanel1.Location = New Point(0, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Size = New Size(150, 43)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' Label1
        ' 
        Label1.Anchor = AnchorStyles.None
        Label1.AutoSize = True
        Label1.Location = New Point(3, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(64, 25)
        Label1.TabIndex = 0
        Label1.Text = "Search"
        ' 
        ' SearchTextBox
        ' 
        SearchTextBox.AcceptsReturn = True
        SearchTextBox.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        SearchTextBox.Location = New Point(73, 6)
        SearchTextBox.Multiline = True
        SearchTextBox.Name = "SearchTextBox"
        SearchTextBox.Size = New Size(28, 31)
        SearchTextBox.TabIndex = 1
        ' 
        ' Button1
        ' 
        Button1.Anchor = AnchorStyles.Left
        Button1.Image = CType(resources.GetObject("Button1.Image"), Image)
        Button1.Location = New Point(107, 4)
        Button1.Name = "Button1"
        Button1.Size = New Size(40, 34)
        Button1.TabIndex = 2
        Button1.UseVisualStyleBackColor = True
        ' 
        ' ContentSplitContainer
        ' 
        ContentSplitContainer.Dock = DockStyle.Fill
        ContentSplitContainer.FixedPanel = FixedPanel.Panel1
        ContentSplitContainer.Location = New Point(0, 0)
        ContentSplitContainer.Name = "ContentSplitContainer"
        ' 
        ' ContentSplitContainer.Panel1
        ' 
        ContentSplitContainer.Panel1.Controls.Add(TagsListBox)
        ContentSplitContainer.Panel1.Controls.Add(Label2)
        ContentSplitContainer.Panel1Collapsed = True
        ' 
        ' ContentSplitContainer.Panel2
        ' 
        ContentSplitContainer.Panel2.Controls.Add(SplitContainer2)
        ContentSplitContainer.Size = New Size(740, 491)
        ContentSplitContainer.SplitterDistance = 176
        ContentSplitContainer.TabIndex = 2
        ' 
        ' TagsListBox
        ' 
        TagsListBox.Dock = DockStyle.Fill
        TagsListBox.FormattingEnabled = True
        TagsListBox.IntegralHeight = False
        TagsListBox.ItemHeight = 25
        TagsListBox.Location = New Point(0, 25)
        TagsListBox.Name = "TagsListBox"
        TagsListBox.Size = New Size(176, 75)
        TagsListBox.TabIndex = 0
        ' 
        ' Label2
        ' 
        Label2.Dock = DockStyle.Top
        Label2.Location = New Point(0, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(176, 25)
        Label2.TabIndex = 1
        Label2.Text = "Tag List"
        Label2.TextAlign = ContentAlignment.TopCenter
        ' 
        ' SplitContainer2
        ' 
        SplitContainer2.Dock = DockStyle.Fill
        SplitContainer2.Location = New Point(0, 0)
        SplitContainer2.Name = "SplitContainer2"
        ' 
        ' SplitContainer2.Panel1
        ' 
        SplitContainer2.Panel1.Controls.Add(TreeView1)
        SplitContainer2.Panel1.Controls.Add(PathLabel)
        ' 
        ' SplitContainer2.Panel2
        ' 
        SplitContainer2.Panel2.Controls.Add(TableLayoutPanel2)
        SplitContainer2.Panel2Collapsed = True
        SplitContainer2.Size = New Size(740, 491)
        SplitContainer2.SplitterDistance = 443
        SplitContainer2.TabIndex = 1
        ' 
        ' PathLabel
        ' 
        PathLabel.Dock = DockStyle.Bottom
        PathLabel.Location = New Point(0, 460)
        PathLabel.Name = "PathLabel"
        PathLabel.ReadOnly = True
        PathLabel.Size = New Size(740, 31)
        PathLabel.TabIndex = 1
        ' 
        ' TableLayoutPanel2
        ' 
        TableLayoutPanel2.ColumnCount = 1
        TableLayoutPanel2.ColumnStyles.Add(New ColumnStyle())
        TableLayoutPanel2.Controls.Add(CloseResultsButton, 0, 0)
        TableLayoutPanel2.Controls.Add(ClearResultsButton, 0, 2)
        TableLayoutPanel2.Controls.Add(ResultsListBox, 0, 1)
        TableLayoutPanel2.Dock = DockStyle.Fill
        TableLayoutPanel2.Location = New Point(0, 0)
        TableLayoutPanel2.Name = "TableLayoutPanel2"
        TableLayoutPanel2.RowCount = 3
        TableLayoutPanel2.RowStyles.Add(New RowStyle())
        TableLayoutPanel2.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        TableLayoutPanel2.RowStyles.Add(New RowStyle())
        TableLayoutPanel2.Size = New Size(96, 100)
        TableLayoutPanel2.TabIndex = 0
        ' 
        ' CloseResultsButton
        ' 
        CloseResultsButton.Anchor = AnchorStyles.Right
        CloseResultsButton.AutoSize = True
        CloseResultsButton.AutoSizeMode = AutoSizeMode.GrowAndShrink
        CloseResultsButton.Image = CType(resources.GetObject("CloseResultsButton.Image"), Image)
        CloseResultsButton.Location = New Point(268, 3)
        CloseResultsButton.Name = "CloseResultsButton"
        CloseResultsButton.Size = New Size(22, 22)
        CloseResultsButton.TabIndex = 0
        CloseResultsButton.UseVisualStyleBackColor = True
        ' 
        ' ClearResultsButton
        ' 
        ClearResultsButton.Anchor = AnchorStyles.None
        ClearResultsButton.Location = New Point(90, 63)
        ClearResultsButton.Name = "ClearResultsButton"
        ClearResultsButton.Size = New Size(112, 34)
        ClearResultsButton.TabIndex = 1
        ClearResultsButton.Text = "Clear"
        ClearResultsButton.UseVisualStyleBackColor = True
        ' 
        ' ResultsListBox
        ' 
        ResultsListBox.Dock = DockStyle.Fill
        ResultsListBox.FormattingEnabled = True
        ResultsListBox.IntegralHeight = False
        ResultsListBox.ItemHeight = 25
        ResultsListBox.Location = New Point(3, 31)
        ResultsListBox.Name = "ResultsListBox"
        ResultsListBox.Size = New Size(287, 26)
        ResultsListBox.TabIndex = 2
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(740, 556)
        Controls.Add(SplitContainer1)
        Controls.Add(StatusStrip1)
        Controls.Add(MenuStrip1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "[REWST] - Folders!"
        ContextMenuStrip1.ResumeLayout(False)
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        ContentSplitContainer.Panel1.ResumeLayout(False)
        ContentSplitContainer.Panel2.ResumeLayout(False)
        CType(ContentSplitContainer, ComponentModel.ISupportInitialize).EndInit()
        ContentSplitContainer.ResumeLayout(False)
        SplitContainer2.Panel1.ResumeLayout(False)
        SplitContainer2.Panel1.PerformLayout()
        SplitContainer2.Panel2.ResumeLayout(False)
        CType(SplitContainer2, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer2.ResumeLayout(False)
        TableLayoutPanel2.ResumeLayout(False)
        TableLayoutPanel2.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents TreeView1 As TreeView
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LoadFoldersToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents StatusLabel As ToolStripStatusLabel
    Friend WithEvents OptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents SearchTextBox As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents CloseResultsButton As Button
    Friend WithEvents ClearResultsButton As Button
    Friend WithEvents ResultsListBox As ListBox
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CollapseAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExpandAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContentSplitContainer As SplitContainer
    Friend WithEvents TagsListBox As ListBox
    Friend WithEvents TagListToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Label2 As Label
    Friend WithEvents SearchToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GoToToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PathLabel As TextBox
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem

End Class
