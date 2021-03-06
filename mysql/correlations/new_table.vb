REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL




Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `new_table`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `new_table` (
'''   `idnew_table` int(11) NOT NULL AUTO_INCREMENT,
'''   `new_tablecol` varchar(45) NOT NULL,
'''   PRIMARY KEY (`idnew_table`),
'''   UNIQUE KEY `idnew_table_UNIQUE` (`idnew_table`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("new_table", Database:="correlations")>
Public Class new_table: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("idnew_table"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property idnew_table As Long
    <DatabaseField("new_tablecol"), NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property new_tablecol As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `new_table` (`new_tablecol`) VALUES ('{0}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `new_table` WHERE `idnew_table` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `new_table` SET `idnew_table`='{0}', `new_tablecol`='{1}' WHERE `idnew_table` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, idnew_table)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, new_tablecol)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, idnew_table, new_tablecol, idnew_table)
    End Function
#End Region
End Class


End Namespace
