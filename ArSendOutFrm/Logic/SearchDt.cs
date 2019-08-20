using System;
using System.Data;
using System.Data.SqlClient;
using ArSendOutFrm.DB;
using CroMaxChangeFrm;

namespace ArSendOutFrm.Logic
{
    public class SearchDt
    {
        SqlList sqlList=new SqlList();

        /// <summary>
        /// 根据指定条件查询所需内容
        /// </summary>
        /// <param name="orderno"></param>
        /// <param name="sdt"></param>
        /// <param name="edt"></param>
        /// <returns></returns>
        public DataTable Searchdt(string orderno, DateTime sdt, DateTime edt)
        {
            var resultdt=new DataTable();
            try
            {
                var sqlscript = sqlList.Get_Search(orderno, sdt, edt);
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Columns.Clear();
                resultdt.Rows.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 获取明细记录
        /// </summary>
        /// <returns></returns>
        public DataTable Searchdtldt(int fid)
        {
            var resultdt = new DataTable();
            try
            {
                var sqlscript = sqlList.Get_ShowDtl(fid);
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Columns.Clear();
                resultdt.Rows.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConn()
        {
            var conn = new Conn();
            var sqlcon = new SqlConnection(conn.GetConnectionString());
            return sqlcon;
        }
    }
}
