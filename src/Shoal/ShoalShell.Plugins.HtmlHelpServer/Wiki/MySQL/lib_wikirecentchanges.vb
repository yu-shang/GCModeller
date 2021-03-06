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
''' DROP TABLE IF EXISTS `lib_wikirecentchanges`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikirecentchanges` (
'''   `rc_id` int(11) NOT NULL AUTO_INCREMENT,
'''   `rc_timestamp` varbinary(14) NOT NULL DEFAULT '',
'''   `rc_user` int(10) unsigned NOT NULL DEFAULT '0',
'''   `rc_user_text` varbinary(255) NOT NULL,
'''   `rc_namespace` int(11) NOT NULL DEFAULT '0',
'''   `rc_title` varbinary(255) NOT NULL DEFAULT '',
'''   `rc_comment` varbinary(767) NOT NULL DEFAULT '',
'''   `rc_minor` tinyint(3) unsigned NOT NULL DEFAULT '0',
'''   `rc_bot` tinyint(3) unsigned NOT NULL DEFAULT '0',
'''   `rc_new` tinyint(3) unsigned NOT NULL DEFAULT '0',
'''   `rc_cur_id` int(10) unsigned NOT NULL DEFAULT '0',
'''   `rc_this_oldid` int(10) unsigned NOT NULL DEFAULT '0',
'''   `rc_last_oldid` int(10) unsigned NOT NULL DEFAULT '0',
'''   `rc_type` tinyint(3) unsigned NOT NULL DEFAULT '0',
'''   `rc_source` varbinary(16) NOT NULL DEFAULT '',
'''   `rc_patrolled` tinyint(3) unsigned NOT NULL DEFAULT '0',
'''   `rc_ip` varbinary(40) NOT NULL DEFAULT '',
'''   `rc_old_len` int(11) DEFAULT NULL,
'''   `rc_new_len` int(11) DEFAULT NULL,
'''   `rc_deleted` tinyint(3) unsigned NOT NULL DEFAULT '0',
'''   `rc_logid` int(10) unsigned NOT NULL DEFAULT '0',
'''   `rc_log_type` varbinary(255) DEFAULT NULL,
'''   `rc_log_action` varbinary(255) DEFAULT NULL,
'''   `rc_params` blob,
'''   PRIMARY KEY (`rc_id`),
'''   KEY `rc_timestamp` (`rc_timestamp`),
'''   KEY `rc_namespace_title` (`rc_namespace`,`rc_title`),
'''   KEY `rc_cur_id` (`rc_cur_id`),
'''   KEY `new_name_timestamp` (`rc_new`,`rc_namespace`,`rc_timestamp`),
'''   KEY `rc_ip` (`rc_ip`),
'''   KEY `rc_ns_usertext` (`rc_namespace`,`rc_user_text`),
'''   KEY `rc_user_text` (`rc_user_text`,`rc_timestamp`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikirecentchanges", Database:="wiki")>
Public Class lib_wikirecentchanges: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rc_id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property rc_id As Long
    <DatabaseField("rc_timestamp"), NotNull, DataType(MySqlDbType.Blob)> Public Property rc_timestamp As Byte()
    <DatabaseField("rc_user"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property rc_user As Long
    <DatabaseField("rc_user_text"), NotNull, DataType(MySqlDbType.Blob)> Public Property rc_user_text As Byte()
    <DatabaseField("rc_namespace"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property rc_namespace As Long
    <DatabaseField("rc_title"), NotNull, DataType(MySqlDbType.Blob)> Public Property rc_title As Byte()
    <DatabaseField("rc_comment"), NotNull, DataType(MySqlDbType.Blob)> Public Property rc_comment As Byte()
    <DatabaseField("rc_minor"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property rc_minor As Long
    <DatabaseField("rc_bot"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property rc_bot As Long
    <DatabaseField("rc_new"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property rc_new As Long
    <DatabaseField("rc_cur_id"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property rc_cur_id As Long
    <DatabaseField("rc_this_oldid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property rc_this_oldid As Long
    <DatabaseField("rc_last_oldid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property rc_last_oldid As Long
    <DatabaseField("rc_type"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property rc_type As Long
    <DatabaseField("rc_source"), NotNull, DataType(MySqlDbType.Blob)> Public Property rc_source As Byte()
    <DatabaseField("rc_patrolled"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property rc_patrolled As Long
    <DatabaseField("rc_ip"), NotNull, DataType(MySqlDbType.Blob)> Public Property rc_ip As Byte()
    <DatabaseField("rc_old_len"), DataType(MySqlDbType.Int64, "11")> Public Property rc_old_len As Long
    <DatabaseField("rc_new_len"), DataType(MySqlDbType.Int64, "11")> Public Property rc_new_len As Long
    <DatabaseField("rc_deleted"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property rc_deleted As Long
    <DatabaseField("rc_logid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property rc_logid As Long
    <DatabaseField("rc_log_type"), DataType(MySqlDbType.Blob)> Public Property rc_log_type As Byte()
    <DatabaseField("rc_log_action"), DataType(MySqlDbType.Blob)> Public Property rc_log_action As Byte()
    <DatabaseField("rc_params"), DataType(MySqlDbType.Blob)> Public Property rc_params As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikirecentchanges` (`rc_timestamp`, `rc_user`, `rc_user_text`, `rc_namespace`, `rc_title`, `rc_comment`, `rc_minor`, `rc_bot`, `rc_new`, `rc_cur_id`, `rc_this_oldid`, `rc_last_oldid`, `rc_type`, `rc_source`, `rc_patrolled`, `rc_ip`, `rc_old_len`, `rc_new_len`, `rc_deleted`, `rc_logid`, `rc_log_type`, `rc_log_action`, `rc_params`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikirecentchanges` (`rc_timestamp`, `rc_user`, `rc_user_text`, `rc_namespace`, `rc_title`, `rc_comment`, `rc_minor`, `rc_bot`, `rc_new`, `rc_cur_id`, `rc_this_oldid`, `rc_last_oldid`, `rc_type`, `rc_source`, `rc_patrolled`, `rc_ip`, `rc_old_len`, `rc_new_len`, `rc_deleted`, `rc_logid`, `rc_log_type`, `rc_log_action`, `rc_params`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikirecentchanges` WHERE `rc_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikirecentchanges` SET `rc_id`='{0}', `rc_timestamp`='{1}', `rc_user`='{2}', `rc_user_text`='{3}', `rc_namespace`='{4}', `rc_title`='{5}', `rc_comment`='{6}', `rc_minor`='{7}', `rc_bot`='{8}', `rc_new`='{9}', `rc_cur_id`='{10}', `rc_this_oldid`='{11}', `rc_last_oldid`='{12}', `rc_type`='{13}', `rc_source`='{14}', `rc_patrolled`='{15}', `rc_ip`='{16}', `rc_old_len`='{17}', `rc_new_len`='{18}', `rc_deleted`='{19}', `rc_logid`='{20}', `rc_log_type`='{21}', `rc_log_action`='{22}', `rc_params`='{23}' WHERE `rc_id` = '{24}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rc_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rc_timestamp, rc_user, rc_user_text, rc_namespace, rc_title, rc_comment, rc_minor, rc_bot, rc_new, rc_cur_id, rc_this_oldid, rc_last_oldid, rc_type, rc_source, rc_patrolled, rc_ip, rc_old_len, rc_new_len, rc_deleted, rc_logid, rc_log_type, rc_log_action, rc_params)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rc_timestamp, rc_user, rc_user_text, rc_namespace, rc_title, rc_comment, rc_minor, rc_bot, rc_new, rc_cur_id, rc_this_oldid, rc_last_oldid, rc_type, rc_source, rc_patrolled, rc_ip, rc_old_len, rc_new_len, rc_deleted, rc_logid, rc_log_type, rc_log_action, rc_params)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rc_id, rc_timestamp, rc_user, rc_user_text, rc_namespace, rc_title, rc_comment, rc_minor, rc_bot, rc_new, rc_cur_id, rc_this_oldid, rc_last_oldid, rc_type, rc_source, rc_patrolled, rc_ip, rc_old_len, rc_new_len, rc_deleted, rc_logid, rc_log_type, rc_log_action, rc_params, rc_id)
    End Function
#End Region
End Class


End Namespace
