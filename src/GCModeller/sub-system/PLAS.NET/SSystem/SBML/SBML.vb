﻿#Region "Microsoft.VisualBasic::08bbc12719c05031d398490ca684f0c7, ..\GCModeller\sub-system\PLAS.NET\SSystem\SBML\SBML.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.Analysis.SSystem.Script
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.Model.SBML.Level2
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Public Class SBML : Inherits Compiler(Of Script.Model)

    Dim SBMLFile As XmlFile

    Public Property AutoFixError As Boolean = False

    Public Overrides Function Compile(Optional args As CommandLine = Nothing) As Script.Model
        Dim Model As New Script.Model

        Call __generateSystem(Model)

        Model.Title = SBMLFile.Model.name
        Model.FinalTime = 100

        Return Model
    End Function

    ''' <summary>
    ''' Generate the equation group of the target modelling system.(生成目标模型系统的方程组)
    ''' </summary>
    ''' <param name="Model"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __generateSystem(Model As Script.Model) As Boolean
        Dim reactions As New List(Of SEquation)
        Dim metabolites As New List(Of var)

        For Each m As Specie In SBMLFile.Model.listOfSpecies
            If Not IsEntry(SBMLFile, m.ID) Then
                reactions += New SEquation(m.ID, GenerateFunction(m.ID))
            End If

            metabolites += New var With {
                .UniqueId = m.ID,
                .Title = m.name,
                .Value = m.InitialAmount
            }
        Next

        Model.sEquations = reactions.ToArray
        Model.Vars = metabolites.ToArray

        Return True
    End Function

    Private Function GenerateFunction(Metabolite As String) As String
        Dim expr As New StringBuilder(1024)
        For Each Reaction In GetAllProduce(SBMLFile, Metabolite)
            expr.AppendFormat("{0}+", __contact(Reaction.Reactants))
        Next
        For Each Reaction In GetAllConsume(SBMLFile, Metabolite)
            expr.AppendFormat("{0}-", __contact(Reaction.Products))
        Next

        Return expr.ToString
    End Function

    Private Function __contact(e As IEnumerable(Of speciesReference)) As String
        Dim sBuilder As StringBuilder = New StringBuilder(128)
        For Each Metabolite In e
            sBuilder.AppendFormat("{0}*", Metabolite.species)
        Next
        Call sBuilder.Remove(sBuilder.Length - 1, 1)
        Return sBuilder.ToString
    End Function

    Public Overloads Shared Function Compile(path As String, Optional AutoFix As Boolean = False) As Script.Model
        Using Compiler As New SBML With {
            .SBMLFile = path,
            .AutoFixError = AutoFix
        }
            Return Compiler.Compile
        End Using
    End Function

    Public Shared Widening Operator CType(path As String) As SBML
        Return New SBML With {
            .SBMLFile = path
        }
    End Operator

    Protected Overrides Function Link() As Integer
        Return -1
    End Function

    Public Overrides Function PreCompile(ARGV As CommandLine) As Integer
        Return -1
    End Function
End Class
