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
''' DROP TABLE IF EXISTS `lib_wikitranscache`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikitranscache` (
'''   `tc_url` varbinary(255) NOT NULL,
'''   `tc_contents` blob,
'''   `tc_time` binary(14) NOT NULL,
'''   UNIQUE KEY `tc_url_idx` (`tc_url`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikitranscache", Database:="wiki")>
Public Class lib_wikitranscache: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("tc_url"), PrimaryKey, NotNull, DataType(MySqlDbType.Blob)> Public Property tc_url As Byte()
    <DatabaseField("tc_contents"), DataType(MySqlDbType.Blob)> Public Property tc_contents As Byte()
    <DatabaseField("tc_time"), NotNull, DataType(MySqlDbType.Blob)> Public Property tc_time As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikitranscache` (`tc_url`, `tc_contents`, `tc_time`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikitranscache` (`tc_url`, `tc_contents`, `tc_time`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikitranscache` WHERE `tc_url` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikitranscache` SET `tc_url`='{0}', `tc_contents`='{1}', `tc_time`='{2}' WHERE `tc_url` = '{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, tc_url)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, tc_url, tc_contents, tc_time)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, tc_url, tc_contents, tc_time)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, tc_url, tc_contents, tc_time, tc_url)
    End Function
#End Region
End Class


End Namespace
