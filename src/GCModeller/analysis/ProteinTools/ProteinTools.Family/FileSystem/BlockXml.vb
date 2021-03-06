﻿#Region "Microsoft.VisualBasic::3ceee96a5a27faee982d982895a5b76f, ..\GCModeller\analysis\ProteinTools\ProteinTools.Family\FileSystem\BlockXml.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Data.Xfam

Namespace FileSystem

    ''' <summary>
    ''' 数据库文件
    ''' </summary>
    Public Class FamilyPfam

        <XmlAttribute> Public Property Build As String
        Public Property Guid As String
        Public Property Title As String
        Public Property Description As String
        Public Property Author As String

        <XmlElement> Public Property Family As Family()
            Get
                Return _families
            End Get
            Set(value As Family())
                _families = value
                If value.IsNullOrEmpty Then
                    _familys = New Dictionary(Of String, Family)
                Else
                    _familys = value.ToDictionary(Function(x) x.Family.ToLower)
                End If
            End Set
        End Property

        Dim _families As Family()
        ''' <summary>
        ''' 小写的键名
        ''' </summary>
        Dim _familys As Dictionary(Of String, Family)

        Public Function Classify(query As Pfam.PfamString.PfamString,
                                 Optional threshold As Double = 0.65,
                                 Optional highlyThreshold As Double = 0.65,
                                 Optional accept As Integer = 10,
                                 Optional parallel As Boolean = True) As String()
            Dim Families As String()

            If parallel Then
                Families = (From fam As Family In Family.AsParallel
                            Let result = fam.IsThisFamily(query, threshold, highlyThreshold, accept)
                            Where result <> FileSystem.Family.MatchStates.NotMatch
                            Select fam.GetName(result)).ToArray
            Else
                Families = (From fam As Family In Family
                            Let result = fam.IsThisFamily(query, threshold, highlyThreshold, accept)
                            Where result <> FileSystem.Family.MatchStates.NotMatch
                            Select fam.GetName(result)).ToArray
            End If

            Dim likes As List(Of String) = (From fm As String
                                            In Families
                                            Where InStr(fm, "-like") > 0
                                            Select fm
                                            Distinct).ToList
            Dim isFamily As String() = (From fm As String
                                        In Families
                                        Where InStr(fm, "-like") = 0
                                        Select fm).ToArray

            For Each name As String In isFamily
                For Each [like] As String In likes.ToArray
                    If InStr([like], name) > 0 Then
                        Call likes.Remove([like])
                    End If
                Next
            Next

            Families = isFamily.Join(likes).ToArray

            Return Families
        End Function

        Public Function IsFamily(PfamString As Pfam.PfamString.PfamString,
                                 Name As String,
                                 Optional threshold As Double = 0.65,
                                 Optional highlyThreshold As Double = 0.65,
                                 Optional accept As Integer = 10) As Family.MatchStates
            Dim Family As Family

            If _familys.ContainsKey(Name.ToLower.ShadowCopy(Name)) Then
                Family = _familys(Name)
            Else
                If _familys.ContainsKey(("*" & Name).ShadowCopy(Name)) Then
                    Family = _familys(Name)
                Else
                    Return False
                End If
            End If

            Return Family.IsThisFamily(PfamString, threshold, highlyThreshold, accept)
        End Function
    End Class
End Namespace
