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
''' DROP TABLE IF EXISTS `lib_wikiuser`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikiuser` (
'''   `user_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `user_name` varbinary(255) NOT NULL DEFAULT '',
'''   `user_real_name` varbinary(255) NOT NULL DEFAULT '',
'''   `user_password` tinyblob NOT NULL,
'''   `user_newpassword` tinyblob NOT NULL,
'''   `user_newpass_time` binary(14) DEFAULT NULL,
'''   `user_email` tinyblob NOT NULL,
'''   `user_touched` binary(14) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
'''   `user_token` binary(32) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
'''   `user_email_authenticated` binary(14) DEFAULT NULL,
'''   `user_email_token` binary(32) DEFAULT NULL,
'''   `user_email_token_expires` binary(14) DEFAULT NULL,
'''   `user_registration` binary(14) DEFAULT NULL,
'''   `user_editcount` int(11) DEFAULT NULL,
'''   `user_password_expires` varbinary(14) DEFAULT NULL,
'''   PRIMARY KEY (`user_id`),
'''   UNIQUE KEY `user_name` (`user_name`),
'''   KEY `user_email_token` (`user_email_token`),
'''   KEY `user_email` (`user_email`(50))
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikiuser", Database:="wiki")>
Public Class lib_wikiuser: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("user_id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property user_id As Long
    <DatabaseField("user_name"), NotNull, DataType(MySqlDbType.Blob)> Public Property user_name As Byte()
    <DatabaseField("user_real_name"), NotNull, DataType(MySqlDbType.Blob)> Public Property user_real_name As Byte()
    <DatabaseField("user_password"), NotNull, DataType(MySqlDbType.Blob)> Public Property user_password As Byte()
    <DatabaseField("user_newpassword"), NotNull, DataType(MySqlDbType.Blob)> Public Property user_newpassword As Byte()
    <DatabaseField("user_newpass_time"), DataType(MySqlDbType.Blob)> Public Property user_newpass_time As Byte()
    <DatabaseField("user_email"), NotNull, DataType(MySqlDbType.Blob)> Public Property user_email As Byte()
    <DatabaseField("user_touched"), NotNull, DataType(MySqlDbType.Blob)> Public Property user_touched As Byte()
    <DatabaseField("user_token"), NotNull, DataType(MySqlDbType.Blob)> Public Property user_token As Byte()
    <DatabaseField("user_email_authenticated"), DataType(MySqlDbType.Blob)> Public Property user_email_authenticated As Byte()
    <DatabaseField("user_email_token"), DataType(MySqlDbType.Blob)> Public Property user_email_token As Byte()
    <DatabaseField("user_email_token_expires"), DataType(MySqlDbType.Blob)> Public Property user_email_token_expires As Byte()
    <DatabaseField("user_registration"), DataType(MySqlDbType.Blob)> Public Property user_registration As Byte()
    <DatabaseField("user_editcount"), DataType(MySqlDbType.Int64, "11")> Public Property user_editcount As Long
    <DatabaseField("user_password_expires"), DataType(MySqlDbType.Blob)> Public Property user_password_expires As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikiuser` (`user_name`, `user_real_name`, `user_password`, `user_newpassword`, `user_newpass_time`, `user_email`, `user_touched`, `user_token`, `user_email_authenticated`, `user_email_token`, `user_email_token_expires`, `user_registration`, `user_editcount`, `user_password_expires`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikiuser` (`user_name`, `user_real_name`, `user_password`, `user_newpassword`, `user_newpass_time`, `user_email`, `user_touched`, `user_token`, `user_email_authenticated`, `user_email_token`, `user_email_token_expires`, `user_registration`, `user_editcount`, `user_password_expires`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikiuser` WHERE `user_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikiuser` SET `user_id`='{0}', `user_name`='{1}', `user_real_name`='{2}', `user_password`='{3}', `user_newpassword`='{4}', `user_newpass_time`='{5}', `user_email`='{6}', `user_touched`='{7}', `user_token`='{8}', `user_email_authenticated`='{9}', `user_email_token`='{10}', `user_email_token_expires`='{11}', `user_registration`='{12}', `user_editcount`='{13}', `user_password_expires`='{14}' WHERE `user_id` = '{15}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, user_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, user_name, user_real_name, user_password, user_newpassword, user_newpass_time, user_email, user_touched, user_token, user_email_authenticated, user_email_token, user_email_token_expires, user_registration, user_editcount, user_password_expires)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, user_name, user_real_name, user_password, user_newpassword, user_newpass_time, user_email, user_touched, user_token, user_email_authenticated, user_email_token, user_email_token_expires, user_registration, user_editcount, user_password_expires)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, user_id, user_name, user_real_name, user_password, user_newpassword, user_newpass_time, user_email, user_touched, user_token, user_email_authenticated, user_email_token, user_email_token_expires, user_registration, user_editcount, user_password_expires, user_id)
    End Function
#End Region
End Class


End Namespace
