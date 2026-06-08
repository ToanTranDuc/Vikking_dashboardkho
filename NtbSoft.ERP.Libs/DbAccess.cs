using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Libs
{
    public class DbConnection
    {
        //private static SqlConnection _cnn = new SqlConnection(clsCommonBS.sConnectionString);
        //
        //private static SqlConnection _cnn =
        //    new SqlConnection(@"Data Source=SYS-PC; Initial Catalog=Framework; User Id=sa; Password=sa123456");
        ////private static SqlConnection _cnn = 
        //    new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\DB\Framework.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True");
        private static SqlConnection _cnn;
        public DbConnection()
        {
            //_cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["strCnn_ln"].ConnectionString);
            //if (_cnn == null)
                _cnn = SqlHelper.GetConnection();
        }
        public SqlConnection SqlConnection
        {
            get { return _cnn; }
            set { _cnn = value; }
        }

        public void Open()
        {
            if (_cnn != null && _cnn.State == ConnectionState.Closed)
                _cnn.Open();
        }

        public void Close()
        {
            if (_cnn != null)
            {
                _cnn.Close();
                _cnn.Dispose();
            }
        }
    }


    public class DbAccess
    {
        DbConnection db = new DbConnection();
        SqlCommand cmd;
        SqlTransaction _sqlTran;
        #region "Open-Close Connection"
        /// <summary>
        /// Open Connection
        /// </summary>
        private void Open()
        {
            db.Open();
        }

        /// <summary>
        /// Close Connection
        /// </summary>
        private void Close()
        {
            db.Close();
        }
        #endregion

        #region "Transaction"
        /// <summary>
        /// BeginTransction
        /// </summary>
        public void BeginTransaction()
        {
            db.Open();
            _sqlTran = db.SqlConnection.BeginTransaction();
        }

        /// <summary>
        /// Close Connection
        /// </summary>
        public void CommitTransaction()
        {
            _sqlTran.Commit();
            db.Close();
        }

        public void RollbackTransaction()
        {
            _sqlTran.Rollback();
            db.Close();
        }
        #endregion

        #region "Create and Add parameter for SqlCommand"
        /// <summary>
        /// Create New SqlCommand
        /// </summary>
        public void CreateNewSqlCommand()
        {
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = db.SqlConnection;
        }

        /// <summary>
        /// Add Parameters for calling stored procedures
        /// </summary>
        /// <param name="paramName">Name of Parameter</param>
        /// <param name="value">Value of Parameter</param>
        public void AddParameter(string paramName, object value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }
        #endregion

        #region "Execute SqlCommand"
        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="sProcName">Name of Stored Procedure</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sProcName)
        {
            try
            {
                cmd.CommandText = sProcName;
                this.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// ExecuteNonQueryWithTransaction
        /// </summary>
        /// <param name="sProcName">Name of Stored Procedure</param>        
        /// <returns></returns>
        public bool ExecuteNonQueryWithTransaction(string sProcName)
        {
            try
            {
                cmd.CommandText = sProcName;
                cmd.Transaction = _sqlTran;
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="sProcName">Name of Stored Procedure</param>
        /// <returns></returns>
        public object ExecuteScalar(string sProcName)
        {
            try
            {
                cmd.CommandText = sProcName;
                this.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="sProcName">Name of Stored Procedure</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string sProcName)
        {
            try
            {
                cmd.CommandText = sProcName;
                this.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ExecuteReaderWithOpenningConnection - Not close connection after reading
        /// </summary>
        /// <param name="sProcName">Name of Stored Procedure</param>        
        /// <returns></returns>
        public SqlDataReader ExecuteReaderWithOpenningConnection(string sProcName)
        {
            try
            {
                cmd.CommandText = sProcName;

                cmd.Transaction = _sqlTran;
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ExecuteDataTable
        /// </summary>
        /// <param name="sProcName">Name of Stored Procedure</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sProcName)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                cmd.CommandText = sProcName;
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// ExecuteDataTable
        /// </summary>
        /// <param name="sProcName">Name of Stored Procedure</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sProcName)
        {
            DataSet _ds = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                cmd.CommandText = sProcName;
                da.SelectCommand = cmd;
                da.Fill(_ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _ds;
        }
        #endregion
    }
}
