REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL




Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `struct_class`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `struct_class` (
'''   `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `fam_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`domain_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("struct_class", Database:="interpro")>
Public Class struct_class: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("domain_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "14")> Public Property domain_id As String
    <DatabaseField("fam_id"), DataType(MySqlDbType.VarChar, "20")> Public Property fam_id As String
    <DatabaseField("dbcode"), DataType(MySqlDbType.VarChar, "1")> Public Property dbcode As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `struct_class` (`domain_id`, `fam_id`, `dbcode`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `struct_class` WHERE `domain_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `struct_class` SET `domain_id`='{0}', `fam_id`='{1}', `dbcode`='{2}' WHERE `domain_id` = '{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, domain_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, domain_id, fam_id, dbcode)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, domain_id, fam_id, dbcode, domain_id)
    End Function
#End Region
End Class


End Namespace
