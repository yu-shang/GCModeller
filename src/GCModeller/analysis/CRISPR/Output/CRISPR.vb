﻿Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.CRISPR.SearchingModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.gbExportService
Imports LANS.SystemsBiology
Imports LANS.SystemsBiology.NCBI.Extensions.Analysis
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.ComponentModel

Namespace Output

    Public Class CRISPR : Implements IAddressHandle

        ''' <summary>
        ''' 每一个CRISPR位点之中的重复片段位点
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property RepeatLocis As Loci()
        ''' <summary>
        ''' 每一个CRISPR位点之中的重复片段之间的间隔序列（外源DNA片段）
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property SpacerLocis As Loci()
        <XmlAttribute> Public Property ID As Integer Implements IAddressHandle.Address
        <XmlAttribute> Public Property Start As Integer

        ''' <summary>
        ''' last_left + last_length
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Right As Integer
            Get
                Return RepeatLocis.Last.Left + Len(RepeatLocis.Last.SequenceData)
            End Get
        End Property

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace