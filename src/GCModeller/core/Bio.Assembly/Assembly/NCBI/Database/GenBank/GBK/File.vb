﻿#Region "Microsoft.VisualBasic::2a95a89f2f7e5d02d34ff00642bbd9a8, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\File.vb"

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
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' Genbank数据库文件的构件
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class IgbComponent

        ''' <summary>
        ''' 这个构件对象所处在的Genbank数据库对象
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend gbRaw As File
    End Class

    ''' <summary>
    ''' NCBI GenBank database file.(NCBI GenBank数据库文件)
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <PackageNamespace("NCBI.Genbank.GBFF")>
    Public Class File : Inherits ITextFile

        Public Property Comment As Keywords.COMMENT
        ''' <summary>
        ''' This GenBank keyword section stores the sequence data for this database.
        ''' </summary>
        ''' <returns></returns>
        Public Property Origin As Keywords.ORIGIN
        Public Property Features As Keywords.FEATURES.FEATURES
        ''' <summary>
        ''' LocusID, GI or AccessionID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Accession As Keywords.ACCESSION
        Public Property Reference As Keywords.REFERENCE
        ''' <summary>
        ''' The definition value for this organism's GenBank data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Definition As Keywords.DEFINITION
        Public Property Version As Keywords.VERSION
        Public Property Source As Keywords.SOURCE
        ''' <summary>
        ''' The brief entry information of this genbank data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Locus As Keywords.LOCUS
        Public Property Keywords As Keywords.KEYWORDS
        Public Property DbLink As DBLINK

        ''' <summary>
        ''' 这个Genbank对象是否为一个质粒的基因组数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsPlasmidSource As Boolean
            Get
                Return Not String.IsNullOrEmpty(Features.SourceFeature.Query("plasmid"))
            End Get
        End Property

        Public ReadOnly Property Taxon As String
            Get
                Dim db_xref As String() = Features.SourceFeature.QueryDuplicated("db_xref")
                Dim LQuery = (From s As String
                              In db_xref
                              Let tokens As String() = s.Split(CChar(":"))
                              Where String.Equals(tokens.First, "taxon", StringComparison.OrdinalIgnoreCase)
                              Select tokens.Last).FirstOrDefault
                Return LQuery
            End Get
        End Property

        ''' <summary>
        ''' 这个Genbank对象是否具有序列数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HasSequenceData As Boolean
            Get
                If Origin Is Nothing Then
                    Return False
                Else
                    Return Not String.IsNullOrEmpty(Origin.SequenceData)
                End If
            End Get
        End Property

        ''' <summary>
        ''' This GenBank data is the WGS(Whole genome shotgun) type data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsWGS As Boolean
            Get
                For Each KeyWord As String In GenBank.GBFF.Keywords.DEFINITION.WGSKeywords
                    If InStr(Definition.Value, KeyWord, CompareMethod.Text) > 0 Then
                        Return True
                    End If

                    For Each s As String In Keywords
                        If InStr(s, KeyWord) = 1 Then
                            Return True
                        End If
                    Next
                Next

                Return False
            End Get
        End Property

        ''' <summary>
        ''' Gets the original source brief entry information of this genome.(获取这个基因组的摘要信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SourceFeature As Feature
            Get
                Return Features.SourceFeature
            End Get
        End Property

        ''' <summary>
        ''' Read the gene nucleic acid sequence of a gene feature and then returns a fasta sequence object.
        ''' (读取一个基因特性的核酸序列，该Feature对象可以为任意形式的Qualifier的值，但是必需要具有Location属性)
        ''' </summary>
        ''' <param name="Feature">The target feature site on the genome DNA sequence.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Read(Feature As Feature) As FASTA.FastaToken
            Dim Left As Long = Feature.Location.Locations.First.Left
            Dim Right As Long = Feature.Location.Locations.Last.Right
            Dim Sequence As String = Mid(Origin, Left, Math.Abs(Left - Right))

            If Feature.Location.Complement Then
                Sequence = (NucleicAcid.Complement(Sequence))
            End If

            Dim fa As New FASTA.FastaToken With {
                .SequenceData = Sequence,
                .Attributes = New String() {
                    "Feature",
                    Feature.Location.ToString,
                    Feature.KeyName
                }
            }

            Return fa
        End Function

        ''' <summary>
        ''' Read a specific GenBank database text file.
        ''' (读取一个特定的GenBank数据库文件)
        ''' </summary>
        ''' <param name="Path">The target database text file to read.(所要读取的目标数据库文件)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Shadows Widening Operator CType(Path As String) As NCBI.GenBank.GBFF.File
            Return NCBI.GenBank.GBFF.File.Read(Path)
        End Operator

        ''' <summary>
        ''' 当发生错误的时候，会返回空值
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Load")>
        Public Shared Function Load(Path As String) As NCBI.GenBank.GBFF.File
            Try
                Return File.Read(Path)
            Catch ex As Exception
                ex = New Exception(Path.ToFileURL, ex)
                Call ex.PrintException
                Return App.LogException(ex)
            End Try
        End Function

        ''' <summary>
        ''' 将一个GBK文件从硬盘文件之中读取出来，当发生错误的时候，会抛出错误
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Read")>
        Public Overloads Shared Function Read(Path As String) As NCBI.GenBank.GBFF.File
            Dim File As String() = IO.File.ReadAllLines(Path)
            Dim GenBank = __loadData(File, Path)

            Return GenBank
        End Function

        Private Shared Function __originReadThread(gb As NCBI.GenBank.GBFF.File, buf As String()) As NCBI.GenBank.GBFF.Keywords.ORIGIN
            Dim ChunkBuffer As String() = File.Internal_readBlock(KeyWord.GBK_FIELD_KEY_ORIGIN, buf)

            If ChunkBuffer.IsNullOrEmpty Then
                Call $"{gb.FilePath.ToFileURL} have no sequence data.".__DEBUG_ECHO
                Return New ORIGIN With {.SequenceData = ""}
            Else
                Return CType(ChunkBuffer.Skip(1).ToArray, ORIGIN)
            End If
        End Function

        Private Shared Function __loadData(innerBufs As String(), Path As String) As NCBI.GenBank.GBFF.File
            Call "Start loading ncbi gbk file...".__DEBUG_ECHO

            Dim Sw As Stopwatch = Stopwatch.StartNew
            Dim gb As New File With {
                .FilePath = Path
            }
            Dim ReadThread As Action(Of File, String()) = AddressOf __readOrigin
            Dim ReadThreadResult As IAsyncResult = ReadThread.BeginInvoke(gb, innerBufs, Nothing, Nothing)

            gb.Comment = Internal_readBlock(KeyWord.GBK_FIELD_KEY_COMMENT, innerBufs)
            gb.Features = Internal_readBlock(KeyWord.GBK_FIELD_KEY_FEATURES, innerBufs).Skip(1).ToArray
            gb.Accession = ACCESSION.CreateObject(NCBI.GenBank.GBFF.File.Internal_readBlock(KeyWord.GBK_FIELD_KEY_ACCESSION, innerBufs), Path.BaseName)
            gb.Reference = REFERENCE.InternalParser(innerBufs)
            gb.Definition = Internal_readBlock(KeyWord.GBK_FIELD_KEY_DEFINITION, innerBufs)
            gb.Version = Internal_readBlock(KeyWord.GBK_FIELD_KEY_VERSION, innerBufs)
            gb.Source = Internal_readBlock(KeyWord.GBK_FIELD_KEY_SOURCE, innerBufs)
            gb.Locus = LOCUS.InternalParser(NCBI.GenBank.GBFF.File.Internal_readBlock(KeyWord.GBK_FIELD_KEY_LOCUS, innerBufs).First)
            gb.Keywords = GBFF.Keywords.KEYWORDS.__innerParser(Internal_readBlock(KeyWord.GBK_FIELD_KEY_KEYWORDS, innerBufs))
            gb.DbLink = GBFF.Keywords.DBLINK.Parser(Internal_readBlock(KeyWord.GBK_FIELD_KEY_DBLINK, innerBufs))

            gb.Accession.gbRaw = gb
            gb.Comment.gbRaw = gb
            gb.Definition.gbRaw = gb
            gb.Features.gbRaw = gb
            gb.Keywords.gbRaw = gb
            gb.Locus.gbRaw = gb
            gb.Reference.gbRaw = gb
            gb.Source.gbRaw = gb
            gb.Version.gbRaw = gb
            gb.DbLink.gbRaw = gb

            Call gb.Features.LinkEntry()
            Call ReadThread.EndInvoke(ReadThreadResult)
            Call $"({gb.Accession.AccessionId})""{gb.Definition.Value}"" data load done!  {FileIO.FileSystem.GetFileInfo(Path).Length}bytes {Sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO

            gb.Origin.gbRaw = gb  '由于使用线程进行读取的，所以不能保证在赋值的时候是否初始化基因组序列完成
            innerBufs = Nothing

            Return gb
        End Function

        Private Shared Sub __readOrigin(gb As File, bufs As String())
            gb.Origin = __originReadThread(gb, buf:=bufs)
        End Sub

        ''' <summary>
        ''' 快速读取数据库文件中的某一个字段的文本块
        ''' </summary>
        ''' <param name="keyword">字段名</param>
        ''' <returns>该字段的内容</returns>
        ''' <remarks></remarks>
        Private Shared Function Internal_readBlock(Keyword As String, ByRef innerBufs As String()) As String()
            Dim Regx As New Regex(String.Format("^{0}\s+.+$", Keyword))
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From str As String
                                           In innerBufs
                                           Where Regx.Match(str).Success OrElse
                                               String.Equals(str, Keyword)
                                           Select str
            Dim index As Integer
            Dim p As Integer
            Dim Data() As String = Nothing

            For Each Head As String In LQuery
                index = Array.IndexOf(innerBufs, Head)
                p = index + 1

                Do While String.IsNullOrEmpty(innerBufs(p)) OrElse innerBufs(p).First = " "c
                    p += 1
                    If p = innerBufs.Length Then
                        Exit Do
                    End If
                Loop

                Dim sBuf As String() = New String(p - index - 1) {}

                Call Array.ConstrainedCopy(innerBufs,
                                           index,
                                           sBuf,
                                           Scan0,
                                           sBuf.Length)

                If Data Is Null Then
                    index = Scan0
                    ReDim Data(sBuf.Length - 1)
                Else
                    index = Data.Length
                    ReDim Preserve Data(Data.Length + sBuf.Length - 1)
                End If
                Call Array.ConstrainedCopy(sBuf, Scan0, Data, index, sBuf.Length)
            Next

            Return Data
        End Function

        Const GENBANK_MULTIPLE_RECORD_SPLIT As String = "^//$"

        ''' <summary>
        ''' 假若一个gbk文件之中包含有多个记录的话，可以使用这个函数进行数据的加载
        ''' </summary>
        ''' <param name="filePath">The file path of the genbank database file, this gb file may contains sevral gb sections</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Load.DbList", Info:="Using this function to load the ncbi genbank database file if the database file contains more than one genome.")>
        Public Shared Iterator Function LoadDatabase(filePath As String) As IEnumerable(Of File)
            Dim Tokens As String() = Regex.Split(FileIO.FileSystem.ReadAllText(filePath),
                                                 GENBANK_MULTIPLE_RECORD_SPLIT,
                                                 RegexOptions.Multiline)
            Dim sBuf As String()() = (From s As String
                                      In Tokens.AsParallel
                                      Let ss As String = s & vbCrLf & "//"
                                      Select ss.lTokens).ToArray
            Try
                For Each buf As String() In sBuf
                    Dim sDat As String() = __trims(buf)
                    If sDat.IsNullOrEmpty Then
                        Continue For
                    End If
                    Dim gb As File = __loadData(sDat, filePath)
                    Yield gb
                Next
            Catch ex As Exception
                ex = New Exception(filePath, ex)
                Throw ex
            End Try
        End Function

        Private Shared Function __trims(buf As String()) As String()
            Dim i As Integer = 0

            If buf.Length < 5 Then
                Return Nothing
            End If

            Do While String.IsNullOrEmpty(buf.Read(i))
            Loop

            If i = 1 Then
                Return buf
            Else
                i -= 1
            End If

            buf = buf.Skip(i).ToArray
            Return buf
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return GbkWriter.WriteGbk(Me, getPath(FilePath), Encoding)
        End Function
    End Class
End Namespace
