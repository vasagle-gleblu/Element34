using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Element34.DataManager
{
    public static class DataWriteLayer
    {
        #region [Fields]
        static readonly Encoding enc = Encoding.Default;
        static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        const int MAXNVARCHAR = 4000;
        const int MAXVARCHAR = 8000;
        #endregion

        #region [Public Functions]
        #region [MS-SQL Server Support]
        public static void AddDataColumnMsSql(SqlConnection sqlConn, DataTable dt, string fieldName, Type dataType, int length = -1)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                DataColumn column = new DataColumn(fieldName, dataType);

                if (length < 0)
                {
                    switch (dataType.Name)
                    {
                        case "String":
                            length = 255;
                            break;
                        case "Byte[]":
                            length = 100;
                            break;
                        case "DateTime":
                            length = 8;
                            break;
                        case "DateTimeOffset":
                            length = 10;
                            break;
                        case "TimeSpan":
                            length = 8;
                            break;
                        case "Guid":
                            length = 16;
                            break;
                        default:
                            // Handle other data types if needed
                            break;
                    }
                }

                if (length > 0)
                    column.MaxLength = length;

                dt.Columns.Add(column);

                try
                {
                    sqlConn.Open();

                    // Check if the column already exists
                    string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{dt.TableName}' AND COLUMN_NAME = '{fieldName}'";
                    using (SqlCommand command = new SqlCommand(query, sqlConn))
                    {
                        int columnCount = (int)command.ExecuteScalar();
                        if (columnCount > 0)
                        {
                            Debug.WriteLine("Column already exists.");
                            return;
                        }
                    }

                    SqlDbType columnType = ResolveSqlDbType(column);

                    // If the column doesn't exist, add it
                    string columnDefinition = length > 0 ? $"[{fieldName}] {columnType}({length})" : $"[{fieldName}] {columnType}";
                    query = $"ALTER TABLE [{dt.TableName}] ADD {columnDefinition}";

                    using (SqlCommand command = new SqlCommand(query, sqlConn))
                    {
                        command.ExecuteNonQuery();
                        Debug.WriteLine($"Column '{fieldName}' added successfully to table '{dt.TableName}'.");
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error adding column: {ex.Message}");
                }
            }
        }

        public static void UpdateMsSql_MapEvents(SqlConnection sqlConn, DataTable dt)
        {
            dt.RowChanged += (sender, e) => UpdateMsSql_HandleRowUpsert(sender, e, sqlConn);
            dt.RowDeleted += (sender, e) => UpdateMsSql_HandleRowDeleted(sender, e, sqlConn);
        }

        private static void UpdateMsSql_HandleRowUpsert(object sender, DataRowChangeEventArgs e, SqlConnection sqlConn)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;
            string selectStmt = $"SELECT * FROM [{tableName}]";
            bool hasPrimaryKey = HasPrimaryKey(dt, sqlConn);
            List<string> pkNames = (hasPrimaryKey) ? FindPrimaryKeys(dt, sqlConn) : null;

            if (e.Row.RowState == DataRowState.Added)
            {
                // Open the SQL connection
                sqlConn.Open();
                SqlTransaction transaction = sqlConn.BeginTransaction();

                try
                {
                    StringBuilder insertCommandText = new StringBuilder();
                    if (hasPrimaryKey)
                    {
                        insertCommandText.Append($"INSERT INTO [{tableName}] (");

                        // Append column names
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string columnName = dt.Columns[i].ColumnName;

                            if (!pkNames.Contains(columnName))
                            {
                                insertCommandText.Append(columnName);
                                if (i < dt.Columns.Count - 1)
                                {
                                    insertCommandText.Append(", ");
                                }
                            }
                        }

                        insertCommandText.Append(") VALUES (");

                        // Append parameter placeholders
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string columnName = dt.Columns[i].ColumnName;

                            if (!pkNames.Contains(columnName))
                            {
                                insertCommandText.Append($"@{columnName}");
                                if (i < dt.Columns.Count - 1)
                                {
                                    insertCommandText.Append(", ");
                                }
                            }
                        }
                        insertCommandText.Append(")");

                        // Add parameters
                        foreach (DataColumn column in dt.Columns)
                        {
                            insertCommandText.Replace($"@{column.ColumnName}", Normalize(e.Row[column], false, true));
                        }
                    }
                    else
                    {
                        insertCommandText.Append($"INSERT INTO [{tableName}] (");

                        // Append column names
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string columnName = dt.Columns[i].ColumnName;
                            insertCommandText.Append(columnName);
                            if (i < dt.Columns.Count - 1)
                            {
                                insertCommandText.Append(", ");
                            }
                        }

                        insertCommandText.Append(") VALUES (");

                        // Append parameter placeholders
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string columnName = dt.Columns[i].ColumnName;
                            insertCommandText.Append($"@{columnName}");
                            if (i < dt.Columns.Count - 1)
                            {
                                insertCommandText.Append(", ");
                            }
                        }
                        insertCommandText.Append(")");

                        // Add parameters
                        foreach (DataColumn column in dt.Columns)
                        {
                            insertCommandText.Replace($"@{column.ColumnName}", Normalize(e.Row[column], false, true));
                        }
                    }

                    SqlCommand command = new SqlCommand(insertCommandText.ToString(), sqlConn, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error inserting record into SQL Server table: {ex.Message}");
                }
                finally
                {
                    // Close the SQL connection in the finally block to ensure it is always closed
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                sqlConn.Open();
                SqlTransaction transaction = sqlConn.BeginTransaction();

                try
                {
                    StringBuilder updateCommandText = new StringBuilder();
                    if (hasPrimaryKey)
                    {
                        // Build custom update command
                        updateCommandText.Append($"UPDATE [{tableName}] SET ");
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (e.Row[column, DataRowVersion.Current] != e.Row[column, DataRowVersion.Original])
                                if (!pkNames.Contains(column.ColumnName))
                                    updateCommandText.Append($"[{column.ColumnName}] = @{column.ColumnName}, ");
                        }
                        updateCommandText.Remove(updateCommandText.Length - 2, 2); // Remove last comma

                        updateCommandText.Append($" WHERE ");
                        foreach (string pk in pkNames)
                        {
                            updateCommandText.Append($"[{pk}]={Normalize(e.Row[pk])}, ");
                        }
                        updateCommandText.Remove(updateCommandText.Length - 2, 2); // Remove last comma

                        // Add parameters
                        foreach (DataColumn column in dt.Columns)
                        {
                            updateCommandText.Replace($"@{column.ColumnName}", Normalize(e.Row[column, DataRowVersion.Current], false, true));
                        }
                    }
                    else
                    {
                        // Build custom update command
                        updateCommandText.Append($"UPDATE [{tableName}] SET ");
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (e.Row[column, DataRowVersion.Current] != e.Row[column, DataRowVersion.Original])
                                updateCommandText.Append($"[{column.ColumnName}] = @{column.ColumnName}, ");
                        }
                        updateCommandText.Remove(updateCommandText.Length - 2, 2); // Remove last comma
                        updateCommandText.Append($" WHERE ");
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (e.Row[column, DataRowVersion.Current] == e.Row[column, DataRowVersion.Original])
                                updateCommandText.Append($"[{column.ColumnName}] = @{column.ColumnName}_old AND ");
                        }
                        updateCommandText.Remove(updateCommandText.Length - 5, 5); // Remove last 'AND'

                        // Add parameters
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (e.Row[column, DataRowVersion.Current] == e.Row[column, DataRowVersion.Original])
                            {
                                updateCommandText.Replace($"@{column.ColumnName}_old", Normalize(e.Row[column, DataRowVersion.Original], false, true));
                                updateCommandText.Replace($"[{column.ColumnName}] = NULL", $"[{column.ColumnName}] IS NULL");
                            }
                            else
                                updateCommandText.Replace($"@{column.ColumnName}", Normalize(e.Row[column, DataRowVersion.Current], false, true));
                        }
                    }

                    SqlCommand command = new SqlCommand(updateCommandText.ToString(), sqlConn, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error updating record into SQL Server table: {ex.Message}");
                }
                finally
                {
                    // Close the SQL connection in the finally block
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        private static void UpdateMsSql_HandleRowDeleted(object sender, DataRowChangeEventArgs e, SqlConnection sqlConn)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;

            if (e.Action == DataRowAction.Delete)
            {
                sqlConn.Open();
                SqlTransaction transaction = sqlConn.BeginTransaction();

                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {tableName} WHERE 1=0", sqlConn)) // Fetch schema only
                    {
                        adapter.SelectCommand.Transaction = transaction;
                        adapter.DeleteCommand = new SqlCommandBuilder(adapter).GetDeleteCommand();

                        // Update the SQL Server table with changes from the DataTable
                        adapter.Update(dt);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error deleting row from SQL Server table: {ex.Message}");
                }
                finally
                {
                    // Close the SQL connection in the finally block to ensure it is always closed
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        private static bool HasPrimaryKey(DataTable dataTable, SqlConnection sqlConn)
        {
            bool result = false;
            string query = $"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYS.INDEXES AS i WHERE i.is_primary_key = 1 AND i.OBJECT_ID = OBJECT_ID('{dataTable.TableName}')) THEN 'true' ELSE 'false' END AS HasPrimaryKey; ";

            // Open the SQL connection
            sqlConn.Open();

            try
            {
                using (SqlCommand command = new SqlCommand(query, sqlConn))
                {
                    result = (bool)ToBooleanOrDefault(command.ExecuteScalar(), false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying SQL Server table: {ex.Message}");
            }
            finally
            {
                // Close the SQL connection in the finally block
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }

            return result;
        }

        private static List<string> FindPrimaryKeys(DataTable dataTable, SqlConnection sqlConn)
        {
            List<string> result = new List<string>();
            string query = $"SELECT c.name AS column_name FROM sys.indexes AS i INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id INNER JOIN sys.columns AS c ON ic.object_id = c.object_id AND ic.column_id = c.column_id WHERE i.object_id = OBJECT_ID('{dataTable.TableName}') AND i.is_primary_key = 1";

            // Open the SQL connection
            sqlConn.Open();

            try
            {
                using (SqlCommand command = new SqlCommand(query, sqlConn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the column name and add it to the list
                        result.Add(reader["column_name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying SQL Server table: {ex.Message}");
            }
            finally
            {
                // Close the SQL connection in the finally block
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Inspects a DataTable and return a SQL string that can be used to CREATE a TABLE in MS-SQL Server.
        /// </summary>
        /// <param name="inTable">System.Data.DataTable object to be inspected for building the SQL CREATE TABLE statement.</param>
        /// <returns>String of SQL</returns>
        private static string DefineSQLTable(DataTable inTable)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder alterSql = new StringBuilder();

            sql.AppendFormat("CREATE TABLE [{0}] (", inTable.TableName);

            for (int i = 0; i < inTable.Columns.Count; i++)
            {
                bool isNumeric = false;
                bool usesColumnDefault = true;

                sql.AppendFormat("\n\t[{0}]", inTable.Columns[i].ColumnName);
                Type columnType = inTable.Columns[i].DataType;

                switch (columnType.ToString())
                {
                    case nameof(System.Byte):
                        sql.Append(" TINYINT");
                        isNumeric = true;
                        break;

                    case nameof(System.Int16):
                        sql.Append(" SMALLINT");
                        isNumeric = true;
                        break;

                    case nameof(System.Int32):
                        sql.Append(" INT");
                        isNumeric = true;
                        break;

                    case nameof(System.Int64):
                        sql.Append(" BIGINT");
                        isNumeric = true;
                        break;

                    case nameof(System.DateTime):
                        sql.Append(" DATETIME");
                        usesColumnDefault = false;
                        break;

                    case nameof(System.String):
                        sql.AppendFormat(" NVARCHAR({0}) ", inTable.Columns[i].MaxLength == -1 ? "max" : inTable.Columns[i].MaxLength.ToString());
                        break;

                    case nameof(System.Single):
                        sql.Append(" SINGLE");
                        isNumeric = true;
                        break;

                    case nameof(System.Double):
                        sql.Append(" DOUBLE");
                        isNumeric = true;
                        break;

                    case nameof(System.Decimal):
                        sql.AppendFormat(" DECIMAL(18, 6)");
                        isNumeric = true;
                        break;

                    default:
                        throw new NotSupportedException("Data type not supported: " + columnType.FullName);
                }

                if (inTable.Columns[i].AutoIncrement)
                {
                    sql.AppendFormat(" IDENTITY({0},{1})",
                        inTable.Columns[i].AutoIncrementSeed,
                        inTable.Columns[i].AutoIncrementStep);
                }
                else
                {
                    // DataColumns will add a blank DefaultValue for any AutoIncrement column. 
                    // We only want to create an ALTER statement for those columns that are not set to AutoIncrement. 
                    if (inTable.Columns[i].DefaultValue != null)
                    {
                        if (usesColumnDefault)
                        {
                            if (isNumeric)
                            {
                                alterSql.AppendFormat("\r\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                                    inTable.TableName,
                                    inTable.Columns[i].ColumnName,
                                    inTable.Columns[i].DefaultValue);
                            }
                            else
                            {
                                alterSql.AppendFormat("\r\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ('{2}') FOR [{1}];",
                                    inTable.TableName,
                                    inTable.Columns[i].ColumnName,
                                    inTable.Columns[i].DefaultValue);
                            }
                        }
                        else
                        {
                            // Default values on Date columns, e.g., "DateTime.Now" will not translate to SQL.
                            // This inspects the caption for a simple XML string to see if there is a SQL compliant default value, e.g., "GETDATE()".
                            try
                            {
                                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                                xml.LoadXml(inTable.Columns[i].Caption);

                                alterSql.AppendFormat("\r\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                                    inTable.TableName,
                                    inTable.Columns[i].ColumnName,
                                    xml.GetElementsByTagName("defaultValue")[0].InnerText);
                            }
                            catch
                            {
                                // Handle
                            }
                        }
                    }
                }

                if (!inTable.Columns[i].AllowDBNull)
                {
                    sql.Append(" NOT NULL");
                }

                sql.Append(",");
            }

            if (inTable.PrimaryKey.Length > 0)
            {
                StringBuilder primaryKeySql = new StringBuilder();

                primaryKeySql.AppendFormat("\n\tCONSTRAINT PK_{0} PRIMARY KEY (", inTable.TableName);

                for (int i = 0; i < inTable.PrimaryKey.Length; i++)
                {
                    primaryKeySql.AppendFormat("{0},", inTable.PrimaryKey[i].ColumnName);
                }

                primaryKeySql.Remove(primaryKeySql.Length - 1, 1);
                primaryKeySql.Append(")");

                sql.Append(primaryKeySql);
            }
            else
            {
                sql.Remove(sql.Length - 1, 1);
            }

            sql.AppendFormat("\r\n);\r\n{0}", alterSql.ToString());

            return sql.ToString();
        }

        private static void DefineSqlServerTable(DataTable inTable)
        {
            // Connection string to your SQL Server database
            string connectionString = "Data Source=YourServer;Initial Catalog=YourDatabase;Integrated Security=True";

            // Create SQL Server table based on DataTable schema
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Construct SQL command to create the table
                string createTableCommand = $"CREATE TABLE {inTable.TableName} (";
                foreach (DataColumn column in inTable.Columns)
                {
                    createTableCommand += $"{column.ColumnName} {ResolveSqlDbType(column)}, ";
                }
                createTableCommand = createTableCommand.TrimEnd(',', ' ') + ")";

                // Execute the CreateTable command
                using (SqlCommand command = new SqlCommand(createTableCommand, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            Debug.WriteLine($"Table '{inTable.TableName}' created in SQL Server.");
        }

        private static SqlDbType ResolveSqlDbType(DataColumn column)
        {
            if (column == null)
            {
                return SqlDbType.Variant;
            }

            int length = column.MaxLength;
            SqlDbType sqlDbType = ToSqlDbType(column.GetType());

            // Adjust SqlDbType for string types based on length
            if (sqlDbType == SqlDbType.NVarChar || sqlDbType == SqlDbType.VarChar || sqlDbType == SqlDbType.Char)
            {
                // For string types, check if length is specified and within valid range
                if (length > 0)
                {
                    if (sqlDbType == SqlDbType.NVarChar)
                    {
                        // Limit length to max value supported by NVARCHAR (4000)
                        sqlDbType = length <= MAXNVARCHAR ? SqlDbType.NVarChar : SqlDbType.NText;
                    }
                    else
                    {
                        // Limit length to max value supported by CHAR (8000)
                        sqlDbType = length <= MAXVARCHAR ? SqlDbType.Char : SqlDbType.Text;
                    }
                }
            }

            return sqlDbType;
        }

        private static SqlDbType ToSqlDbType(Type clrType)
        {
            TypeCode typeCode = Type.GetTypeCode(clrType);
            SqlDbType sqlDbType = SqlDbType.Variant;

            switch (typeCode)
            {
                case TypeCode.String:
                    sqlDbType = SqlDbType.NVarChar;
                    break;
                case TypeCode.Int16:
                    sqlDbType = SqlDbType.SmallInt;
                    break;
                case TypeCode.Int32:
                    sqlDbType = SqlDbType.Int;
                    break;
                case TypeCode.Int64:
                    sqlDbType = SqlDbType.BigInt;
                    break;
                case TypeCode.Byte:
                    sqlDbType = SqlDbType.TinyInt;
                    break;
                case TypeCode.SByte:
                    sqlDbType = SqlDbType.TinyInt;
                    break;
                case TypeCode.UInt16:
                    sqlDbType = SqlDbType.SmallInt;
                    break;
                case TypeCode.UInt32:
                    sqlDbType = SqlDbType.Int;
                    break;
                case TypeCode.UInt64:
                    sqlDbType = SqlDbType.Decimal;
                    break;
                case TypeCode.Char:
                    sqlDbType = SqlDbType.Char;
                    break;
                case TypeCode.Single:
                    sqlDbType = SqlDbType.Float;
                    break;
                case TypeCode.Double:
                    sqlDbType = SqlDbType.Float;
                    break;
                case TypeCode.Decimal:
                    sqlDbType = SqlDbType.Decimal;
                    break;
                case TypeCode.DateTime:
                    sqlDbType = SqlDbType.DateTime;
                    break;
                case TypeCode.Boolean:
                    sqlDbType = SqlDbType.Bit;
                    break;
                case TypeCode.Object:
                    if (clrType == typeof(Guid))
                        sqlDbType = SqlDbType.UniqueIdentifier;
                    else if (clrType == typeof(Guid?))
                        sqlDbType = SqlDbType.UniqueIdentifier;
                    else if (clrType == typeof(TimeSpan))
                        sqlDbType = SqlDbType.Time;
                    else if (clrType == typeof(DateTimeOffset))
                        sqlDbType = SqlDbType.DateTimeOffset;
                    else if (clrType == typeof(XmlDocument))
                        sqlDbType = SqlDbType.Xml;
                    else if (clrType == typeof(XDocument))
                        sqlDbType = SqlDbType.Xml;
                    else if (clrType == typeof(byte[]))
                        sqlDbType = SqlDbType.VarBinary;
                    else if (clrType == typeof(char[]))
                        sqlDbType = SqlDbType.NVarChar;
                    else if (clrType == typeof(string))
                        sqlDbType = SqlDbType.NVarChar;
                    else if (clrType == typeof(short?))
                        sqlDbType = SqlDbType.SmallInt;
                    else if (clrType == typeof(int?))
                        sqlDbType = SqlDbType.Int;
                    else if (clrType == typeof(long?))
                        sqlDbType = SqlDbType.BigInt;
                    else if (clrType == typeof(float?))
                        sqlDbType = SqlDbType.Float;
                    else if (clrType == typeof(double?))
                        sqlDbType = SqlDbType.Float;
                    else if (clrType == typeof(decimal?))
                        sqlDbType = SqlDbType.Decimal;
                    else if (clrType == typeof(bool?))
                        sqlDbType = SqlDbType.Bit;
                    break;
                case TypeCode.DBNull:
                    sqlDbType = SqlDbType.Variant;
                    break;
                default:
                    sqlDbType = SqlDbType.Variant; // Default to Variant for unknown types
                    break;
            }

            return sqlDbType;
        }

        private static Type ToClrType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long?);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool?);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime?);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal?);

                case SqlDbType.Float:
                    return typeof(double?);

                case SqlDbType.Int:
                    return typeof(int?);

                case SqlDbType.Real:
                    return typeof(float?);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid?);

                case SqlDbType.SmallInt:
                    return typeof(short?);

                case SqlDbType.TinyInt:
                    return typeof(byte?);

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);

                case SqlDbType.Structured:
                    return typeof(DataTable);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);

                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }

        private static bool? ToBooleanOrDefault(object o, bool? Default)
        {
            return ToBooleanOrDefault((string)o, Default);
        }

        private static bool? ToBooleanOrDefault(string s, bool? defaultValue)
        {
            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

            // Convert input string to lowercase for case-insensitive comparison
            string lowerCaseString = s.ToLower();

            switch (lowerCaseString)
            {
                case "yes":
                case "affirmative":
                case "positive":
                case "true":
                case "ok":
                case "okay":
                case "y":
                case "+":
                    return true;
                case "no":
                case "negative":
                case "negatory":
                case "false":
                case "n":
                case "-":
                    return false;
                default:
                    bool parsedValue;
                    if (bool.TryParse(lowerCaseString, out parsedValue))
                    {
                        return parsedValue;
                    }
                    else
                    {
                        // Parsing failed, return default value
                        return defaultValue;
                    }
            }
        }
        #endregion

        #region [MS Access File Support]
        public enum MDBType
        {
            Access2000,
            Access2003,
            Access2007,
            Access2010,
            Access2016,
            Access2019
        }

        public static void ExportToMDBwithADOX(string connString, DataSet ds)
        {
            ADOX.Catalog cat = new ADOX.Catalog();
            OleDbConnection con = new OleDbConnection(connString);
            string fileName = con.DataSource;

            if (File.Exists(fileName))
                File.Delete(fileName);

            cat.Create(connString);

            foreach (DataTable dt in ds.Tables)
            {
                ADOX.Table tbl = DefineADOXTable(dt);
                cat.Tables.Append(tbl);
                AddDataTableToMDBwithADOX(cat, dt);
            }

            con = cat.ActiveConnection as OleDbConnection;
            if (con != null)
                con.Close();
            cat = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void ExportToMDBwithFile(string connString, DataSet ds, MDBType mDBType)
        {
            OleDbConnection con = CreateMDBfromFile(connString, mDBType);

            foreach (DataTable dt in ds.Tables)
            {
                AddDataTableToMDB(dt, con);
            }
        }

        public static OleDbConnection CreateMDBfromADOX(string connString)
        {
            ADOX.Catalog cat = new ADOX.Catalog();
            OleDbConnection con = new OleDbConnection(connString);
            string fileName = Path.GetFullPath(con.DataSource);

            if (File.Exists(fileName))
                File.Delete(fileName);

            cat.Create(connString);

            con = cat.ActiveConnection as OleDbConnection;
            cat = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return con;
        }

        public static OleDbConnection CreateMDBfromFile(string connString, MDBType mDBType)
        {
            OleDbConnection con = new OleDbConnection(connString);
            string fileName = Path.GetFullPath(con.DataSource);

            if (File.Exists(fileName))
                File.Delete(fileName);

            // Extract the embedded file.
            try
            {
                Assembly assembly = typeof(DataWriteLayer).Assembly;
                using (System.IO.Stream resource = assembly.GetManifestResourceStream(Enum.GetName(typeof(DataWriteLayer.MDBType), mDBType)))
                using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (resource == null)
                        throw new FileNotFoundException($"Could not find [{resource}] in {assembly.FullName}!");

                    resource.CopyTo(file);

                    // Make the file RW
                    FileAttributes attributes = File.GetAttributes(fileName);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes = attributes & ~FileAttributes.ReadOnly;
                        File.SetAttributes(fileName, attributes);
                    }
                }
                assembly = null;
            }
            catch (Exception e)
            {
                throw new Exception("Resource not extracted successfully.", e);
            }

            // Connect to Access database file.
            try
            {
                con.Open();
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to the database successfully.", e);
            }

            return con;
        }

        private static ADOX.Table DefineADOXTable(DataTable inTable)
        {
            ADOX.Table result = new ADOX.Table();
            result.Name = (new string(inTable.TableName.Where(c => !char.IsPunctuation(c)).ToArray())).Replace("$", string.Empty);
            DataColumnCollection inColumns = inTable.Columns;

            foreach (DataColumn inColumn in inColumns)
            {
                ADOX.Column col = new ADOX.Column();
                col.Name = inColumn.ColumnName;
                col.Type = ResolveADOXType(inColumn);

                if ((col.Type == ADOX.DataTypeEnum.adVarWChar) || (col.Type == ADOX.DataTypeEnum.adWChar))
                {
                    if (inColumn.MaxLength > 0)
                        col.DefinedSize = inColumn.MaxLength;
                    else
                        col.DefinedSize = 255;
                }

                if (inColumn.AllowDBNull)
                    col.Attributes = ADOX.ColumnAttributesEnum.adColNullable;

                result.Columns.Append(col);
            }

            return result;
        }

        private static void AddDataTableToMDBwithADOX(ADOX.Catalog cat, DataTable inTable)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient;
            DataColumnCollection inColumns = inTable.Columns;

            //remove punctuation from inTable.TableName for ADODB.Recordset
            rs.Open(string.Format("SELECT * FROM[{0}]", (new string(inTable.TableName.Where(c => !char.IsPunctuation(c)).ToArray())).Replace("$", string.Empty))
                    , cat.ActiveConnection
                    , ADODB.CursorTypeEnum.adOpenDynamic
                    , ADODB.LockTypeEnum.adLockOptimistic);

            if (!rs.BOF || !rs.EOF)
                rs.MoveFirst();

            foreach (DataRow dr in inTable.Rows)
            {
                rs.AddNew();
                for (int columnIndex = 0; columnIndex < inColumns.Count; columnIndex++)
                {
                    rs.Fields[columnIndex].Value = dr[columnIndex];
                }
                rs.Update();
            }

            rs.Close();
        }

        private static void AddDataTableToMDB(DataTable inTable, OleDbConnection conn)
        {
            StringBuilder sqlStmt;
            conn.Open();

            using (OleDbCommand cmd = new OleDbCommand())
            {
                sqlStmt = new StringBuilder();
                sqlStmt.Append("CREATE TABLE [" + inTable.TableName + "] (");
                foreach (DataColumn inColumn in inTable.Columns)
                {
                    sqlStmt.Append("[" + inColumn.ColumnName + "] " + ResolveOleDbType(inColumn) + ",");
                }

                // remove last comma
                sqlStmt.Length = sqlStmt.Length - 1;
                sqlStmt.Append(");");

                cmd.CommandText = sqlStmt.ToString();
                cmd.ExecuteNonQuery();
            }

            using (OleDbDataAdapter oledbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + inTable.TableName + "];", conn))
            {
                OleDbCommandBuilder oledbCmdBuilder = new OleDbCommandBuilder(oledbDataAdapter);
                oledbDataAdapter.InsertCommand = oledbCmdBuilder.GetInsertCommand();
                oledbDataAdapter.UpdateCommand = oledbCmdBuilder.GetUpdateCommand();

                using (DataTable accDataTable = inTable.Copy())
                {
                    // Set the RowState to added to ensure an Insert is performed
                    foreach (DataRow row in accDataTable.Rows)
                    {
                        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Unchanged)
                        {
                            row.SetAdded();
                        }
                    }

                    oledbDataAdapter.Update(accDataTable);
                }
            }
            conn.Close();
        }

        private static OleDbType ResolveOleDbType(DataColumn column)
        {
            Type type = column.GetType();
            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.String:
                    if (column.MaxLength > MAXNVARCHAR)
                        return OleDbType.LongVarWChar;
                    else
                        return OleDbType.VarWChar;
                case TypeCode.Int16:
                    return OleDbType.SmallInt;
                case TypeCode.Int32:
                    return OleDbType.Integer;
                case TypeCode.Int64:
                    return OleDbType.BigInt;
                case TypeCode.Byte:
                    return OleDbType.UnsignedTinyInt;
                case TypeCode.SByte:
                    return OleDbType.TinyInt;
                case TypeCode.UInt16:
                    return OleDbType.UnsignedSmallInt;
                case TypeCode.UInt32:
                    return OleDbType.UnsignedInt;
                case TypeCode.UInt64:
                    return OleDbType.UnsignedBigInt;
                case TypeCode.Char:
                    return OleDbType.Char;
                case TypeCode.Single:
                    return OleDbType.Single;
                case TypeCode.Double:
                    return OleDbType.Double;
                case TypeCode.Decimal:
                    return OleDbType.Decimal;
                case TypeCode.DateTime:
                    return OleDbType.Date;
                case TypeCode.Boolean:
                    return OleDbType.Boolean;
                case TypeCode.Object:
                    if (type == typeof(Guid))
                        return OleDbType.Guid;
                    else if (type == typeof(byte[]))
                        return OleDbType.Binary;
                    else if (type == typeof(TimeSpan))
                        return OleDbType.DBTime;
                    else if (type == typeof(DateTimeOffset))
                        return OleDbType.DBTimeStamp;
                    else if (type == typeof(byte[]))
                        return OleDbType.VarBinary;
                    else if (type == typeof(char[]))
                        return OleDbType.VarChar;
                    else if (type == typeof(short?))
                        return OleDbType.SmallInt;
                    else if (type == typeof(int?))
                        return OleDbType.Integer;
                    else if (type == typeof(long?))
                        return OleDbType.BigInt;
                    else if (type == typeof(float?))
                        return OleDbType.Single;
                    else if (type == typeof(double?))
                        return OleDbType.Double;
                    else if (type == typeof(decimal?))
                        return OleDbType.Decimal;
                    else if (type == typeof(bool?))
                        return OleDbType.Boolean;
                    else if (type == typeof(Guid?))
                        return OleDbType.Guid;
                    break;
                case TypeCode.DBNull:
                    return OleDbType.Empty;
                default:
                    return OleDbType.Variant; // Default to OleDbType.Variant for unknown types
            }

            // Default to OleDbType.Variant for unsupported types
            return OleDbType.Variant;
        }

        private static ADOX.DataTypeEnum ResolveADOXType(DataColumn column)
        {
            Type type = column.GetType();
            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.String:
                    if (column.MaxLength > MAXNVARCHAR)
                        return ADOX.DataTypeEnum.adLongVarWChar;
                    else
                        return ADOX.DataTypeEnum.adVarWChar;
                case TypeCode.Int16:
                    return ADOX.DataTypeEnum.adSmallInt;
                case TypeCode.Int32:
                    return ADOX.DataTypeEnum.adInteger;
                case TypeCode.Int64:
                    return ADOX.DataTypeEnum.adBigInt;
                case TypeCode.Byte:
                    return ADOX.DataTypeEnum.adUnsignedTinyInt;
                case TypeCode.SByte:
                    return ADOX.DataTypeEnum.adTinyInt;
                case TypeCode.UInt16:
                    return ADOX.DataTypeEnum.adUnsignedSmallInt;
                case TypeCode.UInt32:
                    return ADOX.DataTypeEnum.adUnsignedInt;
                case TypeCode.UInt64:
                    return ADOX.DataTypeEnum.adUnsignedBigInt;
                case TypeCode.Char:
                    return ADOX.DataTypeEnum.adChar;
                case TypeCode.Single:
                    return ADOX.DataTypeEnum.adSingle;
                case TypeCode.Double:
                    return ADOX.DataTypeEnum.adDouble;
                case TypeCode.Decimal:
                    return ADOX.DataTypeEnum.adDecimal;
                case TypeCode.DateTime:
                    return ADOX.DataTypeEnum.adDate;
                case TypeCode.Boolean:
                    return ADOX.DataTypeEnum.adBoolean;
                case TypeCode.Object:
                    if (type == typeof(Guid))
                        return ADOX.DataTypeEnum.adGUID;
                    else if (type == typeof(byte[]))
                        return ADOX.DataTypeEnum.adBinary;
                    else if (type == typeof(TimeSpan))
                        return ADOX.DataTypeEnum.adDBTime;
                    else if (type == typeof(DateTimeOffset))
                        return ADOX.DataTypeEnum.adDBTimeStamp;
                    else if (type == typeof(byte[]))
                        return ADOX.DataTypeEnum.adVarBinary;
                    else if (type == typeof(char[]))
                        return ADOX.DataTypeEnum.adVarChar;
                    else if (type == typeof(short?))
                        return ADOX.DataTypeEnum.adSmallInt;
                    else if (type == typeof(int?))
                        return ADOX.DataTypeEnum.adInteger;
                    else if (type == typeof(long?))
                        return ADOX.DataTypeEnum.adBigInt;
                    else if (type == typeof(float?))
                        return ADOX.DataTypeEnum.adSingle;
                    else if (type == typeof(double?))
                        return ADOX.DataTypeEnum.adDouble;
                    else if (type == typeof(decimal?))
                        return ADOX.DataTypeEnum.adDecimal;
                    else if (type == typeof(bool?))
                        return ADOX.DataTypeEnum.adBoolean;
                    else if (type == typeof(Guid?))
                        return ADOX.DataTypeEnum.adGUID;
                    break;
                case TypeCode.DBNull:
                    return ADOX.DataTypeEnum.adEmpty;
                default:
                    return ADOX.DataTypeEnum.adVariant; // Default to adVariant for unknown types
            }

            // Default to adVariant for unsupported types
            return ADOX.DataTypeEnum.adVariant;
        }
        #endregion

        #region [MS Excel File Support]
        public static void ExportToXLS(string sFilename, DataSet ds)
        {
            if (File.Exists(sFilename))
                File.Delete(sFilename);

            FileInfo newFile = new FileInfo(sFilename);
            DataRow dr;

            using (ExcelPackage pkg = new ExcelPackage(newFile))
            {
                foreach (DataTable dt in ds.Tables)
                {
                    ExcelWorksheet wkSheet = pkg.Workbook.Worksheets.Add(dt.TableName.Replace("$", string.Empty));
                    DataColumnCollection cols = dt.Columns;

                    for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                    {
                        dr = dt.Rows[rowIndex];

                        for (int columnIndex = 0; columnIndex < cols.Count; columnIndex++)
                        {
                            wkSheet.Cells[rowIndex + 1, columnIndex + 1].Value = dr.ItemArray[columnIndex];
                        }
                    }
                }

                pkg.Save();
            }
        }

        public static void CreateXLS(string sFilename, DataTable dt, string sheetName = "")
        {
            if (File.Exists(sFilename))
                File.Delete(sFilename);

            DataRow row = null;
            int offset = 1; // Environment.Version;
            string tmp;

            if (sheetName == string.Empty)
                tmp = (dt.TableName == string.Empty) ? Path.GetFileNameWithoutExtension(Path.GetFileName(sFilename)) : dt.TableName.Replace("$", string.Empty);
            else
                tmp = sheetName;

            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(sFilename)))
            {
                ExcelWorksheet wksht = pkg.Workbook.Worksheets.Add(tmp);

                wksht.TabColor = System.Drawing.Color.Black;
                wksht.DefaultRowHeight = 12;

                // Write columns names
                wksht.Row(1).Height = 20;
                wksht.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wksht.Row(1).Style.Font.Bold = true;

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn column = dt.Columns[i];
                    wksht.Cells[1, i + 1].Value = column.ColumnName;
                }

                // Write out rows
                for (int recordIndex = 0; recordIndex < dt.Rows.Count; recordIndex++)
                {
                    row = dt.Rows[recordIndex];

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        tmp = row[j].ToString();
                        tmp = tmp.Replace(Environment.NewLine, "\r\n");  // OS dependent

                        wksht.Cells[recordIndex + offset + 1, j + 1].Value = tmp;
                    }
                }

                pkg.Save();
            }
        }

        public static void CreateBackupXLS(string sFilename)
        {
            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(sFilename)))
            {
                string path = Path.Combine(Path.GetDirectoryName(sFilename), Path.GetFileNameWithoutExtension(sFilename) + "_bak", Path.GetExtension(sFilename));
                System.IO.Stream stream = File.Create(path);
                pkg.SaveAs(stream);
                stream.Close();
            }
        }

        public static void UpdateWkshtXLS(string sFilename, DataTable dt, string sheetName = "")
        {
            DataRow row = null;
            int offset = 1; // Environment.Version;
            string tmp;

            if (sheetName == string.Empty)
                tmp = (dt.TableName == string.Empty) ? Path.GetFileNameWithoutExtension(Path.GetFileName(sFilename)) : dt.TableName.Replace("$", string.Empty);
            else
                tmp = sheetName;

            CreateBackupXLS(sFilename);

            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(sFilename)))
            {
                ExcelWorksheet wksht = pkg.Workbook.Worksheets[tmp];
                wksht.Cells.Clear();

                wksht.TabColor = System.Drawing.Color.Black;
                wksht.DefaultRowHeight = 12;

                // Write columns names
                wksht.Row(1).Height = 20;
                wksht.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wksht.Row(1).Style.Font.Bold = true;

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn column = dt.Columns[i];
                    wksht.Cells[1, i + 1].Value = column.ColumnName;
                }

                // Write out rows
                for (int recordIndex = 0; recordIndex < dt.Rows.Count; recordIndex++)
                {
                    row = dt.Rows[recordIndex];

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        tmp = row[j].ToString();
                        tmp = tmp.Replace(Environment.NewLine, "\r\n");  // OS dependent

                        wksht.Cells[recordIndex + offset + 1, j + 1].Value = tmp;
                    }
                }

                pkg.Save();
            }
        }

        public static void AddDataColumnXLS(string sFile, DataTable dt, string sheetName, string fieldName, Type dataType)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                dt.Columns.Add(fieldName, dataType);

                // Update table layout
                UpdateWkshtXLS(sFile, dt, sheetName);
            }
        }

        public static void UpdateXLS_MapEvents(DataTable dt, FileInfo oFile, string sheetName)
        {
            dt.RowChanged += (sender, e) => UpdateXLS_HandleRowUpsert(sender, e, oFile, sheetName);
            dt.RowDeleted += (sender, e) => UpdateXLS_HandleRowDeleted(sender, e, oFile, sheetName);
        }

        private static void UpdateXLS_HandleRowUpsert(object sender, DataRowChangeEventArgs e, FileInfo oFile, string sheetName, bool hasHeader = true)
        {
            DataTable dt = (DataTable)sender;
            int offset = (hasHeader) ? 1 : 0;
            int recordIndex = dt.Rows.IndexOf(e.Row) + offset + 1;
            DataRow row;

            if (e.Row.RowState == DataRowState.Added)
            {
                // Open file
                using (ExcelPackage pkg = new ExcelPackage(oFile))
                using (ExcelWorkbook wkbk = pkg.Workbook)
                using (ExcelWorksheet wksht = wkbk.Worksheets[sheetName])
                {
                    // Build added row
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        // Cell values
                        wksht.Cells[recordIndex, i + 1].Value = Normalize(e.Row[i].ToString());
                    }

                    // Write the rest of the data offset by new row
                    for (int j = recordIndex + 1; j < dt.Rows.Count; j++)
                    {
                        row = dt.Rows[j];

                        // Build updated row
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            // Cell values
                            wksht.Cells[j + offset + 1, i + 1].Value = Normalize(row[i].ToString());
                        }
                    }

                    pkg.Save();
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                // Open file
                using (ExcelPackage pkg = new ExcelPackage(oFile))
                using (ExcelWorkbook wkbk = pkg.Workbook)
                using (ExcelWorksheet wksht = wkbk.Worksheets[sheetName])
                {
                    // Build updated row
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        // Cell values
                        wksht.Cells[recordIndex, i + 1].Value = Normalize(e.Row[i].ToString());
                    }

                    pkg.Save();
                }
            }
        }

        private static void UpdateXLS_HandleRowDeleted(object sender, DataRowChangeEventArgs e, FileInfo oFile, string sheetName, bool hasHeader = true)
        {
            DataTable dt = (DataTable)sender;
            int recordIndex = dt.Rows.IndexOf(e.Row);
            int offset = (hasHeader) ? 1 : 0;
            DataRow row;

            if (e.Action == DataRowAction.Delete)
            {
                // Open file
                using (ExcelPackage pkg = new ExcelPackage(oFile))
                using (ExcelWorkbook wkbk = pkg.Workbook)
                using (ExcelWorksheet wksht = wkbk.Worksheets[sheetName])
                {
                    int lineNumber = (hasHeader) ? (recordIndex + 1) : recordIndex;

                    //  Overwrite deleted row and write out remaining rows
                    for (int j = lineNumber; j < dt.Rows.Count; j++)
                    {
                        row = dt.Rows[j];

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            // Cell values
                            wksht.Cells[recordIndex + offset + 1, i + 1].Value = Normalize(row[i].ToString());
                        }
                    }

                    pkg.Save();
                }
            }
        }
        #endregion

        #region [CSV File Support]
        public static void ExportToCSV(DirectoryInfo oDir, DataSet ds)
        {
            if (!oDir.Exists)
                oDir.Create();

            FileInfo oFile;

            foreach (DataTable dt in ds.Tables)
            {
                oFile = new FileInfo(Path.GetFullPath(Path.Combine(oDir.FullName, string.Format("{0}.csv", dt.TableName.Replace("$", string.Empty)))));
                CreateCSV(dt, oFile);
            }
        }

        public static void CreateCSV(DataTable dt, FileInfo oFile)
        {
            string tmp;
            StringBuilder sb = new StringBuilder();

            using (StreamWriter sw = new StreamWriter(oFile.FullName))
            {
                // Write column headers
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.Write(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.WriteLine(); // Newline after headers

                // Write data rows
                foreach (DataRow row in dt.Rows)
                {
                    sb.Clear();

                    // Build updated row
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        // Cell values
                        tmp = row[i].ToString();
                        tmp = tmp.Replace(Environment.NewLine, "\r\n");  // OS dependent

                        if (i < (dt.Columns.Count - 1))
                        {
                            if (!tmp.Contains(','))
                                sb.Append(tmp + ',');
                            else
                                sb.AppendFormat("\"{0}\"", tmp + ',');
                        }
                        else
                        {
                            if (!tmp.Contains(','))
                                sb.Append(tmp);
                            else
                                sb.AppendFormat("\"{0}\"", tmp);
                        }
                    }

                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        public static void AddDataColumnCSV(FileInfo oFile, DataTable dt, string fieldName, Type dataType)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                dt.Columns.Add(fieldName, dataType);

                // Update table layout
                CreateCSV(dt, oFile);
            }
        }

        public static void UpdateCSV_MapEvents(DataTable dt, FileInfo oFile)
        {
            dt.RowChanged += (sender, e) => UpdateCSV_HandleRowUpsert(sender, e, oFile);
            dt.RowDeleted += (sender, e) => UpdateCSV_HandleRowDeleted(sender, e, oFile);
        }

        private static void UpdateCSV_HandleRowUpsert(object sender, DataRowChangeEventArgs e, FileInfo oFile)
        {
            if (e.Action == DataRowAction.Add)
            {
                DataTable dt = (DataTable)sender;
                int iRowIndex = dt.Rows.IndexOf(e.Row);
                int lineNumber = (iRowIndex + 1);  // Additional offset for CSV header

                // Use the BuildCsvRow method to format the new row
                string newRowCsv = BuildCsvRow(e.Row);

                // Save the changes to the CSV file here
                long targetLineBytePosition = 0; // Initialize to zero

                using (FileStream fs = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Calculate how far to skip ahead to the target line
                    for (int i = 0; i < lineNumber; i++)
                    {
                        targetLineBytePosition += enc.GetByteCount(fs.ReadLine() + Environment.NewLine);
                    }

                    // Reset the stream to the calculated position
                    fs.Seek(targetLineBytePosition, SeekOrigin.Begin);

                    // Add the line with the new row
                    fs.WriteLine(newRowCsv, enc);

                    // Write the remainder of the datatable to CSV file
                    for (int j = (lineNumber); j < dt.Rows.Count; j++)
                    {
                        fs.WriteLine(BuildCsvRow(dt.Rows[j]), enc);
                    }
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                DataTable dt = (DataTable)sender;
                int iRowIndex = dt.Rows.IndexOf(e.Row);
                int lineNumber = (iRowIndex + 1);  // Additional offset for CSV header 

                // Build the updated row
                string updatedRow = BuildCsvRow(e.Row);
                long targetLineBytePosition = 0; // Initialize to zero

                using (FileStream fs = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Calculate how far to skip ahead to the target line
                    for (int i = 0; i < lineNumber; i++)
                    {
                        targetLineBytePosition += enc.GetByteCount(fs.ReadLine() + Environment.NewLine);
                    }

                    // Reset the stream to the calculated position
                    fs.Seek(targetLineBytePosition, SeekOrigin.Begin);

                    // Replace the line with the updated row
                    fs.WriteLine(updatedRow, enc);

                    // Write the remainder of the datatable to CSV file
                    for (int j = lineNumber; j < dt.Rows.Count; j++)
                    {
                        fs.WriteLine(BuildCsvRow(dt.Rows[j]), enc);
                    }
                }
            }
        }

        private static void UpdateCSV_HandleRowDeleted(object sender, DataRowChangeEventArgs e, FileInfo oFile)
        {
            // Delete the specific row from the CSV file
            if (e.Action == DataRowAction.Delete)
            {
                DataTable dt = (DataTable)sender;
                int iRowIndex = dt.Rows.IndexOf(e.Row);
                int lineNumber = (iRowIndex + 1);  // Additional offset for CSV header

                long targetLineBytePosition = 0; // Initialize to zero

                using (FileStream fs = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Calculate how far to skip ahead to the target line
                    for (int i = 0; i < lineNumber; i++)
                    {
                        targetLineBytePosition += enc.GetByteCount(fs.ReadLine() + Environment.NewLine);
                    }

                    // Reset the stream to the calculated position
                    fs.Seek(targetLineBytePosition, SeekOrigin.Begin);

                    // Write the remainder of the datatable to CSV file
                    for (int j = lineNumber; j < dt.Rows.Count; j++)
                    {
                        fs.WriteLine(BuildCsvRow(dt.Rows[j]), enc);
                    }
                }
            }
        }

        private static string BuildCsvRow(DataRow row)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                sb.Append(Normalize(row[i].ToString(), true));

                if (i < row.Table.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }
        #endregion

        #region [XML File Support]
        public static string ExportToXml(DataSet ds)
        {
            return ds.SerializeToXML();
        }

        public static string ExportToXml(DataTable dt)
        {
            return dt.SerializeToXML();
        }

        private static string SerializeToXML(this object item)
        {
            StringBuilder result = new StringBuilder();
            DataContractSerializer serializer = new DataContractSerializer(item.GetType());
            using (StringWriter strWriter = new StringWriter(result))
            using (XmlWriter xmlWriter = XmlWriter.Create(strWriter))
            {
                serializer.WriteObject(xmlWriter, item);
            }

            return result.ToString();
        }
        #endregion

        #region [JSON File Support]
        public static string ExportToJSON(DataSet ds)
        {
            return ds.SerializeToJSON();
        }

        public static string ExportToJSON(DataTable dt)
        {
            return dt.SerializeToJSON();
        }

        private static string SerializeToJSON(this object item)
        {
            return JsonConvert.SerializeObject(item);
        }
        #endregion
        #endregion

        #region [Private Functions]
        private static ADODB.Recordset DefineRecordset(DataTable inTable)
        {
            ADODB.Recordset result = new ADODB.Recordset();
            string fieldName;
            ADODB.DataTypeEnum dataType;
            ADODB.FieldAttributeEnum attributes = ADODB.FieldAttributeEnum.adFldUnspecified;
            int DefinedSize;

            foreach (DataColumn inColumn in inTable.Columns)
            {
                fieldName = inColumn.ColumnName;
                dataType = ResolveADODBType(inColumn);
                if (inColumn.MaxLength > 0)
                    DefinedSize = inColumn.MaxLength;
                else
                    DefinedSize = 255;

                if (inColumn.AllowDBNull)
                    attributes = ADODB.FieldAttributeEnum.adFldIsNullable;

                result.Fields.Append(fieldName, dataType, DefinedSize, attributes);
            }

            return result;
        }

        private static ADODB.DataTypeEnum ResolveADODBType(DataColumn column)
        {
            Type type = column.GetType();
            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.String:
                    if (column.MaxLength > MAXNVARCHAR)
                        return ADODB.DataTypeEnum.adLongVarWChar;
                    else
                        return ADODB.DataTypeEnum.adVarWChar;
                case TypeCode.Int16:
                    return ADODB.DataTypeEnum.adSmallInt;
                case TypeCode.Int32:
                    return ADODB.DataTypeEnum.adInteger;
                case TypeCode.Int64:
                    return ADODB.DataTypeEnum.adBigInt;
                case TypeCode.Byte:
                    return ADODB.DataTypeEnum.adUnsignedTinyInt;
                case TypeCode.SByte:
                    return ADODB.DataTypeEnum.adTinyInt;
                case TypeCode.UInt16:
                    return ADODB.DataTypeEnum.adUnsignedSmallInt;
                case TypeCode.UInt32:
                    return ADODB.DataTypeEnum.adUnsignedInt;
                case TypeCode.UInt64:
                    return ADODB.DataTypeEnum.adUnsignedBigInt;
                case TypeCode.Char:
                    return ADODB.DataTypeEnum.adChar;
                case TypeCode.Single:
                    return ADODB.DataTypeEnum.adSingle;
                case TypeCode.Double:
                    return ADODB.DataTypeEnum.adDouble;
                case TypeCode.Decimal:
                    return ADODB.DataTypeEnum.adDecimal;
                case TypeCode.DateTime:
                    return ADODB.DataTypeEnum.adDate;
                case TypeCode.Boolean:
                    return ADODB.DataTypeEnum.adBoolean;
                case TypeCode.Object:
                    if (type == typeof(Guid))
                        return ADODB.DataTypeEnum.adGUID;
                    else if (type == typeof(byte[]))
                        return ADODB.DataTypeEnum.adBinary;
                    else if (type == typeof(TimeSpan))
                        return ADODB.DataTypeEnum.adDBTime;
                    else if (type == typeof(DateTimeOffset))
                        return ADODB.DataTypeEnum.adDBTimeStamp;
                    else if (type == typeof(byte[]))
                        return ADODB.DataTypeEnum.adVarBinary;
                    else if (type == typeof(char[]))
                        return ADODB.DataTypeEnum.adVarChar;
                    else if (type == typeof(short?))
                        return ADODB.DataTypeEnum.adSmallInt;
                    else if (type == typeof(int?))
                        return ADODB.DataTypeEnum.adInteger;
                    else if (type == typeof(long?))
                        return ADODB.DataTypeEnum.adBigInt;
                    else if (type == typeof(float?))
                        return ADODB.DataTypeEnum.adSingle;
                    else if (type == typeof(double?))
                        return ADODB.DataTypeEnum.adDouble;
                    else if (type == typeof(decimal?))
                        return ADODB.DataTypeEnum.adDecimal;
                    else if (type == typeof(bool?))
                        return ADODB.DataTypeEnum.adBoolean;
                    else if (type == typeof(Guid?))
                        return ADODB.DataTypeEnum.adGUID;
                    break;
                case TypeCode.DBNull:
                    return ADODB.DataTypeEnum.adEmpty;
                default:
                    return ADODB.DataTypeEnum.adVariant; // Default to OleDbType.Variant for unknown types
            }

            // Default to OleDbType.Variant for unsupported types
            return ADODB.DataTypeEnum.adVariant;
        }

        private static string Normalize(object o, bool isCSV = false, bool isSQL = false)
        {
            string value = o.ToString();

            value = value.Replace(Environment.NewLine, "\r\n");  // OS dependent

            value = (isCSV) ? value.Replace(",", "\",\"") : value;

            if (isSQL)
            {
                // Handle data type conversions
                if (o.GetType() == typeof(string) || o.GetType() == typeof(DateTime))
                {
                    // Escape single quotes in string values
                    value = value.Replace("'", "''");
                    // Surround string values with single quotes
                    value = $"'{value}'";
                }
                else if (o.GetType() == typeof(bool))
                {
                    // Convert boolean value to 0 or 1
                    value = (bool.Parse(value)) ? "1" : "0";
                }
                else if (o.GetType() == typeof(DateTime))
                {
                    // Format date/time values properly
                    value = $"CONVERT(datetime, '{value}', 121)";
                }
                else if (string.IsNullOrEmpty(value))
                {
                    // Handle NULL values
                    value = "NULL";
                }
            }

            value = value.Replace("\"", "\"\"\"");

            return value;
        }
        #endregion
    }
}
