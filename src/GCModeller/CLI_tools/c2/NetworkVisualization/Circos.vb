﻿Public Module Circos
    Public Function GenerateLinkFile(File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Genome As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile) As String()

        Dim LQuery = (From row In File.Skip(1) Where Not String.IsNullOrEmpty(row(5)) Select row).ToArray '筛选出匹配上调控因子的行
        Dim evalueList As Double() = (From row In LQuery Let n = Val(row(2)) Select n Distinct Order By n Descending).ToArray

        '生成键值对关系
        Const CIRCOS_REGULATION As String = "chr1 {0} {1} chr1 {2} {3} color={4},thickness={5}"
        Dim LinkList As List(Of String) = New List(Of String)
        For Each line In LQuery
            Dim regulator As String = line(5)
            Dim regulatorLocation = (From s In Strings.Split((From fsa In Genome Where String.Equals(regulator, fsa.Attributes.First) Select fsa).First.Attributes(3), " == ") Let n = Val(s) Select n Order By n Ascending).ToArray
            Dim GeneList As String() = (From s In Strings.Split(line(3), "; ") Where Not String.IsNullOrEmpty(s) Select s).ToArray
            Dim regulation As String = line(8)
            Dim evalue As Double = Val(line(2))
            Dim thickness = Int(Array.IndexOf(evalueList, evalue) / 10) + 3

            If String.IsNullOrEmpty(Trim(regulation)) Then
                regulation = "black"
            Else
                regulation = If(InStr(regulation.Split.First, "activ", CompareMethod.Text), "blue", "red")
            End If

            For Each gene In GeneList
                Dim target = (From fsa In Genome Where String.Equals(gene, fsa.Attributes.First) Select fsa).ToArray
                If target.IsNullOrEmpty Then
                    Continue For
                End If
                Dim gene_Location = (From s In Strings.Split(target.First.Attributes(3), " == ") Let n = Val(s) Select n Order By n Ascending).ToArray

                Call LinkList.Add(String.Format(CIRCOS_REGULATION, regulatorLocation(0), regulatorLocation(1), gene_Location(0), gene_Location(1), regulation, thickness))
            Next
        Next

        Return (From s In LinkList Select s Distinct).ToArray
    End Function

    Public Function RegulatorLabels(File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Genome As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile) As String()
        Dim LQuery = (From row In File.Skip(1) Where Not String.IsNullOrEmpty(row(5)) Select row).ToArray '筛选出匹配上调控因子的行

        Dim Families = (From row In LQuery Let s = row(7) Where Not String.IsNullOrEmpty(s) Select s Distinct).ToArray

        '生成键值对关系
        Const CIRCOS_REGULATION As String = "chr1 {0} {1} {2} color={3}"
        Dim LinkList As List(Of String) = New List(Of String)
        For Each line In LQuery
            Dim regulator As String = line(5)
            Dim regulatorLocation = (From s In Strings.Split((From fsa In Genome Where String.Equals(regulator, fsa.Attributes.First) Select fsa).First.Attributes(3), " == ") Let n = Val(s) Select n Order By n Ascending).ToArray
            Dim family = "chr" & Array.IndexOf(Families, line(7))
            If family = "" Then
                family = "chr50"
            End If

            Call LinkList.Add(String.Format(CIRCOS_REGULATION, regulatorLocation(0), regulatorLocation(1), regulator, family))
        Next

        Return (From s In LinkList Select s Distinct).ToArray
    End Function
End Module