﻿#Region "Microsoft.VisualBasic::bccf60045d50a6ee3eccdd77df2df7de, ..\GCModeller\models\SBML\SBML\Level2\Elements\speciesReference.vb"

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
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Level2.Elements

    ''' <summary>
    ''' 生化反应对象对代谢物的引用类型
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("speciesReference", Namespace:="http://www.sbml.org/sbml/level2")>
    Public Class speciesReference : Implements ICompoundSpecies

        ''' <summary>
        ''' Unique-Id for the metabolite.(目标参加本反应的代谢物对象的Unique-ID属性)
        ''' </summary>
        ''' <remarks></remarks>
        <Escaped> <XmlAttribute()>
        Public Property species As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' (化学计量数)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute()>
        Public Property stoichiometry As Double Implements ICompoundSpecies.StoiChiometry

        Sub New()
        End Sub

        Sub New(x As ICompoundSpecies)
            species = x.Identifier
            stoichiometry = x.StoiChiometry
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("species={0}; stoichiometry={1}", species, stoichiometry)
        End Function

        Public Overloads Function Equals(b As speciesReference, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        Public Function CopyData() As speciesReference
            Return New speciesReference With {
                .species = Me.species,
                .stoichiometry = Me.stoichiometry
            }
        End Function
    End Class
End Namespace
