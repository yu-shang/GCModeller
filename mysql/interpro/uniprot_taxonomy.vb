REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL




Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `uniprot_taxonomy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `uniprot_taxonomy` (
'''   `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `tax_id` bigint(15) NOT NULL,
'''   `left_number` bigint(15) NOT NULL,
'''   `right_number` bigint(15) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("uniprot_taxonomy", Database:="interpro")>
Public Class uniprot_taxonomy: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), NotNull, DataType(MySqlDbType.VarChar, "10")> Public Property protein_ac As String
    <DatabaseField("tax_id"), NotNull, DataType(MySqlDbType.Int64, "15")> Public Property tax_id As Long
    <DatabaseField("left_number"), NotNull, DataType(MySqlDbType.Int64, "15")> Public Property left_number As Long
    <DatabaseField("right_number"), NotNull, DataType(MySqlDbType.Int64, "15")> Public Property right_number As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `uniprot_taxonomy` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `uniprot_taxonomy` SET `protein_ac`='{0}', `tax_id`='{1}', `left_number`='{2}', `right_number`='{3}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was not found, unable to generate ___DELETE_SQL_Invoke automatically, please edit this function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, tax_id, left_number, right_number)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was not found, unable to generate ___UPDATE_SQL_Invoke automatically, please edit this function manually!")
    End Function
#End Region
End Class


End Namespace
