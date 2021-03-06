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
''' DROP TABLE IF EXISTS `lib_wikiredirect`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikiredirect` (
'''   `rd_from` int(10) unsigned NOT NULL DEFAULT '0',
'''   `rd_namespace` int(11) NOT NULL DEFAULT '0',
'''   `rd_title` varbinary(255) NOT NULL DEFAULT '',
'''   `rd_interwiki` varbinary(32) DEFAULT NULL,
'''   `rd_fragment` varbinary(255) DEFAULT NULL,
'''   PRIMARY KEY (`rd_from`),
'''   KEY `rd_ns_title` (`rd_namespace`,`rd_title`,`rd_from`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikiredirect", Database:="wiki")>
Public Class lib_wikiredirect: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rd_from"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property rd_from As Long
    <DatabaseField("rd_namespace"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property rd_namespace As Long
    <DatabaseField("rd_title"), NotNull, DataType(MySqlDbType.Blob)> Public Property rd_title As Byte()
    <DatabaseField("rd_interwiki"), DataType(MySqlDbType.Blob)> Public Property rd_interwiki As Byte()
    <DatabaseField("rd_fragment"), DataType(MySqlDbType.Blob)> Public Property rd_fragment As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikiredirect` (`rd_from`, `rd_namespace`, `rd_title`, `rd_interwiki`, `rd_fragment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikiredirect` (`rd_from`, `rd_namespace`, `rd_title`, `rd_interwiki`, `rd_fragment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikiredirect` WHERE `rd_from` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikiredirect` SET `rd_from`='{0}', `rd_namespace`='{1}', `rd_title`='{2}', `rd_interwiki`='{3}', `rd_fragment`='{4}' WHERE `rd_from` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rd_from)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rd_from, rd_namespace, rd_title, rd_interwiki, rd_fragment)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rd_from, rd_namespace, rd_title, rd_interwiki, rd_fragment)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rd_from, rd_namespace, rd_title, rd_interwiki, rd_fragment, rd_from)
    End Function
#End Region
End Class


End Namespace
