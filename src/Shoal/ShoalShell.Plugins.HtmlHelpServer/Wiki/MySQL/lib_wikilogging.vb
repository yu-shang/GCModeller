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
''' DROP TABLE IF EXISTS `lib_wikilogging`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikilogging` (
'''   `log_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `log_type` varbinary(32) NOT NULL DEFAULT '',
'''   `log_action` varbinary(32) NOT NULL DEFAULT '',
'''   `log_timestamp` binary(14) NOT NULL DEFAULT '19700101000000',
'''   `log_user` int(10) unsigned NOT NULL DEFAULT '0',
'''   `log_user_text` varbinary(255) NOT NULL DEFAULT '',
'''   `log_namespace` int(11) NOT NULL DEFAULT '0',
'''   `log_title` varbinary(255) NOT NULL DEFAULT '',
'''   `log_page` int(10) unsigned DEFAULT NULL,
'''   `log_comment` varbinary(767) NOT NULL DEFAULT '',
'''   `log_params` blob NOT NULL,
'''   `log_deleted` tinyint(3) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`log_id`),
'''   KEY `type_time` (`log_type`,`log_timestamp`),
'''   KEY `user_time` (`log_user`,`log_timestamp`),
'''   KEY `page_time` (`log_namespace`,`log_title`,`log_timestamp`),
'''   KEY `times` (`log_timestamp`),
'''   KEY `log_user_type_time` (`log_user`,`log_type`,`log_timestamp`),
'''   KEY `log_page_id_time` (`log_page`,`log_timestamp`),
'''   KEY `type_action` (`log_type`,`log_action`,`log_timestamp`),
'''   KEY `log_user_text_type_time` (`log_user_text`,`log_type`,`log_timestamp`),
'''   KEY `log_user_text_time` (`log_user_text`,`log_timestamp`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikilogging", Database:="wiki")>
Public Class lib_wikilogging: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("log_id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property log_id As Long
    <DatabaseField("log_type"), NotNull, DataType(MySqlDbType.Blob)> Public Property log_type As Byte()
    <DatabaseField("log_action"), NotNull, DataType(MySqlDbType.Blob)> Public Property log_action As Byte()
    <DatabaseField("log_timestamp"), NotNull, DataType(MySqlDbType.Blob)> Public Property log_timestamp As Byte()
    <DatabaseField("log_user"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property log_user As Long
    <DatabaseField("log_user_text"), NotNull, DataType(MySqlDbType.Blob)> Public Property log_user_text As Byte()
    <DatabaseField("log_namespace"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property log_namespace As Long
    <DatabaseField("log_title"), NotNull, DataType(MySqlDbType.Blob)> Public Property log_title As Byte()
    <DatabaseField("log_page"), DataType(MySqlDbType.Int64, "10")> Public Property log_page As Long
    <DatabaseField("log_comment"), NotNull, DataType(MySqlDbType.Blob)> Public Property log_comment As Byte()
    <DatabaseField("log_params"), NotNull, DataType(MySqlDbType.Blob)> Public Property log_params As Byte()
    <DatabaseField("log_deleted"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property log_deleted As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikilogging` (`log_type`, `log_action`, `log_timestamp`, `log_user`, `log_user_text`, `log_namespace`, `log_title`, `log_page`, `log_comment`, `log_params`, `log_deleted`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikilogging` (`log_type`, `log_action`, `log_timestamp`, `log_user`, `log_user_text`, `log_namespace`, `log_title`, `log_page`, `log_comment`, `log_params`, `log_deleted`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikilogging` WHERE `log_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikilogging` SET `log_id`='{0}', `log_type`='{1}', `log_action`='{2}', `log_timestamp`='{3}', `log_user`='{4}', `log_user_text`='{5}', `log_namespace`='{6}', `log_title`='{7}', `log_page`='{8}', `log_comment`='{9}', `log_params`='{10}', `log_deleted`='{11}' WHERE `log_id` = '{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, log_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, log_type, log_action, log_timestamp, log_user, log_user_text, log_namespace, log_title, log_page, log_comment, log_params, log_deleted)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, log_type, log_action, log_timestamp, log_user, log_user_text, log_namespace, log_title, log_page, log_comment, log_params, log_deleted)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, log_id, log_type, log_action, log_timestamp, log_user, log_user_text, log_namespace, log_title, log_page, log_comment, log_params, log_deleted, log_id)
    End Function
#End Region
End Class


End Namespace
