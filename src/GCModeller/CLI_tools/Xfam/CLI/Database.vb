﻿#Region "Microsoft.VisualBasic::f7139a4513b14e9141de9869170c0166, ..\GCModeller\CLI_tools\Xfam\CLI\Database.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports SMRUCC.genomics.Data.Xfam.Rfam

Partial Module CLI

    ''' <summary>
    ''' 包括文件复制以及FormatDb
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Install.Rfam", Usage:="--Install.Rfam /seed <rfam.seed>")>
    Public Function InstallRfam(args As CommandLine.CommandLine) As Integer
        Dim input As String = args("/seed")
        Dim respo As String = GCModeller.FileSystem.Xfam.Rfam.Rfam
        Dim seedDb As Dictionary(Of String, Stockholm) = ReadDb(input)
        Dim outDIR As String = GCModeller.FileSystem.Xfam.Rfam.RfamFasta
        Dim out As String = respo & "/Rfam.Csv"

        For Each rfam As KeyValuePair(Of String, Stockholm) In seedDb
            Dim path As String = outDIR & $"/{rfam.Key}.fasta"
            Call rfam.Value.Alignments.Save(-1, path, Encodings.ASCII)
        Next

        Return seedDb.Values.SaveTo(out).CLICode
    End Function
End Module
