﻿#Region "Microsoft.VisualBasic::a00607113221e45a3af44fdb69e367ed, ..\GCModeller\data\ExternalDBSource\string-db\StrPNet\TCS\SensorInducers.vb"

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
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.StrPNet.TCS

    Public Class SensorInducers : Implements sIdEnumerable

        <Column("Sensor")> <XmlAttribute>
        Public Property SensorId As String Implements sIdEnumerable.Identifier
        <Collection("Inducers")> <XmlElement>
        Public Property Inducers As String()

        Public Overrides Function ToString() As String
            Return SensorId
        End Function
    End Class
End Namespace
