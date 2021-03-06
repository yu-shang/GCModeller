REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL




Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `text_index_entry`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `text_index_entry` (
'''   `id` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `field` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `text` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("text_index_entry", Database:="interpro")>
Public Class text_index_entry: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), DataType(MySqlDbType.VarChar, "9")> Public Property id As String
    <DatabaseField("field"), DataType(MySqlDbType.VarChar, "20")> Public Property field As String
    <DatabaseField("text"), DataType(MySqlDbType.VarChar, "100")> Public Property text As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `text_index_entry` (`id`, `field`, `text`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `text_index_entry` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `text_index_entry` SET `id`='{0}', `field`='{1}', `text`='{2}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was not found, unable to generate ___DELETE_SQL_Invoke automatically, please edit this function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, field, text)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was not found, unable to generate ___UPDATE_SQL_Invoke automatically, please edit this function manually!")
    End Function
#End Region
End Class


End Namespace
