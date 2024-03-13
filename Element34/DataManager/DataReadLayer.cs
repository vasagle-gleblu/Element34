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
        #region [Fields]
        static readonly Encoding enc = Encoding.Default;
        static readonly CultureInfo culture = CultureInfo.CurrentCulture;
        #endregion

        #region [Public Functions]
        #region [MS-SQL Server Support]
        public static DataSet DataSetFromMsSql(SqlConnection sqlConn, string sqlQuery, Dictionary<string, object> paramList = null)
        {
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

        public static DataTable DataTableFromMsSql(SqlConnection sqlConn, string sqlQuery, Dictionary<string, object> paramList = null)
        {
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

        private static string ParameterValueForSQL(SqlParameter sp)
        {
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
            return ToBooleanOrDefault((string)o, Default);
        }

        private static bool ToBooleanOrDefault(string s, bool defaultValue)
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
        public static DataSet DataSetFromMDB(string connString)
        {
            DataSet result = new DataSet();

            // For convenience, the DataSet is identified by the name of the loaded file (without extension).
            string fileName = (new OleDbConnection(connString).DataSource);
            result.DataSetName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");

            // Opening the Access connection
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                // Getting all user tables present in the Access file (Msys* tables are system thus useless for this scenario)
                DataTable dt = conn.GetSchema("Tables");
                List<string> tableNames = dt.AsEnumerable().Select(dr => dr.Field<string>("TABLE_NAME")).Where(dr => !dr.StartsWith("MSys")).ToList();

                // Getting the data for every user tables
                foreach (string tableName in tableNames)
                {
                    using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", tableName), conn))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            // Saving all tables in our result DataSet.
                            DataTable buf = new DataTable("[" + tableName + "]");
                            adapter.Fill(buf);
                            result.Tables.Add(buf);
                        }
                    }
                }
                conn.Close();
            }

            // Return the filled DataSet
            return result;
        }

        public static DataTable DataTableFromMDB(string connString, string sqlStmt, Dictionary<string, object> paramList = null)
        {
            DataTable result = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                using (OleDbCommand cmd = new OleDbCommand(sqlStmt, conn))
                {
                    if (paramList != null)
                    {
                        cmd.Parameters.Clear();
                        foreach (KeyValuePair<string, object> param in paramList)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    conn.Open();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(result);
                    }
                    conn.Close();
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
                ExcelWorksheet worksheet = (string.IsNullOrEmpty(sSheetName)) ? package.Workbook.Worksheets[0] : package.Workbook.Worksheets[sSheetName];
                if (worksheet == null)
                    throw new Exception("Worksheet not found");

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
            DataTable dt = new DataTable();
            dt.TableName = Path.GetFileNameWithoutExtension(Path.GetFileName(sFile));

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

        private static T DeserializefromXML<T>(this string sInput)
        {
            T result = default(T);

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (StringReader strReader = new StringReader(sInput))
            using (XmlReader xmlReader = XmlReader.Create(strReader))
            {
                result = (T)serializer.ReadObject(xmlReader);
            }
            serializer = null;

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

        private static T DeserializeFromJSON<T>(this string sInput)
        {
            return JsonConvert.DeserializeObject<T>(sInput);
        }
        #endregion
        #endregion
    }
}

