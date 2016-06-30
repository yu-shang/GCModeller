﻿Imports System.Security.Cryptography
Imports System.Text
Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.ComponentModel

''' <summary>
''' Extension method collection for the object comes from the .NET Framework in this Modelling Engine.
''' (整个模型引擎中的通用来自于.NET的对象的拓展方法的集合)
''' </summary>
''' <remarks></remarks>
Module ExtensionCommons

    Public Const TOLERANCE As Double = 10 ^ -10

    ''' <summary>
    ''' MD5 hash string calculation function of a specific string, modified from MSDN forum.
    ''' (字符串的MD5哈希字符串的计算函数，修改自MSDN论坛中的一个例子)  
    ''' </summary>
    ''' <param name="input">Target string to computed.(被计算的目标字符串)</param>
    ''' <returns>MD5 hash string.(MD5哈希字符串)</returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetMd5Hash( input As String) As String
        Dim sBuilder As New StringBuilder(32)
        Dim md5Hasher As MD5 = MD5.Create()
        Dim Data As Byte()

        Data = Encoding.Unicode.GetBytes(input)
        Data = md5Hasher.ComputeHash(Data)

        For i As Integer = 0 To Data.Length - 1
            sBuilder.Append(Data(i).ToString("x2"))
        Next

        Return sBuilder.ToString()
    End Function

    ''' <summary>
    ''' The Verification operation returns a failure result.
    ''' (数据校验方法返回了校验错误这个结果)
    ''' </summary>
    ''' <remarks></remarks>
    Public Const FAILURE As Boolean = False

    <Extension> Public Function Description([Class] As Guid.Classes) As String
        Select Case [Class]
            Case Guid.Classes.Entity : Return "f372bcf3"
            Case Guid.Classes.Feature : Return "e0935a5e"
            Case Guid.Classes.Module : Return "9fb60662"
            Case Else : Return "9fb60662"
        End Select
    End Function
End Module