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
using System.Data.Linq.Mapping;
using YK.Core.SqlHelper;
using YK.Core.Enums;
using YK.Unity.Extensions;
using YK.Model;

namespace YK.Core.CoreFramework
{
    /// <summary>
    /// 公共操作类
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    internal partial class CoreFramework<TEntity>
    {
        /// <summary>
        /// 主键
        /// </summary>
        private string PrimaryKey
        {
            get { return GetPrimaryKey(); }
        }
        
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return AttributeHelper.GetEntityTableAtrributes<TEntity>(); }
        }

        /// <summary>
        /// 变更前事件键
        /// </summary>
        public string ChangingEventKey {
            get {
                return "Entity.ChangingEvent." + TableName;
            }
        }

        /// <summary>
        /// 变更后事件键
        /// </summary>
        public string ChangedEventKey {
            get
            {
                return "Entity.ChangedEvent." + TableName;
            }
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        private string GetPrimaryKey()
        {
            var list = AttributeHelper.GetEntityColumnAtrributes<TEntity>();
            foreach (var model in list)
            {
                if (model.isPrimaryKey)
                {
                    return model.fieldName;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取表达式列表对应的参数列表和条件
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        public CoreFrameworkEntity GetParaListAndWhere(List<Expression> express)
        {
            //获取实体列的特性
            List<EntityPropColumnAttributes> columnAttrList = AttributeHelper.GetEntityColumnAtrributes<TEntity>();
            #region 获取参数和条件
            //参数列表
            List<SqlParameter> listPara = new List<SqlParameter>();
            //条件
            string where = "";
            if (express != null)
            {
                if (express.Count == 0)
                {
                    where = "1=1";
                }
                int i = 0;//运行的位置，从下标0开始
                foreach (Expression exp in express)
                {
                    //连接符号
                    if (string.IsNullOrEmpty(exp.Join))
                    {
                        if (string.IsNullOrEmpty(exp.FieldName) || exp.Value == null)
                        {
                            continue;
                        }
                        string fieldName = columnAttrList.Where(w => w.propName.ToLower() == exp.FieldName.ToLower()).Count() > 0
                        ? columnAttrList.Where(w => w.propName.ToLower() == exp.FieldName.ToLower()).First().fieldName : exp.FieldName;

                        //判断个数是为了防止参数名相同
                        int fieldCount = express.Where(e => e.FieldName == exp.FieldName).Count();
                        string paraName = fieldCount == 1 ? exp.FieldName : exp.FieldName + "_p" + i.ToStr();

                        //当两个条件没有使用连接符时，则默认用and拼接,从第一个参数后试用
                        if (i > 0)
                        {
                            //如果上一个参数不是连接符时，默认每个参数是以and的形式拼接
                            if (string.IsNullOrEmpty(express[i - 1].Join))
                                where += " and ";
                        }

                        //加空格
                        where += " ";

                        #region 条件语句
                        switch (exp.Condition)
                        {
                            case ConditionEnum.Like:
                                listPara.Add(new SqlParameter("@" + paraName, "%" + exp.Value + "%"));
                                where += fieldName + " like @" + paraName;
                                break;
                            case ConditionEnum.LeftLike:
                                listPara.Add(new SqlParameter("@" + paraName, "%" + exp.Value));
                                where += fieldName + " like @" + paraName;
                                break;
                            case ConditionEnum.RightLike:
                                listPara.Add(new SqlParameter("@" + paraName, exp.Value + "%"));
                                where += fieldName + " like @" + paraName;
                                break;
                            case ConditionEnum.NotLike:
                                listPara.Add(new SqlParameter("@" + paraName, "%" + exp.Value + "%"));
                                where += fieldName + " not like @" + paraName;
                                break;
                            case ConditionEnum.In:
                                List<SqlParameter> paramConvertIn = GetParamList((List<object>)exp.Value, exp.FieldName);
                                listPara.AddRange(paramConvertIn);
                                where += fieldName + " in (" + string.Join(",", paramConvertIn.Select(s => s.ParameterName).ToArray()) + ")";
                                break;
                            case ConditionEnum.NotIn:
                                List<SqlParameter> paramConvertNotIn = GetParamList((List<object>)exp.Value, exp.FieldName);
                                listPara.AddRange(paramConvertNotIn);
                                where += fieldName + " not in (" + string.Join(",", paramConvertNotIn.Select(s => s.ParameterName).ToArray()) + ")";
                                break;
                            default:
                                string sign = "=";
                                switch (exp.Condition)
                                {
                                    case ConditionEnum.Eq:
                                        sign = "=";
                                        break;
                                    case ConditionEnum.Ne:
                                        sign = "<>";
                                        break;
                                    case ConditionEnum.Gt:
                                        sign = ">";
                                        break;
                                    case ConditionEnum.Ge:
                                        sign = ">=";
                                        break;
                                    case ConditionEnum.Lt:
                                        sign = "<";
                                        break;
                                    case ConditionEnum.Le:
                                        sign = "<=";
                                        break;
                                }
                                listPara.Add(new SqlParameter("@" + paraName, exp.Value));
                                where += fieldName + " " + sign + " @" + paraName;
                                break;
                        }
                        #endregion
                    }
                    else
                    {
                        where += " " + exp.Join + " ";
                    }
                    i++;
                }
            }
            else
            {
                where = "1=1";
            }
            #endregion

            CoreFrameworkEntity entity = new CoreFrameworkEntity();
            entity.ParaList = listPara;
            entity.Where = where;
            return entity;
        }

        /// <summary>
        /// 获取参数列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public List<SqlParameter> GetParamList(List<object> list, string field)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            int num = 0;
            foreach (object item in list)
            {
                num++;
                SqlParameter param = new SqlParameter();
                param.ParameterName = "@" + field + "_list_" + num;
                param.Value = item;
                paramlist.Add(param);
            }
            return paramlist;
        }

        /// <summary>
        /// 将实体列表转换为数据表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable EntityListToDataTable(List<TEntity> list)
        {
            //实体
            TEntity entity = new TEntity();
            //数据表
            DataTable dt = new DataTable(entity.GetType().Name);
            //创建列
            foreach (PropertyInfo prop in entity.GetType().GetProperties())
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = prop.Name;
                dc.DataType = typeof(string);
                dt.Columns.Add(dc);
            }
            //填充行
            foreach (TEntity model in list)
            {
                DataRow dr = dt.NewRow();
                //循环属性
                foreach (PropertyInfo prop in model.GetType().GetProperties())
                {
                    string typeName = prop.PropertyType.Name;
                    dr[prop.Name] = prop.GetValue(model, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        
        /// <summary>
        /// 将实体列表转换为数据表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<TEntity> EntityListByDataTable(DataTable dt)
        {
            List<TEntity> list = new List<TEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                //实体
                TEntity entity = new TEntity();
                //创建列
                foreach (PropertyInfo prop in entity.GetType().GetProperties())
                {
                    if (dt.Columns.Contains(prop.Name))
                    {
                        prop.SetValue(entity, dr[prop.Name], null);
                    }
                }
                list.Add(entity);
            }
            return list;
        }

        #region Insert\Update私有方法
        /// <summary>
        /// 获取实体属性的参数和SQL
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<ParamSqlModel> GetParamSQLByEntityProp(TEntity entity)
        {
            List<ParamSqlModel> paramSqlModels = new List<ParamSqlModel>();

            //获取公共属性
            CommonProperty cmmonProperty = (CommonProperty)entity;
            //如果没有变更则返回null
            if (cmmonProperty.ChanageProperty.Count == 0)
            {
                return null;
            }

            //循环遍历属性得到SQL脚本和参数
            foreach (PropertyInfo prop in entity.GetType().GetProperties())
            {
                ParamSqlModel paramSqlModel = new ParamSqlModel();

                //如果不包含在变更集里面则跳出
                if (cmmonProperty.ChanageProperty.ContainsKey(prop.Name) == false)
                {
                    continue;
                }

                //获取属性数据库信息
                EntityPropColumnAttributes columnAttribute = columnAttrList.Where(w => w.propName.ToLower() == prop.Name.ToLower()).First();

                //字段
                paramSqlModel.Field = columnAttribute.fieldName;
                paramSqlModel.IsDbGenerated = columnAttribute.isDbGenerated;
                paramSqlModel.IsPrimaryKey = columnAttribute.isPrimaryKey;

                //SQL字符
                string propSql = columnAttribute.fieldName + "=@" + columnAttribute.fieldName;
                paramSqlModel.SQL = propSql;

                //参数添加
                object val = prop.GetValue(entity, null);
                SqlParameter param = new SqlParameter("@" + columnAttribute.fieldName, val ?? DBNull.Value);
                DbType dbtype;
                Enum.TryParse(prop.PropertyType.Name, out dbtype);
                param.DbType = dbtype;

                //参数赋值
                paramSqlModel.Param = param;

                paramSqlModels.Add(paramSqlModel);
            }
            return paramSqlModels;
        }
        #endregion
    }
}
