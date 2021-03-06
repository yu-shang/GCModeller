﻿#Region "Microsoft.VisualBasic::b780ea42848f2887c4223f06d477af2a, ..\GCModeller\analysis\Annotation\CEG\Core.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace CEG

    ''' <summary>
    ''' ceg_core.csv
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Core : Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.sIdEnumerable

        ''' <summary>
        ''' 依靠本属性进行Group操作
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> <Column("access_num")> Public Property AccessNum As String
        <XmlAttribute> <Column("gid")> Public Property GId As String _
            Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.sIdEnumerable.Identifier
        <XmlAttribute> <Column("koid")> Public Property KOId As String
        <XmlAttribute> <Column("cogid")> Public Property COGId As String
        <XmlAttribute> <Column("hprd_nid")> Public Property Hprd_nId As String
        <XmlAttribute> <Column("nhit_ref")> Public Property Nhitref As String
        <Column("nevalue")> Public Property nEvalue As String
        <Column("nscore")> Public Property nScore As String
        <XmlAttribute> <Column("hprd_aid")> Public Property Hprd_aId As String
        <XmlAttribute> <Column("ahit_ref")> Public Property Ahitref As String
        <Column("aevalue")> Public Property aEvalue As String
        <Column("ascore")> Public Property aScore As String
        <XmlAttribute> <Column("degid")> Public Property DEGId As String
        <XmlAttribute> <Column("oganismid")> Public Property OganismId As String

        Public Overrides Function ToString() As String
            Return AccessNum
        End Function
    End Class
End Namespace
