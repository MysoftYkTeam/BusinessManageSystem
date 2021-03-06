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
    /// 公共操作类，搜索模块（Lambda Search）
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    internal partial class CoreFramework<TEntity> 
    {

        /// <summary>
        /// 公共搜索
        /// </summary>
        /// <param name="coreFrameworkEntity"></param>
        /// <param name="count"></param>
        /// <param name="selectFields"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        private List<TEntity> CommonSearch(CoreFrameworkEntity coreFrameworkEntity, int? count = null, string selectFields = null, string orderBy = null)
        {

            coreFrameworkEntity.Where = string.IsNullOrEmpty(coreFrameworkEntity.Where) ? "1=1" : coreFrameworkEntity.Where;//条件
            selectFields = string.IsNullOrEmpty(selectFields) ? "*" : selectFields;//查询字段
            orderBy = string.IsNullOrEmpty(orderBy) ? "" : "order by " + orderBy;//排序
            string topStr = count.HasValue == false ? "" : (" top " + count);//Top

            StringBuilder cmdText = new StringBuilder();
            cmdText.Append("select");
            cmdText.Append(" ");
            cmdText.Append(topStr);
            cmdText.Append(" ");
            cmdText.Append(selectFields);
            cmdText.Append(" ");
            cmdText.Append("from");
            cmdText.Append(" ");
            cmdText.Append(TableName);
            cmdText.Append(" ");
            cmdText.Append("where");
            cmdText.Append(" ");
            cmdText.Append(coreFrameworkEntity.Where);
            cmdText.Append(" ");
            cmdText.Append(orderBy);

            IDataReader sdr = SqlConvertHelper.GetInstallSqlHelper(OrgCode).ExecuteReader(cmdText.ToString(), coreFrameworkEntity.ParaList);
            return DynamicBuilder<TEntity>.GetList(sdr, columnAttrList);

        }
    }
    
}
