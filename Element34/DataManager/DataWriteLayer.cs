using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static Element34.DataManager.Common;

namespace Element34.DataManager
{
    public static class DataWriteLayer
    {
        #region [MS-SQL Server Support]
        public static void AddDataColumnMsSql(SqlConnection connection, DataTable dt, string fieldName, Type dataType, int length = -1)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                DataColumn column = new DataColumn(fieldName, dataType);

                if (length < 0)
                {
                    switch (dataType.Name)
                    {
                        case nameof(String):
                            length = 255;
                            break;
                        case nameof(Byte):
                            length = 100;
                            break;
                        case nameof(DateTime):
                            length = 8;
                            break;
                        case nameof(DateTimeOffset):
                            length = 10;
                            break;
                        case nameof(TimeSpan):
                            length = 8;
                            break;
                        case nameof(Guid):
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
                    connection.Open();

                    // Check if the column already exists
                    string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{dt.TableName}' AND COLUMN_NAME = '{fieldName}'";
                    using (SqlCommand command = new SqlCommand(query, connection))
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

                    using (SqlCommand command = new SqlCommand(query, connection))
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

        public static void UpdateMsSql_MapEvents(SqlConnection connection, DataTable dt)
        {
            dt.RowChanged += (sender, e) => UpdateMsSql_HandleRowUpsert(sender, e, connection);
            dt.RowDeleted += (sender, e) => UpdateMsSql_HandleRowDeleted(sender, e, connection);
        }

        private static void UpdateMsSql_HandleRowUpsert(object sender, DataRowChangeEventArgs e, SqlConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;
            bool hasPrimaryKey = HasPrimaryKeyMsSql(dt, connection);
            List<string> pkNames = (hasPrimaryKey) ? FindPrimaryKeysMsSql(dt, connection) : null;

            if (e.Row.RowState == DataRowState.Added)
            {
                // Open the SQL connection
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    StringBuilder insertCommandText = new StringBuilder();
                    insertCommandText.Append($"INSERT INTO [{tableName}] (");

                    if (hasPrimaryKey)
                    {
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

                    SqlCommand command = new SqlCommand(insertCommandText.ToString(), connection, transaction);
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
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    StringBuilder updateCommandText = new StringBuilder();
                    updateCommandText.Append($"UPDATE [{tableName}] SET ");

                    if (hasPrimaryKey)
                    {
                        // Build custom update command
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

                    SqlCommand command = new SqlCommand(updateCommandText.ToString(), connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error updating record in SQL Server table: {ex.Message}");
                }
                finally
                {
                    // Close the SQL connection in the finally block
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static void UpdateMsSql_HandleRowDeleted(object sender, DataRowChangeEventArgs e, SqlConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;

            if (e.Action == DataRowAction.Delete)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {tableName} WHERE 1=0", connection)) // Fetch schema only
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
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static bool HasPrimaryKeyMsSql(DataTable dataTable, SqlConnection connection)
        {
            bool result = false;
            string query = $"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYS.INDEXES AS i WHERE i.is_primary_key = 1 AND i.OBJECT_ID = OBJECT_ID('{dataTable.TableName}')) THEN 'true' ELSE 'false' END AS HasPrimaryKey; ";

            // Open the SQL connection
            connection.Open();

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
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
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        private static List<string> FindPrimaryKeysMsSql(DataTable dataTable, SqlConnection connection)
        {
            List<string> result = new List<string>();
            string query = $"SELECT c.name AS column_name FROM sys.indexes AS i INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id INNER JOIN sys.columns AS c ON ic.object_id = c.object_id AND ic.column_id = c.column_id WHERE i.object_id = OBJECT_ID('{dataTable.TableName}') AND i.is_primary_key = 1";

            // Open the SQL connection
            connection.Open();

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
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
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
        #endregion

        #region [MySQL Server Support]
        public static void AddDataColumnMySql(MySqlConnection connection, DataTable dt, string fieldName, Type dataType, int length = -1)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                DataColumn column = new DataColumn(fieldName, dataType);

                if (length < 0)
                {
                    switch (dataType.Name)
                    {
                        case nameof(String):
                            length = 255;
                            break;
                        case nameof(Byte):
                            length = 100;
                            break;
                        case nameof(DateTime):
                            length = 8;
                            break;
                        case nameof(DateTimeOffset):
                            length = 10;
                            break;
                        case nameof(TimeSpan):
                            length = 8;
                            break;
                        case nameof(Guid):
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
                    connection.Open();

                    // Check if the column already exists
                    string query = $"SHOW COLUMNS FROM `{dt.TableName}` WHERE Field = '{fieldName}'";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Debug.WriteLine("Column already exists.");
                                return;
                            }
                        }
                    }

                    // If the column doesn't exist, add it
                    string columnDefinition = length > 0 ? $"`{fieldName}` {ResolveMySqlDbType(column)}({length})" : $"`{fieldName}` {ResolveMySqlDbType(column)}";
                    query = $"ALTER TABLE `{dt.TableName}` ADD {columnDefinition}";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Debug.WriteLine($"Column '{fieldName}' added successfully to table '{dt.TableName}'.");
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error adding column: {ex.Message}");
                }
                finally
                {
                    // Close the MySQL connection in the finally block to ensure it is always closed
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static void UpdateMySql_MapEvents(MySqlConnection connection, DataTable dt)
        {
            dt.RowChanged += (sender, e) => UpdateMySql_HandleRowUpsert(sender, e, connection);
            dt.RowDeleted += (sender, e) => UpdateMySql_HandleRowDeleted(sender, e, connection);
        }

        private static void UpdateMySql_HandleRowUpsert(object sender, DataRowChangeEventArgs e, MySqlConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;
            bool hasPrimaryKey = HasPrimaryKey(tableName, connection);
            List<string> pkNames = (hasPrimaryKey) ? FindPrimaryKeys(tableName, connection) : null;

            if (e.Row.RowState == DataRowState.Added)
            {
                // Open the MySQL connection
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                MySqlCommand command;

                StringBuilder insertCommandText = new StringBuilder();
                insertCommandText.Append($"INSERT INTO `{tableName}` (");

                try
                {
                    if (hasPrimaryKey)
                    {
                        // Append column names
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string columnName = dt.Columns[i].ColumnName;

                            if (!pkNames.Contains(columnName))
                            {
                                insertCommandText.Append($"`{columnName}`");
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
                        command = new MySqlCommand(insertCommandText.ToString(), connection, transaction);
                        foreach (DataColumn column in dt.Columns)
                        {
                            command.Parameters.AddWithValue($"@{column.ColumnName}", Normalize(e.Row[column], false, true));
                        }
                    }
                    else
                    {
                        DataRow row = e.Row;

                        // Append column names
                        for (int i = 0; i < row.Table.Columns.Count; i++)
                        {
                            insertCommandText.Append($"`{row.Table.Columns[i].ColumnName}`");
                            if (i < row.Table.Columns.Count - 1)
                            {
                                insertCommandText.Append(", ");
                            }
                        }

                        insertCommandText.Append(") VALUES (");

                        // Append parameter placeholders
                        for (int i = 0; i < row.Table.Columns.Count; i++)
                        {
                            insertCommandText.Append($"@{row.Table.Columns[i].ColumnName}");
                            if (i < row.Table.Columns.Count - 1)
                            {
                                insertCommandText.Append(", ");
                            }
                        }
                        insertCommandText.Append(")");

                        // Add parameters and execute the command
                        command = new MySqlCommand(insertCommandText.ToString(), connection, transaction);
                        foreach (DataColumn column in row.Table.Columns)
                        {
                            command.Parameters.AddWithValue($"@{column.ColumnName}", Normalize(row[column], false, true));
                        }

                        // For example, you might want to log an error here or take other appropriate action
                        Debug.WriteLine("Error: Table does not have a primary key.");
                    }

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error inserting record into MySQL table: {ex.Message}");
                }
                finally
                {
                    // Close the MySQL connection in the finally block to ensure it is always closed
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                MySqlCommand command;

                StringBuilder updateCommandText = new StringBuilder();
                updateCommandText.Append($"UPDATE `{tableName}` SET ");

                try
                {
                    DataRow row = e.Row;

                    // Append SET clause dynamically based on updated columns
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        if (row[i, DataRowVersion.Current] != row[i, DataRowVersion.Original])
                        {
                            updateCommandText.Append($"`{row.Table.Columns[i].ColumnName}` = @{row.Table.Columns[i].ColumnName}, ");
                        }
                    }
                    updateCommandText.Remove(updateCommandText.Length - 2, 2); // Remove last comma
                    updateCommandText.Append(" WHERE ");

                    // Append WHERE clause based on primary key or all columns
                    if (hasPrimaryKey)
                    {
                        List<string> pkColumns = FindPrimaryKeys(tableName, connection);
                        foreach (string pkColumn in pkColumns)
                        {
                            updateCommandText.Append($"`{pkColumn}` = @{pkColumn} AND ");
                        }
                        updateCommandText.Remove(updateCommandText.Length - 5, 5); // Remove last 'AND'
                    }
                    else
                    {
                        for (int i = 0; i < row.Table.Columns.Count; i++)
                        {
                            updateCommandText.Append($"`{row.Table.Columns[i].ColumnName}` = @{row.Table.Columns[i].ColumnName}_old AND ");
                        }
                        updateCommandText.Remove(updateCommandText.Length - 5, 5); // Remove last 'AND'
                    }

                    // Create and execute the update command
                    command = new MySqlCommand(updateCommandText.ToString(), connection, transaction);
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        if (row[column, DataRowVersion.Current] != row[column, DataRowVersion.Original])
                        {
                            command.Parameters.AddWithValue($"@{column.ColumnName}", Normalize(row[column, DataRowVersion.Current], false, true));
                        }
                        if (!hasPrimaryKey)
                        {
                            command.Parameters.AddWithValue($"@{column.ColumnName}_old", Normalize(row[column, DataRowVersion.Original], false, true));
                        }
                    }

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error updating record in MySQL table: {ex.Message}");
                }
                finally
                {
                    // Close the MySQL connection in the finally block to ensure it is always closed
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static void UpdateMySql_HandleRowDeleted(object sender, DataRowChangeEventArgs e, MySqlConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;
            bool hasPrimaryKey = HasPrimaryKey(tableName, connection);

            // Open the MySQL connection
            connection.Open();
            MySqlTransaction transaction = connection.BeginTransaction();

            try
            {
                DataRow row = e.Row;

                StringBuilder deleteCommandText = new StringBuilder();
                deleteCommandText.Append($"DELETE FROM `{tableName}` WHERE ");

                // Append WHERE clause based on primary key or all columns
                if (hasPrimaryKey)
                {
                    List<string> pkColumns = FindPrimaryKeys(tableName, connection);
                    foreach (string pkColumn in pkColumns)
                    {
                        deleteCommandText.Append($"`{pkColumn}` = @{pkColumn} AND ");
                    }
                    deleteCommandText.Remove(deleteCommandText.Length - 5, 5); // Remove last 'AND'
                }
                else
                {
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        deleteCommandText.Append($"`{row.Table.Columns[i].ColumnName}` = @{row.Table.Columns[i].ColumnName} AND ");
                    }
                    deleteCommandText.Remove(deleteCommandText.Length - 5, 5); // Remove last 'AND'
                }

                // Create and execute the delete command
                MySqlCommand command = new MySqlCommand(deleteCommandText.ToString(), connection, transaction);
                if (hasPrimaryKey)
                {
                    foreach (string pkColumn in FindPrimaryKeys(tableName, connection))
                    {
                        command.Parameters.AddWithValue($"@{pkColumn}", Normalize(row[pkColumn], false, true));
                    }
                }
                else
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        command.Parameters.AddWithValue($"@{column.ColumnName}", Normalize(row[column], false, true));
                    }
                }

                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine($"Error deleting record from MySQL table: {ex.Message}");
            }
            finally
            {
                // Close the MySQL connection in the finally block to ensure it is always closed
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private static bool HasPrimaryKey(string tableName, MySqlConnection connection)
        {
            bool result = false;
            string query = $"SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS WHERE TABLE_NAME = '{tableName}' AND CONSTRAINT_TYPE = 'PRIMARY KEY'";

            // Open the MySQL connection
            connection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    result = (count > 0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying MySQL table: {ex.Message}");
            }
            finally
            {
                // Close the MySQL connection in the finally block
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        private static List<string> FindPrimaryKeys(string tableName, MySqlConnection connection)
        {
            List<string> result = new List<string>();
            string query = $"SELECT COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{tableName}' AND CONSTRAINT_NAME = 'PRIMARY'";

            // Open the MySQL connection
            connection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the column name and add it to the list
                        result.Add(reader["COLUMN_NAME"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying MySQL table: {ex.Message}");
            }
            finally
            {
                // Close the MySQL connection in the finally block
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
        #endregion

        #region [MS Access File Support]
        public static void AddDataColumnMDB(OleDbConnection connection, DataTable dt, string fieldName, Type dataType, int length = -1)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                DataColumn column = new DataColumn(fieldName, dataType);

                if (length < 0)
                {
                    switch (dataType.Name)
                    {
                        case nameof(String):
                            length = 255;
                            break;
                        case nameof(Byte):
                            length = 100;
                            break;
                        case nameof(DateTime):
                            length = 8;
                            break;
                        case nameof(DateTimeOffset):
                            length = 10;
                            break;
                        case nameof(TimeSpan):
                            length = 8;
                            break;
                        case nameof(Guid):
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
                    connection.Open();

                    // Check if the column already exists
                    string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{dt.TableName}' AND COLUMN_NAME = '{fieldName}'";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        int columnCount = (int)command.ExecuteScalar();
                        if (columnCount > 0)
                        {
                            Debug.WriteLine("Column already exists.");
                            return;
                        }
                    }

                    OleDbType columnType = ResolveOleDbType(column);

                    // If the column doesn't exist, add it
                    string columnDefinition = length > 0 ? $"[{fieldName}] {columnType}({length})" : $"[{fieldName}] {columnType}";
                    query = $"ALTER TABLE [{dt.TableName}] ADD COLUMN {columnDefinition}";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Debug.WriteLine($"Column '{fieldName}' added successfully to table '{dt.TableName}'.");
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error adding column: {ex.Message}");
                }
                finally
                {
                    // Close the MS Access connection in the finally block
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static void ExportToMDBwithADOX(OleDbConnection connection, DataSet ds)
        {
            ADOX.Catalog catalog = new ADOX.Catalog();
            string fileName = connection.DataSource;

            if (File.Exists(fileName))
                File.Delete(fileName);

            catalog.Create(connection.ConnectionString);

            foreach (DataTable dt in ds.Tables)
            {
                ADOX.Table table = DefineADOXTable(dt);
                catalog.Tables.Append(table);
                AddDataTableToMDBwithADOX(catalog, dt);
            }

            connection = catalog.ActiveConnection as OleDbConnection;
            connection?.Close();
            catalog = null;
        }

        public static void ExportToMDBwithFile(OleDbConnection connection, DataSet ds, MDBType mDBType)
        {
            CreateMDBfromFile(connection, mDBType);

            foreach (DataTable dt in ds.Tables)
            {
                AddDataTableToMDB(dt, connection);
            }
        }

        public static OleDbConnection CreateMDBfromADOX(OleDbConnection connection)
        {
            ADOX.Catalog catalog = new ADOX.Catalog();
            string fileName = Path.GetFullPath(connection.DataSource);

            if (File.Exists(fileName))
                File.Delete(fileName);

            catalog.Create(connection.ConnectionString);

            connection = catalog.ActiveConnection as OleDbConnection;
            catalog = null;

            return connection;
        }

        public static OleDbConnection CreateMDBfromFile(OleDbConnection connection, MDBType mDBType)
        {
            string fileName = Path.GetFullPath(connection.DataSource);

            if (File.Exists(fileName))
                File.Delete(fileName);

            // Extract the embedded file.
            try
            {
                Assembly assembly = typeof(DataWriteLayer).Assembly;
                using (System.IO.Stream resource = assembly.GetManifestResourceStream(Enum.GetName(typeof(MDBType), mDBType)))
                using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (resource == null)
                        throw new FileNotFoundException($"Could not find [{resource}] in {assembly.FullName}!");

                    resource.CopyTo(file);

                    // Make the file RW
                    FileAttributes attributes = File.GetAttributes(fileName);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes &= ~FileAttributes.ReadOnly;
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
                connection.Open();
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to the database successfully.", e);
            }

            return connection;
        }

        public static void UpdateMDB_MapEvents(OleDbConnection connection, DataTable dt)
        {
            dt.RowChanged += (sender, e) => UpdateMDB_HandleRowUpsert(sender, e, connection);
            dt.RowDeleted += (sender, e) => UpdateMDB_HandleRowDeleted(sender, e, connection);
        }

        private static void UpdateMDB_HandleRowUpsert(object sender, DataRowChangeEventArgs e, OleDbConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;
            bool hasPrimaryKey = HasPrimaryKeyMDB(dt, connection);
            List<string> pkNames = (hasPrimaryKey) ? FindPrimaryKeysMDB(dt, connection) : null;

            if (e.Row.RowState == DataRowState.Added)
            {
                // Open the MS Access connection
                connection.Open();
                OleDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    StringBuilder insertCommandText = new StringBuilder();
                    insertCommandText.Append($"INSERT INTO [{tableName}] (");

                    if (hasPrimaryKey)
                    {
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

                    OleDbCommand command = new OleDbCommand(insertCommandText.ToString(), connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error inserting record into MS Access table: {ex.Message}");
                }
                finally
                {
                    // Close the Access connection in the finally block to ensure it is always closed
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                connection.Open();
                OleDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    StringBuilder updateCommandText = new StringBuilder();
                    updateCommandText.Append($"UPDATE [{tableName}] SET ");

                    if (hasPrimaryKey)
                    {
                        // Build custom update command
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

                    OleDbCommand command = new OleDbCommand(updateCommandText.ToString(), connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error updating record in MS Access table: {ex.Message}");
                }
                finally
                {
                    // Close the Access connection in the finally block
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static void UpdateMDB_HandleRowDeleted(object sender, DataRowChangeEventArgs e, OleDbConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;

            if (e.Action == DataRowAction.Delete)
            {
                connection.Open();
                OleDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Create a command to delete the row directly in the database
                    string deleteQuery = $"DELETE FROM [{tableName}] WHERE ";
                    List<string> whereClauses = new List<string>();

                    foreach (DataColumn column in dt.Columns)
                    {
                        whereClauses.Add($"[{column.ColumnName}] = @{column.ColumnName}");
                    }

                    deleteQuery += string.Join(" AND ", whereClauses);
                    OleDbCommand deleteCommand = new OleDbCommand(deleteQuery, connection, transaction);

                    // Add parameters for each column in the row being deleted
                    foreach (DataColumn column in dt.Columns)
                    {
                        deleteCommand.Parameters.AddWithValue($"@{column.ColumnName}", e.Row[column]);
                    }

                    // Execute the delete command
                    deleteCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error deleting row from MS Access table: {ex.Message}");
                }
                finally
                {
                    // Close the MS Access connection in the finally block to ensure it is always closed
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static bool HasPrimaryKeyMDB(DataTable dataTable, OleDbConnection connection)
        {
            bool result = false;
            string query = $"SELECT COUNT(*) FROM MSysObjects WHERE Type=1 AND Name='{dataTable.TableName}' AND Flags=0";

            // Open the MS Access connection
            connection.Open();

            try
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    int tableCount = (int)command.ExecuteScalar();
                    result = tableCount > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying MS Access table: {ex.Message}");
            }
            finally
            {
                // Close the MS Access connection in the finally block
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        private static List<string> FindPrimaryKeysMDB(DataTable dataTable, OleDbConnection connection)
        {
            List<string> result = new List<string>();
            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{dataTable.TableName}' AND CONSTRAINT_NAME LIKE 'PrimaryKey%'";

            // Open the MS Access connection
            connection.Open();

            try
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the column name and add it to the list
                        result.Add(reader["COLUMN_NAME"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying MS Access table: {ex.Message}");
            }
            finally
            {
                // Close the MS Access connection in the finally block
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
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
                    DataRow row = dt.Rows[recordIndex];

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
                string path = Path.Combine(Path.GetDirectoryName(sFilename), Path.GetFileNameWithoutExtension(sFilename) + "_bak" + Path.GetExtension(sFilename));
                System.IO.Stream stream = File.Create(path);
                pkg.SaveAs(stream);
                stream.Close();
            }
        }

        public static void UpdateWkshtXLS(string sFilename, DataTable dt, string sheetName = "")
        {
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
                    DataRow row = dt.Rows[recordIndex];

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

        #region [SQLite File Support]
        public static void AddDataColumnSQLite(SqliteConnection connection, DataTable dt, string fieldName, Type dataType, int length = -1)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                DataColumn column = new DataColumn(fieldName, dataType);

                if (length < 0)
                {
                    switch (dataType.Name)
                    {
                        case nameof(String):
                            length = 255;
                            break;
                        case nameof(Byte):
                            length = 100;
                            break;
                        case nameof(DateTime):
                            length = 8;
                            break;
                        case nameof(DateTimeOffset):
                            length = 10;
                            break;
                        case nameof(TimeSpan):
                            length = 8;
                            break;
                        case nameof(Guid):
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
                    connection.Open();

                    // Check if the column already exists
                    string query = $"PRAGMA table_info({dt.TableName})";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string existingColumnName = reader["name"].ToString();
                            if (existingColumnName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                            {
                                Debug.WriteLine("Column already exists.");
                                return;
                            }
                        }
                    }

                    // If the column doesn't exist, add it
                    string columnDefinition = length > 0 ? $"[{fieldName}] {ResolveSQLiteColumnType(dataType)}({length})" : $"[{fieldName}] {ResolveSQLiteColumnType(dataType)}";
                    query = $"ALTER TABLE [{dt.TableName}] ADD COLUMN {columnDefinition}";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Debug.WriteLine($"Column '{fieldName}' added successfully to table '{dt.TableName}'.");
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error adding column: {ex.Message}");
                }
                finally
                {
                    // Close the SQLite connection in the finally block
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static void UpdateSqlite_MapEvents(SqliteConnection connection, DataTable dt)
        {
            dt.RowChanged += (sender, e) => UpdateSqlite_HandleRowUpsert(sender, e, connection);
            dt.RowDeleted += (sender, e) => UpdateSqlite_HandleRowDeleted(sender, e, connection);
        }

        private static void UpdateSqlite_HandleRowUpsert(object sender, DataRowChangeEventArgs e, SqliteConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;
            bool hasPrimaryKey = HasPrimaryKeySqlite(dt, connection);
            List<string> pkNames = (hasPrimaryKey) ? FindPrimaryKeysSqlite(dt, connection) : null;

            if (e.Row.RowState == DataRowState.Added)
            {
                // Open the SQLite connection
                connection.Open();
                SqliteTransaction transaction = connection.BeginTransaction();

                try
                {
                    StringBuilder insertCommandText = new StringBuilder();
                    insertCommandText.Append($"INSERT INTO [{tableName}] (");

                    if (hasPrimaryKey)
                    {
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

                    SqliteCommand command = new SqliteCommand(insertCommandText.ToString(), connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error inserting record into SQLite table: {ex.Message}");
                }
                finally
                {
                    // Close the SQLite connection in the finally block to ensure it is always closed
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                connection.Open();
                SqliteTransaction transaction = connection.BeginTransaction();

                try
                {
                    StringBuilder updateCommandText = new StringBuilder();
                    updateCommandText.Append($"UPDATE [{tableName}] SET ");

                    if (hasPrimaryKey)
                    {
                        // Build custom update command
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

                    SqliteCommand command = new SqliteCommand(updateCommandText.ToString(), connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error updating record in SQLite table: {ex.Message}");
                }
                finally
                {
                    // Close the SQLite connection in the finally block
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static void UpdateSqlite_HandleRowDeleted(object sender, DataRowChangeEventArgs e, SqliteConnection connection)
        {
            DataTable dt = (DataTable)sender;
            string tableName = dt.TableName;

            if (e.Action == DataRowAction.Delete)
            {
                connection.Open();
                SqliteTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Create a command to delete the row directly in the database
                    string deleteQuery = $"DELETE FROM [{tableName}] WHERE ";
                    List<string> whereClauses = new List<string>();

                    foreach (DataColumn column in dt.Columns)
                    {
                        whereClauses.Add($"[{column.ColumnName}] = @{column.ColumnName}");
                    }

                    deleteQuery += string.Join(" AND ", whereClauses);
                    SqliteCommand deleteCommand = new SqliteCommand(deleteQuery, connection, transaction);

                    // Add parameters for each column in the row being deleted
                    foreach (DataColumn column in dt.Columns)
                    {
                        deleteCommand.Parameters.AddWithValue($"@{column.ColumnName}", e.Row[column]);
                    }

                    // Execute the delete command
                    deleteCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine($"Error deleting row from SQLite table: {ex.Message}");
                }
                finally
                {
                    // Close the SQLite connection in the finally block to ensure it is always closed
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static bool HasPrimaryKeySqlite(DataTable dataTable, SqliteConnection connection)
        {
            bool result = false;
            string query = $"PRAGMA table_info({dataTable.TableName})";

            // Open the SQLite connection
            connection.Open();

            try
            {
                using (SqliteCommand command = new SqliteCommand(query, connection))
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool isPrimaryKey = Convert.ToBoolean(reader["pk"]);
                        if (isPrimaryKey)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying SQLite table: {ex.Message}");
            }
            finally
            {
                // Close the SQLite connection in the finally block
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        private static List<string> FindPrimaryKeysSqlite(DataTable dataTable, SqliteConnection connection)
        {
            List<string> result = new List<string>();
            string query = $"PRAGMA table_info({dataTable.TableName})";

            // Open the SQLite connection
            connection.Open();

            try
            {
                using (SqliteCommand command = new SqliteCommand(query, connection))
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool isPrimaryKey = Convert.ToBoolean(reader["pk"]);
                        if (isPrimaryKey)
                        {
                            string columnName = reader["name"].ToString();
                            result.Add(columnName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying SQLite table: {ex.Message}");
            }
            finally
            {
                // Close the SQLite connection in the finally block
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
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
        #endregion
    }
}
