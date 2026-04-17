Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Text.Json.Nodes

Namespace Folders

    Public Class Folder
        Public Property Name As String
        Public Property Index As TagIndex
        Public Property Workflows As New WorkflowCollection
    End Class

    Public Class Workflow
        Public ReadOnly Property Name As String
        Public ReadOnly Property Id As String
        Public ReadOnly Property OrgId As String
        Public ReadOnly Property Index As New TagIndex
        Public ReadOnly Property Type As WorkflowType
        Public Sub New(jsonObj As JsonObject)
            Me.Name = jsonObj.Item("name").AsValue.ToString
            Me.Id = jsonObj.Item("id").AsValue.ToString
            Me.OrgId = jsonObj.Item("org").Item("id").AsValue.ToString
            For Each tag In jsonObj.Item("tags").AsArray
                Me.Index.Add(NormalizeName(tag.Item("name").AsValue.ToString))
            Next
            Me.Type = [Enum].Parse(GetType(WorkflowType), jsonObj.Item("type").AsValue.ToString, True)
        End Sub
        Public Shared Function NormalizeName(name As String) As String
            Dim normalized = name.Trim().ToLower()
            Dim parts = normalized.Split(" "c, StringSplitOptions.RemoveEmptyEntries Or StringSplitOptions.TrimEntries)
            For i = 0 To parts.Length - 1
                parts(i) = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts(i))
            Next
            normalized = String.Join(" ", parts)
            If normalized.StartsWith("[") AndAlso Not normalized.Contains("]") Then normalized = normalized.Substring(1)
            Return normalized
        End Function

    End Class

    Public Enum WorkflowType
        Workflow = 0
        Form = 1
    End Enum

    Public Class TagIndex
        Implements IList(Of String)
        Implements IEquatable(Of TagIndex)
        Implements IComparable(Of TagIndex)
        Implements IComparable

        Protected innerList As New List(Of String)()
        Protected innerHash As New HashSet(Of String)

        Default Public Property Item(index As Integer) As String Implements IList(Of String).Item
            Get
                Return innerList(index)
            End Get
            Set(value As String)
                If Not innerHash.Contains(value) Then
                    innerHash.Remove(innerList(index))
                    innerHash.Add(value)
                    innerList(index) = value
                    innerList.Sort()
                End If
            End Set
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of String).Count
            Get
                Return innerList.Count
            End Get
        End Property

        Private ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of String).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Private Sub Insert(index As Integer, item As String) Implements IList(Of String).Insert
            Throw New NotImplementedException("Insert method is not implemented.")
        End Sub

        Public Sub RemoveAt(index As Integer) Implements IList(Of String).RemoveAt
            If innerHash.Contains(innerList(index)) Then
                innerHash.Remove(innerList(index))
                innerList.RemoveAt(index)
            End If
        End Sub

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(tags As IEnumerable(Of String))
            MyBase.New
            AddRange(tags)
        End Sub

        Public Sub Add(item As String) Implements ICollection(Of String).Add
            If Not innerHash.Contains(item) Then
                innerHash.Add(item)
                innerList.Add(item)
                innerList.Sort()
            End If
        End Sub

        Public Sub AddRange(items As IEnumerable(Of String))
            For Each i In items
                If Not innerHash.Contains(i) Then
                    innerHash.Add(i)
                    innerList.Add(i)
                End If
            Next
            innerList.Sort()
        End Sub

        Public Sub Clear() Implements ICollection(Of String).Clear
            innerList.Clear()
            innerHash.Clear()
        End Sub

        Public Sub CopyTo(array() As String, arrayIndex As Integer) Implements ICollection(Of String).CopyTo
            innerList.CopyTo(array, arrayIndex)
        End Sub

        Public Function IndexOf(item As String) As Integer Implements IList(Of String).IndexOf
            Return innerList.IndexOf(item)
        End Function

        Public Function Contains(item As TagIndex) As Boolean
            Return innerHash.IsProperSupersetOf(item.innerHash)
        End Function

        Public Function Contains(item As String) As Boolean Implements ICollection(Of String).Contains
            Return innerHash.Contains(item)
        End Function

        Public Function Remove(item As String) As Boolean Implements ICollection(Of String).Remove
            innerHash.Remove(item)
            Return innerList.Remove(item)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            Return innerList.GetEnumerator()
        End Function

        Public Overloads Function Equals(other As TagIndex) As Boolean Implements IEquatable(Of TagIndex).Equals
            Return innerHash.SetEquals(other.innerHash) AndAlso other.innerHash.SetEquals(innerHash)
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is TagIndex Then Return Me.Equals(CType(obj, TagIndex))
            Return MyBase.Equals(obj)
        End Function

        Public Function CompareTo(other As TagIndex) As Integer Implements IComparable(Of TagIndex).CompareTo
            Dim testHash1 As New HashSet(Of String)(innerHash)
            Dim testHash2 As New HashSet(Of String)(other.innerHash)
            testHash1.ExceptWith(testHash2)
            testHash2.ExceptWith(innerHash)
            Return testHash1.Count - testHash2.Count
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            If TypeOf obj Is TagIndex Then
                Return CompareTo(CType(obj, TagIndex))
            End If
            Dim other As New TagIndex
            other.Add(obj.ToString)
            Return CompareTo(other)
        End Function

        Public Overloads Shared Operator =(source As TagIndex, target As TagIndex) As Boolean
            Return source.Equals(target)
        End Operator

        Public Overloads Shared Operator <>(source As TagIndex, target As TagIndex) As Boolean
            Return Not source.Equals(target)
        End Operator
    End Class

    Public Class WorkflowCollection
        Inherits KeyedCollection(Of TagIndex, Workflow)

        Protected Overrides Function GetKeyForItem(item As Workflow) As TagIndex
            Return item.Index
        End Function
    End Class

    Public Class FolderCollection
        Inherits KeyedCollection(Of TagIndex, Folder)

        Protected Overrides Function GetKeyForItem(item As Folder) As TagIndex
            Return item.Index
        End Function
    End Class
End Namespace
