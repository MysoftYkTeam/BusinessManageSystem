﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

using YK.Model;
using System.Data;
using System.Reflection;

using Newtonsoft.Json;

namespace YK.Unity.Extensions
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class BasicExtension
    {
        /// <summary>
        /// GUID转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid? ToGuid(this string value)
        {
            Guid guid;
            if (Guid.TryParse(value, out guid) == false)
            {
                return null;
            }
            return guid;
        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
        public static string ToStr(this object obj)
        {
            string str = null;
            if (obj != null)
            {
                try
                {
                    str = obj.ToString();
                }
                catch {
                }
            }
            return str;
        }

        /// <summary>
        /// 转换整数，对象，默认值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns></returns>
        public static int ToInt(this string obj, int defaultVal = 0)
        {
            int num = defaultVal;
            if (obj != null)
            {
                int.TryParse(obj, out num);
            }
            return num;
        }

        /// <summary>
        /// 转换十进制,并设置小数点长度
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="length">小数点长度</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string obj, int length = -1)
        {
            decimal dec = 0.00m;
            if (obj != null)
            {
                if (decimal.TryParse(obj, out dec))
                {
                    if (length != -1)
                    {
                        dec = Math.Round(dec, length);
                    }
                }
            }
            return dec;
        }

        /// <summary>
        /// 转换单精度浮点数字（float）,(限制小数点长度)
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="length">小数点长度</param>
        /// <returns></returns>
        public static float ToFloat(this string obj,int length=-1)
        {
            float f = 0.00f;
            if (obj != null)
            {
                if (float.TryParse(obj.ToString(), out f))
                {
                    if(length!=-1)
                    {
                        f =Convert.ToSingle(Math.Round(Convert.ToDouble(f),length));
                    }
                }
            }
            return f;
        }

        /// <summary>
        /// 转换双精度浮点数字（double）,(限制小数点长度)
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="length">小数点长度</param>
        /// <returns></returns>
        public static double ToDouble(this object obj, int length = -1)
        {
            double d = 0.00;
            if (obj != null)
            {
                if (double.TryParse(obj.ToString(), out d))
                {
                    if (length != -1)
                    {
                        d = Math.Round(d, length);
                    }
                }
            }
            return d;
        }

        /// <summary>
        /// 转换bool值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool ToBool(this string obj)
        {
            return obj.ToLower() == "false" ? false : true;
        }

        /// <summary>
        /// 转换bool值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool ToIntToBool(this int obj)
        {
            return obj <= 0 ? false : true;
        }

        /// <summary>
        /// 转换bool值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string obj)
        {
            DateTime nowDate;
            DateTime.TryParse(obj, out nowDate);           
            return nowDate;
        }

        /// <summary>
        /// 截取object类型的字符串，主要是那些为null的数据进行处理
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string SubstringStr(this string obj, int length)
        {
            if (obj != null)
            {
                if (obj.Length >= length)
                {
                    obj = obj.Substring(0, length);
                }
            }
            return obj;
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this object data) {
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// 将数据表转换为实体列表
        /// </summary>
        /// <returns></returns>
        public static List<TEntity> ToEntityList<TEntity>(this DataTable dt)
        {
            List<TEntity> list = new List<TEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                //实体
                TEntity entity = System.Activator.CreateInstance<TEntity>();
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
    }
}
