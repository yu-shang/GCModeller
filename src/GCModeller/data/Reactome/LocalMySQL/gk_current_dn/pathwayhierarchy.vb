﻿#Region "Microsoft.VisualBasic::382cf7ce9daa854d99354e409a176ddb, ..\GCModeller\data\Reactome\LocalMySQL\gk_current_dn\pathwayhierarchy.vb"

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

REM  Dump @12/3/2015 8:16:51 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current_dn

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `pathwayhierarchy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pathwayhierarchy` (
'''   `pathwayId` int(32) NOT NULL DEFAULT '0',
'''   `childPathwayId` int(32) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`pathwayId`,`childPathwayId`),
'''   CONSTRAINT `PathwayHierarchy_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pathwayhierarchy", Database:="gk_current_dn")>
Public Class pathwayhierarchy: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pathwayId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32")> Public Property pathwayId As Long
    <DatabaseField("childPathwayId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32")> Public Property childPathwayId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pathwayhierarchy` WHERE `pathwayId`='{0}' and `childPathwayId`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pathwayhierarchy` SET `pathwayId`='{0}', `childPathwayId`='{1}' WHERE `pathwayId`='{2}' and `childPathwayId`='{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pathwayId, childPathwayId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pathwayId, childPathwayId)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pathwayId, childPathwayId)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pathwayId, childPathwayId, pathwayId, childPathwayId)
    End Function
#End Region
End Class


End Namespace

