using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using YK.Unity.Extensions;

namespace YK.Core
{
    /// <summary>
    /// 实体映射数据库
    /// </summary>
    public class EntityReflectionDataBase
    {
        /// <summary>
        /// 实体映射数据库
        /// </summary>
        public void StartReflection()
        {
            //程序集所在的路径
            Assembly assembly = Assembly.LoadFrom(System.Web.HttpContext.Current.Server.MapPath("~/Bin/YK.Model.dll"));
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                //表名
                string tableName = type.Name;
                //表名的规则，必须包含tb_
                if (tableName.ToLower().Contains("tb_"))
                {
                //当前点
                nowDot:
                    //核查表是否存在,如果存在则进行核查字段
                    if (ExistTable(tableName))
                    {
                        foreach (PropertyInfo prop in type.GetProperties())
                        {
                            //字段名
                            string fieldName = prop.Name;
                            //核查字段是否存在
                            if (ExistField(tableName, fieldName) == false)
                            {
                                //添加字段
                                AddField(tableName, fieldName, prop.PropertyType.Name);
                            }
                        }
                    }
                    else
                    {
                        //创建表
                        CreateTable(tableName);
                        goto nowDot;
                    }
                }
            }
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="type">字段类型</param>
        public bool AddField(string tableName,string fieldName,string type)
        {
            string cmdText = "ALTER TABLE [" + tableName + "] ADD {0}";
            switch (type.ToLower())
            {
                case "int32":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [int]");
                    break;
                case "string":
                    cmdText = string.Format(cmdText, string.Concat(new object[] { "[", fieldName, "] [nvarchar] (", 256, ")" }));
                    break;
                case "datetime":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [datetime]");
                    break;
                case "boolean":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [bit]");
                    break;
                case "bool":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [bit]");
                    break;
                case "decimal":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [decimal]");
                    break;                
                case "nvarchar":
                    cmdText = string.Format(cmdText, string.Concat(new object[] { "[", fieldName, "] [nvarchar] (", 256, ")" }));
                    break;
                case "varchar":
                    cmdText = string.Format(cmdText, string.Concat(new object[] { "[", fieldName, "] [varchar] (", 256, ")" }));
                    break;
                case "ntext":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [ntext]");
                    break;
                case "text":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [text]");
                    break;
                case "bit":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [bit]");
                    break;                
                case "money":
                    cmdText = string.Format(cmdText, "[" + fieldName + "] [money]");
                    break;
            }
            var sqlHelper = new SqlHelper.SqlHelper();
            return sqlHelper.ExecuteNonQuery(cmdText) == 0 ? false : true;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool CreateTable(string tableName)
        {
            var sqlHelper = new SqlHelper.SqlHelper();
            string cmdText = string.Format("\r\n CREATE TABLE {0} \r\n   (\r\n\t  [ID] INT IDENTITY(1,1) PRIMARY KEY \r\n ) \r\n ",tableName);
            return sqlHelper.ExecuteNonQuery(cmdText) > 0 ? true : false;
        }

        /// <summary>
        /// 核查表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool ExistTable(string tableName)
        {
            var sqlHelper = new SqlHelper.SqlHelper();
            string cmdText = string.Format("SELECT Count(*) FROM SYSOBJECTS WHERE ID = OBJECT_ID('{0}') AND OBJECTPROPERTY(ID, 'IsUserTable') = 1", tableName);
            return sqlHelper.ExecuteScalar(cmdText).ToInt() > 0 ? true : false;
        }

        /// <summary>
        /// 核查字段是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public bool ExistField(string tableName,string fieldName)
        {
            var sqlHelper = new SqlHelper.SqlHelper();
            //查询字段
            string cmdTextField = string.Format("select Count(*) from syscolumns where id=object_id('{0}') and name='{1}'", tableName, fieldName);
            return sqlHelper.ExecuteScalar(cmdTextField).ToInt() > 0 ? true : false;
        }
    }
}
