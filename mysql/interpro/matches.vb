REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL




Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `matches`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `matches` (
'''   `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `pos_from` int(5) DEFAULT NULL,
'''   `pos_to` int(5) DEFAULT NULL,
'''   `status` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `match_date` datetime NOT NULL,
'''   `seq_date` datetime NOT NULL,
'''   `score` double DEFAULT NULL,
'''   KEY `fk_matches$evidence` (`evidence`),
'''   KEY `fk_matches$method` (`method_ac`),
'''   CONSTRAINT `fk_matches$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_matches$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("matches", Database:="interpro")>
Public Class matches: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property protein_ac As String
    <DatabaseField("method_ac"), NotNull, DataType(MySqlDbType.VarChar, "25")> Public Property method_ac As String
    <DatabaseField("pos_from"), DataType(MySqlDbType.Int64, "5")> Public Property pos_from As Long
    <DatabaseField("pos_to"), DataType(MySqlDbType.Int64, "5")> Public Property pos_to As Long
    <DatabaseField("status"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property status As String
    <DatabaseField("evidence"), PrimaryKey, DataType(MySqlDbType.VarChar, "3")> Public Property evidence As String
    <DatabaseField("match_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property match_date As Date
    <DatabaseField("seq_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property seq_date As Date
    <DatabaseField("score"), DataType(MySqlDbType.Double)> Public Property score As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `matches` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `evidence`, `match_date`, `seq_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `matches` WHERE `evidence` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `matches` SET `protein_ac`='{0}', `method_ac`='{1}', `pos_from`='{2}', `pos_to`='{3}', `status`='{4}', `evidence`='{5}', `match_date`='{6}', `seq_date`='{7}', `score`='{8}' WHERE `evidence` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, evidence)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, method_ac, pos_from, pos_to, status, evidence, DataType.ToMySqlDateTimeString(match_date), DataType.ToMySqlDateTimeString(seq_date), score)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, method_ac, pos_from, pos_to, status, evidence, DataType.ToMySqlDateTimeString(match_date), DataType.ToMySqlDateTimeString(seq_date), score, evidence)
    End Function
#End Region
End Class


End Namespace
