using Element34.StringMetrics;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Element34.DataManager
{
    public static class DataReadLayer
    {
        #region Fields

        private static readonly Encoding enc = Encoding.Default;
        private static readonly CultureInfo culture = CultureInfo.CurrentCulture;

        #endregion

        #region Public Functions

        #region MS-SQL Server Support

        /// <summary>
        /// Creates a DataSet from a SQL Server database using a specified SQL query.
        /// </summary>
        /// <param name="sqlConn">The SQL connection.</param>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="paramList">The parameters for the SQL query.</param>
        /// <returns>A DataSet containing the results of the SQL query.</returns>
        public static DataSet DataSetFromMsSql(SqlConnection sqlConn, string sqlQuery, Dictionary<string, object> paramList = null)
        {
            if (sqlConn == null) throw new ArgumentNullException(nameof(sqlConn));
            if (string.IsNullOrEmpty(sqlQuery)) throw new ArgumentNullException(nameof(sqlQuery));

            DataSet result = new DataSet();
            using (SqlCommand command = new SqlCommand(sqlQuery, sqlConn))
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
                    sqlConn.Open();
                    sda.Fill(result);
                    sqlConn.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a DataTable from a SQL Server database using a specified SQL query.
        /// </summary>
        /// <param name="sqlConn">The SQL connection.</param>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="paramList">The parameters for the SQL query.</param>
        /// <returns>A DataTable containing the results of the SQL query.</returns>
        public static DataTable DataTableFromMsSql(SqlConnection sqlConn, string sqlQuery, Dictionary<string, object> paramList = null)
        {
            if (sqlConn == null) throw new ArgumentNullException(nameof(sqlConn));
            if (string.IsNullOrEmpty(sqlQuery)) throw new ArgumentNullException(nameof(sqlQuery));

            DataTable result = new DataTable();
            using (SqlCommand command = new SqlCommand(sqlQuery, sqlConn))
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
                    sqlConn.Open();
                    sda.Fill(result);
                    sqlConn.Close();
                }
            }

            return result;
        }

        #endregion

        #region MS Access File Support

        /// <summary>
        /// Creates a DataSet from an Access database (MDB) file.
        /// </summary>
        /// <param name="connString">The connection string to the Access database.</param>
        /// <returns>A DataSet containing all user tables from the Access database.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the connection string is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an error occurs while accessing the database.</exception>
        public static DataSet DataSetFromMDB(string connString)
        {
            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException(nameof(connString), "Connection string cannot be null or empty.");

            DataSet result = new DataSet();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    string fileName = conn.DataSource;
                    result.DataSetName = Path.GetFileNameWithoutExtension(fileName)?.Replace(" ", "_");

                    DataTable schemaTable = conn.GetSchema("Tables");
                    List<string> tableNames = schemaTable.AsEnumerable()
                        .Where(row => row.Field<string>("TABLE_TYPE") == "TABLE" && !row.Field<string>("TABLE_NAME").StartsWith("MSys"))
                        .Select(row => row.Field<string>("TABLE_NAME"))
                        .ToList();

                    foreach (string tableName in tableNames)
                    {
                        using (OleDbCommand cmd = new OleDbCommand($"SELECT * FROM [{tableName}]", conn))
                        {
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                            {
                                DataTable table = new DataTable(tableName);
                                adapter.Fill(table);
                                result.Tables.Add(table);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the DataSet from the Access database.", ex);
            }

            return result;
        }

        /// <summary>
        /// Creates a DataTable from an Access database (MDB) file using a specified SQL query.
        /// </summary>
        /// <param name="connString">The connection string to the Access database.</param>
        /// <param name="sqlStmt">The SQL statement to execute.</param>
        /// <param name="paramList">An optional dictionary of parameters to add to the SQL statement.</param>
        /// <returns>A DataTable containing the results of the SQL query.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the connection string or SQL statement is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an error occurs while executing the SQL query.</exception>
        public static DataTable DataTableFromMDB(string connString, string sqlStmt, Dictionary<string, object> paramList = null)
        {
            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException(nameof(connString), "Connection string cannot be null or empty.");
            if (string.IsNullOrEmpty(sqlStmt))
                throw new ArgumentNullException(nameof(sqlStmt), "SQL statement cannot be null or empty.");

            DataTable result = new DataTable();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(sqlStmt, conn))
                    {
                        if (paramList != null)
                        {
                            foreach (var param in paramList)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        conn.Open();
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            adapter.Fill(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the DataTable from the Access database.", ex);
            }

            return result;
        }

        #endregion

        #region MS Excel File Support

        /// <summary>
        /// Creates a DataSet from an Excel file.
        /// </summary>
        /// <param name="file">The Excel file to read.</param>
        /// <returns>A DataSet containing the data from the Excel file.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the Excel file is not found.</exception>
        public static DataSet DataSetFromXLS(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (!file.Exists) throw new FileNotFoundException("Excel file not found.", file.FullName);

            DataSet dataSet = new DataSet();

            using (ExcelPackage package = new ExcelPackage(file))
            {
                foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                {
                    DataTable dataTable = new DataTable(worksheet.Name);

                    foreach (ExcelRangeBase headerCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                    {
                        dataTable.Columns.Add(headerCell.Text);
                    }

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        ExcelRange rowCells = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column];
                        DataRow dataRow = dataTable.NewRow();
                        dataRow.ItemArray = rowCells.Select(cell => cell.Text).ToArray();
                        dataTable.Rows.Add(dataRow);
                    }

                    dataSet.Tables.Add(dataTable);
                }
            }

            return dataSet;
        }

        /// <summary>
        /// Creates a DataTable from an Excel file.
        /// </summary>
        /// <param name="file">The Excel file to read.</param>
        /// <param name="sheetName">The name of the worksheet to read. If not specified, the first worksheet is used.</param>
        /// <param name="hasHeader">Indicates whether the worksheet has a header row.</param>
        /// <returns>A DataTable containing the data from the specified worksheet.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the Excel file is not found.</exception>
        /// <exception cref="Exception">Thrown when the specified worksheet is not found.</exception>
        public static DataTable DataTableFromXLS(FileInfo file, string sheetName = "", bool hasHeader = true)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (!file.Exists) throw new FileNotFoundException("Excel file not found.", file.FullName);

            DataTable dataTable = new DataTable();

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = (string.IsNullOrEmpty(sheetName)
                    ? package.Workbook.Worksheets[0]
                    : package.Workbook.Worksheets[sheetName]) ?? throw new Exception($"Worksheet '{sheetName}' not found.");

                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;
                int startRow = hasHeader ? 2 : 1;

                if (hasHeader)
                {
                    foreach (ExcelRangeBase headerCell in worksheet.Cells[1, 1, 1, colCount])
                    {
                        dataTable.Columns.Add(headerCell.GetValue<string>().Trim(), typeof(string));
                    }
                }

                for (int row = startRow; row <= rowCount; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = GetCellText(worksheet.Cells[row, col]);
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        #endregion

        #region CSV File Support

        /// <summary>
        /// Creates a DataSet from a directory of CSV files.
        /// </summary>
        /// <param name="directoryPath">The directory path containing CSV files.</param>
        /// <returns>A DataSet containing DataTables from each CSV file in the directory.</returns>
        public static DataSet DataSetFromCSV(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath)) throw new ArgumentNullException(nameof(directoryPath));
            if (!Directory.Exists(directoryPath)) throw new DirectoryNotFoundException("Directory not found: " + directoryPath);

            DataSet result = new DataSet
            {
                DataSetName = new DirectoryInfo(directoryPath).Name.Replace(" ", "_")
            };

            foreach (string csvFile in Directory.GetFiles(directoryPath, "*.csv"))
            {
                DataTable dt = DataTableFromCSV(csvFile);
                result.Tables.Add(dt);
            }

            return result;
        }

        /// <summary>
        /// Creates a DataTable from a CSV file.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <param name="foldChars">Indicates whether to fold characters to ASCII.</param>
        /// <param name="tableReadOnly">Indicates whether the DataTable should be read-only.</param>
        /// <param name="makeBackup">Indicates whether to create a backup of the CSV file.</param>
        /// <returns>A DataTable containing the data from the CSV file.</returns>
        public static DataTable DataTableFromCSV(string filePath, bool foldChars = false, bool tableReadOnly = false, bool makeBackup = true)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("CSV file not found.", filePath);

            DataTable dt = new DataTable
            {
                TableName = Path.GetFileNameWithoutExtension(filePath)
            };

            if (makeBackup)
            {
                File.Copy(filePath, Path.ChangeExtension(filePath, ".bak"), true);
            }

            using (StreamReader reader = new StreamReader(filePath, enc))
            {
                StreamReader sr = reader;

                if (foldChars)
                {
                    List<string> lines = new List<string>();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(Transforms.FoldToASCII(line.ToCharArray()).ToString());
                    }
                    sr = new StreamReader(new MemoryStream(enc.GetBytes(string.Join(Environment.NewLine, lines))));
                }

                using (TextFieldParser parser = new TextFieldParser(sr))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    string[] headers = parser.ReadFields();
                    if (headers != null)
                    {
                        foreach (string header in headers)
                        {
                            dt.Columns.Add(header);
                        }

                        while (!parser.EndOfData)
                        {
                            string[] fields = parser.ReadFields();
                            if (fields != null)
                            {
                                dt.Rows.Add(fields);
                            }
                        }
                    }
                }
            }

            if (tableReadOnly)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    col.ReadOnly = true;
                }
            }

            return dt;
        }

        #endregion

        #region XML File Support

        /// <summary>
        /// Creates a DataSet from an XML string.
        /// </summary>
        /// <param name="xmlContent">The XML string content.</param>
        /// <returns>A DataSet containing the data from the XML string.</returns>
        public static DataSet DataSetFromXML(string xmlContent)
        {
            if (string.IsNullOrEmpty(xmlContent)) throw new ArgumentNullException(nameof(xmlContent));

            return xmlContent.DeserializefromXML<DataSet>();
        }

        /// <summary>
        /// Creates a DataTable from an XML string.
        /// </summary>
        /// <param name="xmlContent">The XML string content.</param>
        /// <param name="filter">The filter expression to apply to the DataTable.</param>
        /// <returns>A DataTable containing the data from the XML string, optionally filtered.</returns>
        public static DataTable DataTableFromXML(string xmlContent, string filter = "")
        {
            if (string.IsNullOrEmpty(xmlContent)) throw new ArgumentNullException(nameof(xmlContent));

            DataTable result = xmlContent.DeserializefromXML<DataTable>();

            if (!string.IsNullOrEmpty(filter) && result.Rows.Count > 0)
            {
                DataRow[] filteredRows = result.Select(filter);
                if (filteredRows.Length > 0)
                {
                    result = filteredRows.CopyToDataTable();
                }
            }

            return result;
        }

        private static T DeserializefromXML<T>(this string xmlContent)
        {
            if (string.IsNullOrEmpty(xmlContent)) throw new ArgumentNullException(nameof(xmlContent));

            using (StringReader stringReader = new StringReader(xmlContent))
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(xmlReader);
            }
        }

        #endregion

        #region JSON File Support

        /// <summary>
        /// Creates a DataSet from a JSON string.
        /// </summary>
        /// <param name="jsonContent">The JSON string content.</param>
        /// <returns>A DataSet containing the data from the JSON string.</returns>
        public static DataSet DataSetFromJSON(string jsonContent)
        {
            if (string.IsNullOrEmpty(jsonContent)) throw new ArgumentNullException(nameof(jsonContent));

            return jsonContent.DeserializeFromJSON<DataSet>();
        }

        /// <summary>
        /// Creates a DataTable from a JSON string.
        /// </summary>
        /// <param name="jsonContent">The JSON string content.</param>
        /// <param name="filter">The filter expression to apply to the DataTable.</param>
        /// <returns>A DataTable containing the data from the JSON string, optionally filtered.</returns>
        public static DataTable DataTableFromJSON(string jsonContent, string filter = "")
        {
            if (string.IsNullOrEmpty(jsonContent)) throw new ArgumentNullException(nameof(jsonContent));

            DataTable result = jsonContent.DeserializeFromJSON<DataTable>();

            if (!string.IsNullOrEmpty(filter) && result.Rows.Count > 0)
            {
                DataRow[] filteredRows = result.Select(filter);
                if (filteredRows.Length > 0)
                {
                    result = filteredRows.CopyToDataTable();
                }
            }

            return result;
        }

        #endregion

        #endregion

        #region Private Functions

        private static string GetCellText(ExcelRange cell)
        {
            // Handle different cell types based on cell.Value type
            if (cell.Value == null)
            {
                return string.Empty;
            }

            switch (cell.Value)
            {
                case DateTime dateTime:
                    if (dateTime.Date == new DateTime(1899, 12, 30))
                    {
                        // Treat this as a TimeSpan
                        return dateTime.TimeOfDay.ToString(@"hh\:mm\:ss");
                    }
                    return dateTime.ToString("yyyy-MM-dd");
                case double numeric:
                    // Check if the cell's number format indicates a TimeSpan
                    if (cell.Style.Numberformat.Format.Contains("h") || cell.Style.Numberformat.Format.Contains("m") || cell.Style.Numberformat.Format.Contains("s"))
                    {
                        // Convert numeric value to TimeSpan
                        TimeSpan timeSpan = TimeSpan.FromDays(numeric);
                        // Use cell.Style.Numberformat.Format to determine the format string
                        if (cell.Style.Numberformat.Format.Contains("AM/PM"))
                        {
                            // Convert to 12-hour format with AM/PM
                            DateTime dateTime = new DateTime().Add(timeSpan);
                            return dateTime.ToString("hh:mm tt");
                        }
                        return timeSpan.ToString(@"hh\:mm\:ss");
                    }
                    return numeric.ToString();
                case bool boolean:
                    return boolean.ToString();
                default:
                    string value = (string)cell.Value;

                    if (int.TryParse(value, out int resultInteger))
                    {
                        return resultInteger.ToString();
                    }

                    if (TimeSpan.TryParse(value, out TimeSpan resultTimeSpan))
                    {
                        if (cell.Style.Numberformat.Format.Contains("AM/PM"))
                        {
                            DateTime dateTime = DateTime.Today.Add(resultTimeSpan);
                            return dateTime.ToString("hh:mm tt");
                        }
                        return resultTimeSpan.ToString(@"hh\:mm\:ss");
                    }

                    if (DateTime.TryParseExact(value, "hh:mm tt", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        return result.ToString("hh:mm tt");
                    }

                    return cell.Text.Trim();
            }
        }

        private static string ParameterValueForSQL(SqlParameter sp)
        {
            if (sp == null) throw new ArgumentNullException(nameof(sp));

            string retval;
            switch (sp.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                    break;

                case SqlDbType.Bit:
                    retval = ToBooleanOrDefault(sp.Value, false) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        private static string ParameterValueForSQL(OleDbParameter sp)
        {
            if (sp == null) throw new ArgumentNullException(nameof(sp));

            string retval;
            switch (sp.OleDbType)
            {
                case OleDbType.Char:
                case OleDbType.WChar:
                case OleDbType.VarChar:
                case OleDbType.VarWChar:
                case OleDbType.LongVarChar:
                case OleDbType.LongVarWChar:
                case OleDbType.Date:
                case OleDbType.DBTime:
                case OleDbType.DBDate:
                case OleDbType.DBTimeStamp:
                    retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                    break;

                case OleDbType.Boolean:
                    retval = ToBooleanOrDefault(sp.Value, false) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        private static bool ToBooleanOrDefault(object o, bool Default)
        {
            return ToBooleanOrDefault(o?.ToString(), Default);
        }

        private static bool ToBooleanOrDefault(string s, bool defaultValue)
        {
            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

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
                    return bool.TryParse(lowerCaseString, out bool parsedValue) ? parsedValue : defaultValue;
            }
        }

        private static T DeserializeFromJSON<T>(this string sInput)
        {
            return JsonConvert.DeserializeObject<T>(sInput);
        }

        #endregion
    }
}