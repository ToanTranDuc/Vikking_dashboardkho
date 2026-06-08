using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Libs
{
    /// <summary>
    /// Get data from Sql Data
    /// </summary>
    public class clsGetData
    {
        /// <summary>
        /// Exp: columns: ("* get all" or "column1,column2,...");
        /// Exp: whereClause( "a>0 and c='a' and..." Or NULL)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetData(string columns, string TableName, string whereClause)
        {

            SqlConnection _cnn = null;
            try
            {
                string _whereClause = string.Format("Where {0}", whereClause);
                if (string.IsNullOrEmpty(whereClause))
                    _whereClause = string.Empty;

                _cnn = SqlHelper.GetConnection(SqlHelper.StrConnectionString);
                SqlParameter[] sp = new SqlParameter[3];
                sp[0] = new SqlParameter("@Columns", columns);
                sp[1] = new SqlParameter("@TableName", TableName);
                sp[2] = new SqlParameter("@WhereClause", _whereClause);

                DataTable tbl = SqlHelper.ExecuteDataset(_cnn, "sp_Select", sp).Tables[0];

                return tbl;
            }
            catch (Exception ex)
            {
                throw new Exception("GetData-02: " + ex.Message);
            }
            finally
            {
                if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
            }
        }

        /// <summary>
        /// Exp: count(columnName). Only a columnName
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public int ExecuteScalar(string columnName, string TableName, string whereClause)
        {

            SqlConnection _cnn = null;
            try
            {
                string _whereClause = string.Format("Where {0}", whereClause);
                if (string.IsNullOrEmpty(whereClause))
                    _whereClause = string.Empty;

                _cnn = SqlHelper.GetConnection(SqlHelper.StrConnectionString);
                SqlParameter[] sp = new SqlParameter[3];
                sp[0] = new SqlParameter("@Columns", string.Format("Count({0})", columnName));
                sp[1] = new SqlParameter("@TableName", TableName);
                sp[2] = new SqlParameter("@WhereClause", _whereClause);

                int kq = (int)SqlHelper.ExecuteScalar(_cnn, "sp_Select", sp);
                return kq;
            }
            catch (Exception ex)
            {
                throw new Exception("GetData-02: " + ex.Message);
            }
            finally
            {
                if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
            }
        }

        /// <summary>
        /// Exp: columns: ("* get all" or "column1,column2,...");
        /// Exp: clauseJoin(tbl1 a inner join tbl2 on a.id=b.id);
        /// Exp: whereClause(exp: a>0 and c='a' and...)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetDataJoin(string columns, string clauseJoin, string whereClause)
        {
            SqlConnection _cnn = null;
            try
            {
                string _whereClause = string.Format("Where {0}", whereClause);
                if (string.IsNullOrEmpty(whereClause))
                    _whereClause = string.Empty;
                _cnn = SqlHelper.GetConnection(SqlHelper.StrConnectionString);
           
                SqlParameter[] sp = new SqlParameter[3];
                sp[0] = new SqlParameter("@Columns", columns);
                sp[1] = new SqlParameter("@TableName", clauseJoin);
                sp[2] = new SqlParameter("@WhereClause", _whereClause);

                DataTable tbl = SqlHelper.ExecuteDataset(_cnn, "sp_Select", sp).Tables[0];
                return tbl;
            }
            catch (Exception ex)
            {
                throw new Exception("GetData-02: " + ex.Message);
            }
            finally
            {
                if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
            }
        }
    }
}
