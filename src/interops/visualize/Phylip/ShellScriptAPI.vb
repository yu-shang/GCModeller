﻿Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language.UnixBash

Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Phylip.MatrixFile

Imports PathEntry = System.Collections.Generic.KeyValuePair(Of String, String)
Imports System.Text.RegularExpressions
Imports System.Text
Imports LANS.SystemsBiology.NCBI.Extensions.Analysis
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports Microsoft.VisualBasic.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Terminal.Utility
Imports Microsoft.VisualBasic.Parallel
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports
Imports LANS.SystemsBiology.Localblast.Extensions.VennDiagram.BlastAPI
Imports Microsoft.VisualBasic.Parallel.Linq

<[PackageNamespace]("Phylip.Matrix",
                    Cites:="PLOTREE, D. and D. PLOTGRAM (1989). ""PHYLIP-phylogeny inference package (version 3.2).""",
                    Publisher:="amethyst.asuka@gcmodeller.org",
                    Category:=APICategories.ResearchTools)>
Public Module ShellScriptAPI

    ''' <summary>
    ''' {Trimmed_ID, uid}
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Label.Annotation.Generate")>
    Public Function CreateNodeLabelAnnotation(<Parameter("Dir.SourceInput")> PttSource As String,
                                              <Parameter("Path.OutTree")> TreeFile As String,
                                              <Parameter("Genbank.BBH.Entry", "This entries list information data should be the distincted data " &
                                                  "using as the bbh data source for the phylip tree construction.")>
                                              EntryData As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief)) _
        As <FunctionReturns("The node label in the output tree has been trimmed and replaced using the ptt entry annotation on the ncbi FTP website.")> String

        Dim OutTree As StringBuilder = New StringBuilder(FileIO.FileSystem.ReadAllText(TreeFile))
        Dim TreeNodeLabels = (From m As Match In Regex.Matches(OutTree.ToString, "[a-z]+_?\d+", RegexOptions.IgnoreCase) Select m.Value).ToArray
        Dim EntriesList = (From Dir As String
                           In FileIO.FileSystem.GetDirectories(PttSource, FileIO.SearchOption.SearchTopLevelOnly).AsParallel
                           Let GenbankList = Dir.LoadSourceEntryList({"*.ptt"})
                           Select FileIO.FileSystem.GetDirectoryInfo(Dir).Name,
                               Genbanks = (From File As PathEntry In GenbankList Let ID = File.Key Select ID).ToArray)
        Dim EntryLQuery = (From Entry In EntriesList
                           Let InternalLinkLabel = __linkLabel(Entry.Name, Entry.Genbanks)
                           Where Not InternalLinkLabel.IsNullOrEmpty
                           Select InternalLinkLabel).MatrixToList
        For Each NodeLabel As String In TreeNodeLabels
            Dim FindEntry = (From Entry In EntryLQuery.AsParallel Where InStr(Entry.Key, NodeLabel) = 1 Select Entry).ToArray
            '还必须要在Csv去重复的数据源之中存在
            FindEntry = (From Entry As TripleKeyValuesPair
                         In FindEntry
                         Where Not (From Line In EntryData.AsParallel
                                    Where String.Equals(Line.Locus, Entry.Value1, StringComparison.OrdinalIgnoreCase)
                                    Select Line).ToArray.IsNullOrEmpty
                         Select Entry).ToArray
            If Not FindEntry.IsNullOrEmpty Then
                Call OutTree.Replace(NodeLabel, FindEntry.First.Value2)
            End If
        Next

        Return OutTree.ToString
    End Function

    Private Function __linkLabel(entryName As String, genbanks As String()) As TripleKeyValuesPair()
        If genbanks.IsNullOrEmpty Then
            Return Nothing
        End If
        If genbanks.Length = 1 Then
            Return {New TripleKeyValuesPair(genbanks(Scan0), genbanks(Scan0), entryName & "(" & genbanks.First & ")")}
        Else
            Return (From ID As String
                    In genbanks
                    Select New TripleKeyValuesPair(ID.__labelTrimming, ID, entryName & "(" & ID & ")")).ToArray
        End If
    End Function

    <Extension> Private Function __labelTrimming(Id As String) As String
        If Len(Id) > 10 Then
            Return Mid(Id, 1, 10)
        Else
            Return Id
        End If
    End Function

    <ExportAPI("Load.Gendist")>
    Public Function LoadGendist(Path As String) As MatrixFile.Gendist
        Return MatrixFile.Gendist.LoadDocument(Path)
    End Function

    <ExportAPI("SubMatrix")>
    Public Function SubMatrix(MAT As MatrixFile.Gendist, Count As Integer, MainIndex As String) As MatrixFile.Gendist
        Return MAT.SubMatrix(Count, MainIndex)
    End Function

    <ExportAPI("MotifDist.Create")>
    Public Function CreateMotifDist(dat As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Gendist
        Return MatrixFile.Gendist.CreateMotifDistrMAT(dat)
    End Function

    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As MatrixFile.MatrixFile, saveto As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveto)
    End Function

    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As Gendist, saveto As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveto)
    End Function

    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As MatrixFile.NeighborMatrix, saveto As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveto)
    End Function

    <ExportAPI("Load.NewickTree", Info:="Newick format tree is the phylip default output data format.")>
    Public Function LoadNewickTree(tree As String, Optional Name As String = "Evol Tree") As Evolview.PhyloTree
        Dim TreeObject = New Evolview.PhyloTree(Name, tree, "newick")
        Return TreeObject
    End Function

    <ExportAPI("Gendist.Create")>
    Public Function CreateGeneDist(path As String) As Gendist
        Dim Csv = DocumentStream.File.Load(path)
        Return MatrixFile.Gendist.CreateMotifDistrMAT(Csv)
    End Function

    <ExportAPI("Neighbor.Create")>
    Public Function CreateNeighborMatrixFromVennMatrix(path As String, Optional fastLoad As Boolean = True) As MatrixFile.NeighborMatrix
        Call $"Start to load venn matrix data from file: {path.ToFileURL}".__DEBUG_ECHO

        Dim Csv As DocumentStream.File = If(fastLoad, DocumentStream.File.FastLoad(path), DocumentStream.File.Load(path))

        Call $"Venn matrix data load Job done!".__DEBUG_ECHO

        path = $"{path}.Neighbor.csv"
        Call $"Temp matrix was saved at {path.ToFileURL}".__DEBUG_ECHO
        Call Csv.Save(path, False)

        Return NeighborMatrixFromVennMatrix(Csv)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="source">Xml数据的文件夹</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Load.Xml.Besthit.MetaSource")>
    Public Function LoadHitsVennData(<Parameter("DIR.Source",
                                                "The directory which contains the compiled bbh besthit xml data file.")>
                                     source As String) As <FunctionReturns("")> BestHit()
        Dim resHash As Dictionary(Of String, String) = source.LoadSourceEntryList({"*.xml"})
        Dim proc As EventProc = resHash.LinqProc
        Dim LQuery = (From i As SeqValue(Of PathEntry) In resHash.SeqIterator
                      Let path As PathEntry = i.obj
                      Let d As Integer = proc.Tick
                      Select LoadXmlMeta(path)).ToArray

        Call Console.WriteLine("The besthit meta file data load done!")

        Return LQuery
    End Function

    Private Function LoadXmlMeta(Path As PathEntry) As BestHit
        Call $"Start to load data from {Path.Value.ToFileURL}....".__DEBUG_ECHO
        Dim Sw = Stopwatch.StartNew
        Dim Meta = Path.Value.LoadXml(Of BestHit)
        Call $"Data load done!  /// {Sw.ElapsedMilliseconds} ms.".__DEBUG_ECHO

        Return Meta
    End Function

    ''' <summary>
    ''' 矩阵文件的格式要求为：
    ''' 行的标题（每一行中的第一个元素）为基因组的名称
    ''' 每一列为某一个基因的频率或者其他数值
    ''' 例如：
    '''         基因1，基因2，基因3， ...
    ''' 基因组1   1     1      0
    ''' 基因组2   2     1      4
    ''' </summary>
    ''' <param name="besthit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __exportMatrix(besthit As BestHit) As DocumentStream.File
        Dim MAT As DocumentStream.File = New DocumentStream.File
        Dim hits = besthit.InternalSort(False).ToArray

        hits = (From hit As HitCollection
                In hits
                Select prot = hit
                Order By prot.QueryName Ascending).ToArray  '对Query的蛋白质编号进行排序

        On Error Resume Next

        Dim head As New DocumentStream.RowObject("QueryProtein" + hits.First.Hits.ToList(Function(x) x.tag))  '生成表头
        MAT += head

        For Each hit As HitCollection In hits
            Dim Row As New DocumentStream.RowObject(hit.QueryName + hit.Hits.ToList(Function(x) CStr(x.Identities)))
            MAT += Row
        Next

        Return MAT
    End Function

    ''' <summary>
    ''' 直接使用identities值作为最开始的等位基因的频率值
    ''' </summary>
    ''' <param name="MetaSource"></param>
    ''' <param name="Limits">0或者小于零的数值都为不限制,假设做出数量的限制的话，函数只会提取指定数目的基因组数据，都是和外标尺最接近的基因组</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Gendist.Matrix.Generate")>
    <Extension>
    Public Function ExportGendistMatrixFromBesthitMeta(<Parameter("BBH.Meta.Source")> MetaSource As IEnumerable(Of BestHit),
                                                       <Parameter("Index.Main")> Optional MainIndex As String = "",
                                                       <Parameter("Null.Trim")> Optional TrimNull As Boolean = False,
                                                       <Parameter("Limits", "Any value number for limits < 1 is no limits on the export genome number.")>
                                                       Optional Limits As Integer = -1) As <FunctionReturns("")> Gendist

        Call Console.WriteLine("Limits " & Limits)

        Dim DataDict = MetaSource.Randomize.ToDictionary(Function(item) item.sp)
        Dim IndexKey = DataDict.Keys(VennDataModel.__parserIndex(DataDict, MainIndex))
        Dim MainData = DataDict(IndexKey)

        Call DataDict.Remove(IndexKey)

        If MainData.hits.IsNullOrEmpty Then
            Call $"The profile data of your key ""{MainIndex}"" ---> ""{MainData.sp}"" is null!".__DEBUG_ECHO
            Call $"Thread exists...".__DEBUG_ECHO
            Return Nothing
        Else
            Call $"Main index data has {MainData.hits.Length} hits...".__DEBUG_ECHO
        End If

        If Limits > 1 Then
            Dim ChunTemp = __sort(DataDict, MainData)
            ChunTemp = ChunTemp.Take(Limits).ToArray
            Call $"The output genome data was limited of counts {ChunTemp.Length}".__DEBUG_ECHO
            DataDict = ChunTemp.ToDictionary(Function(obj) obj.sp)
            MainData = MainData.Take(DataDict.Keys.ToArray)
        End If

        Dim MAT As DocumentStream.File = __exportMatrix(MainData)
        Dim species As String() = (From hitData As Hit In MainData.hits.First.Hits Select hitData.tag).ToArray

        For deltaInd As Integer = 0 To DataDict.Count - 1
            Dim subMain As BestHit = DataDict.Values(deltaInd)

            If subMain.hits.IsNullOrEmpty Then
                Call $"Profile data {subMain.sp} is null!".__DEBUG_ECHO
                Continue For
            Else
                Call Console.WriteLine(" {0} > " & subMain.sp, deltaInd)
            End If

            Dim di As Integer = deltaInd
            Dim subMainMatched = (From row In MAT Let d = 2 + di + 1 Let id As String = row(d) Where Not String.IsNullOrEmpty(id) Select id).ToArray
            Dim notmatched = (From hit As HitCollection
                              In subMain.hits
                              Where Array.IndexOf(subMainMatched, hit.QueryName) = -1
                              Select hit.QueryName,
                                  hit.Description,
                                  speciesProfile = hit.Hits.ToDictionary(Function(prot) prot.tag))

            For Each SubMainNotHitGene In notmatched  '竖直方向遍历第n列的基因号
                Dim row As New DocumentStream.RowObject From {SubMainNotHitGene.QueryName}

                Call row.AddRange((From nnn In (deltaInd).Sequence Select "0").ToArray)

                For Each sid As String In species.Skip(deltaInd)
                    Dim matched = SubMainNotHitGene.speciesProfile(sid)
                    Call row.Add(matched.Identities)
                Next
                Call MAT.Add(row)
            Next
        Next

        Dim IDChunkBuffer = (From data In DataDict.Values Select data.sp).ToList + MainData.sp
        Call IO.File.WriteAllLines("./MAT_ID.txt", IDChunkBuffer.ToArray()) '会同时输出矩阵之中的基因组的NCBI编号以方便后面的分析
        Call MAT.Save("./VennMatrix.csv", False)
        Call Console.WriteLine("Export data job done! start to create matrix!")

        Dim StringCollection = (From row In MAT Select row.ToArray).ToArray
        StringCollection = StringCollection.MatrixTranspose
        MAT = New DocumentStream.File(From row In StringCollection Select CType(row, DocumentStream.RowObject))

        Return MatrixFile.Gendist.CreateMotifDistrMAT(MAT)
    End Function

    Private Function __sort(MAT As Dictionary(Of String, BestHit), MainData As BestHit) As BestHit()
        Dim ids As String() = MainData.GetTopHits
        Dim buf As BestHit() = (From IDtag As String
                                In ids
                                Where MAT.ContainsKey(IDtag)
                                Select MAT(IDtag)).ToArray
        Return buf
    End Function

    <ExportAPI("Label.fasta_nno")>
    Public Function TreeLabelFastaReplace(Tree As String, genome As IEnumerable(Of gbEntryBrief)) As String
        Dim Replacement As New StringBuilder(Tree)

        For Each Gen As gbEntryBrief In genome
            Dim Def As String = Gen.Definition.Replace(",", " ").Replace(".", " ").Replace("  ", " ").Trim
            Call Replacement.Replace(Gen.AccessionID, Def)
        Next

        Return Replacement.ToString
    End Function

    ''' <summary>
    ''' 从已经生成的韦恩矩阵之中生成距离矩阵
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("Neighbor.From.VennMatrix")>
    Public Function NeighborMatrixFromVennMatrix(VennMatrix As DocumentStream.File) As MatrixFile.NeighborMatrix

        Call Console.WriteLine("Start to preparing data matrix...")

        Dim Data = (From col As String()
                    In VennMatrix.Columns.Skip(1).AsParallel
                    Let ID As String = col(Scan0),
                        genElements As Double() = (From p As String In col.Skip(1) Select Val(p)).ToArray
                    Select ID,
                        genElements).ToArray
        '默认使用欧几里得距离
        '为了防止数据混乱，这里不再使用并行拓展，以保持两两对应的顺序
        Dim Head As New DocumentStream.RowObject("" + (From sp In Data Select sp.ID).ToList)
        Dim MatBuilder As DocumentStream.File = New DocumentStream.File + Head

        Call Console.WriteLine("Start to generate matrix file")

        For Each sp In Data
            Dim row As New DocumentStream.RowObject
            Call row.Add(sp.ID)

            For Each paired In Data '对角线是自己对自己，距离总是为零
                Call row.Add(sp.genElements.EuclideanDistance(paired.genElements))
            Next
            Call MatBuilder.Add(row)
            Call Console.Write(".")
        Next

        Call Console.WriteLine("Job done!")

        Return NeighborMatrix.CreateObject(MatBuilder)
    End Function

    <ExportAPI("Neighbor.From.Meta")>
    Public Function NeighborMatrixFromMeta(DIR As String) As String
        Dim metas As BestHit() =
            LQuerySchedule.LQuery(ls - l - wildcards("*.xml") <= DIR, AddressOf LoadXml(Of BestHit)).ToArray

        Dim Genomes As Dictionary(Of String, List(Of Double)) =
            metas.ToDictionary(Function(obj) obj.sp,
                                  Function(obj) New List(Of Double))

        For Each File As BestHit In metas '不可以使用并行化，因为矩阵之中要求二者两两对应
            For Each sp In Genomes
                sp.Value.Add(File.GetTotalIdentities(sp.Key))
            Next
        Next

        Dim MAT As StringBuilder = New StringBuilder("   " & Genomes.Count)
        Dim idx As Integer

        For Each Line In Genomes
            Dim str As StringBuilder = New StringBuilder(MatrixFile.MatrixFile.MAT_ID(Line.Key))
            Dim j As Integer

            For Each s In Line.Value
                If j = idx Then
                    Call str.Append(" " & "0.0000")
                Else
                    Call str.Append(" " & MatrixFile.MatrixFile.RoundNumber(CStr(s), 6))
                End If

                j += 1
            Next

            idx += 1
            Call MAT.AppendLine(str.ToString)
        Next

        Return MAT.ToString
    End Function

    ''' <summary>
    ''' 不可以使用并行化拓展，因为有一一对应关系
    ''' </summary>
    ''' <param name="overview"></param>
    ''' <returns></returns>
    <ExportAPI("MAT.From.Self.Overviews")>
    Public Function SelfOverviewsMAT(overview As Overview) As DocumentStream.File
        Dim lstId As String() = overview.Queries.ToArray(Function(x) x.Id)
        Dim MAT As DocumentStream.File =
            New DocumentStream.File + "".Join(lstId)

        For Each query In overview.Queries
            Dim row As New List(Of Object)
            Dim hist = query.Hits.ToDictionary(Function(x) x.HitName)

            Call row.Add(query.Id)

            For Each id As String In lstId
                Call row.Add(If(hist.ContainsKey(id), hist(id).identities, 0))
            Next

            Call MAT.Add(New DocumentStream.RowObject(row))
        Next

        Return MAT
    End Function

    <ExportAPI("Gendist.From.Self.Overviews")>
    Public Function SelfOverviewsGendist(overview As Overview) As Gendist
        Dim MAT = SelfOverviewsMAT(overview)
        Dim Gendist = MatrixFile.Gendist.CreateMotifDistrMAT(MAT)
        Return Gendist
    End Function

    <ExportAPI("Gendist.From.Self.Overviews")>
    Public Function SelfOverviewsGendist(blastOut As v228) As Gendist
        Dim overview = blastOut.ExportOverview
        Dim MAT = SelfOverviewsMAT(overview)
        Dim Gendist = MatrixFile.Gendist.CreateMotifDistrMAT(MAT)
        Return Gendist
    End Function
End Module