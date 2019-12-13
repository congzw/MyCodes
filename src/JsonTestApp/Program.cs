using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using JsonTestApp.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestJObject();
            Console.WriteLine("---------------");
            Console.Read();
        }

        static void TestJObject()
        {
            //var json = JObject.Parse(@"{name:'', foo:'Foo', isOk: true}");
            //var mock = json.ToObject<Mock>();

            //var serializeObject = JsonConvert.SerializeObject(mock);
            //Console.WriteLine(serializeObject);
            
            //var blah = new Blah();
            //blah.Bags["args"] = new Mock();
            //var blahJson = JsonConvert.SerializeObject(blah);
            //var blahJObject = JObject.Parse(blahJson);
            //var blahModel = blahJObject.ToObject<Blah>();

            //var blah = new Blah();
            //blah.SetArgs(new Mock());
            //var test = blah.GetArgs((Mock)null);
            
            //var blah = new Blah();
            //blah.SetArgs(new Mock());
            //var blahJson = JsonConvert.SerializeObject(blah);
            //var blahJObject = JsonConvert.DeserializeObject<Blah>(blahJson);

            //var test = blahJObject.GetArgs((Mock)null);
            
            var blah = new Blah();
            dynamic dObj = new ExpandoObject();
            dObj.A = "A";
            dObj.b = "B";
            blah.Bags["args"] = dObj;
            var blahJson = JsonConvert.SerializeObject(blah);
            Console.WriteLine(blahJson);

            var blah2 = new Blah();
            var dic = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            dic["A"] = "A";
            dic["b"] = "B";
            blah2.Bags["args"] = dic;

            var mock = new Mock();
            var mockJson = JsonConvert.SerializeObject(mock);
            var mockDic = JsonConvert.DeserializeObject<IDictionary<string, object>>(mockJson);
            foreach (var o in mockDic)
            {
                Console.WriteLine(o.Key);
            }
            //bad!
            //dynamic test = dic;
            //test.C = "C";
            var blahJson2 = JsonConvert.SerializeObject(blah2);
            Console.WriteLine(blahJson2);
        }
    }
    
    public class Mock
    {
        public string Name { get; set; }
        public string Foo { get; set; }
        public bool IsOk { get; set; }
    }

    public class Blah : IClientMethod
    {
        public Blah()
        {
            Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, object> Bags { get; set; }
        public string Method { get; set; }
    }
}
