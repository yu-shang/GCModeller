﻿#Region "Microsoft.VisualBasic::dd44a3367d639708591bb91498fbd6a0, ..\Microsoft.VisualBasic.Architecture.Framework\Extensions\Collection\ListExtensions.vb"

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

Imports System.Runtime.CompilerServices

''' <summary>
''' Initializes a new instance of the <see cref="List"/>`1 class that
''' contains elements copied from the specified collection and has sufficient capacity
''' to accommodate the number of elements copied.
''' </summary>
Public Module ListExtensions

    ''' <summary>
    ''' Initializes a new instance of the <see cref="List"/>`1 class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="source">The collection whose elements are copied to the new list.</param>
    <Extension> Public Function ToList(Of T, TOut)(source As IEnumerable(Of T),
                                                   [CType] As Func(Of T, TOut),
                                                   Optional parallel As Boolean = False) As List(Of TOut)
        If source Is Nothing Then
            Return New List(Of TOut)
        End If

        Dim result As List(Of TOut)

        If parallel Then
            result = (From x As T In source.AsParallel Select [CType](x)).ToList
        Else
            result = (From x As T In source Select [CType](x)).ToList
        End If

        Return result
    End Function

    ''' <summary>
    ''' Initializes a new instance of the <see cref="List"/>`1 class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="source">The collection whose elements are copied to the new list.</param>
    <Extension> Public Function ToList(Of T)(source As IEnumerable(Of T)) As List(Of T)
        Return New List(Of T)(source)
    End Function

    ''' <summary>
    ''' Initializes a new instance of the <see cref="List"/>`1 class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="linq">The collection whose elements are copied to the new list.</param>
    <Extension> Public Function ToList(Of T)(linq As ParallelQuery(Of T)) As List(Of T)
        Return New List(Of T)(linq)
    End Function
End Module
