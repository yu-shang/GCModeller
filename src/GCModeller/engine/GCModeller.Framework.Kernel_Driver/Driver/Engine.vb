﻿#Region "Microsoft.VisualBasic::a93c0ce2cc0c734be31927684a42bd74, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\Engine.vb"

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

Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.LDM

''' <summary>
''' The simulation mechanism of this calculation math engine is that we calculates the finited iteration of the dynamics expression in this engine.
''' (计算引擎的基本工作原理是对动力学方程组进行有限次的迭代计算)
''' </summary>
''' <remarks></remarks>
Public MustInherit Class IterationMathEngine(Of T_Model As ModelBaseType) : Inherits ReactorMachine(Of Double, Expression)

    ''' <summary>
    ''' 驱动本计算引擎对象的数据采集服务对象的工作
    ''' </summary>
    ''' <remarks></remarks>
    Protected Friend __runDataAdapter As System.Action
    Protected _innerDataModel As T_Model
    Protected _RTime As Integer

    Sub New(Model As T_Model)
        Me._innerDataModel = Model
    End Sub

    Public Overrides Function Initialize() As Integer
        Return -1
    End Function

    Public Overridable Function Run() As Integer
        For Me._RTime = 0 To IterationLoops
            Call __innerTicks(_RTime)
            Call __runDataAdapter()
        Next

        Return 0
    End Function

    Protected MustOverride Overrides Function __innerTicks(KernelCycle As Integer) As Integer

    Public Overrides ReadOnly Property RuntimeTicks As Long
        Get
            Return _RTime
        End Get
    End Property
End Class

