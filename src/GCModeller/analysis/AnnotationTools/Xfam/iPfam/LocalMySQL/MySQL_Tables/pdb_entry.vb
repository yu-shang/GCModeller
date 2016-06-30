REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace iPfam.LocalMySQL

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_entry")>
Public Class pdb_entry: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pdb_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("header"), DataType(MySqlDbType.Text)> Public Property header As String
    <DatabaseField("title"), DataType(MySqlDbType.Text)> Public Property title As String
    <DatabaseField("date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property [date] As Date
    <DatabaseField("resolution"), NotNull, DataType(MySqlDbType.Decimal)> Public Property resolution As Decimal
    <DatabaseField("expt_method"), NotNull, DataType(MySqlDbType.Text)> Public Property expt_method As String
    <DatabaseField("author"), DataType(MySqlDbType.Text)> Public Property author As String
    <DatabaseField("pdb_file"), DataType(MySqlDbType.Int64, "10")> Public Property pdb_file As Long
    <DatabaseField("sifts_file"), DataType(MySqlDbType.Int64, "10")> Public Property sifts_file As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_entry` (`pdb_id`, `header`, `title`, `date`, `resolution`, `expt_method`, `author`, `pdb_file`, `sifts_file`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_entry` (`pdb_id`, `header`, `title`, `date`, `resolution`, `expt_method`, `author`, `pdb_file`, `sifts_file`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_entry` WHERE `pdb_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_entry` SET `pdb_id`='{0}', `header`='{1}', `title`='{2}', `date`='{3}', `resolution`='{4}', `expt_method`='{5}', `author`='{6}', `pdb_file`='{7}', `sifts_file`='{8}' WHERE `pdb_id` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pdb_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdb_id, header, title, DataType.ToMySqlDateTimeString([date]), resolution, expt_method, author, pdb_file, sifts_file)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdb_id, header, title, DataType.ToMySqlDateTimeString([date]), resolution, expt_method, author, pdb_file, sifts_file)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pdb_id, header, title, DataType.ToMySqlDateTimeString([date]), resolution, expt_method, author, pdb_file, sifts_file, pdb_id)
    End Function
#End Region
End Class


End Namespace
