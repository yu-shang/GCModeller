﻿#Region "Microsoft.VisualBasic::436db9180ad23f9f3b554709d092127a, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\CDD\SmpFile.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.NCBI.CDD

    ''' <summary>
    ''' The file data structrue description of each domain smp description data before the CDD database compilation operation.
    ''' (在对CDD数据库编译之前，每一个结构域对象的单独的数据文件格式)
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("cdd.smp", Namespace:="http://code.google.com/p/genome-in-code/cdd/smp")>
    Public Class SmpFile

#Region "Public Property & Constants"

        Implements sIdEnumerable
        Implements I_PolymerSequenceModel

        <XmlAttribute> Public Property Id As Integer
        <XmlElement> Public Property Title As String
        <XmlElement> Public Property Describes As String

        ''' <summary>
        ''' The sequence data of this conserved structure domain.
        ''' (这个保守的结构域的氨基酸分子序列数据)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlText> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData
        ''' <summary>
        ''' UniqueId
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overridable Property Identifier As String Implements sIdEnumerable.Identifier
        <XmlAttribute("name")> Public Property CommonName As String

        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        Const TAGID_REGX As String = "tag id \d+"
        Const DESCR_REGX As String = "title "".+?""},"
        Const SEQDT_REGX As String = "seq-data .+? ""[A-Z]+"""

#End Region

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", Id, Title)
        End Function

        ''' <summary>
        ''' 从一个smp文件之中导出一个FASTA序列数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Export() As SequenceModel.FASTA.FastaToken
            Dim Title As String = $"{Id}/1-2 {Id}.1 {Identifier}.1;{CommonName};"
            Dim FastaObject As SequenceModel.FASTA.FastaToken =
                New SequenceModel.FASTA.FastaToken With {
                    .SequenceData = SequenceData,
                    .Attributes = New String() {Title}
            }

            Return FastaObject
        End Function

        Public Shared Widening Operator CType(File As String) As SmpFile
            Dim Text As String = __contactLines(IO.File.ReadAllLines(File))
            Dim SmpFile As CDD.SmpFile = New SmpFile With {
                .Id = Val(Regex.Match(Text, TAGID_REGX).Value.Split.Last)
            }
            Dim sTemp As String = Regex.Match(Text, DESCR_REGX).Value
            sTemp = Mid(sTemp, 8, Len(sTemp) - 10)

            Dim p As Integer = InStr(sTemp, ". ")
            Dim IdTokens As String()
            If p > 0 Then
                IdTokens = Strings.Split(Mid(sTemp, 1, Length:=p - 1), ",")
            Else
                IdTokens = Strings.Split(sTemp, ",")
            End If

            SmpFile.Identifier = IdTokens(0)
            SmpFile.CommonName = IdTokens(1).Trim
            SmpFile.Title = IdTokens(2).Trim
            If p > 0 Then
                SmpFile.Describes = Mid(sTemp, p + 2).Trim
                SmpFile.Describes = If(String.IsNullOrEmpty(SmpFile.Describes), SmpFile.Title, SmpFile.Describes)
            Else
                SmpFile.Describes = SmpFile.Title
            End If

            SmpFile.SequenceData = Regex.Match(Text, SEQDT_REGX).Value
            SmpFile.SequenceData = Mid(SmpFile.SequenceData, 18).Replace("""", String.Empty)

            Return SmpFile
        End Operator

        Public Shared Function Load(FilePath As String) As SmpFile
            Return CType(FilePath, SmpFile)
        End Function

        Private Shared Function __contactLines(Data As String()) As String
            Dim sBuilder = New StringBuilder(512 * 1024)

            For Each Line As String In Data
                Line = Line.Trim
                If String.Equals(Line, "intermediateData {") Then Exit For
                Call sBuilder.Append(Line)
            Next

            Return sBuilder.ToString
        End Function
    End Class
End Namespace
