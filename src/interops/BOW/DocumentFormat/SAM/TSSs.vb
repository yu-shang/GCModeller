﻿Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.BOW.DocumentFormat.SAM.DocumentElements
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData

<[Namespace]("TSSs.Analysis")>
Module SAM_TSSs

#Const DEBUG = 1

    <ExportAPI("SAM.TSSs")>
    Public Function TSS(SAM As DocumentFormat.SAM.SAM) As GroupResult(Of DocumentFormat.SAM.DocumentElements.AlignmentReads, Integer)()
        Dim Forwards = (From reads In SAM.AsParallel Where reads.Strand = Strands.Forward Select reads).ToArray
        Dim Reversed = (From reads In SAM.AsParallel Where reads.Strand = Strands.Reverse Select reads).ToArray

        Call Console.WriteLine($"[DEBUG {Now.ToString}] There are {Forwards.Length} reads on forward strand and {Reversed.Length} reads on reversed strand...   @{SAM.FilePath.ToFileURL}")
        Call Console.WriteLine($"[DEBUG {Now.ToString}] Start to group alignment reads....")
        Dim ForwardGroup = (From Reads In Forwards.AsParallel Select Reads Group Reads By Reads.POS Into Group).ToArray  'Forwards.ParallelGroup(Of Integer)(Function(Reads) Reads.POS)
        Dim ReversedGroup = (From Reads In Reversed.AsParallel Select Reads Group Reads By Reads.POS Into Group).ToArray  'Reversed.ParallelGroup(Of Integer)(Function(Reads) Reads.POS)
        Call Console.WriteLine($"[DEBUG {Now.ToString}] Start to filtering alignment reads....")
        Dim GrepLQuery = (From GroupReads In ForwardGroup.AsParallel Where GroupReads.Group.Count >= 30 Select GroupReads).ToArray
        Call Console.WriteLine($"[DEBUG {Now.ToString}] There are {NameOf(Strands.Forward)}:{GrepLQuery.Count} has reads number greater than 30 at least....")

        Dim ReversedGrepLQuery = (From GroupReads In ReversedGroup.AsParallel Where GroupReads.Group.Count >= 30 Select GroupReads).ToArray
        Call Console.WriteLine($"[DEBUG {Now.ToString}] There are {NameOf(Strands.Reverse)}:{ReversedGrepLQuery.Count} has reads number greater than 30 at least.....")

        If Not String.IsNullOrEmpty(SAM.FilePath) Then
            Dim CSV_DEBUG_VIEw = (From item In ForwardGroup.AsParallel
                                  Let obj = item.Group.First
                                  Select obj.Strand, obj.TLEN, obj.FLAG, obj.CIGAR, obj.MAPQ, obj.POS, obj.PNEXT, NumberOfReads = item.Group.Count).ToArray
            Call CSV_DEBUG_VIEw.SaveTo($"{SAM.FilePath}_Forwards_view.csv", False)
            CSV_DEBUG_VIEw = (From item In ReversedGroup.AsParallel
                              Let obj = item.Group.First
                              Select obj.Strand, obj.TLEN, obj.FLAG, obj.CIGAR, obj.MAPQ, obj.POS, obj.PNEXT, NumberOfReads = item.Group.Count).ToArray
            Call CSV_DEBUG_VIEw.SaveTo($"{SAM.FilePath}_Reversed_view.csv", False)
        End If

        GrepLQuery = GrepLQuery.Join(ReversedGrepLQuery).ToArray
        Call Console.WriteLine("Filtering job done!")

        Return (From Reads In GrepLQuery.AsParallel Select New GroupResult(Of AlignmentReads, Integer) With {.Tag = Reads.POS, .Group = Reads.Group.ToArray}).ToArray
    End Function

    ''' <summary>
    ''' 将SAM文件里面的Reads数据按照正向和反向分别进行保存到两个文件之中
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Split")>
    Public Function SplitSaved(SAM As DocumentFormat.SAM.SAM, <Parameter("LowQuality.Trim")> Optional TrimLowQuality As Boolean = True,
                               <Parameter("Dir.Export", "If this optional parameter is null then the parent directory of the sam file will be used.")> Optional Export As String = "") As Boolean

        If String.IsNullOrEmpty(Export) Then
            If Not String.IsNullOrEmpty(SAM.FilePath) Then
                Export = FileIO.FileSystem.GetParentPath(SAM.FilePath)
            Else
                Export = FileIO.FileSystem.CurrentDirectory
            End If
        Else
            Export = FileIO.FileSystem.GetDirectoryInfo(Export).FullName
        End If

        Call Console.WriteLine($"[DEBUG {Now.ToString}] Export to location {Export}")
        Call Console.WriteLine($"[DEBUG {Now.ToString}] SAM file contains {SAM.Count} reads....")

        Dim Forwards = (From reads In SAM.AsParallel Where reads.Strand = Strands.Forward Select reads).ToArray
        Dim Reversed = (From reads In SAM.AsParallel Where reads.Strand = Strands.Reverse Select reads).ToArray
        Dim NameToken As String = SAM.FilePath

        Call Console.WriteLine($"[DEBUG {Now.ToString}] {NameOf(Forwards)}:={Forwards.Count};  {NameOf(Reversed)}:={Reversed.Count}")

        If TrimLowQuality Then
            Call Console.WriteLine($"[DEBUG {Now.ToString}] User call trim low quality reads...")
            Forwards = (From reads In Forwards.AsParallel Where Not reads.LowQuality Select reads).ToArray
            Reversed = (From reads In Reversed.AsParallel Where Not reads.LowQuality Select reads).ToArray
            Call Console.WriteLine($"[DEBUG {Now.ToString}] {NameOf(Forwards)}:={Forwards.Count};  {NameOf(Reversed)}:={Reversed.Count}")
        End If

        If Not String.IsNullOrEmpty(NameToken) Then
            NameToken = IO.Path.GetFileNameWithoutExtension(NameToken)
        End If

        Dim fnForward As String = $"{Export}/{NameToken}_Forward.sam", fnReversed As String = $"{Export}/{NameToken}_Reversed.sam"

        Call New DocumentFormat.SAM.SAM With {.AlignmentsReads = Forwards, .Head = SAM.Head}.Save(fnForward)
        Call Console.WriteLine($"[DEBUG {Now.ToString}] Save {NameOf(Forwards)} to {fnForward.ToFileURL }")
        Call New DocumentFormat.SAM.SAM With {.AlignmentsReads = Reversed, .Head = SAM.Head}.Save(fnReversed)
        Call Console.WriteLine($"[DEBUG {Now.ToString}] Save {NameOf(Reversed)} to {fnReversed.ToFileURL }")

        Return True
    End Function

    ''' <summary>
    ''' 将一些标签去除一应用于下游的TSS分析
    ''' </summary>
    ''' <param name="doc">
    ''' 会将文档里面的<see cref="BitFLAGS.Bit0x200"/>,
    ''' <see cref="BitFLAGS.Bit0x4"/>的Reads进行剔除
    ''' </param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("TrimFlags")>
    Public Function TrimForTSSs(doc As DocumentFormat.SAM.SAM) As DocumentFormat.SAM.SAM
        Dim Unmapped As Integer = BitFLAGS.Bit0x4
        Dim LowQuality As Integer = BitFLAGS.Bit0x200

        Call $"There are {doc.AlignmentsReads.Count} reads in the sam mapping file   {doc.FilePath.ToFileURL}".__DEBUG_ECHO
        Call $"Triming reads which has flag [{NameOf(LowQuality)}]{BitFLAGS.Bit0x200} or [{NameOf(Unmapped)}]{BitFLAGS.Bit0x4}".__DEBUG_ECHO
        doc = New DocumentFormat.SAM.SAM With {
            .FilePath = doc.FilePath,
            .Head = doc.Head,
            .AlignmentsReads =
            LinqAPI.Exec(Of AlignmentReads) <= From reads As AlignmentReads
                                               In doc.AsParallel
                                               Where Not reads.IsUnmappedReads AndAlso
                                                   Not reads.LowQuality AndAlso
                                                   reads.POS > 0
                                               Select reads
        }
        Call $"Left {doc.AlignmentsReads.Length} alignment reads after triming data.".__DEBUG_ECHO

        Return doc
    End Function

End Module