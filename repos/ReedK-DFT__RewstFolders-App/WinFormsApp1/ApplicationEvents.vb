Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.

    ' **NEW** ApplyApplicationDefaults: Raised when the application queries default values to be set for the application.

    ' Example:


    Partial Friend Class MyApplication
        Private Sub MyApplication_ApplyApplicationDefaults(sender As Object, e As ApplyApplicationDefaultsEventArgs) Handles Me.ApplyApplicationDefaults
            e.HighDpiMode = HighDpiMode.PerMonitorV2
        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs) Handles Me.UnhandledException
            If MessageBox.Show("Oh chicken-scratch! Something unexpected has gone wrong. Click OK to copy the details to the clipboard and post them to GitHub issues or email them to rkimble@dragonflytech.net, or else click Cancel to close the application.", "Fox in the Hen House", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) = DialogResult.OK Then
                Clipboard.SetText(e.Exception.ToString())
            End If
        End Sub
    End Class
End Namespace
