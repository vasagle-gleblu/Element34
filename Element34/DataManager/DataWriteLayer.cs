using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using static System.Environment;

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
            Mode = CsvMode.RFC4180
        };
        #endregion

        #region [Public Functions]
        public static void ExportToMDB(string fileName, DataSet ds)
        {
            string connection = string.Format("Data Source={0};User Id=Admin;Password=;Provider={1}", fileName, "Microsoft.ACE.OLEDB.12.0");
            ADOX.Catalog cat = new ADOX.Catalog();
            OleDbConnection con = new OleDbConnection(connection);

            if (File.Exists(fileName))
                File.Delete(fileName);

            cat.Create(connection);

            foreach (DataTable dt in ds.Tables)
            {
                ADOX.Table tbl = DefineTable(dt);
                cat.Tables.Append(tbl);
                AddDatatableToMDB(cat, dt);
            }

            con = cat.ActiveConnection as OleDbConnection;
            if (con != null)
                con.Close();
            cat = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void CreateMDB(string connString)
        {
            ADOX.Catalog cat = new ADOX.Catalog();
            OleDbConnection con = new OleDbConnection(connString);
            string fileName = Path.GetFullPath(con.DataSource);

            if (File.Exists(fileName))
                File.Delete(fileName);

            cat.Create(connString);

            con = cat.ActiveConnection as OleDbConnection;
            if (con != null)
                con.Close();
            cat = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void ExportToXLS(string sFilename, DataSet ds)
        {
            if (File.Exists(sFilename))
                File.Delete(sFilename);

            FileInfo newFile = new FileInfo(sFilename);
            DataRow dr;

            using (ExcelPackage ExcelPkg = new ExcelPackage(newFile))
            {
                foreach (DataTable dt in ds.Tables)
                {
                    ExcelWorksheet wkSheet = ExcelPkg.Workbook.Worksheets.Add(dt.TableName.Replace("$", string.Empty));
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

                ExcelPkg.Save();
            }
        }

        public static void CreateXLS(string sFilename)
        {
            if (File.Exists(sFilename))
                File.Delete(sFilename);

            FileInfo newFile = new FileInfo(sFilename);

            using (ExcelPackage ExcelPkg = new ExcelPackage(newFile))
            {
                ExcelPkg.Workbook.Worksheets.Add("Sheet1");
                ExcelPkg.Save();
            }
        }

        public static void ExportToCSV(DirectoryInfo oDir, DataSet ds)
        {
            if (!oDir.Exists)
                oDir.Create();

            string sFile;

            foreach (DataTable dt in ds.Tables)
            {
                sFile = Path.GetFullPath(Path.Combine(oDir.FullName, string.Format("{0}.csv", dt.TableName.Replace("$", string.Empty))));
                ExportToCSV(sFile, dt);
            }
        }

        public static void ExportToCSV(string sFile, DataTable dt)
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

        //public static void ExportToCSV(FileInfo oFile, DataTable dt)
        //{  // this method for creating a CSV file
        //    char separator = ',';
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        sb.Append(dt.Columns[i]);
        //        if (i < dt.Columns.Count - 1)
        //            sb.Append(separator);
        //    }
        //    sb.AppendLine();

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        for (int i = 0; i < dt.Columns.Count; i++)
        //        {
        //            // Types: System.String, System.Int32, System.Boolean, System.TimeSpan, System.DateTime, System.Decimal, System.Byte[]
        //            if (dr[i].GetType().ToString() == "System.String")
        //            {
        //                sb.AppendFormat("\"{0}\"", dr[i].ToString());
        //                if (i < dt.Columns.Count - 1)
        //                    sb.Append(separator);
        //            }
        //            else
        //            {
        //                sb.Append(dr[i].ToString());
        //                if (i < dt.Columns.Count - 1)
        //                    sb.Append(separator);
        //            }
        //        }
        //        sb.AppendLine();
        //    }

        //    //write all text to csv file
        //    if (oFile.Exists)
        //        oFile.Delete();

        //    File.WriteAllText(oFile.FullName, sb.ToString(), enc);
        //}

        public static void UpdateCSV_MapEvents(DataTable dt, FileInfo oFile)
        {
            dt.RowChanged += (sender, e) => UpdateCSV_RowChanged(sender, e, oFile);
            dt.RowDeleted += (sender, e) => UpdateCSV_RowDeleted(sender, e, oFile);
        }

        public static void UpdateCSV_RowChanged(object sender, DataRowChangeEventArgs e, FileInfo oFile)
        {
            if ((e.Action == DataRowAction.Change) || (e.Row.RowState == DataRowState.Added))
            {
                DataTable dt = (DataTable)sender;
                int iRowIndex = dt.Rows.IndexOf(e.Row);
                StringBuilder sb = new StringBuilder();
                string tmp;

                // Open file
                using (FileStream fsFile = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                using (StreamReader sr = new StreamReader(fsFile))
                using (CsvDataReader csvDataReader = new CsvDataReader(new CsvReader(sr, culture)))
                using (StreamWriter sw = new StreamWriter(fsFile))
                using (CsvWriter csvWriter = new CsvWriter(sw, culture))
                {
                    long iSeekAhead = 0;
                    int lineNumber = (csvWriter.Configuration.HasHeaderRecord) ? (iRowIndex + 1) : iRowIndex;
                    try
                    {
                        // Set current position to the beginning
                        fsFile.Position = 0;

                        // Calculate how far to skip ahead to modified row
                        for (int j = 0; j < lineNumber; j++)
                        {
                            iSeekAhead += sr.ReadLine().Length + NewLine.Length;
                        }

                        // Build updated row
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            // Row values
                            tmp = e.Row[i].ToString();
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

                        fsFile.Seek(iSeekAhead - lineNumber, SeekOrigin.Begin);
                        sw.WriteLine(sb.ToString());
                    }

                    catch
                    {
                        return;
                    }
                }
            }
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
                using (FileStream fsFile = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                using (StreamReader sr = new StreamReader(fsFile, enc))
                using (CsvDataReader csvDataReader = new CsvDataReader(new CsvReader(sr, config)))
                using (StreamWriter sw = new StreamWriter(fsFile))
                using (CsvWriter csvWriter = new CsvWriter(sw, config))
                {
                    // Calculate how far to skip ahead to deleted row
                    int lineNumber = (csvWriter.Configuration.HasHeaderRecord) ? (iRowIndex + 1) : iRowIndex;
                    try
                    {
                        // Set current position to the beginning
                        fsFile.Position = 0;

                        // Calculate how far to skip ahead to modified row
                        for (int k = 0; k < lineNumber; k++)
                        {
                            iSeekAhead += sr.ReadLine().Length + NewLine.Length;
                        }

                        //  Overwrite deleted row and write out remaining rows
                        for (int j = lineNumber; j < dt.Rows.Count; j++)
                        {
                            row = dt.Rows[j];
                            sb.Clear();

                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                // Row values
                                tmp = row[i].ToString();
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
                            iSeekAhead = sb.ToString().Length;
                        }
                    }
                    catch
                    {
                        // Abort the update.
                        return;
                    }
                }
            }
        }

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

        public static string ExportToJSON(DataSet ds)
        {
            return ds.SerializeDataSetToJSON();
        }

        public static string ExportToJSON(DataTable dt)
        {
            return dt.SerializeDataTableToJSON();
        }
        #endregion

        #region [Private Functions]
        private static ADOX.Table DefineTable(DataTable inTable)
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
            switch (column.DataType.Name.ToLower())
            {
                case "guid":
                    return ADOX.DataTypeEnum.adVariant;

                case "boolean":
                    return ADOX.DataTypeEnum.adBoolean;

                case "bool":
                    return ADOX.DataTypeEnum.adBoolean;

                case "byte":
                    return ADOX.DataTypeEnum.adUnsignedTinyInt;

                case "char":
                    return ADOX.DataTypeEnum.adWChar;

                case "datetime":
                    return ADOX.DataTypeEnum.adDate;

                case "decimal":
                    return ADOX.DataTypeEnum.adDecimal;

                case "double":
                    return ADOX.DataTypeEnum.adDouble;

                case "int16":
                    return ADOX.DataTypeEnum.adSmallInt;

                case "int32":
                    return ADOX.DataTypeEnum.adInteger;

                case "int64":
                    return ADOX.DataTypeEnum.adBigInt;

                case "sbyte":
                    return ADOX.DataTypeEnum.adTinyInt;

                case "single":
                    return ADOX.DataTypeEnum.adSingle;

                case "string":
                    if (column.MaxLength > 4000)
                        return ADOX.DataTypeEnum.adLongVarWChar;
                    else
                        return ADOX.DataTypeEnum.adVarWChar;

                case "timespan":
                    return ADOX.DataTypeEnum.adBigInt;

                case "uint16":
                    return ADOX.DataTypeEnum.adUnsignedSmallInt;

                case "uint32":
                    return ADOX.DataTypeEnum.adUnsignedInt;

                case "uint64":
                    return ADOX.DataTypeEnum.adUnsignedBigInt;
            }

            return ADOX.DataTypeEnum.adVariant;
        }

        private static void AddDatatableToMDB(ADOX.Catalog cat, DataTable inTable)
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
            using (FileStream fsFile = new FileStream(oFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (StreamWriter sw = new StreamWriter(fsFile))
            using (CsvWriter csvWriter = new CsvWriter(sw, config))
            {
                try
                {
                    // Write columns names
                    foreach (DataColumn column in dt.Columns)
                    {
                        csvWriter.WriteField(column.ColumnName);
                    }
                    csvWriter.NextRecord();
                }
                catch
                {
                    // Abort the update.
                    return;
                }
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
