using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyCodes.Helpers
{
    public class MyModelHelper
    {
        public string MakeIniStringExt(Object obj, string equalOperator = "=", string lastSplit = ";", bool removeLastSplit = true)
        {
            string schema = string.Format("{0}{1}{2}{3}", "{0}", equalOperator, "{1}", lastSplit);
            StringBuilder sb = new StringBuilder();
            if (obj != null)
            {
                //获取类型信息
                Type t = obj.GetType();
                PropertyInfo[] propertyInfos = t.GetProperties();
                foreach (PropertyInfo var in propertyInfos)
                {
                    object value = var.GetValue(obj, null);
                    string temp = "";

                    //如果是string，并且为null
                    if (value == null)
                    {
                        temp = "";
                    }
                    else
                    {
                        temp = value.ToString();
                    }

                    value = ReplaceString(temp, new[] { lastSplit, "=" });
                    sb.AppendFormat(schema, var.Name, value);
                }
            }
            //去掉最后的分号
            if (removeLastSplit)
            {
                string result = sb.ToString();
                return result.Substring(0, result.Length - 1);
            }
            else
            {
                return sb.ToString();
            }
        }

        public List<string> GetKeys<T>()
        {
            List<string> result = new List<string>();
            //获取类型信息
            Type t = typeof(T);
            PropertyInfo[] propertyInfos = t.GetProperties();

            foreach (PropertyInfo var in propertyInfos)
            {
                result.Add(var.Name);
            }
            return result;
        }

        public Dictionary<string, object> GetKeyValueDictionary<T>(T obj)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj != null)
            {
                //获取类型信息
                Type t = typeof(T);
                PropertyInfo[] propertyInfos = t.GetProperties();

                foreach (PropertyInfo var in propertyInfos)
                {
                    result.Add(var.Name, var.GetValue(obj, null));
                }
            }
            return result;
        }
        
        public static MyModelHelper Instance = new MyModelHelper();

        private string ReplaceString(string value, string[] oldValues, string newV = ".")
        {
            string result = value;
            foreach (var old in oldValues)
            {
                result = ReplaceString(result, old, newV);
            }
            return result;
        }
        private string ReplaceString(string value, string oldValue, string newV = ".")
        {
            return value.Replace(oldValue, newV);
        }
    }

    public static class ObjectToIniStringExtensions
    {
        public static string ToIniString(this object model, string nullReturnValue)
        {
            if (model == null)
            {
                return nullReturnValue;
            }
            return MyModelHelper.Instance.MakeIniStringExt(model);
        }
    }
}
