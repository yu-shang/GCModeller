﻿#Region "Microsoft.VisualBasic::5562a782f521ecb5fe129ddea6222acf, ..\VisualBasic_AppFramework\Microsoft.VisualBasic.DataMining.Framework\QLearning\Maze-Q-Learning\Program.vb"

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

Imports Microsoft.VisualBasic.DocumentFormat.Csv


Module Program




    Sub Main()

        Call {New table, New table, New table, New table}.SaveTo("x:\fff_test.csv")

        Dim maze As Maze = New Maze
        Call maze.RunLearningLoop(200)
    End Sub
End Module


Public Class table
    Public Property x As Integer
End Class
