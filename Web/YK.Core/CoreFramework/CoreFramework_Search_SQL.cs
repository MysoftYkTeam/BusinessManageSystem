using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using YK.Core.Model;
using YK.Core.Pager;
using YK.Core.SqlHelper;

namespace YK.Core.CoreFramework
{
    /// <summary>
    /// 公共操作类，SQL命令语句模块（SQL）
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal partial class CoreFramework<TEntity>
    {
        /// <summary>
        /// SQL命令语句,获取数据
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public List<TEntity> SQLSearch(string cmdText)
        {
            return SQLSearch(cmdText,null);
        }

        /// <summary>
        /// SQL命令语句
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public List<TEntity> SQLSearch(string cmdText,List<SqlParameter> listPara)
        {
            IDataReader sdr = SqlConvertHelper.GetInstallSqlHelper(OrgCode).ExecuteReader(cmdText, listPara);
            return DynamicBuilder<TEntity>.GetList(sdr, columnAttrList);
        }

        /// <summary>
        /// SQL命令语句
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public DataSet SQLGetDataSet(string cmdText, List<SqlParameter> listPara)
        {
            return SqlConvertHelper.GetInstallSqlHelper(OrgCode).GetDataSet(cmdText, listPara);
        }

        /// <summary>
        /// SQL命令语句，返回所影响的行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public int SQLExecuteNonQuery(string cmdText, List<SqlParameter> listPara)
        {
            return SqlConvertHelper.GetInstallSqlHelper(OrgCode).ExecuteNonQuery(cmdText, listPara);
        }

        /// <summary>
        /// SQL命令语句，返回数据阅读器
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public IDataReader SQLExecuteReader(string cmdText, List<SqlParameter> listPara)
        {
            return SqlConvertHelper.GetInstallSqlHelper(OrgCode).ExecuteReader(cmdText, listPara);
        }

        /// <summary>
        /// SQL命令语句，返回第一行第一列的值
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public string SQLExecuteScalar(string cmdText, List<SqlParameter> listPara)
        {
            return SqlConvertHelper.GetInstallSqlHelper(OrgCode).ExecuteScalar(cmdText, listPara);
        }
    }
}
