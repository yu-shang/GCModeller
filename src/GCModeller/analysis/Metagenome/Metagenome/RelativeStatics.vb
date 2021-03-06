﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome.gast

Public Module RelativeStatics

    ''' <summary>
    ''' 统计OTU在不同的物种分类层次上面每一个实验样品的相对丰度
    ''' </summary>
    ''' <param name="source">OTU统计数据</param>
    ''' <param name="EXPORT"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ExportByRanks(source As IEnumerable(Of RelativeSample), EXPORT As String) As Boolean
        Dim samples As View() = LinqAPI.Exec(Of View) <=   ' 进行数据视图转换
            From x As RelativeSample
            In source
            Select New View With {
                .OTU = x.OTU,
                .Samples = x.Samples.ToDictionary(
                    Function(o) o.Key,
                    Function(o) o.Value * 100),
                .TaxonTree = New Taxonomy(x.Taxonomy.Split(";"c))
            }

        For Each rank As SeqValue(Of String) In Taxonomy.ranks.SeqIterator   ' 按照rank层次进行计算
            Dim out As String = $"{EXPORT}/{rank.obj}.Csv"
            Dim Groups = (From x As View
                          In samples
                          Let tree As String = x.TaxonTree.GetTree(rank.i)   ' 按照物种树进行数据分组
                          Select x,
                              tree
                          Group By tree Into Group).ToArray
            Dim result As New List(Of RankView)

            For Each g In Groups
                Dim gg As View() = g.Group.ToArray(Function(x) x.x)
                result += New RankView With {
                    .OTUs = gg.ToArray(Function(x) x.OTU),
                    .TaxonomyName = gg.First.TaxonTree(rank.i),
                    .Tree = g.tree,
                    .Samples = (From o As KeyValuePair(Of String, Double)
                                In (From x As View
                                    In gg
                                    Select x.Samples.ToArray).MatrixAsIterator
                                Select o
                                Group o By o.Key Into Group) _
                                     .ToDictionary(Function(x) x.Key,
                                                   Function(x) x.Group.Sum(
                                                   Function(oo) oo.Value) / 100)  ' 计算样品丰度
                }
            Next

            Call result.SaveTo(out)
        Next

        Return True
    End Function

    Public Class RankView

        Public Property OTUs As String()
        Public Property TaxonomyName As String
        Public Property Tree As String
        <Meta(GetType(Double))>
        Public Property Samples As Dictionary(Of String, Double)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class View

        Public Property TaxonTree As Taxonomy
        Public Property Samples As Dictionary(Of String, Double)
        Public Property OTU As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Module
