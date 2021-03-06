﻿#Region "Microsoft.VisualBasic::7ca872af596a078827b71c8cc2be3d41, ..\GCModeller\engine\GCTabular\DataVisualization\STrP.vb"

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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports SMRUCC.genomics.Data

Namespace DataVisualization

    ''' <summary>
    ''' 整合String-DB和Regprecise数据进行信号网络的绘制
    ''' </summary>
    ''' <remarks></remarks>
    Public Class STrP

        Dim STrPNetwork As StringDB.StrPNet.Network, TFRegulations As FileStream.TranscriptUnit()

        Sub New(STrPNetwork As StringDB.StrPNet.Network, TFRegulations As FileStream.TranscriptUnit())
            Me.STrPNetwork = STrPNetwork
            Me.TFRegulations = TFRegulations
        End Sub

        Public Sub CreateObjectNetwork(ByRef Interactions As List(Of DataVisualization.Interactions), ByRef NodeAttributes As List(Of DataVisualization.NodeAttributes))
            For Each STrP In STrPNetwork.Pathway
                Call Interactions.AddRange(GenerateNetwork(STrP))
            Next
            For Each Regulation In TFRegulations
                Call Interactions.AddRange(GenerateNetwork(Regulation))
            Next

            Dim NodeAttributeList = New List(Of NodeAttributes)

            For Each STrP In STrPNetwork.Pathway
                Call NodeAttributeList.AddRange(CreateNodeAttributes(STrP))
            Next
            Call NodeAttributes.AddRange(CytoscapeGenerator.TrimNodeAttributes(NodeAttributeList))
        End Sub

        ''' <summary>
        ''' 构建与目标编号的对象有关的网络的集合
        ''' </summary>
        ''' <param name="IdList"></param>
        ''' <param name="ExportDir">函数会导出两个文件，一个用于定义节点的属性，一个用于定义节点之间的相互作用</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateObjectNetwork(IdList As String(), ExportDir As String) As Interactions()
            Dim GetSTrPLQuery = (From Id As String In IdList
                                 Let strp = (From item In STrPNetwork.Pathway Where Exists(Id, item) Select item).ToArray
                                 Where Not strp.IsNullOrEmpty
                                 Select strp).ToArray
            Dim GetRegulation = (From Id As String In IdList
                                 Let regulations = (From item In TFRegulations Where Exists(Id, item) Select item).ToArray
                                 Where Not regulations.IsNullOrEmpty
                                 Select regulations).ToArray

            Dim List As List(Of Interactions) = New List(Of Interactions)
            For Each STrP In GetSTrPLQuery
                For Each item In STrP
                    Call List.AddRange(GenerateNetwork(item))
                Next
            Next
            For Each Regulation In GetRegulation
                For Each item In Regulation
                    Call List.AddRange(GenerateNetwork(item))
                Next
            Next

            Dim NodeAttributes As List(Of NodeAttributes) = New List(Of NodeAttributes)
            For Each item In GetSTrPLQuery
                For Each STrP In item
                    Call NodeAttributes.AddRange(CreateNodeAttributes(STrP))
                Next
            Next
            Call CytoscapeGenerator.TrimNodeAttributes(NodeAttributes).SaveTo(String.Format("{0}/NodeAttributes.csv", ExportDir), False)
            Call List.SaveTo(String.Format("{0}/Edges.csv", ExportDir), False)

            Return List.ToArray
        End Function

        Private Shared Function CreateNodeAttributes(OCS As KeyValuePair()) As NodeAttributes()
            Return (From Item In OCS Select New NodeAttributes With {.Identifier = Item.Key, .NodeType = "OCS"}).ToArray
        End Function

        Private Shared Function CreateNodeAttributes(TCSSystem As StringDB.StrPNet.TCS.TCS()) As NodeAttributes()
            Dim LQuery = (From TCS In TCSSystem Select New NodeAttributes() {New NodeAttributes With {.Identifier = TCS.Chemotaxis, .NodeType = "Chemotaxis"},
                               New NodeAttributes With {.Identifier = TCS.HK, .NodeType = "HK"},
                               New NodeAttributes With {.Identifier = TCS.RR, .NodeType = "RR"}}).ToArray
            Dim ChunkList As List(Of NodeAttributes) = New List(Of NodeAttributes)
            For Each item In LQuery
                Call ChunkList.AddRange(item)
            Next
            Return ChunkList.ToArray
        End Function

        Private Shared Function CreateNodeAttributes(STrP As StringDB.StrPNet.Pathway) As NodeAttributes()
            Dim List As List(Of NodeAttributes) = New List(Of NodeAttributes)
            Call List.AddRange(CreateNodeAttributes(STrP.OCS))
            Call List.AddRange(CreateNodeAttributes(STrP.TCSSystem))
            Call List.Add(New NodeAttributes With {.Identifier = STrP.TF, .NodeType = "TF"})

            Return List.ToArray
        End Function

        Private Shared Function GenerateNetwork(Regulation As FileStream.TranscriptUnit) As Interactions()
            Dim LQuery = (From Id As String In Regulation.OperonGenes Select (From motif As String In Regulation.Motifs Select New Interactions With {.FromNode = motif, .ToNode = Id, .InteractionType = "Regulates"}).ToArray).ToArray.MatrixToVector
            Return LQuery
        End Function

        Private Shared Function GenerateNetwork(objStrP As StringDB.StrPNet.Pathway) As Interactions()
            Dim List As List(Of Interactions) = New List(Of Interactions)
            Dim CreateOCSNetwork = Sub(ocs)
                                       Call List.Add(New Interactions With {.FromNode = ocs.Key, .ToNode = objStrP.TF, .InteractionType = "Signal Transduct"})
                                   End Sub
            Dim CreateTCSNetwork = Sub(tcs As StringDB.StrPNet.TCS.TCS)
                                       Call List.Add(New Interactions With {.FromNode = tcs.Chemotaxis, .ToNode = tcs.HK, .InteractionType = "Phosphorylate Sensing"})
                                       Call List.Add(New Interactions With {.FromNode = tcs.HK, .ToNode = tcs.RR, .InteractionType = "Cross Talk"})
                                       Call List.Add(New Interactions With {.FromNode = tcs.RR, .ToNode = objStrP.TF, .InteractionType = "Signal Transduct"})
                                   End Sub
            If Not objStrP.OCS.IsNullOrEmpty Then
                For Each item In objStrP.OCS
                    Call CreateOCSNetwork(item)
                Next
            End If
            If Not objStrP.TCSSystem.IsNullOrEmpty Then
                For Each item In objStrP.TCSSystem
                    Call CreateTCSNetwork(item)
                Next
            End If

            Return List.ToArray
        End Function

        Public Shared Function Exists(Id As String, objSTrp As StringDB.StrPNet.Pathway) As Boolean
            If String.Equals(Id, objSTrp.TF) Then
                Return True
            Else
                If (From item In objSTrp.TCSSystem Where item.Exists(Id) Select 1).ToArray.Count > 0 Then
                    Return True
                Else
                    Return (From item In objSTrp.OCS Where String.Equals(item.Key, Id) Select 1).ToArray.Count > 0
                End If
            End If
        End Function

        Private Shared Function Exists(Id As String, objRegulation As FileStream.TranscriptUnit) As Boolean
            If String.Equals(objRegulation.Motifs, Id) Then
                Return True
            Else
                Return Array.IndexOf(objRegulation.OperonGenes, Id) > -1
            End If
        End Function
    End Class
End Namespace
