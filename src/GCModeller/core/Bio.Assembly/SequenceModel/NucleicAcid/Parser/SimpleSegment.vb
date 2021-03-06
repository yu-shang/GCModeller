﻿#Region "Microsoft.VisualBasic::99da3a59398c5f1396503ad7ae478cee, ..\GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Parser\SimpleSegment.vb"

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
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' 没有更多的复杂的继承或者接口实现，只是最简单序列片段对象
    ''' </summary>
    Public Class SimpleSegment : Inherits Contig
        Implements I_PolymerSequenceModel

        ''' <summary>
        ''' Probably synonym, locus_tag data
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String

#Region "Location of this loci"

        Public Property Strand As String
        Public Property Start As Integer
        Public Property Ends As Integer
#End Region

        ''' <summary>
        ''' 当前的这个位点的序列数据
        ''' </summary>
        ''' <returns></returns>
        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData
        ''' <summary>
        ''' The complements sequence of data <see cref="SequenceData"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Complement As String

        Sub New()

        End Sub

        Sub New(loci As SimpleSegment)
            ID = loci.ID
            Strand = loci.Strand
            Start = loci.Start
            Ends = loci.Ends
            SequenceData = loci.SequenceData
            Complement = loci.Complement
        End Sub

        Sub New(loci As SimpleSegment, sId As String)
            Call Me.New(loci)
            ID = sId
        End Sub

        Sub New(site As SimpleSegment, loci As NucleotideLocation)
            Call Me.New(site)

            Start = loci.Left
            Ends = loci.Right
            Strand = loci.Strand.GetBriefCode
        End Sub

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(Start, Ends, Strand).Normalization
        End Function

        Public Function ToPTTGene() As GeneBrief
            Return New GeneBrief With {
                .Code = "-",
                .COG = "-",
                .Gene = ID,
                .IsORF = True,
                .Length = MappingLocation.FragmentSize,
                .Location = MappingLocation,
                .PID = "-",
                .Product = "-",
                .Synonym = ID
            }
        End Function
    End Class
End Namespace
