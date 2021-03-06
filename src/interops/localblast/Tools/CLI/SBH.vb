﻿#Region "Microsoft.VisualBasic::3a9243ccd013d702ed663bf69fb22120, ..\interops\localblast\Tools\CLI\SBH.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BBHLogs
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Partial Module CLI

    <ExportAPI("/Paralog", Usage:="/Paralog /blastp <blastp.txt> [/coverage 0.5 /identities 0.3 /out <out.csv>]")>
    Public Function ExportParalog(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args - "/blastp"
        Dim blastp As v228 = BlastPlus.ParsingSizeAuto([in])
        Dim coverage As Double = args.GetValue("/coverage", 0.5)
        Dim identities As Double = args.GetValue("/identities", 0.3)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".paralogs.csv")
        Dim paralogs As BestHit() = Paralog.ExportParalog(blastp, coverage, identities)
        Return paralogs.SaveTo(out).CLICode
    End Function

    Private Function __evalueRow(hitsTags As String(),
                                 queryName As String,
                                 hashHits As Dictionary(Of String, BestHit()),
                                 flip As Boolean) As RowObject

        Dim row As New DocumentStream.RowObject From {queryName}

        For Each hit As String In hitsTags

            If flip Then

                If hashHits.ContainsKey(hit) Then
                    Dim e As Double = hashHits(hit).First.evalue

                    If e = 0R Then
                        Call row.Add("1")
                    Else
                        Call row.Add(CStr(1 - e))
                    End If

                Else
                    Call row.Add("0")
                End If

            Else
                If hashHits.ContainsKey(hit) Then
                    Call row.Add(hashHits(hit).First.evalue)
                Else
                    Call row.Add("-1")
                End If
            End If


        Next

        Return row
    End Function

    Private Function __HitsRow(hitsTags As String(),
                               queryName As String,
                               hashHits As Dictionary(Of String, BestHit())) As RowObject

        Dim row As New DocumentStream.RowObject From {queryName}

        For Each hit As String In hitsTags
            If hashHits.ContainsKey(hit) Then
                Call row.Add(hashHits(hit).Length)
            Else
                Call row.Add("0")
            End If
        Next

        Return row
    End Function

    <ExportAPI("/MAT.evalue", Usage:="/MAT.evalue /in <sbh.csv> [/out <mat.csv> /flip]")>
    Public Function EvalueMatrix(args As CommandLine.CommandLine) As Integer
        Dim sbh As List(Of BestHit) = args("/in").LoadCsv(Of BestHit)
        Dim out As String = args.GetValue("/out", args("/in").TrimSuffix & ".Evalue.Csv")
        Dim contigs = (From x As BestHit
                       In sbh
                       Select x
                       Group x By x.QueryName Into Group) _
                            .ToDictionary(Function(x) x.QueryName,
                                          Function(x) (From y As BestHit
                                                       In x.Group
                                                       Select y
                                                       Group y By y.HitName Into Group) _
                                                            .ToDictionary(Function(xx) xx.HitName,
                                                                          Function(xx) xx.Group.ToArray))
        Dim hitsTags As String() = (From x As BestHit In sbh Select x.HitName Distinct).ToArray
        Dim flip As Boolean = args.GetBoolean("/flip")
        Dim LQuery = (From contig In contigs.AsParallel Select __evalueRow(hitsTags, contig.Key, contig.Value, flip)).ToArray
        Dim hits = (From contig In contigs.AsParallel Select __HitsRow(hitsTags, contig.Key, contig.Value)).ToArray

        Dim Csv As File = New File + New RowObject("+".Join(hitsTags))
        Csv += LQuery
        Csv.Save(out, Encoding:=System.Text.Encoding.ASCII)

        Csv = New File + New RowObject("+".Join(hitsTags))
        Csv += hits

        Return Csv.Save(out & ".Hits.Csv", Encoding:=System.Text.Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/SBH.Export.Large",
               Usage:="/SBH.Export.Large /in <blast_out.txt> [/trim-kegg /out <bbh.csv> /identities 0.15 /coverage 0.5]")>
    <ParameterInfo("/trim-KEGG", True,
                   Description:="If the fasta sequence source is comes from the KEGG database, and you want to removes the kegg species brief code for the locus_tag, then enable this option.")>
    Public Function ExportBBHLarge(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".bbh.Csv")
        Dim idetities As Double = args.GetValue("/identities", 0.15)
        Dim coverage As Double = args.GetValue("/coverage", 0.5)

        Using IO As New DocumentStream.Linq.WriteStream(Of BestHit)(out)
            Dim handle As Action(Of Query) = IO.ToArray(Of BlastPlus.Query)(
                Function(query) v228.SBHLines(query, coverage:=coverage, identities:=idetities))
            Call BlastPlus.Transform(inFile, 1024 * 1024 * 128, handle)
        End Using

        Return 0
    End Function

    <ExportAPI("--Export.SBH", Usage:="--Export.SBH /in <in.DIR> /prefix <queryName> /out <out.csv> [/txt]")>
    Public Function ExportSBH(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim query As String = args("/prefix")
        Dim isTxtLog As Boolean = args.GetBoolean("/txt")
        Dim out As String = args("/out")
        Dim lst As String() = LoadSBHEntry(inDIR, query)
        Dim blastp As BBH.BestHit()()

        If isTxtLog Then
            blastp = (From x As String
                      In lst
                      Select BlastPlus.Parser.TryParse(x).ExportAllBestHist).ToArray
        Else
            blastp = lst.ToArray(Function(x) x.LoadCsv(Of BBH.BestHit).ToArray)
        End If

        Dim LQuery As BBH.BestHit() =
            LinqAPI.Exec(Of BestHit) <= From x As BBH.BestHit()
                                        In blastp
                                        Select x.ToArray(
                                            Function(xx) xx,
                                            Function(xx) xx.Matched)
        Return LQuery.SaveTo(out).CLICode
    End Function

    <ExportAPI("--Export.Overviews", Usage:="--Export.Overviews /blast <blastout.txt> [/out <overview.csv>]")>
    Public Function ExportOverviews(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/blast")
        Dim fileInfo = FileIO.FileSystem.GetFileInfo(inFile)
        Dim blastOut As v228

        If fileInfo.Length >= 768 * 1024 * 1024 Then
            blastOut = BlastPlus.TryParseUltraLarge(inFile)
        Else
            blastOut = BlastPlus.TryParse(inFile)
        End If

        Dim overviews As BestHit() = blastOut.ExportOverview.GetExcelData
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & "Overviews.csv")

        Return overviews.SaveTo(out).CLICode
    End Function
End Module
