﻿#Region "Microsoft.VisualBasic::ae93d0f4569662e9d1445182fa0abdc1, ..\GCModeller\engine\GCTabular\Compiler\SignalTransductionNetwork\CrossTalks\CrossTalksAnalysis.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly

Namespace Compiler.Components

    ''' <summary>
    ''' 模块主要是分析出同源的和非同源的CrossTalk，并形成注释数据
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <PackageNamespace("GCModeller.Compiler.Crosstalks.API")>
    Public Module CrossTalksAnalysis

        <ExportAPI("TCS.CrossTalks.Net")>
        Public Function CreateNetwork(data As IEnumerable(Of CrossTalks)) As Network
            Dim Nodes = (From item In data Select New Node With {
                                               .Identifier = item.Kinase,
                                               .NodeType = "Kinase"}).Join((From item In data Select New Node With {.Identifier = item.Regulator, .NodeType = "Response Regulator"}).ToArray)
            Dim Edges = (From item In data Select New NetworkEdge With {
                                               .FromNode = item.Kinase,
                                               .ToNode = item.Regulator,
                                               .Confidence = item.Probability,
                                               .InteractionType = "CrossTalk"}).ToArray
            Dim Network As Network = New Network With {
                .Nodes = Nodes.ToArray,
                .Edges = Edges
            }
            Return Network
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <param name="CrossTalks">第一行为RR基因，第一列为HisK基因</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Analysis")>
        Public Function Analysis(Model As FileStream.IO.XmlresxLoader, CrossTalks As DocumentStream.File) As CrossTalks()
            Dim RR = CrossTalks.First.Skip(1).ToArray
            Dim p_cache = RR.Sequence
            Dim LQuery = (From line As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                          In CrossTalks.Skip(1).ToArray
                          Let hisk As String = line.First
                          Let scores = line.Skip(1).ToArray
                          Select (From p As Integer In p_cache Select New CrossTalks With {.Regulator = RR(p), .Probability = Val(scores(p)), .Kinase = hisk}).ToArray).ToArray.MatrixToList

            Dim HisKinase As String() = Model.MisT2.MajorModules.First.TwoComponent.get_HisKinase
            RR = Model.MisT2.MajorModules.First.TwoComponent.GetRR

            For Each Operon In Model.DoorOperon '分析同源的CrossTalk

                For Each Gene As String In Operon.Genes
                    If Array.IndexOf(HisKinase, Gene) > -1 Then
                        Dim RR_LQuery = (From id As String In Operon.Genes Where Array.IndexOf(RR, id) > -1 Select id).ToArray
                        Call LQuery.AddRange((From RR_ID As String In RR_LQuery Select New CrossTalks With {.Kinase = Gene, .Regulator = RR_ID, .Probability = 1}).ToArray)
                    ElseIf Array.IndexOf(RR, Gene) > -1 Then
                        Dim HisK_LQuery = (From id As String In Operon.Genes Where Array.IndexOf(HisKinase, id) > -1 Select id).ToArray
                        Call LQuery.AddRange((From HK_ID As String In HisK_LQuery Select New CrossTalks With {.Kinase = HK_ID, .Regulator = Gene, .Probability = 1}).ToArray)
                    End If
                Next
            Next

            Dim ChunkBuffer = (From item In LQuery Where item.Probability <> 0.0R Select item.get_InternalGUID, item Group By get_InternalGUID Into Group).ToArray '去除可能的重复的同源的数据
            Return (From item In ChunkBuffer Let Distincted As CrossTalks = item.Group.First.item Select Distincted).ToArray
        End Function
    End Module
End Namespace
