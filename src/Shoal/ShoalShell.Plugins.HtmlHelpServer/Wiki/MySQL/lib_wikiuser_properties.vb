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
''' DROP TABLE IF EXISTS `lib_wikiuser_properties`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lib_wikiuser_properties` (
'''   `up_user` int(11) NOT NULL,
'''   `up_property` varbinary(255) NOT NULL,
'''   `up_value` blob,
'''   UNIQUE KEY `user_properties_user_property` (`up_user`,`up_property`),
'''   KEY `user_properties_property` (`up_property`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=binary;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lib_wikiuser_properties", Database:="wiki")>
Public Class lib_wikiuser_properties: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("up_user"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property up_user As Long
    <DatabaseField("up_property"), PrimaryKey, NotNull, DataType(MySqlDbType.Blob)> Public Property up_property As Byte()
    <DatabaseField("up_value"), DataType(MySqlDbType.Blob)> Public Property up_value As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lib_wikiuser_properties` (`up_user`, `up_property`, `up_value`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lib_wikiuser_properties` (`up_user`, `up_property`, `up_value`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lib_wikiuser_properties` WHERE `up_user`='{0}' and `up_property`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lib_wikiuser_properties` SET `up_user`='{0}', `up_property`='{1}', `up_value`='{2}' WHERE `up_user`='{3}' and `up_property`='{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, up_user, up_property)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, up_user, up_property, up_value)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, up_user, up_property, up_value)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, up_user, up_property, up_value, up_user, up_property)
    End Function
#End Region
End Class


End Namespace
