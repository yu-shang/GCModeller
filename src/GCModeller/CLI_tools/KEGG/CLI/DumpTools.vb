﻿#Region "Microsoft.VisualBasic::446041f7149d09108a5beac5a915a505, ..\GCModeller\CLI_tools\KEGG\CLI\DumpTools.vb"

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
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism

Partial Module CLI

    <ExportAPI("--Dump.Db", Info:="", Usage:="--Dump.Db /KEGG.Pathways <DIR> /KEGG.Modules <DIR> /KEGG.Reactions <DIR> /sp <sp.Code> /out <out.Xml>")>
    Public Function DumpDb(args As CommandLine.CommandLine) As Integer
        Dim Pathways As String = args("/KEGG.Pathways")
        Dim Modules As String = args("/KEGG.Modules")
        Dim Reactions As String = args("/KEGG.Reactions")
        Dim sp As String = args("/sp")
        Dim outXml As String = args("/out")
        Dim Modle = SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Compile(Pathways, Modules, Reactions, sp)
        Return Modle.GetXml.SaveTo(outXml)
    End Function

    <ExportAPI("--Get.KO", Usage:="--Get.KO /in <KASS-query.txt>")>
    Public Function GetKOAnnotation(args As CommandLine.CommandLine) As Integer
        Dim input As String = args("/in")
        Dim buffer = IO.File.ReadAllLines(input).ToArray(Function(x) Strings.Split(x, vbTab))
        Dim tbl = buffer.ToArray(Function(x) New KeyValuePair With {.Key = x(Scan0), .Value = x.Get(1)})
        Dim brite = BriteHEntry.Pathway.LoadDictionary
        Dim LQuery = (From prot In tbl Select __queryKO(prot.Key, prot.Value, brite)).ToArray.MatrixToList
        Return LQuery.SaveTo(input.TrimSuffix & ".KO.csv")
    End Function

    <ExportAPI("/Query.KO", Usage:="/Query.KO /in <blastnhits.csv> [/out <out.csv> /evalue 1e-5 /batch]")>
    Public Function QueryKOAnno(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim batch As Boolean = args.GetBoolean("/batch")
        Dim evalue As Double = args.GetValue("/evalue", 0.00001)

        If batch Then
            Dim files = FileIO.FileSystem.GetFiles(inFile, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
            Dim out As String = args.GetValue("/out", inFile & "/PathwayInfo/")

            For Each file As String In files
                Dim outFile As String = out & $"/{IO.Path.GetFileNameWithoutExtension(file)}.PathwayInfo.csv"
                Call __queryKO2(file, outFile, evalue)
            Next
        Else
            Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".PathwayInfo.Csv")
            Call __queryKO2(inFile, out, evalue)
        End If

        Return 0
    End Function

    Private Sub __queryKO2(infile As String, out As String, evalue As Double)
        Dim inHits = infile.LoadCsv(Of bGetObject.SSDB.BlastnHit)
        inHits = (From x In inHits Where x.Eval <= evalue Select x).ToList
        Dim KO As String() = inHits.ToArray([CType]:=Function(x) x.KO, where:=Function(s) Not String.IsNullOrWhiteSpace(s.KO)).Distinct.ToArray
        Dim brite = BriteHEntry.Pathway.LoadDictionary
        Dim name As String = IO.Path.GetFileNameWithoutExtension(infile)
        Dim anno = KO.ToArray(Function(sKO) __queryKO(name, sKO, brite)).MatrixToList
        Call anno.SaveTo(out)
    End Sub

    Private Function __queryKO(prot As String,
                               KO As String,
                               brites As Dictionary(Of String, SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway)) As KOAnno()
        If String.IsNullOrEmpty(KO) Then
            Return {New KOAnno With {.QueryId = prot}}
        End If

        Dim orthology = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.Query(KO)
        If orthology.Pathway.IsNullOrEmpty Then
            Dim anno As KOAnno = __create(prot, KO, Nothing, orthology, brites)
            Return {anno}
        End If

        Dim LQuery = (From pathway In orthology.Pathway Select __create(prot, KO, pathway, orthology, brites)).ToArray
        Return LQuery
    End Function

    Private Function __create(prot As String,
                              KO As String,
                              pathway As KeyValuePair,
                              orthology As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology,
                              brites As Dictionary(Of String, SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway)) As KOAnno
        Dim pwyBrite As SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
        If Not pathway Is Nothing Then
            pwyBrite = brites.TryGetValue(Regex.Match(pathway.Key, "\d+").Value)
            If pwyBrite Is Nothing Then
                Call $"{pathway.Key} is not exists in the KEGG!".__DEBUG_ECHO
                GoTo Null
            End If
        Else
            pathway = New KeyValuePair
Null:       pwyBrite = New BriteHEntry.Pathway With {
                .Entry = New KeyValuePair
            }
        End If
        Return New KOAnno With {
            .QueryId = prot,
            .KO = KO,
            .COG = orthology.GetXRef("COG").ToArray(Function(x) x.Value2).JoinBy("; "),
            .Definition = orthology.Definition,
            .Name = orthology.Name,
            .GO = orthology.GetXRef("GO").ToArray(Function(x) "GO:" & x.Value2),
            .Category = pwyBrite.Category,
            .Class = pwyBrite.Class,
            .PathwayId = pathway.Key,
            .PathwayName = pwyBrite.Entry.Value,
            .Reactions = orthology.GetXRef("RN").ToArray(Function(x) x.Value2)
        }
    End Function

    Public Class KOAnno
        Public Property QueryId As String
        Public Property KO As String
        Public Property Name As String
        Public Property Definition As String
        Public Property PathwayId As String
        Public Property PathwayName As String
        Public Property [Class] As String
        Public Property Category As String
        Public Property COG As String
        Public Property GO As String()
        <Collection("lst.RN", "; ")> Public Property Reactions As String()
    End Class

    <ExportAPI("--part.from",
               Usage:="--part.from /source <source.fasta> /ref <referenceFrom.fasta> [/out <out.fasta> /brief]",
               Info:="source and ref should be in KEGG annotation format.")>
    Public Function GetSource(args As CommandLine.CommandLine) As Integer
        Dim source As New FastaFile(args("/source"))
        Dim ref As New FastaFile(args("/ref"))
        Dim out As String = args.GetValue("/out", args("/source").TrimSuffix & $".{IO.Path.GetFileNameWithoutExtension(args("/ref"))}.fasta")
        Dim sourceKEGG As SMRUCC.genomics.Assembly.KEGG.Archives.SequenceDump() =
            source.ToArray(Function(x) SMRUCC.genomics.Assembly.KEGG.Archives.SequenceDump.Create(x))
        Dim refKEGG As SMRUCC.genomics.Assembly.KEGG.Archives.SequenceDump() =
            ref.ToArray(Function(x) SMRUCC.genomics.Assembly.KEGG.Archives.SequenceDump.Create(x))
        Dim sourceDict = (From x As SMRUCC.genomics.Assembly.KEGG.Archives.SequenceDump
                          In sourceKEGG
                          Select x
                          Group x By x.SpeciesId Into Group) _
                             .ToDictionary(Function(x) x.SpeciesId, elementSelector:=Function(x) x.Group.ToArray)
        Dim refId As String() = refKEGG.ToArray(Function(x) x.SpeciesId).Distinct.ToArray
        Dim LQuery = (From sId As String In refId Where sourceDict.ContainsKey(sId) Select sourceDict(sId)).ToArray.MatrixToList
        Dim outFa As FastaFile = New FastaFile(LQuery)

        If args.GetBoolean("/brief") Then  ' 将基因号去除，只保留三字母的基因组编号，因为在做16S_rRNA进化树的时候，只需要一个基因就可以了
            Dim briefFa = (From x In outFa Select New FastaToken({x.Attributes.First.Split(":"c).First}, x.SequenceData)).ToArray
            outFa = New FastaFile(briefFa)
        End If

        Return outFa.ToUpper.Save(out).CLICode
    End Function

    <ExportAPI("/Pathways.Downloads.All", Usage:="/Pathways.Downloads.All [/out <outDIR>]")>
    Public Function DownloadsAllPathways(args As CommandLine.CommandLine) As Integer
        Dim outDIR As String = args.GetValue("/out", GCModeller.FileSystem.KEGG.GetPathways)
        Return bGetObject.Pathway.DownloadAll(outDIR)
    End Function

    <ExportAPI("/Dump.sp", Usage:="/Dump.sp [/res sp.html /out <out.csv>]")>
    Public Function DumpOrganisms(args As CommandLine.CommandLine) As Integer
        Dim res As String = args.GetValue("/res", "http://www.kegg.jp/kegg/catalog/org_list.html")
        Dim result As KEGGOrganism = EntryAPI.FromResource(res)
        Dim table As List(Of Prokaryote) = result.Prokaryote.ToList + result.Eukaryotes.ToArray(Function(x) New Prokaryote(x))
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/KEGG_Organism.csv")
        Return table.SaveTo(out)
    End Function
End Module
