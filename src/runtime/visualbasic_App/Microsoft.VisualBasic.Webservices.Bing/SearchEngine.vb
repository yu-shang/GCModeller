﻿#Region "Microsoft.VisualBasic::6909c1f462c269734224ecc22d03a5a8, ..\VisualBasic_AppFramework\Microsoft.VisualBasic.Webservices.Bing\SearchEngine.vb"

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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.HtmlParser

''' <summary>
''' Interface wrapper of Microsoft bing.com search engine.
''' </summary>
<[PackageNamespace]("Bing",
                    Publisher:="",
                    Category:=APICategories.UtilityTools,
                    Description:="",
                    Url:="http://cn.bing.com/")>
Public Module SearchEngineProvider

    Const BingURL As String = "https://www.bing.com/search?q={0}&PC=U316&FORM=Firefox"
    Const TotalCount As String = "<span class=""sb_count"">\d+ results</span>"
    Const TranslateThisPage As String = "Translate this page"

    Public Function URLProvider(keyword As String) As String
        Dim Url As String = String.Format(BingURL, WebServiceUtils.UrlEncode(keyword))
        Return Url
    End Function

    ''' <summary>
    ''' Bing.com online web search engine services provider
    ''' </summary>
    ''' <param name="keyword"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("search")>
    Public Function Search(keyword As String) As SearchResult
        Dim url As String = URLProvider(keyword)
        Return DownloadResult(url)
    End Function

    Public Iterator Function GetAllResults(keyword As String) As IEnumerable(Of WebResult)
        Dim page As SearchResult = Search(keyword)

        Do While True
            For Each x As WebResult In page.CurrentPage
                Yield x
            Next

            If page.HaveNext Then
                page = page.NextPage
            Else
                Exit Do
            End If
        Loop
    End Function

    Public Function DownloadResult(url As String) As SearchResult
        Dim web As String = Regex.Replace(url.GET, "<strong>|</strong>", "", RegexICSng)
        Dim count As String = Regex.Match(web, TotalCount).Value
        Dim itms As String() = Strings.Split(
            web.Replace(TranslateThisPage, ""), "<h2>", -1, CompareMethod.Text)
        Dim result As WebResult() = itms.Skip(1).ToArray(AddressOf WebResult.TryParse)
        Dim [next] As String = __getNextPageLink(web)

        count = Regex.Match(count, "\d+").Value

        Return New SearchResult With {
            .CurrentPage = result,
            .Results = Scripting.CTypeDynamic(Of Integer)(count),
            .Next = [next],
            .Title = web.HTMLtitle
        }
    End Function

    Const NextPage As String = "<a href=""/search\?q=.*?"" class=""sb_pagN"" title=""Next """

    Private Function __getNextPageLink(html As String) As String
        Dim link As String = Regex.Match(html, NextPage, RegexICSng).Value
        link = Regex.Matches(link, "<a href=""/search\?q=.*?""", RegexICSng).ToArray.LastOrDefault
        link = link.Get_href
        link = link.Replace("amp;", "")

        If String.IsNullOrEmpty(link) Then
            Return link
        Else
            Return "http://cn.bing.com" & link
        End If
    End Function
End Module

