<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingsDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        TableLayoutPanel1 = New TableLayoutPanel()
        OK_Button = New Button()
        Cancel_Button = New Button()
        TableLayoutPanel2 = New TableLayoutPanel()
        Label1 = New Label()
        Label2 = New Label()
        TextBox1 = New TextBox()
        TextBox2 = New TextBox()
        Label3 = New Label()
        FontButton = New Button()
        FontDialog1 = New FontDialog()
        TableLayoutPanel1.SuspendLayout()
        TableLayoutPanel2.SuspendLayout()
        SuspendLayout()
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TableLayoutPanel1.ColumnCount = 2
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Controls.Add(OK_Button, 0, 0)
        TableLayoutPanel1.Controls.Add(Cancel_Button, 1, 0)
        TableLayoutPanel1.Location = New Point(462, 144)
        TableLayoutPanel1.Margin = New Padding(5, 6, 5, 6)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Size = New Size(243, 56)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' OK_Button
        ' 
        OK_Button.Anchor = AnchorStyles.None
        OK_Button.Location = New Point(5, 6)
        OK_Button.Margin = New Padding(5, 6, 5, 6)
        OK_Button.Name = "OK_Button"
        OK_Button.Size = New Size(111, 44)
        OK_Button.TabIndex = 0
        OK_Button.Text = "OK"
        ' 
        ' Cancel_Button
        ' 
        Cancel_Button.Anchor = AnchorStyles.None
        Cancel_Button.Location = New Point(126, 6)
        Cancel_Button.Margin = New Padding(5, 6, 5, 6)
        Cancel_Button.Name = "Cancel_Button"
        Cancel_Button.Size = New Size(112, 44)
        Cancel_Button.TabIndex = 1
        Cancel_Button.Text = "Cancel"
        ' 
        ' TableLayoutPanel2
        ' 
        TableLayoutPanel2.ColumnCount = 2
        TableLayoutPanel2.ColumnStyles.Add(New ColumnStyle())
        TableLayoutPanel2.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel2.Controls.Add(Label1, 0, 0)
        TableLayoutPanel2.Controls.Add(Label2, 0, 1)
        TableLayoutPanel2.Controls.Add(TextBox1, 1, 0)
        TableLayoutPanel2.Controls.Add(TextBox2, 1, 1)
        TableLayoutPanel2.Controls.Add(Label3, 0, 2)
        TableLayoutPanel2.Controls.Add(FontButton, 1, 2)
        TableLayoutPanel2.Location = New Point(12, 12)
        TableLayoutPanel2.Name = "TableLayoutPanel2"
        TableLayoutPanel2.RowCount = 3
        TableLayoutPanel2.RowStyles.Add(New RowStyle())
        TableLayoutPanel2.RowStyles.Add(New RowStyle())
        TableLayoutPanel2.RowStyles.Add(New RowStyle())
        TableLayoutPanel2.Size = New Size(701, 119)
        TableLayoutPanel2.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.Anchor = AnchorStyles.Right
        Label1.AutoSize = True
        Label1.Location = New Point(20, 6)
        Label1.Name = "Label1"
        Label1.Size = New Size(125, 25)
        Label1.TabIndex = 0
        Label1.Text = "Webhook URL"
        ' 
        ' Label2
        ' 
        Label2.Anchor = AnchorStyles.Right
        Label2.AutoSize = True
        Label2.Location = New Point(3, 43)
        Label2.Name = "Label2"
        Label2.Size = New Size(142, 25)
        Label2.TabIndex = 1
        Label2.Text = "Webhook Secret"
        ' 
        ' TextBox1
        ' 
        TextBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        TextBox1.Location = New Point(151, 3)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(547, 31)
        TextBox1.TabIndex = 2
        ' 
        ' TextBox2
        ' 
        TextBox2.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        TextBox2.Location = New Point(151, 40)
        TextBox2.Name = "TextBox2"
        TextBox2.PasswordChar = "*"c
        TextBox2.Size = New Size(547, 31)
        TextBox2.TabIndex = 3
        ' 
        ' Label3
        ' 
        Label3.Anchor = AnchorStyles.Right
        Label3.AutoSize = True
        Label3.Location = New Point(97, 84)
        Label3.Name = "Label3"
        Label3.Size = New Size(48, 25)
        Label3.TabIndex = 4
        Label3.Text = "Font"
        ' 
        ' FontButton
        ' 
        FontButton.Anchor = AnchorStyles.None
        FontButton.Location = New Point(368, 79)
        FontButton.Name = "FontButton"
        FontButton.Size = New Size(112, 34)
        FontButton.TabIndex = 5
        FontButton.Text = "Font"
        FontButton.UseVisualStyleBackColor = True
        ' 
        ' SettingsDialog
        ' 
        AcceptButton = OK_Button
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = Cancel_Button
        ClientSize = New Size(725, 223)
        Controls.Add(TableLayoutPanel2)
        Controls.Add(TableLayoutPanel1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Margin = New Padding(5, 6, 5, 6)
        MaximizeBox = False
        MinimizeBox = False
        Name = "SettingsDialog"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Settings"
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel2.ResumeLayout(False)
        TableLayoutPanel2.PerformLayout()
        ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents FontButton As Button
    Friend WithEvents FontDialog1 As FontDialog

End Class
