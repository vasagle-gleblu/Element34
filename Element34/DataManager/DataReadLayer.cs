using Element34.StringMetrics;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Element34.DataManager.Common;

namespace Element34.DataManager
{
    public static class DataReadLayer
    {
        #region [MS-SQL Server Support]
        public static DataSet DataSetFromMsSql(SqlConnection connection, string query, Dictionary<string, object> paramList = null)
        {
            DataSet result = new DataSet();

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (paramList != null)
                    {
                        command.Parameters.Clear();
                        foreach (KeyValuePair<string, object> param in paramList)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        connection.Open();
                        sda.Fill(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading data from MS-SQL Server: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        public static DataTable DataTableFromMsSql(SqlConnection connection, string query, Dictionary<string, object> paramList = null)
        {
            DataTable result = new DataTable();

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (paramList != null)
                    {
                        command.Parameters.Clear();
                        foreach (KeyValuePair<string, object> param in paramList)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        connection.Open();
                        adapter.Fill(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading data from MS-SQL Server: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
        #endregion

        #region [MySQL Server Support]
        public static DataSet DataSetFromMySql(MySqlConnection connection, string query, Dictionary<string, object> paramList = null)
        {
            DataSet result = new DataSet();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (paramList != null)
                    {
                        command.Parameters.Clear();
                        foreach (KeyValuePair<string, object> param in paramList)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    connection.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading data from MySQL Server: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        public static DataTable DataTableFromMySql(MySqlConnection connection, string query, Dictionary<string, object> paramList = null)
        {
            DataTable result = new DataTable();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (paramList != null)
                    {
                        command.Parameters.Clear();
                        foreach (KeyValuePair<string, object> param in paramList)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    connection.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading data from MySQL Server: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
        #endregion

        #region [MS Access File Support]
        public static DataSet DataSetFromMDB(OleDbConnection connection)
        {
            DataSet result = new DataSet();

            // For convenience, the DataSet is identified by the name of the loaded file (without extension).
            string fileName = (connection.DataSource);
            result.DataSetName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");

            try
            {
                // Opening the Access connection
                connection.Open();

                // Getting all user tables present in the Access file (Msys* tables are system thus useless for this scenario)
                DataTable dt = connection.GetSchema("Tables");
                List<string> tableNames = dt.AsEnumerable().Select(dr => dr.Field<string>("TABLE_NAME")).Where(dr => !dr.StartsWith("MSys")).ToList();

                // Getting the data for every user tables
                foreach (string tableName in tableNames)
                {
                    using (OleDbCommand command = new OleDbCommand(string.Format("SELECT * FROM [{0}]", tableName), connection))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                        {
                            // Saving all tables in our result DataSet.
                            dt = new DataTable("[" + tableName + "]");
                            adapter.Fill(dt);
                            result.Tables.Add(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading from MS Access database: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            // Return the filled DataSet
            return result;
        }

        public static DataTable DataTableFromMDB(OleDbConnection connection, string query, Dictionary<string, object> paramList = null)
        {
            DataTable result = new DataTable();

            try
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    if (paramList != null)
                    {
                        command.Parameters.Clear();
                        foreach (KeyValuePair<string, object> param in paramList)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    connection.Open();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading table from MS Access database: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
        #endregion

        #region [MS Excel File Support]
        public static DataSet DataSetFromXLS(FileInfo oFile)
        {
            if (!oFile.Exists)
                throw new FileNotFoundException("Excel file not found.");

            // Create a new DataSet to hold the data
            DataSet result = new DataSet();

            using (ExcelPackage pkg = new ExcelPackage(oFile))
            {
                // Iterate through all worksheets in the Excel file
                foreach (ExcelWorksheet WkSht in pkg.Workbook.Worksheets)
                {
                    // Create a DataTable for each worksheet
                    DataTable dt = new DataTable(WkSht.Name);

                    // Add columns to the DataTable
                    foreach (ExcelRangeBase headerCell in WkSht.Cells[1, 1, 1, WkSht.Dimension.End.Column])
                    {
                        dt.Columns.Add(headerCell.Text);
                    }

                    // Add rows to the DataTable
                    for (int row = 2; row <= WkSht.Dimension.End.Row; row++)
                    {
                        ExcelRange excelRow = WkSht.Cells[row, 1, row, WkSht.Dimension.End.Column];
                        DataRow newRow = dt.Rows.Add();
                        newRow.ItemArray = excelRow.Select(cell => cell.Text).ToArray();
                    }

                    // Add the DataTable to the DataSet
                    result.Tables.Add(dt);
                }
            }

            return result;
        }

        public static DataTable DataTableFromXLS(FileInfo oFile, string sSheetName = "", bool hasHeader = true)
        {
            if (!oFile.Exists)
                throw new FileNotFoundException("Excel file not found.");

            DataTable dt = new DataTable();

            using (ExcelPackage package = new ExcelPackage(oFile))
            {
                // Select named worksheet, default to first if name not supplied
                ExcelWorksheet worksheet = ((string.IsNullOrEmpty(sSheetName)) ? package.Workbook.Worksheets[0] : package.Workbook.Worksheets[sSheetName]) ?? throw new Exception("Worksheet not found");
                int colCount = worksheet.Dimension.End.Column;  // get column count
                int rowCount = worksheet.Dimension.End.Row;     // get row count
                int start = 1;

                if (hasHeader)
                {
                    start = 2;
                    ExcelRange wsHeader = worksheet.Cells[1, 1, 1, colCount];
                    foreach (ExcelRangeBase firstRowCell in wsHeader)
                    {
                        dt.Columns.Add(firstRowCell.Value.ToString().Trim(), typeof(string));
                    }
                }

                for (int row = start; row <= rowCount; row++)
                {
                    ExcelRange wsRow = worksheet.Cells[row, 1, row, colCount];
                    DataRow dr = dt.Rows.Add();

                    foreach (ExcelRangeBase cell in wsRow)
                    {

                        if (cell.Value != null)
                            dr[cell.Start.Column - 1] = cell.Value.ToString().Trim();
                    }
                }
            }

            return dt;
        }
        #endregion

        #region [SQLite File Support]
        public static DataSet DataSetFromSqlite(SqliteConnection connection)
        {
            DataSet result = new DataSet();
            List<string> tables = new List<string>();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string tableName = reader.GetString(0);
                    tables.Add(tableName);
                }

                foreach (string table in tables)
                {
                    command = new SqliteCommand($"SELECT * FROM {table}", connection);
                    reader = command.ExecuteReader();

                    // Load data into a DataTable
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    // Add the DataTable to the DataSet
                    result.Tables.Add(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data from SQLite database: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        public static DataTable DataTableFromSqlite(SqliteConnection connection, string query, Dictionary<string, object> paramList = null)
        {
            DataTable result = new DataTable();

            try
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    if (paramList != null)
                    {
                        command.Parameters.Clear();
                        foreach (KeyValuePair<string, object> param in paramList)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    connection.Open();
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        result.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data from SQLite database: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }
        #endregion

        #region [CSV File Support]
        public static DataSet DataSetFromCSV(string sPath)
        {
            DataSet result = new DataSet();
            DataTable dt;

            // For convenience, the DataSet is identified by the name of the named folder.
            string sFolder = Path.GetDirectoryName(sPath);
            result.DataSetName = sFolder.Substring(sFolder.LastIndexOf('\\'), sFolder.Length - sFolder.LastIndexOf('\\') - 1).Replace(" ", "_");

            // Process the list of files found in the folder.
            foreach (string sFile in Directory.GetFiles(sFolder, "*.csv"))
            {
                dt = DataTableFromCSV(sFile);
                result.Tables.Add(dt);
            }

            // Return the filled DataSet
            return result;
        }

        public static DataTable DataTableFromCSV(string sFile, bool blnFoldChars = false, bool blnTableReadOnly = false, bool blnMakeBackup = true)
        {
            // Create a new DataTable
            DataTable dt = new DataTable
            {
                TableName = Path.GetFileNameWithoutExtension(Path.GetFileName(sFile))
            };

            // Create backup
            if (blnMakeBackup)
            {
                FileInfo destFile = new FileInfo(Path.ChangeExtension(sFile, ".bak"));
                File.Copy(sFile, destFile.FullName, true);
            }


            // Load data from file 
            using (StreamReader reader = new StreamReader(sFile, enc))
            {
                StreamReader sr;

                if (blnFoldChars)
                {
                    string line;
                    IList<string> lines = new List<string>();

                    while ((line = reader.ReadLine()) != null)
                    {
                        line = Transforms.FoldToASCII(line.ToCharArray()).ToString();

                        lines.Add(line);
                    }
                    reader.Close();
                    reader.Dispose();

                    byte[] bytes = enc.GetBytes(string.Join(Environment.NewLine, lines.ToArray()));
                    Stream s = new MemoryStream(bytes);
                    sr = new StreamReader(s);
                }
                else
                    sr = reader;

                using (TextFieldParser parser = new TextFieldParser(sr))
                {
                    // Set the delimiter (comma in this case)
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the first line to create column headers
                    string[] headers = parser.ReadFields();

                    // Add columns to the DataTable
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }

                    // Read and add data rows
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        dt.Rows.Add(fields);
                    }
                }
            }

            // DataTable readonly or not.
            if (!blnTableReadOnly)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    col.ReadOnly = blnTableReadOnly;
                }
            }

            return dt;
        }
        #endregion

        #region [XML File Support]
        public static DataSet DataSetFromXML(string sInput)
        {
            DataSet result = sInput.DeserializefromXML<DataSet>();
            return result;
        }

        public static DataTable DataTableFromXML(string sInput, string sFilter = "")
        {
            DataTable result = sInput.DeserializefromXML<DataTable>();

            if (result.Rows.Count > 0)
            {
                if (sFilter.Length > 0)
                {
                    DataRow[] dr = result.Select(sFilter);
                    if (dr.Length > 0)
                    {
                        DataTable dt = dr.CopyToDataTable();
                        result = dt;
                    }
                }
            }

            return result;
        }
        #endregion

        #region [JSON File Support]
        public static DataSet DataSetFromJSON(string sInput)
        {
            return sInput.DeserializeFromJSON<DataSet>();
        }

        public static DataTable DataTableFromJSON(string sInput, string sFilter = "")
        {
            DataTable result = sInput.DeserializeFromJSON<DataTable>();

            if (result.Rows.Count > 0)
            {
                if (sFilter.Length > 0)
                {
                    DataRow[] dr = result.Select(sFilter);
                    if (dr.Length > 0)
                    {
                        DataTable dt = dr.CopyToDataTable();
                        result = dt;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}

