﻿---
title: TableParser
---

# TableParser
_namespace: [Microsoft.VisualBasic.HtmlParser](N-Microsoft.VisualBasic.HtmlParser.html)_

The string parser for the table html text block



### Methods

#### GetColumnsHTML
```csharp
Microsoft.VisualBasic.HtmlParser.TableParser.GetColumnsHTML(System.String)
```
The td tag is trimmed in this function.(请注意，在本函数之中，<td>标签是被去除掉了的)

|Parameter Name|Remarks|
|--------------|-------|
|row|-|


#### GetRowsHTML
```csharp
Microsoft.VisualBasic.HtmlParser.TableParser.GetRowsHTML(System.String)
```
Parsing the html text betweens the tag <tr></tr> by using regex expression.

|Parameter Name|Remarks|
|--------------|-------|
|table|-|


#### GetTablesHTML
```csharp
Microsoft.VisualBasic.HtmlParser.TableParser.GetTablesHTML(System.String)
```
Parsing the html text betweens the tag <table></table> by using regex expression.

|Parameter Name|Remarks|
|--------------|-------|
|html|-|



