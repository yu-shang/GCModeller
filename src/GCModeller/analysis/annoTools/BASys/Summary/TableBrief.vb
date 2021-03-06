﻿
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.HtmlParser
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' BASys Annotation Table for 1329830 
''' </summary>
Public Class TableBrief

    Public Property Start As Integer
    Public Property [End] As Integer
    Public Property Strand As Char
    Public Property Accession As String
    Public Property Gene As String
    Public Property COG As String

    ''' <summary>
    ''' Protein Function
    ''' </summary>
    ''' <returns></returns>
    <Field("Protein Function")>
    Public Property [Function] As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function TableParser(path As String) As TableBrief()
        Dim html As String = path.GET
        html = Strings.Split(html, "<!-- MAIN TABLE MAIN COLUMN -->").Last
        html = Regex.Match(html, "<table.+?</table>", RegexICSng).Value
        Dim rows = html.GetRowsHTML
        Dim i As New Pointer(Scan0)
        Dim out As New List(Of TableBrief)

        For Each row As String In rows.Skip(1)
            Dim s As String() = row.GetColumnsHTML

            out += New TableBrief With {
                .Start = CInt(Val(s(++i))),
                .End = CInt(Val(s(++i))),
                .Strand = s(++i).FirstOrDefault,
                .Accession = s(++i).GetValue,
                .Gene = s(++i),
                .COG = s(++i),
                .Function = s(++i)
            }
            i = 0
        Next

        Return out.ToArray
    End Function
End Class
