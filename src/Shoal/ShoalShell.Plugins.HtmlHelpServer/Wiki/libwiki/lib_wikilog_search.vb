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
''' DROP TABLE IF EXISTS `lib_wikilog_search`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikilog_search` (
'''   `ls_field` varbinary(32) NOT NULL,
'''   `ls_value` varbinary(255) NOT NULL,
'''   `ls_log_id` int(10) unsigned NOT NULL DEFAULT '0',
'''   UNIQUE KEY `ls_field_val` (`ls_field`,`ls_value`,`ls_log_id`),
'''   KEY `ls_log_id` (`ls_log_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikilog_search", Database:="wiki")>
Public Class lib_wikilog_search: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ls_field"), PrimaryKey, NotNull, DataType(MySqlDbType.Blob)> Public Property ls_field As Byte()
    <DatabaseField("ls_value"), PrimaryKey, NotNull, DataType(MySqlDbType.Blob)> Public Property ls_value As Byte()
    <DatabaseField("ls_log_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ls_log_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikilog_search` (`ls_field`, `ls_value`, `ls_log_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikilog_search` (`ls_field`, `ls_value`, `ls_log_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikilog_search` WHERE `ls_field`='{0}' and `ls_value`='{1}' and `ls_log_id`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikilog_search` SET `ls_field`='{0}', `ls_value`='{1}', `ls_log_id`='{2}' WHERE `ls_field`='{3}' and `ls_value`='{4}' and `ls_log_id`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ls_field, ls_value, ls_log_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ls_field, ls_value, ls_log_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ls_field, ls_value, ls_log_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ls_field, ls_value, ls_log_id, ls_field, ls_value, ls_log_id)
    End Function
#End Region
End Class


End Namespace
