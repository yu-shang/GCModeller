﻿#Region "Microsoft.VisualBasic::f3c9a80fc0665325ff3908c3026708a6, ..\interops\localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BatchParallel\VennDataBuilder.vb"

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

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel.Threads

Namespace LocalBLAST.Application.BatchParallel

    ''' <summary>
    ''' The batch blast module for the preparations of the Venn diagram drawing data model.(为文氏图的绘制准备数据的批量blast模块)
    ''' </summary>
    ''' <remarks>这里面的方法都是完全的两两组合的BBH</remarks>
    '''
    <PackageNamespace("Venn.Builder",
                      Category:=APICategories.ResearchTools,
                      Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module VennDataBuilder

        ''' <summary>
        ''' The formatdb and blast operation should be include in this function pointer.(在这个句柄之中必须要包含有formatdb和blast这两个步骤)
        ''' </summary>
        ''' <param name="Query"></param>
        ''' <param name="Subject"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="Export"></param>
        ''' <returns>返回blast的日志文件名</returns>
        ''' <remarks></remarks>
        Public Delegate Function INVOKE_BLAST_HANDLE(Query As String,
                                                     Subject As String,
                                                     num_threads As Integer,
                                                     Evalue As String,
                                                     EXPORT As String,
                                                     [Overrides] As Boolean) As String

        ''' <summary>
        ''' The recommended num_threads parameter for the blast operation base on the current system hardware information.
        ''' (根据当前的系统硬件配置所推荐的num_threads参数)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RecommendedThreads As Integer
            Get
                Return Environment.ProcessorCount / 2
            End Get
        End Property

#Region "为了接口的统一的需要"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="input">输入的文件夹，fasta序列的文件拓展名必须要为*.fasta或者*.fsa</param>
        ''' <param name="EXPORT">结果导出的文件夹，导出blast日志文件</param>
        ''' <param name="InvokedBLASTAction">所执行的blast命令，函数返回日志文件名</param>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Task.Builder")>
        Public Function TaskBuilder(input As String,
                                    EXPORT As String,
                                    evalue As String,
                                    InvokedBLASTAction As INVOKE_BLAST_HANDLE,
                                    Optional [Overrides] As Boolean = False) As AlignEntry()
            Dim Files As String() = ls - l - r - wildcards("*.fasta", "*.fsa") <= input
            Dim ComboList = Comb(Of String).CreateCompleteObjectPairs(Files)

            Call FileIO.FileSystem.CreateDirectory(EXPORT)

            Dim FileList As New List(Of String)

            For Each pairedList In ComboList
                For Each paired As KeyValuePair(Of String, String) In pairedList
                    FileList += InvokedBLASTAction(Query:=paired.Key,
                                                   Subject:=paired.Value,
                                                   Evalue:=evalue,
                                                   EXPORT:=EXPORT,
                                                   num_threads:=RecommendedThreads,
                                                   [Overrides]:=[Overrides])
                Next
            Next

            On Error Resume Next

            Return FileList.ToArray(AddressOf LogNameParser)
        End Function

        ''' <summary>
        ''' The parallel edition for the invoke function <see cref="TaskBuilder"></see>.(<see cref="TaskBuilder"></see>的并行版本)
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="evalue"></param>
        ''' <param name="InvokedBLASTAction"></param>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Task.Builder.Parallel")>
        Public Function TaskBuilder_p(Input As String,
                                      EXPORT As String,
                                      Evalue As String,
                                      InvokedBLASTAction As INVOKE_BLAST_HANDLE,
                                      Optional [Overrides] As Boolean = False) As AlignEntry()

            Dim Files As String() = ls - l - r - wildcards("*.fasta", "*.fsa", "*.fa") <= Input
            Dim ComboList As KeyValuePair(Of String, String)()() =
                Comb(Of String).CreateCompleteObjectPairs(Files)

            Call FileIO.FileSystem.CreateDirectory(EXPORT)

            Dim FileList As New List(Of String)

            For Each pairedList As KeyValuePair(Of String, String)() In ComboList
                Dim LQuery As String() =
                    LinqAPI.Exec(Of String) <= From x As KeyValuePair(Of String, String)
                                               In pairedList.AsParallel
                                               Select InvokedBLASTAction(
                                                   Query:=x.Key,
                                                   Subject:=x.Value,
                                                   Evalue:=Evalue,
                                                   EXPORT:=EXPORT,
                                                   num_threads:=RecommendedThreads,
                                                   [Overrides]:=[Overrides])
                FileList += LQuery
            Next

            'On Error Resume Next
            Return FileList.ToArray(AddressOf LogNameParser)
        End Function

#End Region

        ''' <summary>
        ''' 这个函数相比较于<see cref="TaskBuilder_p"/>更加高效
        ''' </summary>
        ''' <param name="inputDIR"></param>
        ''' <param name="outDIR"></param>
        ''' <param name="evalue"></param>
        ''' <param name="blastTask"></param>
        ''' <param name="[overrides]"></param>
        ''' <param name="num_threads"></param>
        ''' <returns>返回日志文件列表</returns>
        Public Function ParallelTask(inputDIR As String,
                                     outDIR As String,
                                     evalue As String,
                                     blastTask As INVOKE_BLAST_HANDLE,
                                     Optional [overrides] As Boolean = False,
                                     Optional num_threads As Integer = -1) As AlignEntry()

            Dim Files As String() = ls - l - r - wildcards("*.fasta", "*.fsa", "*.fa") <= inputDIR
            Dim ComboList As KeyValuePair(Of String, String)()() =
                Comb(Of String).CreateCompleteObjectPairs(Files)
            Dim taskList As Func(Of String)() = (From task As KeyValuePair(Of String, String)
                                                 In ComboList.MatrixAsIterator.AsParallel
                                                 Let taskHandle As Func(Of String) =
                                                     Function() blastTask(Query:=task.Key,
                                                                          Subject:=task.Value,
                                                                          Evalue:=evalue,
                                                                          EXPORT:=outDIR,
                                                                          num_threads:=RecommendedThreads,
                                                                          [Overrides]:=[overrides])
                                                 Select taskHandle).ToArray

            Call $"Fasta source is {Files.Length} genomes...".__DEBUG_ECHO
            Call $"Build bbh task list of {taskList.Length} tasks...".__DEBUG_ECHO
            Call FileIO.FileSystem.CreateDirectory(outDIR)
            Call App.StartGC(True)
            Call "Start BLAST threads...".__DEBUG_ECHO
            Call $"     {NameOf(num_threads)} => {num_threads}".__DEBUG_ECHO
            Call $"     {NameOf(taskList)}    => {taskList.Length}".__DEBUG_ECHO
            Call Console.WriteLine(New String("+", 200))

            Dim fileList As String() = BatchTask(taskList,
                                                 numThreads:=num_threads, ' 启动批量本地blast操作
                                                 TimeInterval:=10)
            'On Error Resume Next
            Return fileList.ToArray(AddressOf LogNameParser)
        End Function

        Public Const QUERY_LINKS_SUBJECT As String = "_vs__"

        ''' <summary>
        ''' <paramref name="query"/> <see cref="QUERY_LINKS_SUBJECT"/> <paramref name="subject"/>.(去掉了fasta文件的后缀名)
        ''' </summary>
        ''' <param name="Query"></param>
        ''' <param name="Subject"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Build.Entry")>
        Public Function BuildFileName(Query As String, Subject As String, EXPORT As String) As String
            Query = IO.Path.GetFileNameWithoutExtension(Query)
            Subject = IO.Path.GetFileNameWithoutExtension(Subject)
            Return $"{EXPORT}/{Query}{QUERY_LINKS_SUBJECT}{Subject}.txt"
        End Function

        ''' <summary>
        ''' 尝试从给出的日志文件名之中重新解析出比对的对象列表
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Entry.Parsing")>
        Public Function LogNameParser(path As String) As AlignEntry
            Dim ID As String = IO.Path.GetFileNameWithoutExtension(path).Replace(".besthit", "")
            Dim Temp As String() = Strings.Split(ID, QUERY_LINKS_SUBJECT)
            Return New AlignEntry With {
                .QueryName = Temp.First,
                .FilePath = path,
                .HitName = Temp.Last
            }
        End Function

        ''' <summary>
        ''' 批量两两比对blastp，以用于生成文氏图的分析数据
        ''' </summary>
        ''' <param name="localblast"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Get.Blastp.Handle")>
        <Extension>
        Public Function BuildBLASTP_InvokeHandle(localblast As Programs.BLASTPlus) As INVOKE_BLAST_HANDLE
            Dim Handle As INVOKE_BLAST_HANDLE = Function(Query As String, Subject As String,
                                                         num_threads As Integer,
                                                         Evalue As String,
                                                         EXPORT As String,
                                                         [Overrides] As Boolean) localblast.__blastpHandle(
                                                                                    Query:=Query,
                                                                                    Evalue:=Evalue,
                                                                                    EXPORT:=EXPORT,
                                                                                    Num_Threads:=num_threads,
                                                                                    [Overrides]:=[Overrides],
                                                                                    Subject:=Subject)
            Return Handle
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="ServiceHandle"></param>
        ''' <param name="Query"></param>
        ''' <param name="Subject"></param>
        ''' <param name="Num_Threads"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="Overrides">当目标文件存在并且长度不为零的时候，是否进行覆盖，假若为否，则直接忽略过这个文件</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Private Function __blastpHandle(ServiceHandle As Programs.BLASTPlus,
                                        Query As String,
                                        Subject As String,
                                        Num_Threads As Integer,
                                        Evalue As String,
                                        EXPORT As String,
                                        [Overrides] As Boolean) As String

            Dim logOut As String = BuildFileName(Query, Subject, EXPORT)

            If IsAvailable(logOut) Then
                If Not [Overrides] Then
                    Call Console.Write(".")
                    Return logOut  '文件已经存在，则会更具是否进行覆写这个参数来决定是否需要进行blast操作
                End If
            End If

            ServiceHandle.NumThreads = Num_Threads

            Call ServiceHandle.FormatDb(Query, ServiceHandle.MolTypeProtein).Start(True)
            Call ServiceHandle.FormatDb(Subject, ServiceHandle.MolTypeProtein).Start(True)
            Call ServiceHandle.Blastp(Query, Subject, logOut, Evalue).Start(True)

            Return logOut
        End Function

        <ExportAPI("Get.Blastn.Handle")>
        <Extension>
        Public Function BuildBLASTN_InvokeHandle(service As Programs.BLASTPlus) As INVOKE_BLAST_HANDLE
            Dim Handle As INVOKE_BLAST_HANDLE =
                AddressOf New __handle With {.service = service}.invokeHandle
            Return Handle
        End Function

        Private Structure __handle

            Public service As Programs.BLASTPlus

            Public Function invokeHandle(Query As String, Subject As String, num_threads As Integer, Evalue As String, ExportDir As String, [Overrides] As Boolean) As String
                Dim LogOut As String = BuildFileName(Query, Subject, ExportDir)

                If IsAvailable(LogOut) Then                    '文件已经存在，则会更具是否进行覆写这个参数来决定是否需要进行blast操作
                    If Not [Overrides] Then
                        Call Console.Write(".")
                        Return LogOut
                    End If
                End If

                service.NumThreads = num_threads

                Call service.FormatDb(Query, service.MolTypeNucleotide).Start(True)
                Call service.FormatDb(Subject, service.MolTypeNucleotide).Start(True)
                Call service.Blastp(Query, Subject, LogOut, Evalue).Start(True)

                Return LogOut
            End Function
        End Structure
    End Module
End Namespace
