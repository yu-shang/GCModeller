﻿#Region "Microsoft.VisualBasic::f986dc0a919024c42fb6a35539c0fc2a, ..\GCModeller\CLI_tools\S.M.A.R.T\CLI\MPAlignment.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Data.Xfam.Pfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports ProteinTools.SMART.Common.Extensions

Partial Module CLI

    ''' <summary>
    ''' SBH -> MPAlignment，这个方法特别适用于调控因子的同源比对操作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--MPAlignment", Usage:="--MPAlignment /sbh <sbh.csv> /query <pfam-string.csv> /subject <pfam-string.csv> [/mp <0.65> /out <out.csv>]")>
    Public Function SBHAlignment(args As CommandLine.CommandLine) As Integer
        Dim sbh = args("/sbh").LoadCsv(Of BBH.BestHit)
        Dim query = args("/query").LoadCsv(Of Pfam.PfamString.PfamString).ToDictionary(Function(x) x.ProteinId)
        Dim subject = args("/subject").LoadCsv(Of Pfam.PfamString.PfamString).ToDictionary(Function(x) x.ProteinId)
        Dim MP As Double = args.GetValue("/mp", 0.65)
        Dim LQuery = (From hit As BBH.BestHit
                      In sbh
                      Where query.ContainsKey(hit.QueryName) AndAlso
                          subject.ContainsKey(hit.HitName)
                      Select Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(query(hit.QueryName), subject(hit.HitName), MP)).ToArray
        Dim out As String = args.GetValue("/out", args("/sbh").TrimSuffix & ".MPAlignment.csv")
        Return AlignmentOutput2Csv(LQuery).SaveTo(out).CLICode
    End Function

    <ExportAPI("--align", Usage:="--align /query <query.csv> /subject <subject.csv> [/out <out.DIR> /inst]")>
    Public Function Align(args As CommandLine.CommandLine) As Integer
        If args.GetBoolean("/inst") Then
            Return __alignInst(args)
        End If

        Dim query = args("/query").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim subject = args("/subject").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim resultSet As New List(Of LevAlign)

        out = $"{out}/{IO.Path.GetFileNameWithoutExtension(args("/query"))}_vs.{IO.Path.GetFileNameWithoutExtension(args("/subject"))}"

        Dim outHTML As String = out & "/HTML/"

        For Each prot As PfamString.PfamString In query
            Dim alignInvoke As LevAlign() = (From b As PfamString.PfamString
                                             In subject.AsParallel
                                             Select Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(prot, b, 0.8)).ToArray

            resultSet += alignInvoke

            For Each paired In alignInvoke
                Dim html As String = paired.Visualize
                Dim path As String = outHTML & $"/{paired.QueryPfam.ProteinId.NormalizePathString}_vs.{paired.SubjectPfam.ProteinId.NormalizePathString}.html"
                Call html.SaveTo(path)
            Next

            Call prot.__DEBUG_ECHO
            Call FlushMemory()
        Next

        Call $"Alignment Done! Export to {out}".__DEBUG_ECHO
        Call resultSet.GetXml.SaveTo(out & "/MPAlignment.xml")
        Call resultSet.ToArray(Function(x) x.ToRow).SaveTo($"{out}/MPAlignment.csv")

        Return 0
    End Function

    Private Function __alignInst(args As CommandLine.CommandLine) As Integer
        Dim query = Pfam.PfamString.CLIParser(args("/query"))
        Dim subject = Pfam.PfamString.CLIParser(args("/subject"))
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim result = Algorithm.PfamStringEquals(query, subject, 0.8)

        out = $"{out}/{query.ProteinId.NormalizePathString}_vs.{subject.ProteinId.NormalizePathString}"

        Call result.GetXml.SaveTo($"{out}/MPAlignment.xml")
        Call result.Visualize.SaveTo(out & "/MPAlignment.html")

        Dim outTxt As String = result.ToString
        Call Console.WriteLine(outTxt)
        Call outTxt.SaveTo(out & "/MPAlignment.txt")

        Return 0
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    ''' <summary>
    ''' 1. 完全比对得上
    ''' 2. 部分比对得上，但是必须要有一个结构域是和家族名称一致的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--align.family", Usage:="--align.family /In <In.bbh.csv> /query <query-pfam.csv> /subject <subject-pfam.csv> [/out <out.DIR> /mp <mp-align:0.65> /lev <lev-align:0.65>]")>
    Public Function FamilyAlign(args As CommandLine.CommandLine) As Integer
        Dim input As String = args("/in")
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim out As String = args.GetValue("/out", input.ParentDirName & "/MPAlignment/")
        Dim inBBH = input.LoadCsv(Of BiDirectionalBesthit)
        Dim align = MotifParallelAlignment.AlignProteins(inBBH, query.LoadCsv(Of Pfam.PfamString.PfamString), subject.LoadCsv(Of Pfam.PfamString.PfamString))
        Call align.ToArray.GetXml.SaveTo($"{out}/{IO.Path.GetFileNameWithoutExtension(input)}.xml")
        Call align.ToArray(Function(x) x.ToRow).SaveTo($"{out}/{IO.Path.GetFileNameWithoutExtension(input)}.csv")

        Dim Regprecise = GCModeller.FileSystem.KEGGFamilies.LoadCsv(Of FastaReaders.Regulator) _
                .ToDictionary(Function(prot) prot.LocusTag) '.LoadXml(Of SMRUCC.genomics.DatabaseServices.Regprecise.WebServices.Regulations)
        Dim mp As Double = args.GetValue("/mp", 0.65)
        ' Dim lev As Double = args.GetValue("/lev", 0.65)
        Dim havMatches = (From mm In (From match In align.AsParallel
                                      Where match.NumMatches > 0 AndAlso
                                          match.Score > mp AndAlso
                                          (match.Score = 1.0R OrElse ' 完全比对的上的
                                          match.QueryPfam.__contains(Regprecise(match.SubjectPfam.ProteinId.Split(":"c).Last).KEGGFamily)) ' 或者具备有目标结构域的
                                      Select match).ToArray
                          Select mm.QueryPfam.ProteinId,
                              subjectId = mm.SubjectPfam.ProteinId
                          Group By ProteinId Into Group) _
                             .ToDictionary(Function(match) match.ProteinId,
                                           Function(match) match.Group.ToArray(Function(prot) prot.subjectId))

        Dim matchesBBH = (From match In inBBH.AsParallel
                          Where havMatches.ContainsKey(match.QueryName) AndAlso
                              Array.IndexOf(havMatches(match.QueryName), match.HitName) > -1
                          Select match).ToArray

        Call matchesBBH.SaveTo($"{out}/{IO.Path.GetFileNameWithoutExtension(input)}.bbh_matches.csv")

        Return 0
    End Function

    <Extension> Private Function __contains(prot As Pfam.PfamString.PfamString, Family As String) As Boolean
        Dim Tokens As String() = FamilyTokens(Family)
        Dim LQuery = (From domain
                      In prot.GetDomainData(True)
                      Where Not (From fam As String In Tokens Where InStr(domain.Identifier, fam, CompareMethod.Text) > 0 Select 1).ToArray.IsNullOrEmpty
                      Select 1).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    <ExportAPI("--SelfAlign", Usage:="--SelfAlign /query <pfam-string.csv> /subject <subject.csv> /aln <mpalignment.csv> [/lstID <lstID.txt> /mp <0.65> /id <id>]")>
    <ParameterInfo("/lstID", True, Description:="If this parameter is not empty, then the /aln parameter will be disable")>
    <ParameterInfo("/id", True,
                          Description:="If this parameter is not null, then the record of this query or hits will be used to subset the alignment set.")>
    Public Function SelfAlign(args As CommandLine.CommandLine) As Integer
        Dim id As String = args("/id")
        Dim queryFile As String = args("/query")
        Dim subject = args("/subject").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim query = queryFile.LoadCsv(Of Pfam.PfamString.PfamString)
        Dim cut As Double = args.GetValue("/mp", 0.65)
        Dim lstId As String()
        Dim idFile As String = args("/lstID")
        Dim path As String

        If Not String.IsNullOrEmpty(idFile) Then
            lstId = IO.File.ReadAllLines(idFile)
            path = idFile.TrimSuffix & ".SelfAlign.csv"
            GoTo GET_ID
        Else
            path = args("/aln").TrimSuffix & ".SelfAlign.csv"
        End If

        Dim aln = args("/aln").LoadCsv(Of Pfam.ProteinDomainArchitecture.MPAlignment.MPCsvArchive)

        If String.IsNullOrEmpty(id) Then
            lstId = aln.ToArray(Function(x) x.QueryName).Join(aln.ToArray(Function(x) x.HitName)).Distinct.ToArray
        Else
            Dim ql = aln.ToArray(Function(x) x.QueryName)
            Dim subSet As Pfam.ProteinDomainArchitecture.MPAlignment.MPCsvArchive()

            If Array.IndexOf(ql, id) > -1 Then
                subSet = (From x In aln Where String.Equals(id, x.QueryName, StringComparison.OrdinalIgnoreCase) Select x).ToArray
            Else
                subSet = (From x In aln Where String.Equals(id, x.HitName, StringComparison.OrdinalIgnoreCase) Select x).ToArray
            End If

            lstId = subSet.ToArray(Function(x) x.QueryName).Join(subSet.ToArray(Function(x) x.HitName)).Distinct.ToArray
        End If

GET_ID:
        Dim pfSubSet As Pfam.PfamString.PfamString() =
            (From x In query Where Array.IndexOf(lstId, x.ProteinId) > -1 Select x).ToArray
        Call pfSubSet.Add((From x In subject Where Array.IndexOf(lstId, x.ProteinId) > -1 Select x).ToArray)
        Dim alnResult = (From x In pfSubSet Select (From y In pfSubSet Select Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(x, y, cut)).ToArray).MatrixToList
        Return Pfam.ProteinDomainArchitecture.MPAlignment.AlignmentOutput2Csv(alnResult).SaveTo(path)
    End Function
End Module
