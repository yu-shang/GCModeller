﻿#Region "Microsoft.VisualBasic::0c24f1ec9791b1dd765456dc1b11d600, ..\GCModeller\analysis\RNA-Seq\Toolkits.RNA-Seq\Matrix\ExprSamples.vb"

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
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace dataExprMAT

    ''' <summary>
    ''' 每一个基因的表达量的实验样本，{GeneId, value1, value2, value3, ...}
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExprSamples : Inherits Streams.Array.Double
        Implements IKeyValuePairObject(Of String, Double())
        Implements sIdEnumerable, IEnumerable(Of Double)

        Public Sub New()
        End Sub

        Sub New(locus As String, samples As IEnumerable(Of Double))
            locusId = locus
            Values = samples.ToArray
        End Sub

        <XmlAttribute("Id")>
        Public Property locusId As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, Double()).Identifier
        Public Overrides Property Values As Double() Implements IKeyValuePairObject(Of String, Double()).Value

        Public Overrides Function ToString() As String
            Return $"{locusId} --> {Values.GetJson}"
        End Function

        ''' <summary>
        ''' Convert the data line in the csv file into a object model in the pcc matrix(将Csv文件之中的数据行转换为对象模型)
        ''' </summary>
        ''' <param name="rowData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ConvertObject(rowData As RowObject) As ExprSamples
            Dim samples As Double() = (From c As String In rowData.Skip(1) Select Val(c)).ToArray
            Return New ExprSamples With {
                .locusId = rowData.First,
                .Values = samples
            }
        End Function

        Public Function ToRow() As RowObject
            Dim row As RowObject = New RowObject(Me.locusId + Values.ToList(Function(x) CStr(x)))
            Return row
        End Function

        ''' <summary>
        ''' Convert the Pcc matrix object into a csv document for save the data into filesystem.(将Pcc矩阵转换为Csv数据文件以进行保存)
        ''' </summary>
        ''' <param name="DataSet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateFile(DataSet As IEnumerable(Of ExprSamples)) As File
            Dim Table As List(Of RowObject) = (From item As ExprSamples In DataSet Select item.ToRow).ToList
            Dim FirstRow As RowObject = New RowObject((From item As ExprSamples In DataSet Select item.locusId).ToArray)
            Dim File As File = New File(FirstRow + Table)
            Return File
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Double) Implements IEnumerable(Of Double).GetEnumerator
            For Each n As Double In Values
                Yield n
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
