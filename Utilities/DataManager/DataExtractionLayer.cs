using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Element34.Utilities.DataManager
{
    public static class DataExtractionLayer
    {
        #region [Public Functions]
        public static DataTable DataTableFromMsSql(string sqlConn, string sqlQuery, Dictionary<string, object> paramList = null)
        {
            DataTable result = new DataTable();
            using (SqlConnection connection = new SqlConnection(sqlConn))
            {
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    if (paramList != null)
                    {
                        command.Parameters.Clear();
                        foreach (var param in paramList)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        connection.Open();
                        sda.Fill(result);
                        connection.Close();
                    }
                }
            }

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
                    conn.Close();
                }
            }

            return result;
        }

        public static DataSet DataSetFromMDB(string connString)
        {
            DataSet result = new DataSet();

            // For convenience, the DataSet is identified by the name of the loaded file (without extension).
            string fileName = (new SqlConnectionStringBuilder(connString).DataSource);
            result.DataSetName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");

            // Opening the Access connection
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                // Getting all user tables present in the Access file (Msys* tables are system thus useless for us)
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

        public static DataTable DataTableFromXLS(string connString, string sqlStmt, Dictionary<string, object> paramList = null)
        {
            DataTable result;
            if (sqlStmt.Contains("FROM[") && sqlStmt.Contains("$]"))
            {
                int start = sqlStmt.IndexOf("FROM [") + "FROM [".Length;
                int end = sqlStmt.IndexOf("$]", start) + "$]".Length;
                string tableName = sqlStmt.Substring(start, end - start);
                result = new DataTable(tableName);
            }
            else
                result = new DataTable();

            if (!connString.Contains("Extended Properties"))
                connString += "Extended Properties = 'Excel 12.0 Xml;IMEX=1;";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                using (OleDbCommand cmd = new OleDbCommand(sqlStmt, conn))
                {
                    if (paramList != null)
                    {
                        cmd.Parameters.Clear();
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
                    conn.Close();
                }
            }

            return result;
        }

        public static DataSet DataSetFromXLS(string fileName)
        {
            DataSet result = new DataSet();

            // For convenience, the DataSet is identified by the name of the loaded file (without extension).
            result.DataSetName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");

            // Compute the ConnectionString (using the OLEDB v12.0 driver compatible with XLS and XLSX files)
            fileName = Path.GetFullPath(fileName);
            string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;IMEX=1'", fileName);

            // Opening the connection
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                // Getting all worksheets present in the Excel file
                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                List<string> tableNames = dt.AsEnumerable().Select(dr => dr.Field<string>("TABLE_NAME")).Where(dr => dr.ToString().Contains("$")).ToList();

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

        public static DataTable DataTableFromCSV(string sFilename, string sFilter = "")
        {
            string sFolder = Path.GetDirectoryName(sFilename);
            sFilename = sFilename.Substring(sFilename.LastIndexOf('\\') + 1, sFilename.Length - sFilename.LastIndexOf('\\') - 1);
            sFilename.Replace('[', '(').Replace(']', ')');
            DataTable dt;

            using (OdbcConnection conn = new OdbcConnection("Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + sFolder + ";Extensions=asc,csv,tab,txt;"))
            {
                using (OdbcCommand cmd = new OdbcCommand("SELECT * FROM [" + sFilename + "]", conn))
                {
                    conn.Open();
                    using (OdbcDataAdapter adapter = new OdbcDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        dt.TableName = Path.GetFileNameWithoutExtension(sFilename);
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }

                if (dt.Rows.Count > 0)
                {
                    if (sFilter.Length > 0)
                    {
                        DataRow[] dr = dt.Select(sFilter);
                        if (dr.Length > 0)
                        {
                            DataTable tmp = dr.CopyToDataTable();
                            dt = tmp;
                        }
                    }
                }
            }

            return dt;
        }

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
                string sFilename = sFile.Substring(sFile.LastIndexOf('\\') + 1, sFile.Length - sFile.LastIndexOf('\\') - 1);

                using (OdbcConnection conn = new OdbcConnection("Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + sFolder + ";Extensions=asc,csv,tab,txt;"))
                {
                    conn.Open();
                    OdbcDataAdapter objDA = new OdbcDataAdapter("SELECT * FROM [" + sFilename + "]", conn);
                    dt = new DataTable();
                    dt.TableName = sFilename;
                    objDA.Fill(dt);
                    objDA.Dispose();
                    conn.Close();
                }

                result.Tables.Add(dt);
            }


            // Return the filled DataSet
            return result;
        }

        public static DataTable DataTableFromJSON(string sInput, string sFilter = "")
        {
            DataTable dt = sInput.DeserializeDataTableFromJSON();

            if (dt.Rows.Count > 0)
            {
                if (sFilter.Length > 0)
                {
                    DataRow[] dr = dt.Select(sFilter);
                    if (dr.Length > 0)
                    {
                        DataTable tmp = dr.CopyToDataTable();
                        dt = tmp;
                    }
                }
            }

            return dt;
        }

        public static DataSet DataSetFromJSON(string sInput)
        {
            return sInput.DeserializeDataSetFromJSON();
        }
        #endregion

        #region [Private Functions]
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

        private static bool ToBooleanOrDefault(string s, bool Default)
        {
            return ToBooleanOrDefault((object)s, Default);
        }

        private static bool ToBooleanOrDefault(object o, bool Default)
        {
            bool ReturnVal = Default;
            try
            {
                if (o != null)
                {
                    switch (o.ToString().ToLower())
                    {
                        case "yes":
                        case "affirmative":
                        case "positive":
                        case "true":
                        case "ok":
                        case "y":
                            ReturnVal = true;
                            break;
                        case "no":
                        case "negative":
                        case "false":
                        case "n":
                            ReturnVal = false;
                            break;
                        default:
                            ReturnVal = bool.Parse(o.ToString());
                            break;
                    }
                }
            }
            catch
            {
            }
            return ReturnVal;
        }

        private static DataTable DeserializeDataTableFromJSON(this string sInput)
        {
            return JsonConvert.DeserializeObject<DataTable>(sInput);
        }

        private static DataSet DeserializeDataSetFromJSON(this string sInput)
        {
            return JsonConvert.DeserializeObject<DataSet>(sInput);
        }
        #endregion
    }
}

