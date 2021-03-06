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
''' DROP TABLE IF EXISTS `lib_wikiinterwiki`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikiinterwiki` (
'''   `iw_prefix` varbinary(32) NOT NULL,
'''   `iw_url` blob NOT NULL,
'''   `iw_api` blob NOT NULL,
'''   `iw_wikiid` varbinary(64) NOT NULL,
'''   `iw_local` tinyint(1) NOT NULL,
'''   `iw_trans` tinyint(4) NOT NULL DEFAULT '0',
'''   UNIQUE KEY `iw_prefix` (`iw_prefix`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikiinterwiki", Database:="wiki")>
Public Class lib_wikiinterwiki: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("iw_prefix"), PrimaryKey, NotNull, DataType(MySqlDbType.Blob)> Public Property iw_prefix As Byte()
    <DatabaseField("iw_url"), NotNull, DataType(MySqlDbType.Blob)> Public Property iw_url As Byte()
    <DatabaseField("iw_api"), NotNull, DataType(MySqlDbType.Blob)> Public Property iw_api As Byte()
    <DatabaseField("iw_wikiid"), NotNull, DataType(MySqlDbType.Blob)> Public Property iw_wikiid As Byte()
    <DatabaseField("iw_local"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property iw_local As Long
    <DatabaseField("iw_trans"), NotNull, DataType(MySqlDbType.Int64, "4")> Public Property iw_trans As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikiinterwiki` (`iw_prefix`, `iw_url`, `iw_api`, `iw_wikiid`, `iw_local`, `iw_trans`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikiinterwiki` (`iw_prefix`, `iw_url`, `iw_api`, `iw_wikiid`, `iw_local`, `iw_trans`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikiinterwiki` WHERE `iw_prefix` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikiinterwiki` SET `iw_prefix`='{0}', `iw_url`='{1}', `iw_api`='{2}', `iw_wikiid`='{3}', `iw_local`='{4}', `iw_trans`='{5}' WHERE `iw_prefix` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, iw_prefix)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, iw_prefix, iw_url, iw_api, iw_wikiid, iw_local, iw_trans)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, iw_prefix, iw_url, iw_api, iw_wikiid, iw_local, iw_trans)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, iw_prefix, iw_url, iw_api, iw_wikiid, iw_local, iw_trans, iw_prefix)
    End Function
#End Region
End Class


End Namespace
