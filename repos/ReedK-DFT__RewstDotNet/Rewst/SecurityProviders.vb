Imports Azure.Core
Imports Azure.Identity
Imports Azure.Security.KeyVault.Secrets

Namespace Security
    Public Interface ISecretProvider
        ReadOnly Property IsAuthenticated As Boolean
        Sub Authenticate()
        Function GetSecret() As String
    End Interface

    Public Class KeyVaultSecretProvider
        Implements ISecretProvider

        Private client As SecretClient

        Public Property Credential As TokenCredential

        Public ReadOnly Property IsAuthenticated As Boolean Implements ISecretProvider.IsAuthenticated
            Get
                Return client IsNot Nothing
            End Get
        End Property

        Public Property SecretName As String
        Public Property VaultAddress As Uri

        Public Sub New(vault As Uri, secret As String)
            Me.New(vault, secret, Nothing)
        End Sub

        Public Sub New(vault As Uri, secret As String, creds As TokenCredential)
            VaultAddress = vault
            Credential = creds
            SecretName = secret
        End Sub

        Public Sub Authenticate() Implements ISecretProvider.Authenticate
            Try
                If Credential Is Nothing Then Credential = New InteractiveBrowserCredential()
                Try
                    client = New SecretClient(VaultAddress, Credential)
                Catch ex As Exception
                    Throw New Exception("Unable to acquire secret value. Check vault address and verify that the supplied credentials have access to the resource.", ex)
                End Try
            Catch ex As Exception
                Throw New AuthenticationFailedException("Authentication flow failed or was canceled.", ex)
            End Try
        End Sub

        Public Function GetSecret() As String Implements ISecretProvider.GetSecret
            Dim result = client.GetSecret(SecretName)
            Return result.Value.Value
        End Function

    End Class

    Public Class PlainTextSecretProvider
        Implements ISecretProvider

        Public Property Value As String

        Public Sub New(secret As String)
            Value = secret
        End Sub

        Public ReadOnly Property IsAuthenticated As Boolean Implements ISecretProvider.IsAuthenticated
            Get
                Return True
            End Get
        End Property

        Public Sub Authenticate() Implements ISecretProvider.Authenticate
        End Sub

        Public Function GetSecret() As String Implements ISecretProvider.GetSecret
            Return Value
        End Function
    End Class

End Namespace