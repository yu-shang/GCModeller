﻿#Region "Microsoft.VisualBasic::f2fcaa567933079396a14bce85bd6442, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\ToolsAPI\PartitioningData.vb"

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

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' The genome partitional data.(基因组分区数据)
''' </summary>
''' <remarks></remarks>
Public Class PartitioningData : Implements IAbstractFastaToken

    ''' <summary>
    ''' 当前的功能分区的标签信息
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Tag")> Public Property PartitioningTag As String
    ''' <summary>
    ''' 分区的起始位置
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Loci.Sp")> Public Property LociLeft As Integer
    ''' <summary>
    ''' 分区的结束位置
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Loci.St")> Public Property LociRight As Integer
    Public Property ORFList As String()
    Public Property GenomeID As String
    Public ReadOnly Property Length As Integer
        Get
            If LociRight > LociLeft Then
                Return LociRight - LociLeft
            Else
                'Join Locations
                Return Len(SequenceData)
            End If
        End Get
    End Property

    Public ReadOnly Property GC As Double
        Get
            Return NucleotideModels.GCContent(Me)
        End Get
    End Property

    Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
        Get
            Return String.Format("{0} ({1})", GenomeID, PartitioningTag)
        End Get
    End Property

    ''' <summary>
    ''' 当前的基因组分区之中的基因序列
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Ignored> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

    Public Property Attributes As String() Implements IAbstractFastaToken.Attributes

    Public Overrides Function ToString() As String
        Return Title & ":  " & SequenceData
    End Function

    Public Function ToFasta() As FastaToken
        Return New FastaToken With {
            .SequenceData = SequenceData,
            .Attributes = New String() {Me.GenomeID, Me.PartitioningTag}
        }
    End Function
End Class

