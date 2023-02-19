using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Element34.Utilities.DataManager
{
    public static class DataInsertionLayer
    {
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

        public static void ExportToCSV(FileInfo oFile, DataTable dt)
        {  // this method for creating a CSV file
            char seperator = ',';
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i]);
                if (i < dt.Columns.Count - 1)
                    sb.Append(seperator);
            }
            sb.AppendLine();

            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dr[i].GetType().ToString().ToLower() == "string")
                    {
                        sb.AppendFormat("\"{0}\"", dr[i].ToString());
                        if (i < dt.Columns.Count - 1)
                            sb.Append(seperator);
                    }
                    else
                    {
                        sb.Append(dr[i].ToString());
                        if (i < dt.Columns.Count - 1)
                            sb.Append(seperator);
                    }
                }
                sb.AppendLine();
            }

            //write all text to csv file
            if (oFile.Exists)
                oFile.Delete();

            File.WriteAllText(oFile.FullName, sb.ToString());
        }

        public static void ExportToCSV(DirectoryInfo oDir, DataSet ds)
        {
            if (!oDir.Exists)
                oDir.Create();

            foreach (DataTable dt in ds.Tables)
            {
                FileInfo oFile = new FileInfo(Path.GetFullPath(Path.Combine(oDir.FullName, string.Format("{0}.csv", dt.TableName.Replace("$", string.Empty)))));
                ExportToCSV(oFile, dt);
            }
        }

        public static string ExportToXml(this DataTable dt)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(DataTable));
                    xmlSerializer.Serialize(streamWriter, dt);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

        public static string ExportToXml(this DataSet ds)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(DataSet));
                    xmlSerializer.Serialize(streamWriter, ds);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

        public static string ExportToJSON(DataTable dt)
        {
            return dt.SerializeDataTableToJSON();
        }

        public static string ExportToJSON(DataSet ds)
        {
            return ds.SerializeDataSetToJSON();
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
