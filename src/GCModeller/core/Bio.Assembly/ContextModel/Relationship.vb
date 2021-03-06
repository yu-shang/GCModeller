﻿#Region "Microsoft.VisualBasic::407f6a35a2d182a2870ee948f26d12a3, ..\GCModeller\core\Bio.Assembly\ContextModel\Relationship.vb"

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

Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace ContextModel

    ''' <summary>
    ''' 描述位点在基因组上面的位置，可以使用<see cref="ToString"/>函数获取得到位置描述
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class Relationship(Of T As IGeneBrief) : Inherits ClassObject
        Implements IReadOnlyId

        ''' <summary>
        ''' Target gene object
        ''' </summary>
        ''' <returns></returns>
        Public Property Gene As T
        Public Property Relation As SegmentRelationships

        ''' <summary>
        ''' <see cref="IGeneBrief.Identifier"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property locus_tag As String Implements IReadOnlyId.Identity
            Get
                If Gene Is Nothing Then
                    Return ""
                Else
                    Return Gene.Identifier
                End If
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(gene As T, rel As SegmentRelationships)
            Me.Gene = gene
            Me.Relation = rel
        End Sub

        Public Function ATGDist(loci As NucleotideLocation) As Integer
            Dim ATG As Integer
            Dim s As Integer

            If Gene.Location.Strand = Strands.Forward Then
                ATG = Gene.Location.Left
                s = 1
            Else
                ATG = Gene.Location.Right
                s = -1
            End If

            Dim d1 As Integer = loci.Left - ATG
            Dim d2 As Integer = loci.Right - ATG

            If Math.Abs(d1) < Math.Abs(d2) Then
                Return d1 * s
            Else
                Return d2 * s
            End If
        End Function

        ''' <summary>
        ''' Gets loci location relationship description with this <see cref="Gene"/> object.
        ''' (位置关系的描述信息)
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Relation.LocationDescription(Gene)
        End Function
    End Class
End Namespace
