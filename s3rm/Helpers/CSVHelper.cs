using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Reflection;
using System.IO;
using System.Data.Odbc;

/***********************************************************
 * 文件用途：操作CSV文件
 * 文件版本：1.0.0.0
 * 拷贝日期：2013年4月19日
 * 其它说明：原封不动从网上拷贝，目前存在一些问题，但将来再处理。
 ***********************************************************/

namespace Com.DRPENG.SDXY.UI.Common
{
    public class CSVHelper
    {
        #region Fields
        string _fileName;
        DataTable _dataSource;//数据源
        string[] _titles = null;//列标题
        string[] _fields = null;//字段名

        #endregion

        #region .ctor
        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="dataSource">数据源</param> 
        public CSVHelper()
        {

        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="titles">要输出到 Excel 的列标题的数组</param> 
        /// <param name="fields">要输出到 Excel 的字段名称数组</param> 
        /// <param name="dataSource">数据源</param> 
        public CSVHelper(string[] titles, string[] fields, DataTable dataSource)
            : this(titles, dataSource)
        {
            if (fields == null || fields.Length == 0)
                throw new ArgumentNullException("fields");
            if (titles.Length != fields.Length)
                throw new ArgumentException("titles.Length != fields.Length", "fields");

            _fields = fields;
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="titles">要输出到 Excel 的列标题的数组</param> 
        /// <param name="dataSource">数据源</param> 
        public CSVHelper(string[] titles, DataTable dataSource)
            : this(dataSource)
        {
            if (titles == null || titles.Length == 0)
                throw new ArgumentNullException("titles");

            _titles = titles;
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="dataSource">数据源</param> 
        public CSVHelper(DataTable dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            // maybe more checks needed here (IEnumerable, IList, IListSource, ) ??? 
            // 很难判断，先简单的使用 DataTable

            _dataSource = dataSource;
        }

        #endregion

        #region public Methods

        #region  导出到CSV文件并且提示下载
        /// <summary>
        /// 导出到CSV文件并且提示下载
        /// </summary>
        /// <param name="fileName"></param>
        public void DataToCSV(string fileName)
        {
            // 确保有一个合法的输出文件名 
            //if (fileName == null || fileName == string.Empty || !(fileName.ToLower().EndsWith(".csv")))
            //    fileName = GetRandomFileName();
            string data = ExportCSV();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Expires = 0;
            HttpContext.Current.Response.BufferOutput = true;
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}.csv", System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)));
            HttpContext.Current.Response.ContentType = "text/h323;charset=gbk";
            HttpContext.Current.Response.Write(data);
            HttpContext.Current.Response.End();
        }

        #endregion

        /// <summary>
        /// 获取CSV导入的数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称(.csv不用加)</param>
        /// <returns></returns>
        public DataTable GetCsvData(string filePath, string fileName)
        {
            string path = Path.Combine(filePath, fileName + ".csv");
            string connString = @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + filePath + ";Extensions=asc,csv,tab,txt;";
            try
            {
                using (OdbcConnection odbcConn = new OdbcConnection(connString))
                {
                    odbcConn.Open();
                    OdbcCommand oleComm = new OdbcCommand();
                    oleComm.Connection = odbcConn;
                    oleComm.CommandText = "select * from [" + fileName + "#csv]";
                    OdbcDataAdapter adapter = new OdbcDataAdapter(oleComm);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, fileName);
                    return ds.Tables[0];
                    odbcConn.Close();
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                throw ex;
            }
        }
        #endregion

        #region 返回写入CSV的字符串
        /// <summary>
        /// 返回写入CSV的字符串
        /// </summary>
        /// <returns></returns>
        private string ExportCSV()
        {
            if (_dataSource == null)
                throw new ArgumentNullException("dataSource");

            StringBuilder strbData = new StringBuilder();
            if (_titles == null)
            {
                //添加列名
                foreach (DataColumn column in _dataSource.Columns)
                {
                    strbData.Append(column.ColumnName + ",");
                }
                strbData.Append("\n");
                foreach (DataRow dr in _dataSource.Rows)
                {
                    for (int i = 0; i < _dataSource.Columns.Count; i++)
                    {
                        strbData.Append(dr[i].ToString() + ",");
                    }
                    strbData.Append("\n");
                }
                return strbData.ToString();
            }
            else
            {
                foreach (string columnName in _titles)
                {
                    strbData.Append(columnName + ",");
                }
                strbData.Append("\n");
                if (_fields == null)
                {
                    foreach (DataRow dr in _dataSource.Rows)
                    {
                        for (int i = 0; i < _dataSource.Columns.Count; i++)
                        {
                            strbData.Append(dr[i].ToString() + ",");
                        }
                        strbData.Append("\n");
                    }
                    return strbData.ToString();
                }
                else
                {
                    foreach (DataRow dr in _dataSource.Rows)
                    {
                        for (int i = 0; i < _fields.Length; i++)
                        {
                            strbData.Append(_fields[i].ToString() + ",");
                        }
                        strbData.Append("\n");
                    }
                    return strbData.ToString();
                }
            }
        }
        #endregion

        #region 得到一个随意的文件名
        /// <summary> 
        /// 得到一个随意的文件名 
        /// </summary> 
        /// <returns></returns> 
        private string GetRandomFileName()
        {
            Random rnd = new Random((int)(DateTime.Now.Ticks));
            string s = rnd.Next(Int32.MaxValue).ToString();
            return DateTime.Now.ToShortDateString() + "_" + s + ".csv";
        }
        #endregion
    }
}