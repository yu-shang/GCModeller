REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 7:34:48 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' kegg orthology cross reference to go database
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref_ko2go`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref_ko2go` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `ko` varchar(45) NOT NULL,
'''   `go` varchar(45) NOT NULL,
'''   `url` text,
'''   PRIMARY KEY (`ko`,`go`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COMMENT='kegg orthology cross reference to go database';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref_ko2go", Database:="jp_kegg2")>
Public Class xref_ko2go: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property uid As Long
    <DatabaseField("ko"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property ko As String
    <DatabaseField("go"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property go As String
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `xref_ko2go` (`ko`, `go`, `url`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `xref_ko2go` (`ko`, `go`, `url`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `xref_ko2go` WHERE `ko`='{0}' and `go`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `xref_ko2go` SET `uid`='{0}', `ko`='{1}', `go`='{2}', `url`='{3}' WHERE `ko`='{4}' and `go`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ko, go)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ko, go, url)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ko, go, url)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, ko, go, url, ko, go)
    End Function
#End Region
End Class


End Namespace
