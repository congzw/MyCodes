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
            TestArgs();
            TestArgsPropValue();
            Console.WriteLine("---------------");
            Console.Read();
        }

        static void TestArgs()
        {
            var newModel = new Mock();
            newModel.IsOk = false;
            newModel.Name = "New";

            Console.WriteLine("--------Dynamic-------");
            var blahDynamic = new Blah();
            dynamic dynamicVo = new ExpandoObject();
            dynamicVo.IsOk = true;
            dynamicVo.Name = "A";
            blahDynamic.Bags["args"] = dynamicVo;
            Console.WriteLine(JsonConvert.SerializeObject(blahDynamic));

            Console.WriteLine(JsonConvert.SerializeObject(blahDynamic.GetArgs<Mock>()));
            blahDynamic.SetArgs(newModel);
            Console.WriteLine(JsonConvert.SerializeObject(blahDynamic.GetArgs<Mock>()));

            Console.WriteLine("-------Model--------");
            var blahModel = new Blah();
            var model = new Mock();
            model.IsOk = true;
            model.Name = "A";
            blahModel.Bags["args"] = model;
            Console.WriteLine(JsonConvert.SerializeObject(blahModel));
            
            Console.WriteLine(JsonConvert.SerializeObject(blahModel.GetArgs<Mock>()));
            blahModel.SetArgs(newModel);
            Console.WriteLine(JsonConvert.SerializeObject(blahModel.GetArgs<Mock>()));

            Console.WriteLine("--------Dic-------");
            var blahDic = new Blah();
            var dic = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            dic["IsOk"] = true;
            dic["Name"] = "A";
            blahDic.Bags["args"] = dic;
            Console.WriteLine(JsonConvert.SerializeObject(blahDic));

            Console.WriteLine(JsonConvert.SerializeObject(blahDic.GetArgs<Mock>()));
            blahDic.SetArgs(newModel);
            Console.WriteLine(JsonConvert.SerializeObject(blahDic.GetArgs<Mock>()));
        }

        static void TestArgsPropValue()
        {
            Console.WriteLine("--------Dynamic-------");
            var blahDynamic = new Blah();
            dynamic dynamicVo = new ExpandoObject();
            dynamicVo.IsOk = true;
            dynamicVo.Name = "A";
            blahDynamic.Bags["args"] = dynamicVo;
            var blahJson = JsonConvert.SerializeObject(blahDynamic);
            Console.WriteLine(blahJson);
            blahDynamic.SetArgsPropValue("Name", "AA");
            var dynamicGet = blahDynamic.GetArgsPropValue<string>("Name");
            Console.WriteLine(dynamicGet);

            Console.WriteLine("-------Model--------");
            var blahModel = new Blah();
            var model = new Mock();
            model.IsOk = true;
            model.Name = "A";
            blahModel.Bags["args"] = model;
            var blahModelJson = JsonConvert.SerializeObject(blahModel);
            Console.WriteLine(blahModelJson);

            blahModel.SetArgsPropValue("Name", "AA");
            var modelGet = blahModel.GetArgsPropValue<string>("Name");
            Console.WriteLine(modelGet);

            Console.WriteLine("--------Dic-------");
            var blahDic = new Blah();
            var dic = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            dic["IsOk"] = true;
            dic["Name"] = "A";
            blahDic.Bags["args"] = dic;
            var blahDicJson = JsonConvert.SerializeObject(blahDic);
            Console.WriteLine(blahDicJson);

            blahDic.SetArgsPropValue("Name", "AA");
            var dicGet = blahDic.GetArgsPropValue<string>("Name");
            Console.WriteLine(dicGet);
        }
    }
    
    public class Mock
    {
        public bool IsOk { get; set; }
        public string Name { get; set; }
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
