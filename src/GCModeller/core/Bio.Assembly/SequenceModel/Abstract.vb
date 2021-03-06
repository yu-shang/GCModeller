﻿#Region "Microsoft.VisualBasic::e23bfc8ca6cff17f1bdc706c68dce461, ..\GCModeller\core\Bio.Assembly\SequenceModel\Abstract.vb"

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
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language

Namespace SequenceModel

    ''' <summary>
    ''' Sequence model for a macro biomolecule sequence.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface I_PolymerSequenceModel

        ''' <summary>
        ''' <see cref="System.String"></see> type sequence data for the target <see cref="ISequenceModel"/> sequence model object.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property SequenceData As String
    End Interface

    ''' <summary>
    ''' This class can be using for build a <see cref="FASTA.FastaToken"/> object.
    ''' </summary>
    Public MustInherit Class ISequenceBuilder : Inherits ClassObject

        ''' <summary>
        ''' <see cref="GetSequenceData()"/> length.(序列的长度)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Length As Integer
            Get
                Return Len(GetSequenceData)
            End Get
        End Property

        ''' <summary>
        ''' This property is using for generates the fasta sequence title.(用于进行序列标识的标题摘要)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Overridable Property Name As String

        ''' <summary>
        ''' Data source method for gets the sequence data to create a fasta object.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetSequenceData() As String
    End Class
End Namespace
