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
''' DROP TABLE IF EXISTS `lib_wikicategory`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikicategory` (
'''   `cat_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `cat_title` varbinary(255) NOT NULL,
'''   `cat_pages` int(11) NOT NULL DEFAULT '0',
'''   `cat_subcats` int(11) NOT NULL DEFAULT '0',
'''   `cat_files` int(11) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`cat_id`),
'''   UNIQUE KEY `cat_title` (`cat_title`),
'''   KEY `cat_pages` (`cat_pages`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikicategory", Database:="wiki")>
Public Class lib_wikicategory: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cat_id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property cat_id As Long
    <DatabaseField("cat_title"), NotNull, DataType(MySqlDbType.Blob)> Public Property cat_title As Byte()
    <DatabaseField("cat_pages"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property cat_pages As Long
    <DatabaseField("cat_subcats"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property cat_subcats As Long
    <DatabaseField("cat_files"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property cat_files As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikicategory` (`cat_title`, `cat_pages`, `cat_subcats`, `cat_files`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikicategory` (`cat_title`, `cat_pages`, `cat_subcats`, `cat_files`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikicategory` WHERE `cat_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikicategory` SET `cat_id`='{0}', `cat_title`='{1}', `cat_pages`='{2}', `cat_subcats`='{3}', `cat_files`='{4}' WHERE `cat_id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cat_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cat_title, cat_pages, cat_subcats, cat_files)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cat_title, cat_pages, cat_subcats, cat_files)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cat_id, cat_title, cat_pages, cat_subcats, cat_files, cat_id)
    End Function
#End Region
End Class


End Namespace
