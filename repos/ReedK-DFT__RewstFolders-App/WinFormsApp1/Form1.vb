Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports RewstFolders.Folders

Public Class Form1
    Private workflowJson As JsonObject
    Private nameIndex As New Dictionary(Of String, List(Of TreeNode))
    Private tagList As New Dictionary(Of String, JsonObject)

    Private Function LoadFolders() As FolderCollection
        Dim folders As New Folders.FolderCollection

        Try
            If workflowJson.ContainsKey("error") Then
                MessageBox.Show($"Error loading workflows: {vbCrLf}{workflowJson.Item("error").AsValue.ToString}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return folders
            End If
            If workflowJson IsNot Nothing Then
                Dim items = workflowJson.Item("result").AsArray()
                For Each item In items
                    Dim itemName = item.Item("name").AsValue.ToString.Trim
                    If itemName.StartsWith("[") Then
                        Dim newtag = Workflow.NormalizeName(itemName.Substring(1, itemName.IndexOf("]") - 1))
                        For Each t In item.Item("tags").AsArray()
                            If Workflow.NormalizeName(t.Item("name").AsValue.ToString) = newtag Then
                                newtag = String.Empty
                                Exit For
                            End If
                        Next
                        If Not String.IsNullOrEmpty(newtag) Then
                            Dim itemArray = item.Item("tags").AsArray()
                            itemArray.Add(New JsonObject From {{"name", newtag}})
                            If newtag.ToLower.StartsWith("rewst") Then
                                itemArray.Insert(0, New JsonObject From {{"name", "Rewst"}})
                            End If
                        Else
                            If itemName.Trim.ToLower.StartsWith("rewst") Then
                                Dim itemTags = item.Item("tags").AsArray()
                                itemTags.Insert(0, New JsonObject From {{"name", "Rewst"}})
                            End If
                        End If
                    End If
                    For Each t In item.Item("tags").AsArray
                        If Not tagList.ContainsKey(t.Item("name").AsValue.ToString) Then
                            tagList.Add(t.Item("name").AsValue.ToString, t)
                        End If
                    Next
                    Dim tags = (From t In item.Item("tags").AsArray Select Workflow.NormalizeName(t.Item("name").AsValue.ToString))
                    Dim index As New TagIndex(tags)
                    If Not folders.Contains(index) Then
                        Dim Folder = New Folders.Folder
                        Folder.Name = itemName
                        Folder.Index = index
                        folders.Add(Folder)
                    End If
                Next

                Dim getImageIndex As Func(Of Workflow, Integer) = Function(wfo As Workflow) As Integer
                                                                      Select Case wfo.Type
                                                                          Case WorkflowType.Workflow
                                                                              Return 2 ' Workflow icon
                                                                          Case WorkflowType.Form
                                                                              Return 3 ' Form icon
                                                                          Case Else
                                                                              Return 0 ' Default icon for unknown types
                                                                      End Select
                                                                  End Function
                If folders.Count > 0 Then
                    TreeView1.Nodes.Clear()

                    For Each item In items
                        Dim wfo As New Workflow(item)
                        Dim nodeName = Convert.ToBase64String(Encoding.UTF8.GetBytes(wfo.Name))
                        If wfo.Index.Count = 0 Then
                            Dim imageIndex = getImageIndex(wfo)
                            Dim workflowNode As New TreeNode(wfo.Name) With {.ImageIndex = imageIndex, .SelectedImageIndex = imageIndex, .Tag = wfo, .Name = nodeName}
                            TreeView1.Nodes.Add(workflowNode)
                            AddToIndex(workflowNode)
                        Else
                            For Each folder In folders
                                If folder.Index.Equals(wfo.Index) Then
                                    Dim curNodes = TreeView1.Nodes
                                    Dim tags = folder.Index.ToList()
                                    For Each t In tags
                                        Dim keyName = Convert.ToBase64String(Encoding.UTF8.GetBytes(t))
                                        If Not curNodes.ContainsKey(keyName) Then
                                            Dim newNode = New TreeNode(t) With {.ImageIndex = 0, .SelectedImageIndex = 0, .Name = keyName}
                                            curNodes.Add(newNode)
                                            curNodes = newNode.Nodes
                                        Else
                                            curNodes = curNodes(keyName).Nodes
                                        End If
                                    Next
                                    If Not curNodes.ContainsKey(nodeName) Then
                                        Dim imageIndex = getImageIndex(wfo)
                                        Dim workflowNode As New TreeNode(wfo.Name) With {.ImageIndex = imageIndex, .SelectedImageIndex = imageIndex, .Tag = wfo, .Name = nodeName}
                                        curNodes.Add(workflowNode)
                                        AddToIndex(workflowNode)
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If

                For Each t In From k In tagList.Keys Order By k
                    TagsListBox.Items.Add(t)
                Next
            End If
        Catch ex As Exception
            MessageBox.Show($"Error loading folders: {vbCrLf}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return folders
    End Function

    Private Async Function LoadWorkflows() As Task
        nameIndex.Clear()
        workflowJson = Nothing
        Dim client As New HttpClient()
        Dim request As New HttpRequestMessage(HttpMethod.Get, My.Settings.Webhook)
        request.Headers.Add("x-rewst-secret", My.Settings.Rewst)
        Try
            Using result = Await client.SendAsync(request)
                Dim resultStream = result.Content.ReadAsStream
                workflowJson = Await JsonObject.ParseAsync(resultStream)
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error loading workflows: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            client.Dispose()
        End Try
    End Function

    Private Sub AddToIndex(node As TreeNode)
        If Not nameIndex.ContainsKey(node.Text) Then
            nameIndex(node.Text) = New List(Of TreeNode)()
        End If
        nameIndex(node.Text).Add(node)
    End Sub

    Private Function GetNodeUrl(node As TreeNode) As String
        If node?.Tag IsNot Nothing AndAlso TypeOf node?.Tag Is Workflow Then
            Dim wfo As Workflow = CType(node.Tag, Workflow)
            Return $"https://app.rewst.io/organizations/{wfo.OrgId}/{wfo.Type.ToString.ToLower}s/{wfo.Id}"
        End If
        Return String.Empty
    End Function

    Private Sub NavigateToNode(node As TreeNode)
        If node?.Tag IsNot Nothing AndAlso TypeOf node?.Tag Is Workflow Then
            Dim url As String = GetNodeUrl(node)
            Process.Start(New ProcessStartInfo("cmd", $"/c start {url}") With {.CreateNoWindow = True, .UseShellExecute = False})
        End If
    End Sub

    Private Async Sub LoadFoldersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadFoldersToolStripMenuItem.Click
        LoadFoldersToolStripMenuItem.Enabled = False
        Me.Cursor = Cursors.WaitCursor

        Await LoadWorkflows()
        Dim folders = LoadFolders()

        Dim nodeSorter As Action(Of TreeNodeCollection) = Sub(nodes As TreeNodeCollection)
                                                              Dim folderNodes As New List(Of TreeNode)()
                                                              For i = nodes.Count - 1 To 0 Step -1
                                                                  Dim node = nodes(i)
                                                                  If node.Nodes.Count > 0 Then
                                                                      folderNodes.Add(node)
                                                                      nodes.RemoveAt(i)
                                                                  End If
                                                              Next
                                                              folderNodes.Sort(Function(a, b) String.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase))
                                                              For i = folderNodes.Count - 1 To 0 Step -1
                                                                  nodes.Insert(0, folderNodes(i))
                                                              Next
                                                              For Each node In folderNodes
                                                                  nodeSorter(node.Nodes)
                                                              Next
                                                          End Sub

        nodeSorter(TreeView1.Nodes)
        StatusLabel.Text = $"Loaded {folders.Count} workflows."
        Me.Cursor = Cursors.Default
        LoadFoldersToolStripMenuItem.Enabled = True
    End Sub

    Private Sub TreeView1_DoubleClick(sender As Object, e As EventArgs) Handles TreeView1.DoubleClick
        NavigateToNode(TreeView1.SelectedNode)
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        Using dlg As New SettingsDialog
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                Me.Font = My.Settings.Font
            End If
        End Using
    End Sub

    Private Sub SearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchToolStripMenuItem.Click
        SplitContainer1.Panel1Collapsed = Not SearchToolStripMenuItem.Checked
    End Sub

    Private Sub SearchTextBox_KeyUp(sender As Object, e As KeyEventArgs) Handles SearchTextBox.KeyUp
        If e.KeyCode = Keys.Enter Then
            SearchTextBox.Text = SearchTextBox.Text.Trim()
            Button1.PerformClick()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If SearchTextBox.TextLength > 0 Then
            For Each key In nameIndex.Keys
                If key.ToLower().Contains(SearchTextBox.Text.ToLower()) Then
                    For i = 0 To nameIndex(key).Count - 1
                        ResultsListBox.Items.Add(nameIndex(key)(i))
                    Next
                End If
            Next
            ResultsListBox.DisplayMember = "Text"
        End If

        If ResultsListBox.Items.Count > 0 Then
            SplitContainer2.Panel2Collapsed = False
            StatusLabel.Text = $"{ResultsListBox.Items.Count} results listed."
        Else
            SplitContainer2.Panel2Collapsed = True
            StatusLabel.Text = "No results found."
        End If
        SearchTextBox.Clear()
    End Sub

    Private Sub CloseResultsButton_Click(sender As Object, e As EventArgs) Handles CloseResultsButton.Click
        SplitContainer2.Panel2Collapsed = True
    End Sub

    Private Sub ClearResultsButton_Click(sender As Object, e As EventArgs) Handles ClearResultsButton.Click
        ResultsListBox.Items.Clear()
        SplitContainer2.Panel2Collapsed = True
    End Sub

    Private Sub ResultsListBox_DoubleClick(sender As Object, e As EventArgs) Handles ResultsListBox.DoubleClick
        TreeView1.SelectedNode = CType(ResultsListBox.SelectedItem, TreeNode)
        TreeView1.SelectedNode.EnsureVisible()
    End Sub

    Private Sub TreeView1_AfterExpand(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterExpand
        e.Node.ImageIndex = 1
        e.Node.SelectedImageIndex = 1
    End Sub

    Private Sub TreeView1_AfterCollapse(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterCollapse
        e.Node.ImageIndex = 0
        e.Node.SelectedImageIndex = 0
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Font = My.Settings.Font
        If String.IsNullOrEmpty(My.Settings.Webhook) OrElse String.IsNullOrEmpty(My.Settings.Rewst) Then
            Using dlg As New SettingsDialog
                If dlg.ShowDialog(Me) <> DialogResult.OK Then
                    Application.Exit()
                End If
            End Using
        End If
    End Sub

    Private Sub CollapseAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CollapseAllToolStripMenuItem.Click
        TreeView1.CollapseAll()
    End Sub

    Private Sub ExpandAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExpandAllToolStripMenuItem.Click
        TreeView1.ExpandAll()
    End Sub

    Private Sub TagListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TagListToolStripMenuItem.Click
        ContentSplitContainer.Panel1Collapsed = Not TagListToolStripMenuItem.Checked
    End Sub

    Private Sub TagsListBox_DoubleClick(sender As Object, e As EventArgs) Handles TagsListBox.DoubleClick
        If TagsListBox.SelectedItem IsNot Nothing Then
            Dim found = TreeView1.Nodes.Find(Convert.ToBase64String(Encoding.UTF8.GetBytes(TagsListBox.SelectedItem.ToString)), True).ToList()
            For i = 0 To found.Count - 1
                If found(i).IsSelected Then
                    If i = found.Count - 1 Then i = 0 Else i += 1
                    TreeView1.SelectedNode = found(i)
                    TreeView1.SelectedNode.EnsureVisible()
                    Exit Sub
                End If
            Next
            If found.Count > 0 Then
                TreeView1.SelectedNode = found(0)
                TreeView1.SelectedNode.EnsureVisible()
            End If
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        Clipboard.SetText(GetNodeUrl(TreeView1.SelectedNode))
    End Sub

    Private Sub GoToToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GoToToolStripMenuItem.Click
        NavigateToNode(TreeView1.SelectedNode)
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        If TreeView1.SelectedNode IsNot Nothing Then
            Dim item As JsonObject = TryCast(TreeView1.SelectedNode.Tag, JsonObject)
            If item IsNot Nothing Then
                GoToToolStripMenuItem.Enabled = True
                CopyToolStripMenuItem.Enabled = True
            Else
                GoToToolStripMenuItem.Enabled = False
                CopyToolStripMenuItem.Enabled = False
            End If
        Else
            GoToToolStripMenuItem.Enabled = False
            CopyToolStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        If TreeView1.SelectedNode IsNot Nothing Then
            Dim level As TreeNode = TreeView1.SelectedNode
            Dim path As New StringBuilder
            While level IsNot Nothing
                If path.Length > 0 Then
                    path.Insert(0, "\")
                End If
                path.Insert(0, level.Text)
                level = level.Parent
            End While
            PathLabel.Text = path.ToString()
        Else
            PathLabel.Text = "No node selected."
        End If
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        Using aboutBox As New AboutBox1
            aboutBox.ShowDialog(Me)
        End Using
    End Sub
End Class
