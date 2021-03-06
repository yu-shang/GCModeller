﻿---
title: TextDoc
---

# TextDoc
_namespace: [Microsoft.VisualBasic](N-Microsoft.VisualBasic.html)_





### Methods

#### IsTextFile
```csharp
Microsoft.VisualBasic.TextDoc.IsTextFile(System.String,System.Int32)
```
Determined that the target file is a text file or binary file?
 (判断是否是文本文件)

|Parameter Name|Remarks|
|--------------|-------|
|FilePath|文件全路径名称|
|chunkSize|文件检查的长度，假若在这个长度内都没有超过null的阈值数，则认为该文件为文本文件，默认区域长度为4KB|

_returns: 是返回True，不是返回False_
> 2012年12月5日

#### IterateAllLines
```csharp
Microsoft.VisualBasic.TextDoc.IterateAllLines(System.String)
```
通过具有缓存的流对象读取文本数据，使用迭代器来读取文件之中的所有的行，大文件推荐使用这个方法进行读取操作

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### ReadAllLines
```csharp
Microsoft.VisualBasic.TextDoc.ReadAllLines(System.String,System.Text.Encoding)
```
这个函数只建议读取小文本文件的时候使用

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|Encoding|Default value is UTF8|


#### ReadAllText
```csharp
Microsoft.VisualBasic.TextDoc.ReadAllText(System.String,System.Text.Encoding)
```
这个函数只建议读取小文本文件的时候使用

|Parameter Name|Remarks|
|--------------|-------|
|FilePath|-|
|Encoding|Default value is UTF8|


#### ReadFirstLine
```csharp
Microsoft.VisualBasic.TextDoc.ReadFirstLine(System.String)
```
Read the first line of the text in the file.

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### SaveTo
```csharp
Microsoft.VisualBasic.TextDoc.SaveTo(System.Collections.Generic.IEnumerable{System.String},System.String,System.Text.Encoding)
```
将目标字符串数据全部写入到文件之中，当所写入的文件位置之上没有父文件夹存在的时候，会自动创建文件夹

|Parameter Name|Remarks|
|--------------|-------|
|array|-|
|path|-|
|encoding|-|



