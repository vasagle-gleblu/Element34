using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Element34.DataManager
{
    public enum MDBType
    {
        Access2000,
        Access2003,
        Access2007,
        Access2010,
        Access2016,
        Access2019
    }

    static class Common
    {
        #region [Fields]
        public static readonly Encoding enc = Encoding.Default;
        public static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        public static readonly int MAXNVARCHAR = 4000;
        public static readonly int MAXVARCHAR = 8000;
        #endregion

        public static string BuildCsvRow(DataRow row)
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

        public static string SerializeToXML(this object item)
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

        public static T DeserializefromXML<T>(this string sInput)
        {
            T result = default;

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (StringReader strReader = new StringReader(sInput))
            using (XmlReader xmlReader = XmlReader.Create(strReader))
            {
                result = (T)serializer.ReadObject(xmlReader);
            }

            return result;
        }

        public static string SerializeToJSON(this object item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public static T DeserializeFromJSON<T>(this string sInput)
        {
            return JsonConvert.DeserializeObject<T>(sInput);
        }

        public static ADODB.Recordset DefineRecordset(DataTable inTable)
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

        public static OleDbType ResolveOleDbType(DataColumn column)
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

        public static ADOX.DataTypeEnum ResolveADOXType(DataColumn column)
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

        public static ADODB.DataTypeEnum ResolveADODBType(DataColumn column)
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

        public static ADOX.Table DefineADOXTable(DataTable inTable)
        {
            ADOX.Table result = new ADOX.Table
            {
                Name = (new string(inTable.TableName.Where(c => !char.IsPunctuation(c)).ToArray())).Replace("$", string.Empty)
            };
            DataColumnCollection inColumns = inTable.Columns;

            foreach (DataColumn inColumn in inColumns)
            {
                ADOX.Column col = new ADOX.Column
                {
                    Name = inColumn.ColumnName,
                    Type = ResolveADOXType(inColumn)
                };

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

        public static void AddDataTableToMDBwithADOX(ADOX.Catalog cat, DataTable inTable)
        {
            ADODB.Recordset rs = new ADODB.Recordset
            {
                CursorLocation = ADODB.CursorLocationEnum.adUseClient
            };
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

        public static void AddDataTableToMDB(DataTable inTable, OleDbConnection conn)
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
                sqlStmt.Length--;
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

        public static string ParameterValueForSQL(SqlParameter sp)
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
                    retval = (bool)ToBooleanOrDefault(sp.Value, false) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        public static string ParameterValueForSQL(OleDbParameter sp)
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
                    retval = (bool)ToBooleanOrDefault(sp.Value, false) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        public static SqlDbType ResolveSqlDbType(DataColumn column)
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

        public static SqlDbType ToSqlDbType(Type clrType)
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

        public static Type ToClrType(SqlDbType sqlType)
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

        public static void DefineSqlServerTable(DataTable inTable, SqlConnection connection)
        {
            try
            {
                connection.Open();

                // Construct SQL command to create the table
                string createTableCommand = DefineSQLTable(inTable).ToString();

                // Execute the CreateTable command
                using (SqlCommand command = new SqlCommand(createTableCommand, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Error creating SQL Server table: {ex.Message}");
            }
            finally 
            { 
                connection.Close();
            }            
        }

        /// <summary>
        /// Inspects a DataTable and return an SQL statement that can be used to CREATE a TABLE in MS-SQL Server.
        /// </summary>
        /// <param name="inTable">System.Data.DataTable object to be inspected for building the SQL CREATE TABLE statement.</param>
        /// <returns>String of SQL</returns>
        private static StringBuilder DefineSQLTable(DataTable inTable)
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

            return sql;
        }

        public static string ResolveMySqlDbType(DataColumn column)
        {
            Type dataType = column.DataType;
            if (dataType == typeof(string))
            {
                return "VARCHAR";
            }
            else if (dataType == typeof(byte))
            {
                return "TINYINT";
            }
            else if (dataType == typeof(DateTime))
            {
                return "DATETIME";
            }
            else if (dataType == typeof(DateTimeOffset))
            {
                return "DATETIME";
            }
            else if (dataType == typeof(TimeSpan))
            {
                return "TIME";
            }
            else if (dataType == typeof(Guid))
            {
                return "CHAR(36)";
            }
            // Handle other data types if needed
            else
            {
                throw new ArgumentException($"Unsupported data type: {dataType.Name}");
            }
        }

        public static string ResolveSQLiteColumnType(Type dataType)
        {
            if (dataType == typeof(String))
                return "TEXT";
            if (dataType == typeof(Int32))
                return "INTEGER";
            if (dataType == typeof(Double))
                return "REAL";
            if (dataType == typeof(DateTime))
                return "DATETIME";
            // Add more type resolutions as needed
            return "TEXT"; // Default to TEXT if type not recognized
        }

        public static bool? ToBooleanOrDefault(object o, bool? Default)
        {
            return ToBooleanOrDefault((string)o, Default);
        }

        public static bool? ToBooleanOrDefault(string s, bool? defaultValue)
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

        public static string Normalize(object o, bool isCSV = false, bool isSQL = false)
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
    }
}
