﻿#Region "Microsoft.VisualBasic::59321bc37e95594eb699bb7e574005f2, ..\GCModeller\data\GO_gene-ontology\MySQL\Tables\term.vb"

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

REM  Dump @12/3/2015 8:49:20 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `term`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `term` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `name` varchar(255) NOT NULL DEFAULT '',
'''   `term_type` varchar(55) NOT NULL,
'''   `acc` varchar(255) NOT NULL,
'''   `is_obsolete` int(11) NOT NULL DEFAULT '0',
'''   `is_root` int(11) NOT NULL DEFAULT '0',
'''   `is_relation` int(11) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `acc` (`acc`),
'''   UNIQUE KEY `t0` (`id`),
'''   KEY `t1` (`name`),
'''   KEY `t2` (`term_type`),
'''   KEY `t3` (`acc`),
'''   KEY `t4` (`id`,`acc`),
'''   KEY `t5` (`id`,`name`),
'''   KEY `t6` (`id`,`term_type`),
'''   KEY `t7` (`id`,`acc`,`name`,`term_type`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=43827 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("term", Database:="go")>
Public Class term: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property name As String
    <DatabaseField("term_type"), NotNull, DataType(MySqlDbType.VarChar, "55")> Public Property term_type As String
    <DatabaseField("acc"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property acc As String
    <DatabaseField("is_obsolete"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property is_obsolete As Long
    <DatabaseField("is_root"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property is_root As Long
    <DatabaseField("is_relation"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property is_relation As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `term` (`name`, `term_type`, `acc`, `is_obsolete`, `is_root`, `is_relation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `term` (`name`, `term_type`, `acc`, `is_obsolete`, `is_root`, `is_relation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `term` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `term` SET `id`='{0}', `name`='{1}', `term_type`='{2}', `acc`='{3}', `is_obsolete`='{4}', `is_root`='{5}', `is_relation`='{6}' WHERE `id` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, term_type, acc, is_obsolete, is_root, is_relation)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, term_type, acc, is_obsolete, is_root, is_relation)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, name, term_type, acc, is_obsolete, is_root, is_relation, id)
    End Function
#End Region
End Class


End Namespace
