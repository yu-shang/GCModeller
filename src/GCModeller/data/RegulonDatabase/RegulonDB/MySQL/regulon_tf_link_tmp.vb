﻿#Region "Microsoft.VisualBasic::82f7bbbcd84380c20150dfe4f61bd405, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\regulon_tf_link_tmp.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `regulon_tf_link_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulon_tf_link_tmp` (
'''   `regulon_id` char(12) DEFAULT NULL,
'''   `transcription_factor_id` char(12) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulon_tf_link_tmp", Database:="regulondb_7_5")>
Public Class regulon_tf_link_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("regulon_id"), DataType(MySqlDbType.VarChar, "12")> Public Property regulon_id As String
    <DatabaseField("transcription_factor_id"), DataType(MySqlDbType.VarChar, "12")> Public Property transcription_factor_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `regulon_tf_link_tmp` (`regulon_id`, `transcription_factor_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `regulon_tf_link_tmp` (`regulon_id`, `transcription_factor_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `regulon_tf_link_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `regulon_tf_link_tmp` SET `regulon_id`='{0}', `transcription_factor_id`='{1}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, regulon_id, transcription_factor_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, regulon_id, transcription_factor_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

