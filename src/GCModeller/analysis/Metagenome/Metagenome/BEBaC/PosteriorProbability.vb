﻿#Region "Microsoft.VisualBasic::74ad1ac11017d5e8292e2b50df9b0262, ..\Metagenome\BEBaC\PosteriorProbability.vb"

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
Imports Microsoft.VisualBasic.DataMining
Imports Microsoft.VisualBasic.DataMining.DirichletDistribution
Imports Microsoft.VisualBasic.Linq

Namespace BEBaC

    Public Module PosteriorProbability

        ''' <summary>
        ''' For all ``3-mer`` count vectors in a crude cluster ``c``, **we
        ''' assume the probability To observe any ``3-mer`` Is the same.**
        ''' 
        ''' Here we denote the probabilities To observe the ``3-mers`` In
        ''' cluster ``c`` As ``(pc1, pc2, ... , pc64)``. Then, the conditional 
        ''' likelihood of the data Is defined as
        ''' 
        ''' ```
        ''' p(y{n}|D,S) = ∏{c,1->k}∏{j,1->64}pcj
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        <Extension>
        Public Function Probability(c As IEnumerable(Of I3merVector)) As Double
            Dim a As Double = gamma1 / (1.0R + I3Mersx.Select(Function(j) c.nj(j)).Sum).Γ
            Dim b As Double = I3Mersx.Select(Function(j) (lambda_cj + c.nj(j)).Γ / gammaj).π
            Dim o As Double = a * b

            Return o
        End Function

        <Extension>
        Public Function Probability(c As Cluster) As Double
            Return c.members.Probability
        End Function

        ''' <summary>
        ''' Provides an analytical form of the marginal likelihood Of ``y(N)`` 
        ''' given the partition ``S``, which Is proportional to the posterior 
        ''' probability as suggested by Equation (2).
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        <Extension>
        Public Function MarginalLikelihood(s As IEnumerable(Of Cluster)) As Double
            Return s.Select(AddressOf Probability).π
        End Function

        ReadOnly gamma1 As Double = 1.0R.Γ
        ReadOnly gammaj As Double = lambda_cj.Γ

        Const lambda_cj As Double = 1.0R / 64.0R

        ''' <summary>
        ''' Where ``ncj = ∑yij`` is the total count Of the j-th ``3-mer`` in cluster ``<paramref name="c"/>``.
        ''' </summary>
        ''' <param name="c"></param>
        ''' <param name="j"></param>
        ''' <returns></returns>
        <Extension>
        Public Function nj(c As IEnumerable(Of I3merVector), j As I3Mers) As Integer
            Return c.Select(Function(x) x.Vector(j)).Sum
        End Function
    End Module
End Namespace
