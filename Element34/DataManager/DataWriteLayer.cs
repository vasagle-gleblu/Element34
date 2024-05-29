using ADOX;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
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
        #region Fields
        private static readonly Encoding _enc = Encoding.Default;
        private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private const int MAXNVARCHAR = 4000;
        private const int MAXVARCHAR = 8000;
        #endregion

        #region Public Functions

        #region MS-SQL Server Support

        /// <summary>
        /// Adds a data column to a SQL Server table.
        /// </summary>
        /// <param name="sqlConn">The SQL connection.</param>
        /// <param name="dt">The DataTable.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="dataType">The data type of the field.</param>
        /// <param name="length">The length of the field.</param>
        public static void AddDataColumnMsSql(SqlConnection sqlConn, DataTable dt, string fieldName, Type dataType, int length = -1)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                DataColumn column = new DataColumn(fieldName, dataType)
                {
                    MaxLength = length < 0 ? GetDefaultLength(dataType) : length
                };

                dt.Columns.Add(column);

                try
                {
                    sqlConn.Open();

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
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        /// <summary>
        /// Maps events to update the SQL Server table when rows are changed or deleted in the DataTable.
        /// </summary>
        /// <param name="sqlConn">The SQL connection.</param>
        /// <param name="dt">The DataTable.</param>
        public static void UpdateMsSql_MapEvents(SqlConnection sqlConn, DataTable dt)
        {
            dt.RowChanged += (sender, e) => UpdateMsSql_HandleRowUpsert(sender, e, sqlConn);
            dt.RowDeleted += (sender, e) => UpdateMsSql_HandleRowDeleted(sender, e, sqlConn);
        }

        #endregion

        #region MS Access File Support

        /// <summary>
        /// Specifies the versions of Microsoft Access database (MDB) files.
        /// </summary>
        public enum MDBType
        {
            /// <summary>
            /// Microsoft Access 2000 database format.
            /// </summary>
            Access2000,

            /// <summary>
            /// Microsoft Access 2003 database format.
            /// </summary>
            Access2003,

            /// <summary>
            /// Microsoft Access 2007 database format.
            /// </summary>
            Access2007,

            /// <summary>
            /// Microsoft Access 2010 database format.
            /// </summary>
            Access2010,

            /// <summary>
            /// Microsoft Access 2016 database format.
            /// </summary>
            Access2016,

            /// <summary>
            /// Microsoft Access 2019 database format.
            /// </summary>
            Access2019
        }

        /// <summary>
        /// Creates a new Microsoft Access database (MDB) file using ADOX and returns an open <see cref="OleDbConnection"/> to the created database.
        /// </summary>
        /// <param name="connString">The connection string used to create the database.</param>
        /// <param name="logger">An optional logger instance for logging purposes.</param>
        /// <returns>An <see cref="OleDbConnection"/> object connected to the newly created MDB file.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connString"/> is null or empty.</exception>
        /// <exception cref="IOException">Thrown when an error occurs while deleting an existing file or creating a new database file.</exception>
        public static OleDbConnection CreateMDBfromADOX(string connString, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(connString))
            {
                throw new ArgumentNullException(nameof(connString), "Connection string cannot be null or empty.");
            }

            Catalog cat = null;
            OleDbConnection con = null;

            try
            {
                cat = new Catalog();
                con = new OleDbConnection(connString);
                string fileName = Path.GetFullPath(con.DataSource);

                if (File.Exists(fileName))
                {
                    logger?.LogInformation($"File {fileName} already exists. Deleting it.");
                    File.Delete(fileName);
                }

                logger?.LogInformation($"Creating new database with connection string: {connString}");
                cat.Create(connString);

                con = cat.ActiveConnection as OleDbConnection;
                logger?.LogInformation($"Database created successfully at {fileName}");

                return con;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while creating the database.");
                throw new IOException("An error occurred while creating the database.", ex);
            }
            finally
            {
                // Ensure resources are cleaned up
                if (cat != null)
                {
                    try
                    {
                        // Attempt to close the active connection
                        if (cat.ActiveConnection != null)
                        {
                            (cat.ActiveConnection as IDisposable)?.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex, "An error occurred while disposing the Catalog's active connection.");
                    }
                }

                // Explicitly release the Catalog object
                if (cat != null)
                {
                    cat = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        /// <summary>
        /// Exports a DataSet to an Access database using ADOX.
        /// </summary>
        /// <param name="connString">The connection string.</param>
        /// <param name="ds">The DataSet to export.</param>
        public static void ExportToMDBwithADOX(string connString, DataSet ds)
        {
            ADOX.Catalog cat = null;
            try
            {
                cat = new ADOX.Catalog();
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

                if (cat.ActiveConnection is OleDbConnection activeConnection)
                {
                    activeConnection.Close();
                }
            }
            finally
            {
                if (cat != null)
                {
                    cat = null;
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Exports a DataSet to an Access database using a file template.
        /// </summary>
        /// <param name="connString">The connection string.</param>
        /// <param name="ds">The DataSet to export.</param>
        /// <param name="mDBType">The type of MDB file to create.</param>
        public static void ExportToMDBwithFile(string connString, DataSet ds, MDBType mDBType)
        {
            using (OleDbConnection con = CreateMDBfromFile(connString, mDBType))
            {
                foreach (DataTable dt in ds.Tables)
                {
                    AddDataTableToMDB(dt, con);
                }
            }
        }

        #endregion

        #region MS Excel File Support

        /// <summary>
        /// Exports a DataSet to an Excel file.
        /// </summary>
        /// <param name="sFilename">The Excel file name.</param>
        /// <param name="ds">The DataSet to export.</param>
        public static void ExportToXLS(string sFilename, DataSet ds)
        {
            if (File.Exists(sFilename))
                File.Delete(sFilename);

            FileInfo newFile = new FileInfo(sFilename);

            using (ExcelPackage pkg = new ExcelPackage(newFile))
            {
                foreach (DataTable dt in ds.Tables)
                {
                    ExcelWorksheet wkSheet = pkg.Workbook.Worksheets.Add(dt.TableName.Replace("$", string.Empty));

                    for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                    {
                        DataRow dr = dt.Rows[rowIndex];

                        for (int columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++)
                        {
                            wkSheet.Cells[rowIndex + 1, columnIndex + 1].Value = dr[columnIndex];
                        }
                    }
                }

                pkg.Save();
            }
        }

        /// <summary>
        /// Creates an Excel file from a DataTable.
        /// </summary>
        /// <param name="sFilename">The Excel file name.</param>
        /// <param name="dt">The DataTable to export.</param>
        /// <param name="sheetName">The sheet name.</param>
        public static void CreateXLS(string sFilename, DataTable dt, string sheetName = "")
        {
            if (File.Exists(sFilename))
                File.Delete(sFilename);

            string tmp = string.IsNullOrEmpty(sheetName)
                ? string.IsNullOrEmpty(dt.TableName)
                    ? Path.GetFileNameWithoutExtension(Path.GetFileName(sFilename))
                    : dt.TableName.Replace("$", string.Empty)
                : sheetName;

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
                        if (row[j] != DBNull.Value)
                        {
                            tmp = row[j] is TimeSpan timeSpan ? string.Format("{0:hh\\:mm\\:ss}", timeSpan) : row[j].ToString().Replace(Environment.NewLine, "\r\n");

                            wksht.Cells[recordIndex + 2, j + 1].Value = tmp;
                        }
                    }
                }

                pkg.Save();
            }
        }

        /// <summary>
        /// Creates a backup of an Excel file.
        /// </summary>
        /// <param name="sFilename">The Excel file name.</param>
        public static void CreateBackupXLS(string sFilename)
        {
            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(sFilename)))
            {
                string path = Path.Combine(Path.GetDirectoryName(sFilename), Path.GetFileNameWithoutExtension(sFilename) + "_bak" + Path.GetExtension(sFilename));
                using (Stream stream = File.Create(path))
                {
                    pkg.SaveAs(stream);
                }
            }
        }

        /// <summary>
        /// Updates an Excel worksheet with data from a DataTable.
        /// </summary>
        /// <param name="sFilename">The Excel file name.</param>
        /// <param name="dt">The DataTable.</param>
        /// <param name="sheetName">The sheet name.</param>
        public static void UpdateWkshtXLS(string sFilename, DataTable dt, string sheetName = "")
        {
            CreateBackupXLS(sFilename);

            sheetName = string.IsNullOrEmpty(sheetName)
                ? string.IsNullOrEmpty(dt.TableName)
                    ? Path.GetFileNameWithoutExtension(Path.GetFileName(sFilename))
                    : dt.TableName.Replace("$", string.Empty)
                : sheetName;

            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(sFilename)))
            {
                ExcelWorksheet wksht = pkg.Workbook.Worksheets[sheetName];
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
                        if (row[j] != DBNull.Value)
                        {
                            var tmp = row[j] is TimeSpan timeSpan ? string.Format("{0:hh\\:mm\\:ss}", timeSpan) : row[j].ToString().Replace(Environment.NewLine, "\r\n");

                            wksht.Cells[recordIndex + 2, j + 1].Value = tmp;
                        }
                    }
                }

                pkg.Save();
            }
        }

        /// <summary>
        /// Adds a data column to an Excel worksheet.
        /// </summary>
        /// <param name="sFile">The Excel file name.</param>
        /// <param name="dt">The DataTable.</param>
        /// <param name="sheetName">The sheet name.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="dataType">The data type of the field.</param>
        public static void AddDataColumnXLS(string sFile, DataTable dt, string sheetName, string fieldName, Type dataType)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                dt.Columns.Add(fieldName, dataType);
                UpdateWkshtXLS(sFile, dt, sheetName);
            }
        }

        /// <summary>
        /// Maps events to update the Excel worksheet when rows are changed or deleted in the DataTable.
        /// </summary>
        /// <param name="dt">The DataTable.</param>
        /// <param name="oFile">The Excel file.</param>
        /// <param name="sheetName">The sheet name.</param>
        public static void UpdateXLS_MapEvents(DataTable dt, FileInfo oFile, string sheetName)
        {
            dt.RowChanged += (sender, e) => UpdateXLS_HandleRowUpsert(sender, e, oFile, sheetName);
            dt.RowDeleted += (sender, e) => UpdateXLS_HandleRowDeleted(sender, e, oFile, sheetName);
        }

        #endregion

        #region CSV File Support

        /// <summary>
        /// Exports a DataSet to a directory of CSV files.
        /// </summary>
        /// <param name="oDir">The directory path.</param>
        /// <param name="ds">The DataSet to export.</param>
        public static void ExportToCSV(DirectoryInfo oDir, DataSet ds)
        {
            if (!oDir.Exists)
                oDir.Create();

            foreach (DataTable dt in ds.Tables)
            {
                FileInfo oFile = new FileInfo(Path.GetFullPath(Path.Combine(oDir.FullName, $"{dt.TableName.Replace("$", string.Empty)}.csv")));
                CreateCSV(dt, oFile);
            }
        }

        /// <summary>
        /// Creates a CSV file from a DataTable.
        /// </summary>
        /// <param name="dt">The DataTable.</param>
        /// <param name="oFile">The CSV file.</param>
        public static void CreateCSV(DataTable dt, FileInfo oFile)
        {
            using (StreamWriter sw = new StreamWriter(oFile.FullName))
            {
                StringBuilder sb = new StringBuilder();

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

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string tmp = row[i].ToString().Replace(Environment.NewLine, "\r\n");

                        if (i < dt.Columns.Count - 1)
                        {
                            sb.Append(tmp.Contains(',') ? $"\"{tmp}\"," : $"{tmp},");
                        }
                        else
                        {
                            sb.Append(tmp.Contains(',') ? $"\"{tmp}\"" : tmp);
                        }
                    }

                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                }
            }
        }

        /// <summary>
        /// Adds a data column to a CSV file.
        /// </summary>
        /// <param name="oFile">The CSV file.</param>
        /// <param name="dt">The DataTable.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="dataType">The data type of the field.</param>
        public static void AddDataColumnCSV(FileInfo oFile, DataTable dt, string fieldName, Type dataType)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                dt.Columns.Add(fieldName, dataType);
                CreateCSV(dt, oFile);
            }
        }

        /// <summary>
        /// Maps events to update the CSV file when rows are changed or deleted in the DataTable.
        /// </summary>
        /// <param name="dt">The DataTable.</param>
        /// <param name="oFile">The CSV file.</param>
        public static void UpdateCSV_MapEvents(DataTable dt, FileInfo oFile, bool blnAppend = false)
        {
            dt.RowChanged += (sender, e) => UpdateCSV_HandleRowUpsert(sender, e, oFile, blnAppend);
            dt.RowDeleted += (sender, e) => UpdateCSV_HandleRowDeleted(sender, e, oFile);
        }

        #endregion

        #region XML File Support

        /// <summary>
        /// Exports a DataSet to an XML string.
        /// </summary>
        /// <param name="ds">The DataSet to export.</param>
        /// <returns>An XML string representing the DataSet.</returns>
        public static string ExportToXml(DataSet ds)
        {
            return ds.SerializeToXML();
        }

        /// <summary>
        /// Exports a DataTable to an XML string.
        /// </summary>
        /// <param name="dt">The DataTable to export.</param>
        /// <returns>An XML string representing the DataTable.</returns>
        public static string ExportToXml(DataTable dt)
        {
            return dt.SerializeToXML();
        }

        #endregion

        #region JSON File Support

        /// <summary>
        /// Exports a DataSet to a JSON string.
        /// </summary>
        /// <param name="ds">The DataSet to export.</param>
        /// <returns>A JSON string representing the DataSet.</returns>
        public static string ExportToJSON(DataSet ds)
        {
            return ds.SerializeToJSON();
        }

        /// <summary>
        /// Exports a DataTable to a JSON string.
        /// </summary>
        /// <param name="dt">The DataTable to export.</param>
        /// <returns>A JSON string representing the DataTable.</returns>
        public static string ExportToJSON(DataTable dt)
        {
            return dt.SerializeToJSON();
        }

        #endregion

        #endregion

        #region Private Functions

        private static string Normalize(object o, bool isCSV = false, bool isSQL = false)
        {
            if (o == null || o == DBNull.Value)
                return isSQL ? "NULL" : "";

            string value = o.ToString();

            if (isCSV)
            {
                if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
                {
                    value = value.Replace("\"", "\"\"");
                    value = $"\"{value}\"";
                }
            }

            if (isSQL)
            {
                if (o is string || o is DateTime)
                {
                    value = $"'{value.Replace("'", "''")}'";
                }
                else if (o is bool v)
                {
                    value = v ? "1" : "0";
                }
                else if (o is DateTime time)
                {
                    value = $"CONVERT(datetime, '{time:yyyy-MM-dd HH:mm:ss}', 121)";
                }
                else if (string.IsNullOrEmpty(value))
                {
                    value = "NULL";
                }
            }

            return value;
        }

        private static string SerializeToJSON(this object item)
        {
            return JsonConvert.SerializeObject(item);
        }

        private static string BuildCsvRow(DataRow row)
        {
            return string.Join(",", row.ItemArray.Select(field => Normalize(field, isCSV: true)).ToArray());
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

        private static void UpdateCSV_HandleRowUpsert(object sender, DataRowChangeEventArgs e, FileInfo oFile, bool blnAppendToFile = false)
        {
            DataTable dt = (DataTable)sender;
            int iRowIndex = dt.Rows.IndexOf(e.Row);
            int lineNumber = (iRowIndex + 1); 
            long targetLineBytePosition = 0;
            string rowCsv = BuildCsvRow(e.Row);

            if (e.Action == DataRowAction.Add)
            {
                if (blnAppendToFile)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(oFile.FullName, FileMode.Append, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            fs.WriteLine(rowCsv, _enc);
                        }
                    }
                    catch { }
                }
                else
                {
                    using (FileStream fs = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        for (int i = 0; i < lineNumber; i++)
                        {
                            targetLineBytePosition += _enc.GetByteCount(fs.ReadLine() + Environment.NewLine);
                        }

                        fs.Seek(targetLineBytePosition, SeekOrigin.Begin);
                        fs.WriteLine(rowCsv, _enc);

                        for (int j = (lineNumber); j < dt.Rows.Count; j++)
                        {
                            fs.WriteLine(BuildCsvRow(dt.Rows[j]), _enc);
                        }
                    }
                }
            }
            else if (e.Action == DataRowAction.Change)
            {
                using (FileStream fs = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    for (int i = 0; i < lineNumber; i++)
                    {
                        targetLineBytePosition += _enc.GetByteCount(fs.ReadLine() + Environment.NewLine);
                    }

                    fs.Seek(targetLineBytePosition, SeekOrigin.Begin);
                    fs.WriteLine(rowCsv, _enc);

                    for (int j = lineNumber; j < dt.Rows.Count; j++)
                    {
                        fs.WriteLine(BuildCsvRow(dt.Rows[j]), _enc);
                    }
                }
            }
        }

        private static void UpdateCSV_HandleRowDeleted(object sender, DataRowChangeEventArgs e, FileInfo oFile)
        {
            if (e.Action == DataRowAction.Delete)
            {
                DataTable dt = (DataTable)sender;
                int lineNumber = dt.Rows.IndexOf(e.Row) + 1;
                long targetLineBytePosition = 0;

                using (FileStream fs = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    for (int i = 0; i < lineNumber; i++)
                    {
                        targetLineBytePosition += _enc.GetByteCount(fs.ReadLine() + Environment.NewLine);
                    }

                    fs.Seek(targetLineBytePosition, SeekOrigin.Begin);

                    for (int j = lineNumber; j < dt.Rows.Count; j++)
                    {
                        fs.WriteLine(BuildCsvRow(dt.Rows[j]), _enc);
                    }
                }
            }
        }

        private static void UpdateXLS_HandleRowUpsert(object sender, DataRowChangeEventArgs e, FileInfo oFile, string sheetName, bool hasHeader = true)
        {
            DataTable dt = (DataTable)sender;
            int offset = hasHeader ? 1 : 0;
            int recordIndex = dt.Rows.IndexOf(e.Row) + offset + 1;

            using (ExcelPackage pkg = new ExcelPackage(oFile))
            {
                ExcelWorksheet wksht = pkg.Workbook.Worksheets[sheetName];

                if (e.Row.RowState == DataRowState.Added)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        wksht.Cells[recordIndex, i + 1].Value = Normalize(e.Row[i].ToString());
                    }

                    for (int j = recordIndex + 1; j < dt.Rows.Count; j++)
                    {
                        DataRow row = dt.Rows[j];
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            wksht.Cells[j + offset + 1, i + 1].Value = Normalize(row[i].ToString());
                        }
                    }
                }
                else if (e.Action == DataRowAction.Change)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        wksht.Cells[recordIndex, i + 1].Value = Normalize(e.Row[i].ToString());
                    }
                }

                pkg.Save();
            }
        }

        private static void UpdateXLS_HandleRowDeleted(object sender, DataRowChangeEventArgs e, FileInfo oFile, string sheetName, bool hasHeader = true)
        {
            DataTable dt = (DataTable)sender;
            int recordIndex = dt.Rows.IndexOf(e.Row);
            int offset = hasHeader ? 1 : 0;

            if (e.Action == DataRowAction.Delete)
            {
                using (ExcelPackage pkg = new ExcelPackage(oFile))
                {
                    ExcelWorksheet wksht = pkg.Workbook.Worksheets[sheetName];
                    int lineNumber = hasHeader ? (recordIndex + 1) : recordIndex;

                    for (int j = lineNumber; j < dt.Rows.Count; j++)
                    {
                        DataRow row = dt.Rows[j];
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            wksht.Cells[recordIndex + offset + 1, i + 1].Value = Normalize(row[i].ToString());
                        }
                    }

                    pkg.Save();
                }
            }
        }

        private static OleDbConnection CreateMDBfromFile(string connString, MDBType mDBType)
        {
            OleDbConnection con = new OleDbConnection(connString);
            string fileName = Path.GetFullPath(con.DataSource);

            if (File.Exists(fileName))
                File.Delete(fileName);

            // Extract the embedded file.
            try
            {
                Assembly assembly = typeof(DataWriteLayer).Assembly;
                using (Stream resource = assembly.GetManifestResourceStream(Enum.GetName(typeof(MDBType), mDBType)))
                using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (resource == null)
                        throw new FileNotFoundException($"Could not find resource in {assembly.FullName}!");

                    resource.CopyTo(file);
                }

                // Make the file RW
                FileAttributes attributes = File.GetAttributes(fileName);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(fileName, attributes);
                }
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
            ADOX.Table result = new ADOX.Table
            {
                Name = new string(inTable.TableName.Where(c => !char.IsPunctuation(c)).ToArray()).Replace("$", string.Empty)
            };

            foreach (DataColumn inColumn in inTable.Columns)
            {
                ADOX.Column col = new ADOX.Column
                {
                    Name = inColumn.ColumnName,
                    Type = ResolveADOXType(inColumn)
                };

                if (col.Type == ADOX.DataTypeEnum.adVarWChar || col.Type == ADOX.DataTypeEnum.adWChar)
                {
                    col.DefinedSize = inColumn.MaxLength > 0 ? inColumn.MaxLength : 255;
                }

                if (inColumn.AllowDBNull)
                    col.Attributes = ADOX.ColumnAttributesEnum.adColNullable;

                result.Columns.Append(col);
            }

            return result;
        }

        private static void AddDataTableToMDBwithADOX(ADOX.Catalog cat, DataTable inTable)
        {
            ADODB.Recordset rs = new ADODB.Recordset
            {
                CursorLocation = ADODB.CursorLocationEnum.adUseClient
            };

            try
            {
                rs.Open($"SELECT * FROM [{new string(inTable.TableName.Where(c => !char.IsPunctuation(c)).ToArray()).Replace("$", string.Empty)}]",
                    cat.ActiveConnection,
                    ADODB.CursorTypeEnum.adOpenDynamic,
                    ADODB.LockTypeEnum.adLockOptimistic);

                if (!rs.BOF || !rs.EOF)
                    rs.MoveFirst();

                foreach (DataRow dr in inTable.Rows)
                {
                    rs.AddNew();
                    for (int columnIndex = 0; columnIndex < inTable.Columns.Count; columnIndex++)
                    {
                        rs.Fields[columnIndex].Value = dr[columnIndex];
                    }
                    rs.Update();
                }
            }
            finally
            {
                rs?.Close();
            }
        }

        private static void AddDataTableToMDB(DataTable inTable, OleDbConnection conn)
        {
            conn.Open();

            using (OleDbCommand cmd = new OleDbCommand())
            {
                StringBuilder sqlStmt = new StringBuilder();
                sqlStmt.Append($"CREATE TABLE [{inTable.TableName}] (");
                foreach (DataColumn inColumn in inTable.Columns)
                {
                    sqlStmt.Append($"[{inColumn.ColumnName}] {ResolveOleDbType(inColumn)},");
                }

                // remove last comma
                sqlStmt.Length--;
                sqlStmt.Append(");");

                cmd.CommandText = sqlStmt.ToString();
                cmd.ExecuteNonQuery();
            }

            using (OleDbDataAdapter oledbDataAdapter = new OleDbDataAdapter($"SELECT * FROM [{inTable.TableName}];", conn))
            {
                OleDbCommandBuilder oledbCmdBuilder = new OleDbCommandBuilder(oledbDataAdapter)
                {
                    DataAdapter = oledbDataAdapter
                };

                using (DataTable accDataTable = inTable.Copy())
                {
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
            switch (Type.GetTypeCode(column.DataType))
            {
                case TypeCode.String:
                    return column.MaxLength > MAXNVARCHAR ? OleDbType.LongVarWChar : OleDbType.VarWChar;
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
                    if (column.DataType == typeof(Guid))
                        return OleDbType.Guid;
                    if (column.DataType == typeof(byte[]))
                        return OleDbType.VarBinary;
                    if (column.DataType == typeof(TimeSpan))
                        return OleDbType.DBTime;
                    if (column.DataType == typeof(DateTimeOffset))
                        return OleDbType.DBTimeStamp;
                    return OleDbType.Variant;
                default:
                    return OleDbType.Variant;
            }
        }

        private static ADOX.DataTypeEnum ResolveADOXType(DataColumn column)
        {
            switch (Type.GetTypeCode(column.DataType))
            {
                case TypeCode.String:
                    return column.MaxLength > MAXNVARCHAR ? ADOX.DataTypeEnum.adLongVarWChar : ADOX.DataTypeEnum.adVarWChar;
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
                    if (column.DataType == typeof(Guid))
                        return ADOX.DataTypeEnum.adGUID;
                    if (column.DataType == typeof(byte[]))
                        return ADOX.DataTypeEnum.adBinary;
                    if (column.DataType == typeof(TimeSpan))
                        return ADOX.DataTypeEnum.adDBTime;
                    if (column.DataType == typeof(DateTimeOffset))
                        return ADOX.DataTypeEnum.adDBTimeStamp;
                    return ADOX.DataTypeEnum.adVariant;
                default:
                    return ADOX.DataTypeEnum.adVariant;
            }
        }

        private static void UpdateMsSql_HandleRowUpsert(object sender, DataRowChangeEventArgs e, SqlConnection sqlConn)
        {
            DataTable dt = (DataTable)sender;

            try
            {
                sqlConn.Open();
                string tableName = dt.TableName;

                if (e.Row.RowState == DataRowState.Added)
                {
                    string columns = string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    string values = string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => $"@{c.ColumnName}"));
                    string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                    using (SqlCommand command = new SqlCommand(query, sqlConn))
                    {
                        foreach (DataColumn column in dt.Columns)
                        {
                            command.Parameters.AddWithValue($"@{column.ColumnName}", e.Row[column]);
                        }

                        command.ExecuteNonQuery();
                    }
                }
                else if (e.Action == DataRowAction.Change)
                {
                    string setClause = string.Join(",", dt.Columns.Cast<DataColumn>()
                        .Where(c => e.Row[c, DataRowVersion.Current] != e.Row[c, DataRowVersion.Original])
                        .Select(c => $"{c.ColumnName} = @{c.ColumnName}"));

                    string whereClause = HasPrimaryKey(dt)
                        ? string.Join(" AND ", dt.PrimaryKey.Select(c => $"{c.ColumnName} = @{c.ColumnName}"))
                        : string.Join(" AND ", dt.Columns.Cast<DataColumn>().Select(c => $"{c.ColumnName} = @{c.ColumnName}_old"));

                    string query = $"UPDATE [{tableName}] SET {setClause} WHERE {whereClause};";

                    using (SqlCommand command = new SqlCommand(query, sqlConn))
                    {
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (!HasPrimaryKey(dt))
                                command.Parameters.AddWithValue($"@{column.ColumnName}_old", e.Row[column, DataRowVersion.Original]);

                            if (e.Row[column, DataRowVersion.Current] != e.Row[column, DataRowVersion.Original])
                                command.Parameters.AddWithValue($"@{column.ColumnName}", e.Row[column, DataRowVersion.Current]);
                        }

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating SQL Server table: {ex.Message}");
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private static void UpdateMsSql_HandleRowDeleted(object sender, DataRowChangeEventArgs e, SqlConnection sqlConn)
        {
            DataTable dt = (DataTable)sender;

            if (e.Action == DataRowAction.Delete)
            {
                try
                {
                    sqlConn.Open();
                    string tableName = dt.TableName;
                    string whereClause = string.Join(" AND ", dt.PrimaryKey.Select(c => $"{c.ColumnName} = @{c.ColumnName}"));
                    string query = $"DELETE FROM {tableName} WHERE {whereClause}";

                    using (SqlCommand command = new SqlCommand(query, sqlConn))
                    {
                        foreach (DataColumn column in dt.PrimaryKey)
                        {
                            command.Parameters.AddWithValue($"@{column.ColumnName}", e.Row[column, DataRowVersion.Original]);
                        }

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting row from SQL Server table: {ex.Message}");
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        private static bool HasPrimaryKey(DataTable dataTable)
        {
            return dataTable.PrimaryKey != null && dataTable.PrimaryKey.Length > 0;
        }

        private static int GetDefaultLength(Type dataType)
        {
            switch (dataType.Name)
            {
                case "String": return 255;
                case "Byte[]": return 100;
                case "DateTime": return 8;
                case "DateTimeOffset": return 10;
                case "TimeSpan": return 8;
                case "Guid": return 16;
                default: return -1;
            }
        }

        private static SqlDbType ResolveSqlDbType(DataColumn column)
        {
            switch (column.DataType.Name)
            {
                case "Byte": return SqlDbType.TinyInt;
                case "Int16": return SqlDbType.SmallInt;
                case "Int32": return SqlDbType.Int;
                case "Int64": return SqlDbType.BigInt;
                case "Single": return SqlDbType.Real;
                case "Double": return SqlDbType.Float;
                case "Decimal": return SqlDbType.Decimal;
                case "DateTime": return SqlDbType.DateTime;
                case "String": return column.MaxLength <= MAXNVARCHAR ? SqlDbType.NVarChar : SqlDbType.NText;
                case "Boolean": return SqlDbType.Bit;
                case "Guid": return SqlDbType.UniqueIdentifier;
                case "Byte[]": return SqlDbType.VarBinary;
                default: return SqlDbType.Variant;
            }
        }

        #endregion
    }
}
