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

    public static class HaveBagsExtensions
    {
        public static T GetBagItem<T>(this IHaveBags haveBags, string itemKey, T defaultItemValue = default(T))
        {
            if (!haveBags.Bags.ContainsKey(itemKey))
            {
                return defaultItemValue;
            }

            var bagItemValue = haveBags.Bags[itemKey];
            var bagItemConvertTo = haveBags.BagItemConvertTo<T>(bagItemValue);
            return bagItemConvertTo;
        }

        public static void SetBagItem<T>(this IHaveBags haveBags, T args)
        {
            haveBags.Bags["args"] = args;
        }

        public static T BagItemConvertTo<T>(this IHaveBags haveBags, object bagItemValue)
        {
            if (bagItemValue is T itemValue)
            {
                return itemValue;
            }

            if (bagItemValue is JObject theJObject)
            {
                return theJObject.ToObject<T>();
            }
            
            var json = JsonConvert.SerializeObject(bagItemValue);
            var argsT = JsonConvert.DeserializeObject<T>(json);
            return argsT;
        }
    }

    public static class ClientMethodExtensions
    {
        public static TArgs GetArgs<TArgs>(this IClientMethod clientMethod, TArgs defaultArgs = default(TArgs))
        {
            return clientMethod.GetBagItem<TArgs>("args", defaultArgs);
        }

        public static void SetArgs<TArgs>(this IClientMethod clientMethod, TArgs args)
        {
            clientMethod.SetBagItem(args);
        }
        
        public static T GetArgsPropValue<T>(this IClientMethod clientMethod, string argsPropKey, T defaultValue = default(T))
        {
            var containsArgs = clientMethod.Bags.ContainsKey("args");
            if (!containsArgs)
            {
                return defaultValue;
            }

            var theArgs = clientMethod.Bags["args"];
            if (theArgs is JObject theJObject)
            {
                //有可能来自网络序列化
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

        public static void SetArgsPropValue<T>(this IClientMethod clientMethod, string argsPropKey, T argsPropValue)
        {
            var containsArgs = clientMethod.Bags.ContainsKey("args");
            if (!containsArgs)
            {
                clientMethod.Bags["args"] = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            var theArgs = clientMethod.Bags["args"];
            if (theArgs is JObject theJObject)
            {
                //有可能来自网络序列化
                theJObject.SetPropertyContent(argsPropKey, argsPropValue);
                return;
            }

            var argsJson = JsonConvert.SerializeObject(theArgs);
            var argsDic = JsonConvert.DeserializeObject<IDictionary<string, object>>(argsJson);
            argsDic[argsPropKey] = argsPropValue;
            clientMethod.Bags["args"] = argsDic;
        }
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