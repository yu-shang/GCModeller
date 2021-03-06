﻿#Region "Microsoft.VisualBasic::de3cbd5b3de3d605a20aaad7c987d9d0, ..\GCModeller\sub-system\PLAS.NET\PLAS\CLI.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports SMRUCC.genomics.Analysis.SSystem
Imports SMRUCC.genomics.Analysis.SSystem.Script

<PackageNamespace("PLAS.CLI", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gmail.com")>
Public Module CLI

    <ExportAPI("Run", Info:="run a model file of the biochemical network system.",
        Usage:="run -i <model_file> -f <script/model/sbml> -chart <T/F> -o <output_csv>",
        Example:="run -i ""/home/xieguigang/proj/xcc8004.sbml"" -f sbml -chart T -o ""/home/xieguigang/Desktop/xcc8004.csv""")>
    <ParameterInfo("-i", False, Description:="The file path of the input model file that will be run on the PLAS program.", Example:="/home/xieguigang/proj/xcc8004.sbml")>
    <ParameterInfo("-f", True,
        Description:="This parameter specific that the file format of the model file which will be run on the PLAS program." & vbCrLf &
                     " script - The input file that specific by the switch parameter ""-i"" is a PLAS script file," & vbCrLf &
                     " model - The input file is a compiled PLAS model, run it directly," & vbCrLf &
                     " sbml - The input file is a sbml model file, it needs to be compiled to a PLAS model first.",
        Example:="model")>
    <ParameterInfo("-chart", True,
        Description:="Optional, This switch specific that PLAS displaying a chart windows after the calculation or not, default is F for not displaying." & vbCrLf &
                     " T - (True) display a chart window after the calculation," & vbCrLf &
                     " F - (False) not display a chart window after the calculation.",
        Example:="/home/xieguigang/proj/xcc8004.sbml")>
    <ParameterInfo("-o", False, Description:="The file path of the output data file for the calculation.", Example:="/home/xieguigang/Desktop/xcc8004.csv")>
    Public Function Run(args As CommandLine) As Integer
        Return RunMethods(args <= "-f")(args)
    End Function

    <ExportAPI("Compile", Info:="Compile a script file or sbml file into the plas model file.",
        Usage:="compile -i <file> -f <script/sbml> -o <output_file> [/auto-fix]",
        Example:="compile -i ""/home/xieguigang/proj/metacyc/xcc8004/17.0/data/metabolic-reactions.sbml"" -f sbml -o ""/home/xieguigang/Desktop/xcc8004.xml""")>
    Public Function Compile(args As CommandLine) As Integer
        Dim AutoFix As Boolean = args.GetBoolean("/auto-fix")
        Return Compilers(args <= "-f")(args <= "-i", args <= "-o", AutoFix).CLICode
    End Function
End Module

