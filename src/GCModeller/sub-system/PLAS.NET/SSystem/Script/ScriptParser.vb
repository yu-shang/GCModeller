﻿#Region "Microsoft.VisualBasic::123f896535b0c23f6f2a0b00ee191bd1, ..\GCModeller\sub-system\PLAS.NET\SSystem\Script\ScriptParser.vb"

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

Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Script

    Public Module ScriptParser

        Public Function sEquationParser(x As Token(Of Tokens)) As SEquation
            Dim value = x.TokenValue.GetTagValue("=")
            Return New SEquation With {
                .x = value.Name,
                .Expression = value.x
            }
        End Function

        Public Function ExperimentParser(line As String) As Experiment
            Dim Tokens As String() = line.Split
            Dim Dict As New Dictionary(Of String, String)
            Dim Disturb As New Experiment

            For i As Integer = 1 To Tokens.Length - 2 Step 2
                Dict.Add(Tokens(i), Tokens(i + 1))
            Next

            Disturb.Start = Val(Dict("START"))
            Disturb.Interval = Val(Dict("INTERVAL"))
            Disturb.Kicks = Val(Dict("KICKS"))
            Disturb.Id = Dict("OBJECT")
            Dim Value As String = Dict("VALUE")

            If InStr(Value, "++") = 1 Then
                Disturb.DisturbType = Types.Increase
                Disturb.Value = Val(Mid(Value, 3))
            ElseIf InStr(Value, "--") = 1 Then
                Disturb.DisturbType = Types.Decrease
                Disturb.Value = Val(Mid(Value, 3))
            Else
                Disturb.DisturbType = Types.ChangeTo
                Disturb.Value = Val(Value)
            End If

            Return Disturb
        End Function

        Public Function ParseFile(path As String) As Model
            Dim lines As String() = path.ReadAllLines
            Dim tokens As Token(Of Tokens)() = TokenIcer.TryParse(lines)
            Dim typeTokens = (From x As Token(Of Tokens)
                              In tokens
                              Select x
                              Group x By x.Type Into Group) _
                                   .ToDictionary(Function(x) x.Type,
                                                 Function(x) x.Group.ToArray)

            Dim equations = typeTokens(Script.Tokens.Reaction).ToArray(AddressOf sEquationParser)
            Dim inits = typeTokens(Script.Tokens.InitValue).ToArray(Function(x) CType(x.Text, var))
            Dim Disturbs As Experiment()
            Dim FinalTime As Integer

            If typeTokens.ContainsKey(Script.Tokens.Disturb) Then
                Disturbs = typeTokens(Script.Tokens.Disturb).ToArray(Function(x) ExperimentParser(x.Text))
            Else
                Disturbs = {}
            End If

            If Not typeTokens.ContainsKey(Script.Tokens.Time) Then
                FinalTime = 100
            Else
                FinalTime = Val(typeTokens(Script.Tokens.Time).First.Text)
            End If

            Dim Title As String

            If Not typeTokens.ContainsKey(Script.Tokens.Title) Then
                Title = "UNNAMED TITLE"
            Else
                Title = typeTokens(Script.Tokens.Title).First.Text
            End If

            Dim Comments As String() =
                If(typeTokens.ContainsKey(Script.Tokens.Comment),
                typeTokens(Script.Tokens.Comment).ToArray(Function(x) x.Text),
                {})

            Dim model As New Model With {
                .sEquations = equations,
                .Vars = inits,
                .Experiments = Disturbs,
                .Comment = Comments.JoinBy(vbCrLf),
                .FinalTime = FinalTime,
                .Title = Title
            }
            Dim NameList As String()

            If typeTokens.ContainsKey(Script.Tokens.Alias) Then
                NameList = typeTokens(Script.Tokens.Alias).ToArray(Function(x) x.Text)
            Else
                NameList = {}
            End If

            For Each s As String In NameList
                s = Mid(s, 7)
                Dim Name As String = s.Split.First
                model.FindObject(s).Title = Mid(s, Len(Name) + 2)
            Next

            Dim sc As IEnumerable(Of String) =
                If(typeTokens.ContainsKey(Script.Tokens.SubsComments),
                typeTokens(Script.Tokens.SubsComments).Select(Function(x) x.Text),
                {})

            For Each s As String In sc
                s = Mid(s, 9)
                Dim Name As String = s.Split.First
                model.FindObject(Name).Comment = Mid(s, Len(Name) + 2)
            Next

            model.UserFunc =
                If(typeTokens.ContainsKey(Script.Tokens.Function),
                typeTokens(Script.Tokens.Function).ToArray(Function(x) CType(x.Text, [Function])),
                {})
            model.Constant =
                If(typeTokens.ContainsKey(Script.Tokens.Constant),
                typeTokens(Script.Tokens.Constant).ToArray(Function(x) ScriptParser.ConstantParser(x.Text)),
                {})

            Return model
        End Function

        Public Function ConstantParser(expr As String) As NamedValue(Of String)
            Dim name As String = expr.Trim.ShadowCopy(expr).Split.First
            expr = Mid(expr, name.Length + 1).Trim
            Return New NamedValue(Of String) With {
                .x = expr,
                .Name = name
            }
        End Function
    End Module
End Namespace
