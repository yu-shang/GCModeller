﻿#Region "Microsoft.VisualBasic::e97510322769afafb4a5aeca09124036, ..\Microsoft.VisualBasic.Architecture.Framework\ComponentModel\Settings\ConfigEngine.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.Settings

    ''' <summary>
    ''' 只包含有对数据映射目标对象的属性读写，并不包含有文件数据的读写操作
    ''' </summary>
    ''' 
    Public Class ConfigEngine : Inherits ITextFile
        Implements IDisposable

        ''' <summary>
        ''' 所映射的数据源
        ''' </summary>
        Protected _SettingsData As IProfile
        ''' <summary>
        ''' 键名都是小写的
        ''' </summary>
        Protected ProfileItemCollection As IDictionary(Of String, BindMapping)

        ''' <summary>
        ''' List all of the available settings nodes in this profile data session.
        ''' (枚举出当前配置会话之中的所有可用的配置节点)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AllItems As BindMapping()
            Get
                Return ProfileItemCollection.Values.ToArray
            End Get
        End Property

        Public Overrides Property FilePath As String
            Get
                Return _SettingsData.FilePath
            End Get
            Set(value As String)
                _SettingsData.FilePath = value
            End Set
        End Property

        Sub New(obj As IProfile)
            _SettingsData = obj
            ProfileItemCollection =
                ConfigEngine.Load(Of IProfile)(
                    obj.GetType,
                    obj:=_SettingsData).ToDictionary(Function(x) x.Name,
                                                     Function(x) x.x)
        End Sub

        Protected Sub New()
        End Sub

        Protected Friend Shared Function Load(Of EntityType)(type As Type, obj As EntityType) As NamedValue(Of BindMapping)()
            Dim LQuery As IEnumerable(Of BindMapping) =
 _
                From [Property] As PropertyInfo
                In type.GetProperties
                Let attributes = [Property].GetCustomAttributes(attributeType:=ProfileItemType, inherit:=False)
                Where attributes.Length > 0
                Let attr = DirectCast(attributes(0), ProfileItem)
                Select BindMapping.Initialize(attr, [Property], obj) '

            Dim out As List(Of NamedValue(Of BindMapping)) =
                LinqAPI.MakeList(Of NamedValue(Of BindMapping)) <=
 _
                    From ProfileItem As BindMapping
                    In LQuery
                    Let name As String = GetName(ProfileItem, ProfileItem.BindProperty)
                    Select New NamedValue(Of BindMapping) With {
                        .Name = name,
                        .x = ProfileItem
                    }

            Dim Nodes = From [property] As PropertyInfo
                        In type.GetProperties
                        Let attributes = [property].GetCustomAttributes(attributeType:=ProfileItemNode, inherit:=False)
                        Where attributes.Length = 1
                        Select New With {
                            .[Property] = [property],
                            .Entity = [property].GetValue(obj, Nothing)
                        } ' 在这里是用匿名类型而不是直接使用Linq的匿名类型的原因是在后面还需要进行赋值操作，而Linq的匿名类型的属性是ReadOnly的

            Dim innerNodes = Nodes.ToArray

            If innerNodes.Length > 0 Then
                For Each x In innerNodes

                    If x.Entity Is Nothing Then
                        Try
                            x.Entity = Activator.CreateInstance(type:=x.[Property].PropertyType)
                        Catch ex As Exception
                            Dim view As String =
                                $"{x.Property.Name} As {x.Property.PropertyType.FullName}"
                            ex = New Exception(view, ex)

                            Throw ex
                        Finally
                            Call x.Property.SetValue(obj, x.Entity, Nothing)
                        End Try
                    End If

                    out += Load(x.[Property].PropertyType, x.Entity)
                Next

                Return out.ToArray
            End If

            Return out.ToArray
        End Function

        Protected Shared Function GetName(ProfileItem As ProfileItem, [Property] As PropertyInfo) As String
            If String.IsNullOrEmpty(ProfileItem.Name) Then
                ProfileItem.Name = [Property].Name.ToLower
            End If
            Return ProfileItem.Name.ToLower
        End Function

        ''' <summary>
        ''' 大小写不敏感
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Node.Exists")>
        Public Overridable Function ExistsNode(Name As String) As Boolean
            Return ProfileItemCollection.ContainsKey(Name.ToLower)
        End Function

        ''' <summary>
        ''' 请注意，<paramref name="name"/>必须是小写的
        ''' </summary>
        ''' <param name="Name">The name of the configuration entry should be in lower case.</param>
        ''' <param name="Value"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("SetValue")>
        Public Overridable Function [Set](Name As String, Value As String) As Boolean
            Dim keyFind As String = Name.ToLower

            If ProfileItemCollection.ContainsKey(keyFind) Then
                Call ProfileItemCollection(keyFind).Set(Value)
            Else
                Return False
            End If
            Return True
        End Function

        <ExportAPI("GetValue")>
        Public Overridable Function GetSettings(Name As String) As String
            Dim keyFind As String = Name.ToLower

            If ProfileItemCollection.ContainsKey(keyFind) Then
                Dim item = ProfileItemCollection(keyFind)
                Dim result = item.Value
                Return result
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        '''假若函数参数<paramref name="name"/>为空，则函数输出所有的变量的值，请注意，这个函数并不在终端上面显示任何消息
        ''' </summary>
        ''' <param name="name">假若本参数为空，则函数输出所有的变量的值，大小写不敏感的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("View")>
        Public Overridable Function View(Optional name As String = "") As String
            If String.IsNullOrEmpty(name) Then
                Return Prints(Me.ProfileItemCollection.Values)
            Else
                Dim data = GetSettingsNode(name)
                Return data.AsOutString
            End If
        End Function

        <ExportAPI("Prints")>
        Public Shared Function Prints(data As IEnumerable(Of BindMapping)) As String
            Dim source As NamedValue(Of String)() =
                LinqAPI.Exec(Of NamedValue(Of String)) <=
 _
                From x As BindMapping
                In data
                Let value As String =
                    If(String.IsNullOrEmpty(x.Value), "null", x.Value)
                Select New NamedValue(Of String) With {
                    .Name = x.Name,
                    .x = value,
                    .Description = x.Description
                }

            Return Prints(source)
        End Function

        <ExportAPI("Prints")>
        Public Shared Function Prints(data As IEnumerable(Of NamedValue(Of String))) As String
            Dim keys As String() = data.ToArray(Function(x) x.Name)
            Dim maxLen As Integer = keys.Select(AddressOf Len).Max
            Dim sb As New StringBuilder(New String("-"c, 120))

            Call sb.AppendLine()

            For Each line As NamedValue(Of String) In data
                Dim blank As String = New String(" "c, maxLen - Len(line.Name) + 2)
                Dim str As String = String.Format("  {0}{1}  = {2}", line.Name, blank, line.x)

                If Not String.IsNullOrEmpty(line.Description) Then
                    str &= "     // " & line.Description
                End If

                Call sb.AppendLine(str)
            Next

            Return sb.ToString
        End Function

        ''' <summary>
        ''' 大小写不敏感的
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("GetNode")>
        Public Function GetSettingsNode(Name As String) As BindMapping
            Return ProfileItemCollection(Name.ToLower)
        End Function

        Public Overrides Function ToString() As String
            Return _SettingsData.FilePath
        End Function

        <ExportAPI("Save")>
        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Dim Xml As String = _SettingsData.GetXml
            Return Xml.SaveTo(getPath(FilePath), Encoding)
        End Function

        Protected Friend Shared ReadOnly Property ProfileItemType As Type = GetType(ProfileItem)
        Protected Friend Shared ReadOnly Property ProfileItemNode As Type = GetType(ProfileNodeItem)

#Region "IDisposable Support"
        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                    Call _SettingsData.Save()
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub
#End Region

        Protected Overrides Function __getDefaultPath() As String
            Return FilePath
        End Function
    End Class
End Namespace
