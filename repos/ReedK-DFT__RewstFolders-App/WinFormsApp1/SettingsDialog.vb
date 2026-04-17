Imports System.Windows.Forms

Public Class SettingsDialog

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If String.IsNullOrWhiteSpace(TextBox1.Text) Then
            MessageBox.Show("Please enter a valid Webhook URL.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MessageBox.Show("Please enter a valid Rewst Secret.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        My.Settings.Webhook = TextBox1.Text.Trim()
        My.Settings.Rewst = TextBox2.Text.Trim()
        My.Settings.Font = Me.Font
        My.Settings.Save()
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub SettingsDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = My.Settings.Webhook
        TextBox2.Text = My.Settings.Rewst
        Me.Font = My.Settings.Font
    End Sub

    Private Sub FontButton_Click(sender As Object, e As EventArgs) Handles FontButton.Click
        If FontDialog1.ShowDialog = DialogResult.OK Then
            Me.Font = FontDialog1.Font
        End If
    End Sub

End Class
