
Imports System.Net.Http
Imports System.Net.Http.Json
Imports System.Text.Json
Imports Rewst.Security

Public Class Webhook
    Implements IDisposable

    Const SECRET_HEADER As String = "x-rewst-secret"
    Public Property Address As Uri
    Public Property SecretProvider As ISecretProvider
    Public Property JsonOptions As New JsonSerializerOptions

    Private client As New HttpClient
    Private disposedValue As Boolean

    Public Sub New()
        JsonOptions.IncludeFields = True
        JsonOptions.PropertyNameCaseInsensitive = True
        JsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    End Sub

    Public Sub New(webhookAddress As String)
        Me.New(New Uri(webhookAddress), Nothing, Nothing)
    End Sub

    Public Sub New(webhookAddress As Uri)
        Me.New(webhookAddress, Nothing, Nothing)
    End Sub

    Public Sub New(webhookAddress As String, secretValue As String)
        Me.New(New Uri(webhookAddress), Nothing, secretValue)
    End Sub

    Public Sub New(webhookAddress As Uri, secretValue As String)
        Me.New(webhookAddress, Nothing, secretValue)
    End Sub

    Public Sub New(webhookAddress As String, keyvaultAddress As String, secretName As String)
        Me.New(New Uri(webhookAddress), New Uri(keyvaultAddress), secretName)
    End Sub

    Public Sub New(webhookAddress As Uri, keyvaultAddress As Uri, secretName As String)
        Me.New()
        Address = webhookAddress
        If keyvaultAddress IsNot Nothing AndAlso Not String.IsNullOrEmpty(secretName) Then
            SecretProvider = New KeyVaultSecretProvider(keyvaultAddress, secretName)
        ElseIf keyvaultAddress Is Nothing AndAlso Not String.IsNullOrEmpty(secretName) Then
            SecretProvider = New PlainTextSecretProvider(secretName)
        End If
    End Sub

    Public Async Function CallAsync(Of T As Class)(method As HttpMethod) As Task(Of T)
        Return Await CallAsync(Of Object, T)(method, Nothing)
    End Function

    Public Async Function CallAsync(Of TContent As Class, TOut As Class)(method As HttpMethod, content As TContent) As Task(Of TOut)
        If client.DefaultRequestHeaders.Contains(SECRET_HEADER) Then
            client.DefaultRequestHeaders.Remove(SECRET_HEADER)
        End If
        If SecretProvider IsNot Nothing Then
            If Not SecretProvider.IsAuthenticated Then SecretProvider.Authenticate()
            client.DefaultRequestHeaders.Add(SECRET_HEADER, SecretProvider.GetSecret())
        End If
        Dim jContent As JsonContent = Nothing
        If content IsNot Nothing Then
            jContent = JsonContent.Create(Of TContent)(content, Nothing, JsonOptions)
        End If
        Dim response As HttpResponseMessage
        Select Case method
            Case HttpMethod.Delete
                response = Await client.DeleteAsync(Address)
            Case HttpMethod.Get
                response = Await client.GetAsync(Address)
            Case HttpMethod.Patch
                If jContent Is Nothing Then Throw New Exception("Content cannot be null.")
                response = Await client.PatchAsync(Address, jContent)
            Case HttpMethod.Post
                If jContent Is Nothing Then Throw New Exception("Content cannot be null.")
                response = Await client.PostAsync(Address, jContent)
            Case HttpMethod.Put
                If jContent Is Nothing Then Throw New Exception("Content cannot be null.")
                response = Await client.PutAsync(Address, jContent)
            Case Else
                Throw New Exception("Invalid HTTP method.")
        End Select
        Dim doc As JsonDocument
        Using stream = response.Content.ReadAsStream
            doc = Await JsonDocument.ParseAsync(stream)
        End Using
        response.Dispose()
        Return doc.Deserialize(Of TOut)(JsonOptions)
    End Function

    Public Function [Call](Of T As Class)(method As HttpMethod) As T
        Return [Call](Of Object, T)(method, Nothing)
    End Function

    Public Function [Call](Of TContent As Class, TOut As Class)(method As HttpMethod, content As TContent) As TOut
        Dim result = Task.Run(Function() CallAsync(Of TContent, TOut)(method, content)).GetAwaiter.GetResult()
        Return result
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                client.Dispose()
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

