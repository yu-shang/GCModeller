﻿#Region "Microsoft.VisualBasic::acc93205f22754f9a126e1b6a2600365, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\ExportServices\GenbankExportInfo.vb"

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

Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.NCBI.GenBank.CsvExports

    Public Class GenbankExportInfo(Of T As GenBank.CsvExports.gbEntryBrief)

#Region "Csv data storage region."

        Dim _entryInfo As Microsoft.VisualBasic.ComponentModel.LazyLoader(Of T(), String)
        Dim _ORFInfo As Microsoft.VisualBasic.ComponentModel.LazyLoader(Of GeneDumpInfo(), String)

        Public Property EntryInfo As T()
            Get
                Return _entryInfo.Value
            End Get
            Set(value As T())
                _entryInfo.Value = value
            End Set
        End Property

        Public Property ORFInfo As GeneDumpInfo()
            Get
                Return _ORFInfo.Value
            End Get
            Set(value As GeneDumpInfo())
                _ORFInfo.Value = value
            End Set
        End Property
#End Region

#Region "Total fasta sequence info storage region."

        Public ReadOnly Property ORF As FASTA.FastaFile
            Get
                Return FASTA.FastaFile.Read(_root & "/orf.fasta")
            End Get
        End Property

        Public ReadOnly Property Gene As FASTA.FastaFile
            Get
                Return FASTA.FastaFile.Read(_root & "/genes.fasta")
            End Get
        End Property

        Public ReadOnly Property Genome As FASTA.FastaFile
            Get
                Return FASTA.FastaFile.Read(_root & "/genomes.fasta")
            End Get
        End Property
#End Region

        Public Delegate Function GenbankEntryInfoLoadMethod(Path As String) As T()
        Public Delegate Function ORFInfoLoadMethod(Path As String) As GeneDumpInfo()

        ''' <summary>
        ''' Repository root for this export source.
        ''' </summary>
        ''' <remarks></remarks>
        Dim _root As String

        ''' <summary>
        ''' 根目录
        ''' </summary>
        ''' <param name="Root"></param>
        ''' <remarks></remarks>
        Sub New(Root As String, GenbankEntryInfoLoad As GenbankEntryInfoLoadMethod, ORFInfoLoad As ORFInfoLoadMethod)
            _root = Root
            _entryInfo = New Microsoft.VisualBasic.ComponentModel.LazyLoader(Of T(), String)(Root & "/genbank.info.csv", Function(path) GenbankEntryInfoLoad(path))
            _ORFInfo = New Microsoft.VisualBasic.ComponentModel.LazyLoader(Of GeneDumpInfo(), String)(Root & "/genbank.orf.csv", Function(path) ORFInfoLoad(path))
        End Sub

        Public ReadOnly Property ORFSource As Dictionary(Of String, String)
            Get
                Return (_root & "/ORF/").LoadSourceEntryList
            End Get
        End Property

        Public ReadOnly Property GeneSource As Dictionary(Of String, String)
            Get
                Return (_root & "/Gene/").LoadSourceEntryList
            End Get
        End Property

        Public ReadOnly Property GenomeSource As Dictionary(Of String, String)
            Get
                Return (_root & "/Genomes/").LoadSourceEntryList
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return _root
        End Function
    End Class
End Namespace
