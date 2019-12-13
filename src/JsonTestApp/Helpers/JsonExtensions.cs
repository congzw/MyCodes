using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonTestApp.Helpers
{
    public interface IShouldHaveBags
    {
        //IDictionary<string, object> Bags { get; set; }
    }

    public interface IHaveBags : IShouldHaveBags
    {
        IDictionary<string, object> Bags { get; set; }
    }
    public interface IClientMethod : IHaveBags
    {
        string Method { get; set; }
    }

    public static class ClientMethodExtensions
    {
        public static TArgs GetArgs<TArgs>(this IClientMethod clientMethod, TArgs defaultArgs = default(TArgs))
        {
            if (!clientMethod.Bags.ContainsKey("args"))
            {
                return defaultArgs;
            }

            var bagArgs = clientMethod.Bags["args"];

            if (bagArgs is JObject theJObject)
            {
                return theJObject.ToObject<TArgs>();
            }

            if (bagArgs is TArgs)
            {
                return (TArgs)bagArgs;
            }

            var json = JsonConvert.SerializeObject(bagArgs);
            var argsT = JsonConvert.DeserializeObject<TArgs>(json);
            return argsT;
        }

        public static void SetArgs(this IClientMethod clientMethod, object args)
        {
            clientMethod.Bags["args"] = args;
        }

        #region new version to be tested!

        public static T GetArgsPropValue<T>(this IClientMethod clientMethodInvoke, string argsPropKey, T defaultValue)
        {
            var containsArgs = clientMethodInvoke.Bags.ContainsKey("args");
            if (!containsArgs)
            {
                return defaultValue;
            }

            var theArgs = clientMethodInvoke.Bags["args"];
            if (theArgs is JObject theJObject)
            {
                //来自网络序列化
                var jToken = theJObject.GetValue(argsPropKey, StringComparison.OrdinalIgnoreCase);
                return jToken == null ? defaultValue : jToken.ToObject<T>();
            }

            var argsJson = JsonConvert.SerializeObject(theArgs);
            var argsDic = JsonConvert.DeserializeObject<IDictionary<string, object>>(argsJson);
            foreach (var argsDicKey in argsDic.Keys)
            {
                if (argsDicKey.Equals(argsPropKey, StringComparison.OrdinalIgnoreCase))
                {
                    var propValue = argsDic[argsPropKey];
                    var propValueJson = JsonConvert.SerializeObject(propValue);
                    return JsonConvert.DeserializeObject<T>(propValueJson);
                }
            }
            return defaultValue;
        }

        public static void SetArgsPropValue<T>(this IClientMethod clientMethodInvoke, string argsPropKey, object argsPropValue)
        {
            var containsArgs = clientMethodInvoke.Bags.ContainsKey("args");
            if (!containsArgs)
            {
                clientMethodInvoke.Bags["args"] = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            var theArgs = clientMethodInvoke.Bags["args"];
            if (theArgs is JObject theJObject)
            {
                //来自网络序列化
                theJObject.SetPropertyContent(argsPropKey, argsPropValue);
                return;
            }

            var argsJson = JsonConvert.SerializeObject(theArgs);
            var argsDic = JsonConvert.DeserializeObject<IDictionary<string, object>>(argsJson);
            argsDic[argsPropKey] = argsPropValue;
        }

        #endregion
    }

    public static class JsonExtensions
    {
        public static JObject SetPropertyContent(this JObject source, string name, object content)
        {
            var prop = source.Property(name);

            if (prop == null)
            {
                prop = new JProperty(name, content);

                source.Add(prop);
            }
            else
            {
                prop.Value = JContainer.FromObject(content);
            }

            return source;
        }

        public static IDictionary<string, object> ToDictionary(this JObject jObject)
        {
            var result = jObject.ToObject<Dictionary<string, object>>();

            var JObjectKeys = (from r in result
                               let key = r.Key
                               let value = r.Value
                               where value.GetType() == typeof(JObject)
                               select key).ToList();

            var JArrayKeys = (from r in result
                              let key = r.Key
                              let value = r.Value
                              where value.GetType() == typeof(JArray)
                              select key).ToList();

            JArrayKeys.ForEach(key => result[key] = ((JArray)result[key]).Values().Select(x => ((JValue)x).Value).ToArray());
            JObjectKeys.ForEach(key => result[key] = ToDictionary(result[key] as JObject));

            return result;
        }
    }


}