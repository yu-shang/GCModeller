﻿#Region "Microsoft.VisualBasic::7a0af505e2e29cc4f33b5afd8c3aa23a, ..\VisualBasic_AppFramework\DocumentFormats\VB_DataFrame\VB_DataFrame\DocumentStream\File.vb"

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
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.FileIO

Namespace DocumentStream

    ''' <summary>
    ''' A comma character seperate table file that can be read and write in the EXCEL.(一个能够被Excel程序所读取的表格文件)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class File : Inherits ITextFile
        Implements IEnumerable(Of RowObject)
        Implements IList(Of RowObject)

        ''' <summary>
        ''' First line in the table is the column name definition line.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _innerTable As List(Of RowObject) = New List(Of RowObject)

        Public Sub New()
        End Sub

        Sub New(data As IEnumerable(Of RowObject))
            _innerTable = data.ToList
        End Sub

        Sub New(path As String)
            FilePath = path
            _innerTable = __loads(path, Encoding.Default)
        End Sub

        Sub New(source As IEnumerable(Of RowObject), path As String)
            Call Me.New(source)
            FilePath = path
        End Sub

        Default Public Overloads Property Item(x As Integer, y As Integer) As String
            Get
                Dim row As RowObject = Me(x)
                Return row(y)
            End Get
            Set(value As String)
                Dim row As RowObject = Me(x)
                row(y) = value
            End Set
        End Property

        ''' <summary>
        ''' Get the max width number of the rows in the table.(返回表中的元素最多的一列的列数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Width As Integer
            Get
                Dim LQuery = From row In _innerTable.AsParallel Select row.NumbersOfColumn '
                Return LQuery.Max
            End Get
        End Property

        ''' <summary>
        ''' Get all data of a column of a specific column number.(获取文件中的某一列中的所有数据)
        ''' </summary>
        ''' <param name="Index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Iterator Property Column(Index As Integer) As IEnumerable(Of String)
            Get
                For Each row As RowObject In _innerTable
                    Yield row.Column(Index)
                Next
            End Get
            Set(value As IEnumerable(Of String))
                Call __setColumn(value.ToArray, Index)
            End Set
        End Property

        Private Sub __setColumn(value As String(), index As Integer)
            If _innerTable.Count < value.Length Then
                Dim d As Integer = value.Length - _innerTable.Count

                For i As Integer = 0 To d - 1
                    Call _innerTable.Add(New RowObject)
                Next
            End If

            For i As Integer = 0 To value.Length - 1
                _innerTable(i).Column(index) = value(i)
            Next
        End Sub

        ''' <summary>
        ''' 将本文件之中的所有列取出来，假若有任意一个列的元素的数目不够的话，则会在相应的位置之上使用空白来替换
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Iterator Property Columns As IEnumerable(Of String())
            Get
                If _innerTable.Count = 0 Then
                    Return
                End If
                For Each column As IEnumerable(Of String) In
                    From col As Integer
                    In Width.Sequence
                    Select Me.Column(col)

                    Yield column.ToArray
                Next
            End Get
        End Property

        Public ReadOnly Property EstimatedFileSize As Double
            Get
                Dim LQuery = (From row As RowObject
                              In Me.AsParallel
                              Select (From col As String
                                      In row
                                      Select CDbl(Len(col))).Sum).Sum
                Return LQuery * 8
            End Get
        End Property

        Public Sub AppendLine(Row As RowObject)
            Call _innerTable.Add(Row)
        End Sub

        Public Sub AppendLine(row As IEnumerable(Of String))
            Call _innerTable.Add(New RowObject(row))
        End Sub

        Public Sub AppendLine()
            Call _innerTable.Add(New String() {" "})
        End Sub

        Public Sub Append(dataframe As File)
            Call _innerTable.AddRange(dataframe._innerTable)
        End Sub

        ''' <summary>
        ''' Add a data row collection into this Csv file object instance and then return the total row number after the add operation.
        ''' (向CSV文件之中批量添加行记录，之后返回当前所打开的文件在添加纪录之后的总行数)
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AppendRange(source As IEnumerable(Of RowObject)) As Long
            Call _innerTable.AddRange(source)
            Return _innerTable.Count
        End Function

        ''' <summary>
        ''' Get a data row in the specific row number, when a row is not exists in current csv file then the function will return a empty row.
        ''' (当目标对象不存在的时候，会返回一个空行对象)
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByLine(line As Integer) As RowObject
            If line > _innerTable.Count - 1 Then
                Return New RowObject
            Else
                Return _innerTable(line)
            End If
        End Function

        ''' <summary>
        ''' 使用迭代器返回包含有目标关键词的行
        ''' </summary>
        ''' <param name="KeyWord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Iterator Function FindAll(KeyWord As String) As IEnumerable(Of RowObject)
            For Each row As RowObject In _innerTable
                If row.LocateKeyWord(KeyWord) > -1 Then
                    Yield row
                End If
            Next
        End Function

        ''' <summary>
        ''' Using the content in a specific column as the target for search using a specific keyword, and then return all of the rows that have the query keyword.
        ''' (以指定的列中的内容搜索关键词，并返回检索成功的行的集合)
        ''' </summary>
        ''' <param name="KeyWord"></param>
        ''' <param name="Column"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FindAtColumn(KeyWord As String, Column As Integer) As RowObject()
            Return LinqAPI.Exec(Of RowObject) <= From row As RowObject
                                                 In _innerTable.AsParallel
                                                 Let strCell As String = row.Column(Column)
                                                 Where InStr(strCell, KeyWord, CompareMethod.Text) > 0 OrElse
                                                     String.Equals(strCell, KeyWord, StringComparison.OrdinalIgnoreCase)
                                                 Select row
        End Function

        ''' <summary>
        ''' Get and set the string content in a specific table cell.(设置或者获取某一个指定的单元格中的字符串内容)
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Cell(X As Integer, Y As Integer) As String
            Get
                Return _innerTable(X).Column(Y)
            End Get
            Set(value As String)
                _innerTable(X).Column(Y) = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return FilePath.ToFileURL
        End Function

        Public Function ToArray() As RowObject()
            Return _innerTable.ToArray
        End Function

        Public Function ToArray(Of T)([ctype] As Func(Of RowObject, T)) As T()
            Dim array As T() = _innerTable.ToArray([ctype])
            Return array
        End Function

        ''' <summary>
        ''' 对当前的csv矩阵进行转置之后返回新的文件
        ''' </summary>
        ''' <returns></returns>
        Public Function Transpose() As File
            Dim buf As String()() = Me.Columns.MatrixTranspose

            Return New File With {
                .FilePath = FilePath,
                ._innerTable =
                    LinqAPI.MakeList(Of RowObject) <=
                        From i As Integer
                        In buf.First.Sequence
                        Select New RowObject(From line As String() In buf Select line(i))
            }
        End Function

        ''' <summary>
        ''' Delete all of the row that meet the delete condition.(将所有满足条件的行进行删除)
        ''' </summary>
        ''' <param name="condition"></param>
        ''' <remarks></remarks>
        Public Function Remove(condition As Func(Of RowObject, Boolean)) As RowObject()
            Dim LQuery As RowObject() =
                LinqAPI.Exec(Of RowObject) <= From row As RowObject
                                              In Me._innerTable
                                              Where True = condition(row)
                                              Select row
            Call RemoveRange(LQuery)
            Return LQuery
        End Function

        ''' <summary>
        ''' 删除目标列中的单元格中符合条件的内容
        ''' </summary>
        ''' <param name="Condition">条件测试</param>
        ''' <param name="index">列标号</param>
        ''' <remarks></remarks>
        Public Sub DeleteCell(Condition As Func(Of String, Boolean), index As Integer)
            For i As Integer = 0 To _innerTable.Count - 1
                Dim row = _innerTable(i)
                If Condition(row(index)) = True Then
                    row(index) = ""
                End If
            Next
        End Sub

        ''' <summary>
        ''' Remove the item in a specific row collection.
        ''' </summary>
        ''' <param name="source"></param>
        ''' <remarks></remarks>
        Public Sub RemoveRange(source As IEnumerable(Of RowObject))
            For Each row As RowObject In source
                Call _innerTable.Remove(row)
            Next
        End Sub

        ''' <summary>
        ''' Generate the csv data file document using the table data.(将表格对象转换为文本文件以进行保存)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Generate() As String
            Dim sBuilder As StringBuilder = New StringBuilder(2048)
            For Each Row As RowObject In _innerTable
                Call sBuilder.AppendLine(Row.AsLine)
            Next

            Return sBuilder.ToString
        End Function

        Public Function GetAllStringTokens() As String()
            Return (From row In Me._innerTable Select row.ToArray).MatrixToVector
        End Function

        ''' <summary>
        ''' 将表对象转换为文本文件之中的文本内容
        ''' </summary>
        ''' <param name="Parallel">假若是不需要顺序的大文件，请设置为True</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GenerateDocument(Parallel As Boolean) As String()
            If Parallel Then
                Return (From row As RowObject In _innerTable.AsParallel Select row.AsLine).ToArray
            Else
                Return (From row As RowObject In _innerTable Select row.AsLine).ToArray
            End If
        End Function

        ''' <summary>
        ''' Insert a new empty line of row data before the specific row number.(在指定列标号的列之前插入一行空列)
        ''' </summary>
        ''' <param name="column"></param>
        ''' <remarks></remarks>
        Public Function InsertEmptyColumnBefore(column As Integer) As Integer
            Dim LQuery = From row As RowObject
                         In _innerTable.AsParallel
                         Select row.InsertAt("", column) '
            Return LQuery.ToArray.Count
        End Function

        ''' <summary>
        ''' 统计某一个指定的列之中的各个项目的出现次数
        ''' </summary>
        ''' <param name="ColumnIndex"></param>
        ''' <param name="FirstLineTitle"></param>
        ''' <param name="IgnoreBlanks"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TokenCounts(ColumnIndex As Integer,
                                    Optional FirstLineTitle As Boolean = True,
                                    Optional IgnoreBlanks As Boolean = True,
                                    Optional IgnoreCase As Boolean = False) As File

            Dim source = If(FirstLineTitle, _innerTable.Skip(1), _innerTable)
            Dim Values As String() = (From row As RowObject
                                      In source.AsParallel
                                      Select row.Column(ColumnIndex)).ToArray ' 选择出该特定的列对象之中的数据
            Dim GroupInit = (From str As String In Values Select str, lower = Strings.LCase(str)).ToArray

            If IgnoreBlanks Then
                GroupInit = (From strData In GroupInit Where Not String.IsNullOrEmpty(strData.str) Select strData).ToArray
            End If

            Dim TokensGroup As KeyValuePair(Of String, Integer)()
            If IgnoreCase Then
                TokensGroup = (From item In GroupInit
                               Select item
                               Group item By item.lower Into Group).ToArray(Function(obj) New KeyValuePair(Of String, Integer)(obj.Group.First.str, obj.Group.Count))
            Else
                TokensGroup = (From item In GroupInit
                               Select item
                               Group item By item.str Into Group).ToArray(Function(obj) New KeyValuePair(Of String, Integer)(obj.str, obj.Group.Count))
            End If

            Dim CsvFile As File = New File
            CsvFile += New RowObject({If(FirstLineTitle, $"Item values for '{First.Column(ColumnIndex)}'", "Item values"), "Counts"})
            CsvFile += From token As KeyValuePair(Of String, Integer)
                       In TokensGroup.AsParallel
                       Select New RowObject({token.Key, CStr(token.Value)})
            Return CsvFile
        End Function

#Region "List Operations"

        ''' <summary>
        ''' Add a row collection
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="source"></param>
        ''' <returns></returns>
        Public Shared Operator +(file As File, source As IEnumerable(Of RowObject)) As File
            Call file.AppendRange(source)
            Return file
        End Operator

        ''' <summary>
        ''' Add a row
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="row"></param>
        ''' <returns></returns>
        Public Shared Operator +(file As File, row As IEnumerable(Of String)) As File
            Call file.Add(New RowObject(row))
            Return file
        End Operator
#End Region

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return Save(FilePath, False, Encoding)
        End Function

        ''' <summary>
        ''' Save this csv document into a specific file location <paramref name="path"/>.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="LazySaved">Optional, this is for the consideration of performance and memory consumption.
        ''' When a data file is very large, then you may encounter a out of memory exception on a 32 bit platform,
        ''' then you should set this parameter to True to avoid this problem. Defualt is False for have a better
        ''' performance.
        ''' (当估计到文件的数据量很大的时候，请使用本参数，以避免内存溢出致使应用程序崩溃，默认为False，不开启缓存)
        ''' </param>
        ''' <remarks>当目标保存路径不存在的时候，会自动创建文件夹</remarks>
        Public Overridable Overloads Function Save(Optional Path As String = "",
                                                   Optional LazySaved As Boolean = False,
                                                   Optional encoding As Encoding = Nothing) As Boolean
            Return StreamIO.SaveDataFrame(Me, getPath(Path), LazySaved, encoding)
        End Function

        ''' <summary>
        ''' 这个方法是保存<see cref="Csv.DataFrame"></see>对象之中的数据所需要的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overridable Function __createTableVector() As RowObject()
            Return Me._innerTable.ToArray
        End Function

#Region "CSV file data loading methods"

        ''' <summary>
        ''' Read a Csv file, default encoding is utf8
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Widening Operator CType(Path As String) As File
            Return File.Load(Path)
        End Operator

        Public Overloads Shared Widening Operator CType(Lines As String()) As File
            'Dim LoadMethod As LoadMethod =
            '    [If](Of LoadMethod)(Lines.Length > 1024, AddressOf __PBS_LOAD, AddressOf __LINQ_LOAD)
            Dim sw As Stopwatch = Stopwatch.StartNew
            Dim CSV As File = __LINQ_LOAD(data:=Lines)

            Call $"Csv load {Lines.Length} lines data in {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO ' //{LoadMethod.ToString}".__DEBUG_ECHO

            Return CSV
        End Operator

        Private Delegate Function LoadMethod(data As String()) As File

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks>为了提高数据的加载效率，先使用LINQ预加载数据，之后使用Parallel LINQ进行数据的解析操作</remarks>
        Private Shared Function __LINQ_LOAD(data As String()) As File
            Dim LQuery = (From line As String
                          In data.AsParallel
                          Let row As RowObject = CType(line, RowObject)
                          Select row).ToList
            Return New File With {
                ._innerTable = LQuery
            }
        End Function

        Public Overloads Shared Widening Operator CType(rows As RowObject()) As File
            Return New File With {
                ._innerTable = rows.ToList
            }
        End Operator

        Public Overloads Shared Widening Operator CType(rows As List(Of RowObject)) As File
            Return New File With {
                ._innerTable = rows
            }
        End Operator

        ''' <summary>
        ''' If you are sure about your csv data document have no character such like " or, in a cell, then you can try using this fast load method to load your csv data.
        ''' if not, please using the <see cref="load"></see> method to avoid of the data damages.
        ''' (假若你确信你的数据文件之中仅含有数字之类的数据，则可以尝试使用本方法进行快速加载，假若文件之中每一个单元格还含有引起歧义的例如双引号或者逗号，则请不要使用本方法进行加载)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FastLoad(path As String, Optional Parallel As Boolean = True, Optional encoding As Encoding = Nothing) As File
            If encoding Is Nothing Then
                encoding = Encoding.Default
            End If

            Dim sw = Stopwatch.StartNew
            Dim lines As String() = IO.File.ReadAllLines(path.MapNetFile, encoding)
            Dim cData As File = New File With {
                .FilePath = path
            }

            If Parallel Then
                Dim cache = (From x As SeqValue(Of String) In lines.SeqIterator Select x)
                Dim Rows = (From line As SeqValue(Of String)
                            In cache.AsParallel
                            Let __innerList As List(Of String) = line.obj.Split(","c).ToList
                            Select i = line.i,
                                data = New RowObject With {._innerColumns = __innerList}
                            Order By i Ascending)
                cData._innerTable = (From item In Rows Select item.data).ToList
            Else
                Dim Rows = (From strLine As String In lines
                            Let InternalList As List(Of String) = strLine.Split(","c).ToList
                            Select New RowObject With {._innerColumns = InternalList}).ToList
                cData._innerTable = Rows
            End If

            Call $"CSV data ""{path.ToFileURL}"" load done!   {sw.ElapsedMilliseconds}ms.".__DEBUG_ECHO

            Return cData
        End Function

        ''' <summary>
        ''' Load the csv data document from a given path.(从指定的文件路径之中加载一个CSV格式的数据文件)
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(Path As String, Optional encoding As System.Text.Encoding = Nothing) As File
            If encoding Is Nothing Then
                encoding = Encoding.Default
            End If
            Dim buf As List(Of RowObject) = __loads(Path, encoding)
            Dim Csv As New File With {
                .FilePath = Path,
                ._innerTable = buf
            }
            Return Csv
        End Function

        ''' <summary>
        ''' 同时兼容本地文件和网络文件的
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Private Shared Function __loads(path As String, encoding As Encoding) As List(Of RowObject)
            Dim lines As String() = IO.File.ReadAllLines(path.MapNetFile, encoding)
            Return Load(lines)
        End Function

        Public Shared Function Load(buf As String()) As List(Of RowObject)
            Dim first As New RowObject(buf(Scan0))
            Dim rows As List(Of RowObject) = (From s As String
                                              In buf.Skip(1).AsParallel
                                              Select New RowObject(s)).ToList
            Return first + rows
        End Function
#End Region

        Public Shared Function Join(CsvList As File()) As File
            CsvList = (From Csv In CsvList Select Csv Order By Csv.Count Descending).ToArray
            Dim CsvFile As File = New File

            For RowId As Integer = 0 To CsvList.First.Count - 1
                Dim Row As RowObject = New RowObject
                Row.AddRange(CsvList.First()(RowId))
                For i As Integer = 1 To CsvList.Count - 1
                    If RowId < CsvList(i).Count Then
                        Row.AddRange(CsvList(i)(RowId))
                    End If
                Next

                CsvFile._innerTable.Add(Row)
            Next

            Return CsvFile
        End Function

        ''' <summary>
        ''' 去除Csv文件之中的重复记录
        ''' </summary>
        ''' <param name="File"></param>
        ''' <param name="OrderBy">当为本参数指定一个非负数值的时候，程序会按照指定的列值进行排序</param>
        ''' <param name="Asc">当进行排序操作的时候，是否按照升序进行排序，否则按照降序排序</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Distinct(File As String, Optional OrderBy As Integer = -1, Optional Asc As Boolean = True) As File
            Dim Csv As Csv.DocumentStream.File = Load(File)
            Return Distinct(Csv, OrderBy, Asc)
        End Function

        ''' <summary>
        ''' 将一些奇怪的符号去除
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Normalization(path As String, replaceAs As String) As File
            Dim Text As String = FileIO.FileSystem.ReadAllText(path)
            Dim Data As String() = Strings.Split(Text, vbCrLf)
            Data = (From strLine As String In Data Select strLine.Replace(vbLf, replaceAs)).ToArray
            Return __LINQ_LOAD(Data)
        End Function

        ''' <summary>
        ''' 去除Csv文件之中的重复记录
        ''' </summary>
        ''' <param name="OrderBy">当为本参数指定一个非负数值的时候，程序会按照指定的列值进行排序</param>
        ''' <param name="Asc">当进行排序操作的时候，是否按照升序进行排序，否则按照降序排序</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Distinct(Csv As File, Optional OrderBy As Integer = -1, Optional Asc As Boolean = True) As File
            Dim Query As Generic.IEnumerable(Of String)

            If OrderBy >= 0 Then
                If Asc Then
                    Query = From row In Csv
                            Let Line As String = row.AsLine
                            Select Line
                            Distinct
                            Order By CType(Line, RowObject).Column(OrderBy) Ascending  '
                Else
                    Query = From row In Csv
                            Let Line As String = row.AsLine
                            Select Line
                            Distinct
                            Order By CType(Line, RowObject).Column(OrderBy) Descending  '
                End If
            Else '仅去重
                Query = From row In Csv
                        Let Line As String = row.AsLine
                        Select Line Distinct '
            End If

            Return New File With {
                ._innerTable = (From Line As String In Query.ToArray Select CType(Line, RowObject)).ToList,
                .FilePath = Csv.FilePath
            }
        End Function

        Public Shared Function RemoveSubRow(df As File) As File
            Dim Query = From row In df.AsParallel Select row Order By row.NotNullColumns.Count Descending '
            df._innerTable = Query.ToList
            For i As Integer = 0 To df.Count - 1
                Dim MRow = df(i)
                Dim RQuery = From row In df.Skip(i + 1).AsParallel Where MRow.Contains(row) Select row '
                Call df.RemoveRange(RQuery.ToArray)
            Next
            Return df
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of RowObject) Implements IEnumerable(Of RowObject).GetEnumerator
            For i As Integer = 0 To _innerTable.Count - 1
                Yield _innerTable(i)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        ''' <summary>
        ''' 判断目标数据文件是否为空
        ''' </summary>
        ''' <param name="df"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsNullOrEmpty(df As IEnumerable(Of RowObject)) As Boolean
            Return df Is Nothing OrElse df.Count = 0
        End Function

        Public Shared Operator <=(df As File, type As Type) As Object()
            Return Reflector.LoadDataToObject(Extensions.DataFrame(df), type, False)
        End Operator

        Public Shared Operator >=(df As File, TypeInfo As System.Type) As Object()
            Return df <= TypeInfo
        End Operator

        Public Shared Operator >(File As File, path As String) As Boolean
            Return File.Save(path, Encoding.ASCII)
        End Operator

        Public Shared Operator <(file As File, path As String) As Boolean
            Throw New NotSupportedException
        End Operator

#Region "Implements of Generic.IList(Of DocumentFormat.Csv.DocumentStream.File.Row) interface"

        Public Sub Add(item As RowObject) Implements ICollection(Of RowObject).Add
            Call _innerTable.Add(item)
        End Sub

        Public Sub Add(ParamArray row As Object())
            Call Add(row.ToArray(Function(c) Scripting.ToString(c)))
        End Sub

        Public Sub Add(ParamArray row As String())
            Call _innerTable.Add(New RowObject(row))
        End Sub

        Public Sub Clear() Implements ICollection(Of RowObject).Clear
            Call _innerTable.Clear()
        End Sub

        Public Function Contains(item As RowObject) As Boolean Implements ICollection(Of RowObject).Contains
            Return _innerTable.Contains(item)
        End Function

        Public Overloads Sub CopyTo(array() As RowObject, arrayIndex As Integer) Implements ICollection(Of RowObject).CopyTo
            Call _innerTable.CopyTo(array, arrayIndex)
        End Sub

        ''' <summary>
        ''' Row Counts
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RowNumbers As Integer Implements ICollection(Of RowObject).Count
            Get
                Return _innerTable.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of RowObject).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(item As RowObject) As Boolean Implements ICollection(Of RowObject).Remove
            Return _innerTable.Remove(item)
        End Function

        Public Function IndexOf(item As RowObject) As Integer Implements IList(Of RowObject).IndexOf
            Return _innerTable.IndexOf(item)
        End Function

        ''' <summary>
        ''' 在所指定的行号下面插入一行数据
        ''' </summary>
        ''' <param name="rowId"></param>
        ''' <param name="Row"></param>
        ''' <remarks></remarks>
        Public Sub Insert(rowId As Integer, Row As RowObject)
            Call _innerTable.Insert(rowId + 1, Row)
        End Sub

        ''' <summary>
        ''' 使用IList接口本身的Insert方法来执行插入
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="item"></param>
        ''' <remarks></remarks>
        Public Sub InsertAt(index As Integer, item As RowObject) Implements IList(Of RowObject).Insert
            Call _innerTable.Insert(index, item)
        End Sub

        ''' <summary>
        ''' Gets a row in the document stream object.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Default Public Overloads Property Item(index As Integer) As RowObject Implements IList(Of RowObject).Item
            Get
                Return _innerTable(index)
            End Get
            Set(value As RowObject)
                _innerTable(index) = value
            End Set
        End Property

        Public Sub RemoveAt(index As Integer) Implements IList(Of RowObject).RemoveAt
            Call _innerTable.RemoveAt(index)
        End Sub

        Protected Overrides Function __getDefaultPath() As String
            Return FilePath
        End Function
#End Region
    End Class
End Namespace
