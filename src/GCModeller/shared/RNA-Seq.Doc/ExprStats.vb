﻿#Region "Microsoft.VisualBasic::42c3e374cf2e788ee63a514c559fd5cc, ..\GCModeller\analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\RNA-Seq.Doc\ExprStats.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq

Namespace DESeq2

    Public Class ExprStats : Implements IDynamicMeta(Of String), IReadOnlyId, sIdEnumerable

        Public Const LEVEL As String = "[Level]"
        Public Const STATS As String = "[stat]"
        ''' <summary>
        ''' 样品值与最大值的百分比
        ''' </summary>
        Public Const LEVEL2 As String = "[Level2]"

        Public Property locus As String Implements IReadOnlyId.Identity, sIdEnumerable.Identifier
        Public Property log2FoldChange As Double

        ''' <summary>
        ''' <see cref="Level">[Level]</see> 表达等级映射;
        ''' [stat] 状态枚举: + 活跃，- 休眠
        ''' </summary>
        ''' <returns></returns>
        <Meta(GetType(String))>
        Public Property Samples As Dictionary(Of String, String) Implements IDynamicMeta(Of String).Properties

        Public Function GetMaxLevel() As Integer
            Dim LQuery = (From x In Samples
                          Where InStr(x.Key, LEVEL) = 1
                          Select x.Key,
                              data = Val(x.Value)).ToArray
            Return LQuery.ToArray(Function(x) x.data).Max
        End Function

        ''' <summary>
        ''' 获取同一个样本实验里面的等级映射的结果
        ''' </summary>
        ''' <param name="sample"></param>
        ''' <returns></returns>
        Public Function GetLevel(sample As String) As Integer
            Dim name As String = ExprStats.LEVEL & sample
            Return CInt(Samples(name))
        End Function

        ''' <summary>
        ''' 获取本基因对象在不同的实验样本之间计算出来的的相对活跃度
        ''' </summary>
        ''' <param name="sample"></param>
        ''' <returns></returns>
        Public Function GetLevel2(sample As String) As Double
            Dim name As String = ExprStats.LEVEL2 & sample
            Return Scripting.CastDouble(Samples(name))
        End Function

        ''' <summary>
        ''' 当前的这个基因在指定的条件下是否处于表达状态？
        ''' </summary>
        ''' <param name="sample"></param>
        ''' <returns></returns>
        Public Function IsActive(sample As String) As Boolean
            Dim name As String = ExprStats.STATS & sample
            If Samples.ContainsKey(name) Then
                Return InStr(Samples(name), "+") = 1
            Else
                Dim ex As New Exception("Name: " & name)
                Dim lst = Samples.Keys.ToArray(Function(x) $"[{x}]")
                ex = New Exception("Keys:  " & lst.JoinBy("+"), ex)
                Throw ex
            End If
        End Function

        ''' <summary>
        ''' 当前的这个基因在指定的条件下是否处于低表达状态？
        ''' </summary>
        ''' <param name="sample"></param>
        ''' <returns></returns>
        Public Function IsLowExpression(sample As String) As Boolean
            Dim name As String = ExprStats.STATS & sample
            Return String.Equals(Samples(name), "+?")
        End Function

        Public Overrides Function ToString() As String
            Return locus
        End Function
    End Class
End Namespace
