﻿#Region "Microsoft.VisualBasic::043d2334151610ac8136c6dd29a260b3, ..\interops\meme_suite\MEME\Analysis\VirtualFootprints\MotifDb\Motif.vb"

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
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace Analysis.GenomeMotifFootPrints.MotifDb

    Public Class Motif : Implements sIdEnumerable
        Implements IKeyValuePairObject(Of String, String())

        ''' <summary>
        ''' [Motif].[MotifId]，当前的这个属性值可以唯一的标识一个Motif对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property MotifId As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, String()).Identifier
        <XmlAttribute> Public Property Evalue As Double
        <XmlAttribute> Public Property Width As Integer
        <XmlAttribute> Public Property LogLikelihoodRatio As Double
        <XmlAttribute> Public Property InformationContent As Double
        <XmlAttribute> Public Property RelativeEntropy As Double
        Public Property Family As String

        ''' <summary>
        ''' 这个Motif的序列特征，使用一个正则表达式来表示
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Signature")> <XmlElement>
        Public Property Signature As String
        Public Property MatchedSites As SiteInfo()
        <XmlAttribute> Public Property RegpreciseRegulators As String() Implements IKeyValuePairObject(Of String, String()).Value

        Public Overrides Function ToString() As String
            Return MotifId
        End Function

        Public Shared Function CopyFrom(data As MEME.HTML.Motif, ObjectId As String) As Motif
            Dim Motif As Motif = New Motif With {
                    .Evalue = data.Evalue,
                    .InformationContent = data.InformationContent, .LogLikelihoodRatio = data.LogLikelihoodRatio,
                    .MatchedSites = data.MatchedSites, .MotifId = data.MotifId(ObjectId),
                    .RelativeEntropy = data.RelativeEntropy,
                    .Signature = data.RegularExpression,
                    .Width = data.Width}
            Return Motif
        End Function
    End Class
End Namespace
