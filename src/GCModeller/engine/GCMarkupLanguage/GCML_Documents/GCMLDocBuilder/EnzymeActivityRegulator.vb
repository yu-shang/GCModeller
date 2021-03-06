﻿#Region "Microsoft.VisualBasic::e35ad4a72ee1d8c3904c99526c68d798, ..\GCModeller\engine\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\EnzymeActivityRegulator.vb"

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

Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Assembly

Namespace Builder

    Public Class EnzymeActivityRegulator : Inherits IBuilder

        Sub New(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder, Model As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
            Call MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Return MyBase.Model
        End Function
    End Class
End Namespace
