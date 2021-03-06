﻿#Region "Microsoft.VisualBasic::ab7799494e51e9e3704beaa7ea7c6307, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\Pathways.vb"

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

Imports System.Net
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Terminal

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' LinkDB Search for KEGG pathways
    ''' </summary>
    Public Module Pathways

        Const KEGGPathwayLinkDB_URL As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+pathway+genome:{0}"

        Public Function URLProvider(sp As String) As String
            Dim url As String = String.Format(KEGGPathwayLinkDB_URL, sp)
            Return url
        End Function

        Const LinkItem As String = "<a href="".+?"">.+?</a>.+?$"

        Public Iterator Function AllEntries(sp As String) As IEnumerable(Of ListEntry)
            Dim html As String = Strings.Split(URLProvider(sp).GET, Modules.SEPERATOR).Last
            Dim Entries As String() =
                Regex.Matches(html, LinkItem, RegexICMul).ToArray

            For Each entry As String In Entries.Take(Entries.Length - 1)
                Dim key As String = Regex.Match(entry, ">.+?</a>").Value
                key = Mid(key, 2, Len(key) - 5)
                Dim Description As String = Strings.Split(entry, "</a>").Last.Trim
                Dim url As String = "http://www.genome.jp" & entry.Get_href

                Yield New ListEntry With {
                    .EntryID = key,
                    .Description = Description,
                    .Url = url
                }
            Next
        End Function

        Public Iterator Function Downloads(sp As String, Optional EXPORT As String = "./LinkDB-Pathways/") As IEnumerable(Of Pathway)
            Dim entries As New List(Of ListEntry)
            Dim briefHash As Dictionary(Of String, BriteHEntry.Pathway) =
                BriteHEntry.Pathway.LoadDictionary
            Dim Downloader As New WebClient()
            Dim Progress As New ProgressBar("KEGG LinkDB Downloads KEGG Pathways....")

            VBDebugger.Mute = True

            Dim all As ListEntry() = AllEntries(sp).ToArray
            Dim i As Integer

            For Each entry As ListEntry In all
                Dim ImageUrl = String.Format("http://www.genome.jp/kegg/pathway/{0}/{1}.png", sp, entry.EntryID)
                Dim pathwayPage = "http://www.genome.jp/dbget-bin/www_bget?pathway+" & entry.EntryID
                Dim path As String = EXPORT & "/webpages/" & entry.EntryID & ".html"
                Dim img As String = EXPORT & $"/{entry.EntryID}.png"
                Dim bCode As String = Regex.Match(entry.EntryID, "\d+").Value

                Call pathwayPage.GET.SaveTo(path)
                Call Downloader.DownloadFile(ImageUrl, img)

                Dim data As Pathway = Pathway.DownloadPage(path)

                entries += entry
                data.Genes = KEGGgenes.Download($"http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+path:{entry.EntryID}").ToArray

                path = BriteHEntry.Pathway.CombineDIR(briefHash(bCode), EXPORT)
                path = path & $"/{entry.EntryID}.Xml"

                Call data.SaveAsXml(path)

                Yield data

                i += 1
                Call Progress.SetProgress(i / all.Length * 100, data.Name)
            Next

            VBDebugger.Mute = False

            Call entries.GetJson.SaveTo(EXPORT & $"/{sp}.json")
        End Function
    End Module
End Namespace
