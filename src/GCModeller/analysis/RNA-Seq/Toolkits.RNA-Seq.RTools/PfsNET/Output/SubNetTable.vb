﻿Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.PFSNet
Imports SMRUCC.genomics.ComponentModel

Namespace PfsNET.TabularArchives

    ''' <summary>
    ''' 从pfsnet的R输出之中直接解析出来的pfsnet的计算结果
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SubNetTable

        ''' <summary>
        ''' 使用结果文件名来表示
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PhenotypePair As String
        Public Property UniqueId As String
        Public Property Description As String
        Public Property n As Integer
        <Column("flag")> Public Property Flag As Boolean
        Public Property Statistics As Double
        Public Property PValue As Double
        Public Property SignificantGeneObjects As String()

        Public Property weights As Double()
        Public Property weight2 As Double()
        Public Property SubNET_Vector As Double()

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", UniqueId, PhenotypePair)
        End Function

        Protected Friend Function Copy(Of T As SubNetTable)() As T
            Dim Copied As T = Activator.CreateInstance(Of T)()
            Copied.Description = Description
            Copied.Flag = Flag
            Copied.n = n
            Copied.PhenotypePair = PhenotypePair
            Copied.PValue = PValue
            Copied.SignificantGeneObjects = SignificantGeneObjects
            Copied.Statistics = Statistics
            Copied.SubNET_Vector = SubNET_Vector
            Copied.UniqueId = UniqueId
            Copied.weight2 = weight2
            Copied.weights = weights

            Return Copied
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PfsNETResult">结果文件的XML文件名</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(PfsNETResult As String, PathwayBrief As Dictionary(Of String, PathwayBrief)) As SubNetTable()
            Dim ResultSet = PfsNETResult.LoadXml(Of PfsNET())()
            Dim PhenotypeName As String = PfsNETResult.BaseName
            Dim LQuery As SubNetTable() =
                LinqAPI.Exec(Of SubNetTable) <= From x As PfsNET
                                                In ResultSet
                                                Select __creates(x, PhenotypeName, PathwayBrief)
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ResultSet">集合之中的数据必须都是同一个表型类别之下的</param>
        ''' <param name="PhenotypeName"></param>
        ''' <param name="PathwayBrief"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(ResultSet As DataStructure.PFSNetGraph(),
                                            PhenotypeName As String,
                                            PathwayBrief As Dictionary(Of String, PathwayBrief),
                                            [Class] As String) As SubNetTable()

            Dim result = From graph As DataStructure.PFSNetGraph
                         In ResultSet
                         Let subNET = New NetDetails With {
                             .Nodes = (From node In graph.Nodes Select node.Name).ToArray,
                             .weight = New Vector With {
                                .x = (From node In graph.Nodes Select node.weight).ToArray
                             },
                             .weight2 = New Vector With {
                                .x = (From node In graph.Nodes Select node.weight2).ToArray
                             },
                             .Pvalue = graph.pvalue,
                             .statistics = graph.statistics
                         }
                         Select New PfsNET With {
                             .Class = [Class],
                             .Flag = True,
                             .n = graph.Length,
                             .Identifier = graph.Id,
                             .SubNET = subNET
                         }
            Dim LQuery As SubNetTable() =
                LinqAPI.Exec(Of SubNetTable) <= From x As PfsNET
                                                In result
                                                Select __creates(x, PhenotypeName, PathwayBrief)
            Return LQuery
        End Function

        Public Shared Function CreateObject(ResultSet As PfsNET(),
                                            PhenotypeName As String,
                                            PathwayBrief As Dictionary(Of String, ComponentModel.PathwayBrief)) As SubNetTable()

            Dim LQuery As SubNetTable() =
                LinqAPI.Exec(Of SubNetTable) <= From x As PfsNET
                                                In ResultSet
                                                Select __creates(x, PhenotypeName, PathwayBrief)
            Return LQuery
        End Function

        Private Shared Function __creates(net As PfsNET,
                                          PhenotypeName As String,
                                          PathwayBrief As Dictionary(Of String, ComponentModel.PathwayBrief)) As SubNetTable

            Dim tbl As New SubNetTable With {
                .PhenotypePair = PhenotypeName
            }
            tbl.Flag = net.Flag
            tbl.n = net.n
            tbl.PValue = net.SubNET.Pvalue
            tbl.SignificantGeneObjects = net.SubNET.Nodes
            tbl.Statistics = net.SubNET.statistics
            tbl.SubNET_Vector = net.SubNET.Vector.x
            tbl.UniqueId = net.Identifier
            tbl.weight2 = net.SubNET.weight2.x
            tbl.weights = net.SubNET.weight.x
            tbl.Description = PathwayBrief(tbl.UniqueId).Description

            Return tbl
        End Function
    End Class
End Namespace