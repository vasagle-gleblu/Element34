using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Element34.DataManager
{
    public static class DataWriteLayer
    {
        #region [Fields]
        static readonly Encoding enc = Encoding.Default;
        static readonly CultureInfo culture = CultureInfo.CurrentCulture;
        static readonly CsvConfiguration config = new CsvConfiguration(culture)
        {
            Delimiter = ",",
            Encoding = enc,
            Mode = CsvMode.RFC4180,
            ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace)
        };

        public enum MDBType
        {
            Access2000,
            Access2003,
            Access2007,
            Access2010,
            Access2016,
            Access2019
        }
        #endregion

        #region [Public Functions]
        #region [MS-SQL Server Support]
        #endregion

        #region [MS Access File Support]
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
                AddDataTableToMDB(cat, dt);
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
                //something, something
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
                using (Stream resource = assembly.GetManifestResourceStream(Enum.GetName(typeof(DataWriteLayer.MDBType), mDBType)))
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

            FileInfo newFile = new FileInfo(sFilename);
            DataRow row; int offset = 1;
            string tmp;

            if (sheetName == string.Empty)
                tmp = (dt.TableName == string.Empty) ? Path.GetFileNameWithoutExtension(Path.GetFileName(sFilename)) : dt.TableName.Replace("$", string.Empty);
            else
                tmp = sheetName;

            using (ExcelPackage pkg = new ExcelPackage(newFile))
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

        public static void AddDataColumnXLS(string sFile, DataTable dt, string sheetName, string fieldName, Type dataType)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                dt.Columns.Add(fieldName, dataType);

                // Update table layout
                CreateXLS(sFile, dt, sheetName);
            }
        }

        public static void UpdateXLS_MapEvents(DataTable dt, FileInfo oFile, string sheetName)
        {
            dt.RowChanged += (sender, e) => UpdateXLS_RowChanged(sender, e, oFile, sheetName);
            dt.RowDeleted += (sender, e) => UpdateXLS_RowDeleted(sender, e, oFile, sheetName);
        }

        public static void UpdateXLS_RowDeleted(object sender, DataRowChangeEventArgs e, FileInfo oFile, string sheetName, bool hasHeader = true)
        {
            if (e.Action == DataRowAction.Delete)
            {
                DataTable dt = (DataTable)sender;
                int recordIndex = dt.Rows.IndexOf(e.Row);
                string tmp; int offset = (hasHeader) ? 1 : 0;

                // Open file
                using (ExcelPackage pkg = new ExcelPackage(oFile))
                using (ExcelWorkbook wkbk = pkg.Workbook)
                using (ExcelWorksheet wksht = wkbk.Worksheets[sheetName])
                {
                    // Build updated row
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        // Cell values
                        tmp = e.Row[i].ToString();
                        tmp = tmp.Replace(Environment.NewLine, "\r\n");  // OS dependent

                        wksht.Cells[recordIndex + offset + 1, i + 1].Value = tmp;
                    }

                    pkg.Save();
                }
            }
        }

        public static void UpdateXLS_RowChanged(object sender, DataRowChangeEventArgs e, FileInfo oFile, string sheetName, bool hasHeader = true)
        {
            if ((e.Action == DataRowAction.Change) || (e.Row.RowState == DataRowState.Added))
            {
                DataTable dt = (DataTable)sender;
                int recordIndex = dt.Rows.IndexOf(e.Row);
                string tmp; int offset = (hasHeader) ? 1 : 0;
                DataRow row;

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
                            tmp = row[i].ToString();
                            tmp = tmp.Replace(Environment.NewLine, "\r\n");  // OS dependent

                            wksht.Cells[recordIndex + offset + 1, i + 1].Value = tmp;
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

            string sFile;

            foreach (DataTable dt in ds.Tables)
            {
                sFile = Path.GetFullPath(Path.Combine(oDir.FullName, string.Format("{0}.csv", dt.TableName.Replace("$", string.Empty))));
                CreateCSV(sFile, dt);
            }
        }

        public static void CreateCSV(string sFile, DataTable dt)
        {
            using (StreamWriter textWriter = File.CreateText(sFile))
            using (CsvWriter csvWriter = new CsvWriter(textWriter, culture))
            {
                // Write columns names
                foreach (DataColumn column in dt.Columns)
                {
                    csvWriter.WriteField(column.ColumnName);
                }
                csvWriter.NextRecord();

                // Write out rows
                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        csvWriter.WriteField(row[i]);
                    }
                    csvWriter.NextRecord();
                }

                textWriter.Flush();
            }
        }

        public static void AddDataColumnCSV(string sFile, DataTable dt, string fieldName, Type dataType)
        {
            if (!dt.Columns.Contains(fieldName))
            {
                dt.Columns.Add(fieldName, dataType);

                // Update table layout
                CreateCSV(sFile, dt);
            }
        }

        public static void UpdateCSV_MapEvents(DataTable dt, FileInfo oFile)
        {
            dt.RowChanged += (sender, e) => UpdateCSV_RowChanged(sender, e, oFile);
            dt.RowDeleted += (sender, e) => UpdateCSV_RowDeleted(sender, e, oFile);
        }

        public static void UpdateCSV_RowDeleted(object sender, DataRowChangeEventArgs e, FileInfo oFile)
        {
            if (e.Action == DataRowAction.Delete)
            {
                DataTable dt = (DataTable)sender;
                DataRow row; string tmp;
                long iSeekAhead = 0;
                int iRowIndex = dt.Rows.IndexOf(e.Row);
                StringBuilder sb = new StringBuilder();

                // Open file
                using (FileStream fsFile = new FileStream(oFile.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                using (StreamReader sr = new StreamReader(fsFile, enc))
                using (StreamWriter sw = new StreamWriter(fsFile, enc))
                using (CsvReader csvReader = new CsvReader(sr, culture))
                using (CsvDataReader csvDataReader = new CsvDataReader(csvReader))
                using (CsvWriter csvWriter = new CsvWriter(sw, culture))
                {
                    // Calculate how far to skip ahead to deleted row
                    int lineNumber = (csvWriter.Configuration.HasHeaderRecord) ? (iRowIndex + 1) : iRowIndex;

                    // Set current position to the beginning
                    fsFile.Position = 0;

                    // Calculate how far to skip ahead to modified row
                    for (int j = 0; j < lineNumber; j++)
                    {
                        iSeekAhead += enc.GetByteCount(sr.ReadLine() + Environment.NewLine);
                    }

                    //  Overwrite deleted row and write out remaining rows
                    for (int j = lineNumber; j < dt.Rows.Count; j++)
                    {
                        row = dt.Rows[j];
                        sb.Clear();

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            // Cell values
                            tmp = e.Row[i].ToString();
                            tmp = tmp.Replace(Environment.NewLine, "\r\n");

                            if (i < (dt.Columns.Count - 1))
                            {
                                if (!tmp.Contains(','))
                                    sb.Append(tmp + csvWriter.Configuration.Delimiter);
                                else
                                    sb.AppendFormat("\"{0}\"", tmp + csvWriter.Configuration.Delimiter);
                            }
                            else
                            {
                                if (!tmp.Contains(','))
                                    sb.Append(tmp);
                                else
                                    sb.AppendFormat("\"{0}\"", tmp);
                            }
                        }

                        fsFile.Seek(iSeekAhead, SeekOrigin.Begin);
                        sw.WriteLine(sb.ToString());

                        // Calculate how far to move the current position for the next write.
                        iSeekAhead = enc.GetByteCount(sr.ReadLine() + Environment.NewLine);
                    }
                }
            }
        }

        public static void UpdateCSV_RowChanged(object sender, DataRowChangeEventArgs e, FileInfo oFile)
        {
            if ((e.Action == DataRowAction.Change) || (e.Row.RowState == DataRowState.Added))
            {
                DataTable dt = (DataTable)sender;
                int iRowIndex = dt.Rows.IndexOf(e.Row);
                long iSeekAhead = 0;
                StringBuilder sb = new StringBuilder();
                string tmp;

                // Open file
                using (FileStream fsFile = new FileStream(oFile.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                using (StreamReader sr = new StreamReader(fsFile, enc))
                using (StreamWriter sw = new StreamWriter(fsFile, enc))
                using (CsvReader csvReader = new CsvReader(sr, culture))
                using (CsvDataReader csvDataReader = new CsvDataReader(csvReader))
                using (CsvWriter csvWriter = new CsvWriter(sw, culture))
                {
                    int lineNumber = (csvWriter.Configuration.HasHeaderRecord) ? (iRowIndex + 1) : iRowIndex;

                    // Set current position to the beginning
                    fsFile.Position = 0;

                    // Calculate how far to skip ahead to modified row
                    for (int j = 0; j < lineNumber; j++)
                    {
                        iSeekAhead += enc.GetByteCount(sr.ReadLine() + Environment.NewLine);
                    }

                    // Build updated row
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        // Cell values
                        tmp = e.Row[i].ToString();
                        tmp = tmp.Replace(Environment.NewLine, "\r\n");  // OS dependent

                        if (i < (dt.Columns.Count - 1))
                        {
                            if (!tmp.Contains(','))
                                sb.Append(tmp + csvWriter.Configuration.Delimiter);
                            else
                                sb.AppendFormat("\"{0}\"", tmp + csvWriter.Configuration.Delimiter);
                        }
                        else
                        {
                            if (!tmp.Contains(','))
                                sb.Append(tmp);
                            else
                                sb.AppendFormat("\"{0}\"", tmp);
                        }
                    }

                    fsFile.Seek(iSeekAhead, SeekOrigin.Begin);
                    sw.WriteLine(sb.ToString());
                }
            }
        }
        #endregion

        #region [XML File Support]
        public static string ExportToXml(DataSet ds)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(DataSet));
                    xmlSerializer.Serialize(streamWriter, ds);
                    return enc.GetString(memoryStream.ToArray());
                }
            }
        }

        public static string ExportToXml(DataTable dt)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(DataTable));
                    xmlSerializer.Serialize(streamWriter, dt);
                    return enc.GetString(memoryStream.ToArray());
                }
            }
        }
        #endregion

        #region [JSON File Support]
        public static string ExportToJSON(DataSet ds)
        {
            return ds.SerializeDataSetToJSON();
        }

        public static string ExportToJSON(DataTable dt)
        {
            return dt.SerializeDataTableToJSON();
        }
        #endregion
        #endregion

        #region [Private Functions]
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

        private static string InsertSQLDataTable(DataTable inTable)
        {
            StringBuilder sbInsertQuery = new StringBuilder();
            foreach (DataRow row in inTable.Rows)
            {
                sbInsertQuery.AppendFormat("INSERT INTO {0} (", inTable.TableName);

                // Generate comma separated list of column names
                sbInsertQuery.Append(string.Join(", ", inTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList()));

                sbInsertQuery.Append(") VALUES (");

                // Build up properly formatted list of values
                foreach (DataColumn column in inTable.Columns)
                {
                    sbInsertQuery.Append("?, ");
                }

                sbInsertQuery.Append(");\r\n");
            }

            return sbInsertQuery.ToString();
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
                col.Type = TranslateToADOXType(inColumn);

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

        private static ADOX.DataTypeEnum TranslateToADOXType(DataColumn column)
        {
            switch (column.DataType.Name)
            {
                case nameof(System.Guid):
                    return ADOX.DataTypeEnum.adGUID;

                case nameof(System.Boolean):
                    return ADOX.DataTypeEnum.adBoolean;

                case nameof(System.Byte):
                    return ADOX.DataTypeEnum.adUnsignedTinyInt;

                case nameof(System.Char):
                    return ADOX.DataTypeEnum.adWChar;

                case nameof(System.DateTime):
                    return ADOX.DataTypeEnum.adDate;

                case nameof(System.Decimal):                    
                    return ADOX.DataTypeEnum.adDecimal;

                case nameof(System.Double):
                    return ADOX.DataTypeEnum.adDouble;

                case nameof(System.Int16):
                    return ADOX.DataTypeEnum.adSmallInt;

                case nameof(System.Int32):
                    return ADOX.DataTypeEnum.adInteger;

                case nameof(System.Int64):
                    return ADOX.DataTypeEnum.adBigInt;

                case nameof(System.SByte):
                    return ADOX.DataTypeEnum.adTinyInt;

                case nameof(System.Single):
                    return ADOX.DataTypeEnum.adSingle;

                case nameof(System.String):
                    if (column.MaxLength > 4000)
                        return ADOX.DataTypeEnum.adLongVarWChar;
                    else
                        return ADOX.DataTypeEnum.adVarWChar;

                case nameof(System.Object):
                    return ADOX.DataTypeEnum.adLongVarBinary;

                case nameof(System.TimeSpan):
                    return ADOX.DataTypeEnum.adBigInt;

                case nameof(System.UInt16):
                    return ADOX.DataTypeEnum.adUnsignedSmallInt;

                case nameof(System.UInt32):
                    return ADOX.DataTypeEnum.adUnsignedInt;

                case nameof(System.UInt64):
                    return ADOX.DataTypeEnum.adUnsignedBigInt;
            }

            return ADOX.DataTypeEnum.adVariant;
        }

        private static void AddDataTableToMDB(ADOX.Catalog cat, DataTable inTable)
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
                dataType = TranslateToADODBType(inColumn);
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

        private static ADODB.DataTypeEnum TranslateToADODBType(DataColumn column)
        {
            switch (column.DataType.Name.ToLower())
            {
                case "guid":
                    return ADODB.DataTypeEnum.adVariant;

                case "boolean":
                    return ADODB.DataTypeEnum.adBoolean;

                case "bool":
                    return ADODB.DataTypeEnum.adBoolean;

                case "byte":
                    return ADODB.DataTypeEnum.adUnsignedTinyInt;

                case "char":
                    return ADODB.DataTypeEnum.adWChar;

                case "datetime":
                    return ADODB.DataTypeEnum.adDate;

                case "decimal":
                    return ADODB.DataTypeEnum.adDecimal;

                case "double":
                    return ADODB.DataTypeEnum.adDouble;

                case "int16":
                    return ADODB.DataTypeEnum.adSmallInt;

                case "int32":
                    return ADODB.DataTypeEnum.adInteger;

                case "int64":
                    return ADODB.DataTypeEnum.adBigInt;

                case "sbyte":
                    return ADODB.DataTypeEnum.adTinyInt;

                case "single":
                    return ADODB.DataTypeEnum.adSingle;

                case "string":
                    if (column.MaxLength > 4000)
                        return ADODB.DataTypeEnum.adLongVarWChar;
                    else
                        return ADODB.DataTypeEnum.adVarWChar;

                case "timespan":
                    return ADODB.DataTypeEnum.adBigInt;

                case "uint16":
                    return ADODB.DataTypeEnum.adUnsignedSmallInt;

                case "uint32":
                    return ADODB.DataTypeEnum.adUnsignedInt;

                case "uint64":
                    return ADODB.DataTypeEnum.adUnsignedBigInt;
            }

            return ADODB.DataTypeEnum.adVariant;
        }

        private static void UpdateCSV_WriteHeader(object sender, DataRowChangeEventArgs e, FileInfo oFile)
        {
            DataTable dt = (DataTable)sender;

            // Open file
            using (FileStream fsFile = new FileStream(oFile.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            using (StreamWriter sw = new StreamWriter(fsFile))
            using (CsvWriter csvWriter = new CsvWriter(sw, config))
            {
                // Write columns names
                foreach (DataColumn column in dt.Columns)
                {
                    csvWriter.WriteField(column.ColumnName);
                }
                csvWriter.NextRecord();
            }
        }

        private static string SerializeDataTableToJSON(this DataTable dt)
        {
            return JsonConvert.SerializeObject(dt);
        }

        private static string SerializeDataSetToJSON(this DataSet ds)
        {
            return JsonConvert.SerializeObject(ds);
        }
        #endregion
    }
}
