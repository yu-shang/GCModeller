﻿#Region "Microsoft.VisualBasic::56596f770719239e3ad3f6d2bc7a8918, ..\GCModeller\core\Bio.Assembly\SequenceModel\MWCalculator.vb"

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

Imports SMRUCC.genomics.SequenceModel.Polypeptides.Polypeptides
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SequenceModel

    ''' <summary>
    ''' 核酸链或者多肽链分子的相对分子质量的计算工具
    ''' </summary>
    ''' <remarks></remarks>
    <PackageNamespace("MolecularWeights", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module MolecularWeightCalculator

        Private ReadOnly AminoAcidMolecularWeights As SortedDictionary(Of AminoAcid, Double) =
            New SortedDictionary(Of Polypeptides.Polypeptides.AminoAcid, Double) From {
                {Polypeptides.Polypeptides.AminoAcid.Alanine, 71.0779},
                {Polypeptides.Polypeptides.AminoAcid.Arginine, 156.1857},
                {Polypeptides.Polypeptides.AminoAcid.Asparagine, 114.1026},
                {Polypeptides.Polypeptides.AminoAcid.AsparticAcid, 115.0874},
                {Polypeptides.Polypeptides.AminoAcid.Cysteine, 103.1429},
                {Polypeptides.Polypeptides.AminoAcid.GlutamicAcid, 129.114},
                {Polypeptides.Polypeptides.AminoAcid.Glutamine, 128.1292},
                {Polypeptides.Polypeptides.AminoAcid.Glycine, 57.0513},
                {Polypeptides.Polypeptides.AminoAcid.Histidine, 137.1393},
                {Polypeptides.Polypeptides.AminoAcid.Isoleucine, 113.1576},
                {Polypeptides.Polypeptides.AminoAcid.Leucine, 113.1576},
                {Polypeptides.Polypeptides.AminoAcid.Lysine, 128.1723},
                {Polypeptides.Polypeptides.AminoAcid.Methionine, 131.1961},
                {Polypeptides.Polypeptides.AminoAcid.Phenylalanine, 147.1739},
                {Polypeptides.Polypeptides.AminoAcid.Praline, 97.1152},
                {Polypeptides.Polypeptides.AminoAcid.Serine, 87.0773},
                {Polypeptides.Polypeptides.AminoAcid.Threonine, 101.1039},
                {Polypeptides.Polypeptides.AminoAcid.Tryptophane, 186.2099},
                {Polypeptides.Polypeptides.AminoAcid.Tyrosine, 163.1733},
                {Polypeptides.Polypeptides.AminoAcid.Valine, 99.1311}}

        Private ReadOnly NucleicAcidsMolecularWeights As SortedDictionary(Of Char, Double) =
            New SortedDictionary(Of Char, Double) From {
                {"A"c, 491.2},
                {"C"c, 467.2},
                {"G"c, 507.2},
                {"T"c, 482.2},
                {"U"c, 324.2}}

        ''' <summary>
        ''' 计算蛋白质序列的相对分子质量
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <returns></returns>
        <ExportAPI("MW.Polypeptide")>
        Public Function CalcMW_Polypeptide(SequenceData As ISequenceModel) As Double
            Dim Polypeptide = Polypeptides.Polypeptides.ConstructVector(SequenceData.SequenceData)
            Dim LQuery = (From aa In Polypeptide Select AminoAcidMolecularWeights(aa)).Sum
            Return LQuery
        End Function

        <ExportAPI("MW.Polypeptide")>
        Public Function CalcMW_Polypeptide(SequenceData As String) As Double
            Dim Polypeptide = Polypeptides.Polypeptides.ConstructVector(SequenceData)
            Dim LQuery = (From aa In Polypeptide Select AminoAcidMolecularWeights(aa)).Sum
            Return LQuery
        End Function

        ''' <summary>
        ''' 计算核酸序列的相对分子质量
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <returns></returns>
        <ExportAPI("MW.NT")>
        Public Function CalcMW_Nucleotides(SequenceData As ISequenceModel) As Double
            Dim LQuery As Double = (From ch As Char
                                    In SequenceData.SequenceData.ToUpper
                                    Select NucleicAcidsMolecularWeights(ch)).Sum
            Return LQuery
        End Function
    End Module
End Namespace
