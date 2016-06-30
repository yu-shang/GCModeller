﻿Imports LANS.SystemsBiology.AnalysisTools.CRISPR.SearchingModel
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Runtime.CompilerServices

<PackageNamespace("CRISPR.Searchs.Reports")>
Public Module ConsoleReport

    <ExportAPI("Prints")>
    <Extension> Public Function Print(dat As IEnumerable(Of SearchingModel.CRISPR)) As String
        If dat.IsNullOrEmpty Then
            Return ""
        Else
            Return dat.__print
        End If
    End Function

    <Extension> Private Function __print(dat As IEnumerable(Of SearchingModel.CRISPR)) As String
        Dim positionMaxLength As Integer = Len((From item In dat Select item.Repeats).MatrixAsIterator.Max.ToString)
        Dim RepeatsMaxLength As Integer = (From item In dat Select item.RepeatLength).Max
        Dim SpacersMaxLength As Integer = (From item In dat Select item.AverageSpacerLength).Max
        Dim sBuilder As StringBuilder = New StringBuilder(2048)
        Dim i As Integer = 0

        positionMaxLength = If(positionMaxLength > 9, positionMaxLength, 9)

        For Each site As SearchingModel.CRISPR In dat
            Call sBuilder.AppendLine(String.Format("  ======> CRISPR {0}", i + 1))
            Call sBuilder.__print(site, positionMaxLength, RepeatsMaxLength, SpacersMaxLength)

            i += 1
        Next

        Return sBuilder.ToString
    End Function

    <Extension> Private Sub __print(sb As StringBuilder,
                                    site As SearchingModel.CRISPR,
                                    positionMaxLength As Integer,
                                    RepeatsMaxLength As Integer,
                                    SpacersMaxLength As Integer)
        Call sb.AppendLine("Position" & New String(" "c, positionMaxLength - 8) & vbTab & "Repeats" & New String(" "c, RepeatsMaxLength) & vbTab & "Space Sequence")
        Call sb.AppendLine(New String("-"c, positionMaxLength) & vbTab & New String("-"c, RepeatsMaxLength) & vbTab & New String("-"c, SpacersMaxLength))

        For j As Integer = 0 To site.NumberOfRepeats - 1
            Call sb.Append((site.RepeatAt(j) + 1) & vbTab & vbTab & site.RepeatStringAt(j) & vbTab)

            ' print spacer
            ' because there are no spacers after the last repeat, we stop early (m < crisprIndexVector.size() - 1)
            If j < site.NumberOfSpacers() Then
                Dim Spacer As String = site.SpacerStringAt(j)

                Call sb.Append(Spacer & New String(" "c, SpacersMaxLength - Len(Spacer)))
                Call sb.AppendLine(String.Format(vbTab & "[ {0}, {1} ]", site.RepeatStringAt(j).Length, site.SpacerStringAt(j).Length))
            End If
        Next

        Call sb.Append(vbLf & New String("-"c, positionMaxLength) & vbTab)
        Call sb.Append(New String("-"c, RepeatsMaxLength) & vbTab)
        Call sb.AppendLine(New String("-"c, SpacersMaxLength))
    End Sub
End Module