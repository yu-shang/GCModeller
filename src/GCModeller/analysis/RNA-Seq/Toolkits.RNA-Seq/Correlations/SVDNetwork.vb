﻿#Region "Microsoft.VisualBasic::24204324fe833af53989578a0de8c390, ..\GCModeller\analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\SVDNetwork.vb"

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
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Mathematical.Matrix
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT

<[Namespace]("Network.SVD",
             Description:="Try creates a draft gene expression regulation network by using Matrix SVD method.")>
Public Module SVDNetwork

    <ExportAPI("Matrix.Create.From.Chipdata")>
    Public Function CreateMatrix(ChipData As MatrixFrame) As GeneralMatrix
        Dim MAT As Double()() = (From csvLine As DocumentStream.RowObject
                                 In ChipData.GetOriginalMatrix
                                 Select (From col As String
                                         In csvLine.Skip(1)
                                         Select Val(col)).ToArray).ToArray
        Dim Matrix = New GeneralMatrix(MAT)
        Return Matrix
    End Function

    <ExportAPI("Reconstruct")>
    Public Function Reconstruct(MAT As GeneralMatrix) As GeneralMatrix
        Dim SVD = New SingularValueDecomposition(MAT)
        Dim U = SVD.GetU
        Dim V = SVD.GetV.Transpose
        Dim E = SVD.S

        Dim Y = New GeneralMatrix((From Line As Double() In E.Array
                                   Select (From i As Double In Line
                                           Let GetValue As Double = If(i <> 0.0R, 0R, 1.0R)
                                           Select GetValue).ToArray).ToArray)
        E = New GeneralMatrix((From Line As Double() In E.Array
                               Select (From i As Double In Line
                                       Let getValue As Double = If(i = 0.0R, 0R, 1 / i)
                                       Select getValue).ToArray).ToArray)
        Dim YV = Y.ArrayMultiply(V)
        Dim UEV = U.Multiply(E)
        UEV = UEV.Multiply(V)

        Dim W = UEV.Multiply(MAT.Transpose)
        Return W
    End Function

    <ExportAPI("Write.Matrix")>
    Public Function SaveMatrix(MAT As GeneralMatrix, saveto As String) As Boolean
        Dim LQuery = (From line As Double() In MAT.Array
                      Select CType((From col In line Select CStr(col)).ToArray, DocumentStream.RowObject)).ToArray
        Call CType(LQuery, DocumentStream.File).Save(saveto, False)
        Return True
    End Function
End Module

