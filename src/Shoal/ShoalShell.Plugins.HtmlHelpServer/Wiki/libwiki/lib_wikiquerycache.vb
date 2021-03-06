REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:20:41 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Wiki.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `lib_wikiquerycache`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikiquerycache` (
'''   `qc_type` varbinary(32) NOT NULL,
'''   `qc_value` int(10) unsigned NOT NULL DEFAULT '0',
'''   `qc_namespace` int(11) NOT NULL DEFAULT '0',
'''   `qc_title` varbinary(255) NOT NULL DEFAULT '',
'''   KEY `qc_type` (`qc_type`,`qc_value`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikiquerycache", Database:="wiki")>
Public Class lib_wikiquerycache: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("qc_type"), PrimaryKey, NotNull, DataType(MySqlDbType.Blob)> Public Property qc_type As Byte()
    <DatabaseField("qc_value"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property qc_value As Long
    <DatabaseField("qc_namespace"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property qc_namespace As Long
    <DatabaseField("qc_title"), NotNull, DataType(MySqlDbType.Blob)> Public Property qc_title As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikiquerycache` (`qc_type`, `qc_value`, `qc_namespace`, `qc_title`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikiquerycache` (`qc_type`, `qc_value`, `qc_namespace`, `qc_title`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikiquerycache` WHERE `qc_type`='{0}' and `qc_value`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikiquerycache` SET `qc_type`='{0}', `qc_value`='{1}', `qc_namespace`='{2}', `qc_title`='{3}' WHERE `qc_type`='{4}' and `qc_value`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, qc_type, qc_value)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, qc_type, qc_value, qc_namespace, qc_title)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, qc_type, qc_value, qc_namespace, qc_title)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, qc_type, qc_value, qc_namespace, qc_title, qc_type, qc_value)
    End Function
#End Region
End Class


End Namespace
