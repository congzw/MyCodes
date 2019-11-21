using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MyApp.Captures;
using MyApp.Extensions;
using MyApp.Security;
using MyApp.Tasks;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoAsyncTaskCancel();
            //DemoEncrypt();
            //DemoCapture();
            //DemoBase64();
            Console.Read();
        }

        private static void DemoAsyncTaskCancel()
        {
            AsyncCancelDemo.Run();
        }

        private static void DemoEncrypt()
        {
            var generateEncryptionKey = SimpleAes.GenerateEncryptionKey();
            foreach (var b in generateEncryptionKey)
            {
                Console.Write(b);
                Console.Write(",");
            }
            Console.WriteLine();

            var generateEncryptionVector = SimpleAes.GenerateEncryptionVector();
            foreach (var b in generateEncryptionVector)
            {
                Console.Write(b);
                Console.Write(",");
            }
            Console.WriteLine();

            Console.WriteLine("------SimpleAes-----");
            var simpleAes = new SimpleAes();
            var @join = string.Join(",",Enumerable.Repeat("b2f28db601599e3c1271117acfd9caed1d4abfb3", 1000).ToArray());
            //var @join = simpleAes.EncryptToString("b2f28db601599e3c1271117acfd9caed1d4abfb3");
            var encrypt = simpleAes.EncryptToString(@join);
            Console.WriteLine(encrypt.Length);
            var decryptString = simpleAes.DecryptString(encrypt);
            Console.WriteLine(decryptString.Length);

            Console.WriteLine("----StringCompressor----");
            var stringCompressor = StringCompressor.Instance;
            var compressString = stringCompressor.CompressString(encrypt);
            Console.WriteLine(compressString.Length);
            var decompressString = stringCompressor.DecompressString(compressString);
            Console.WriteLine(decompressString.Length);


            Console.WriteLine("----ZipHelper----");
            var bytes = ZipHelper.Zip(encrypt);
            Console.WriteLine(bytes.Length);

            var unzip = ZipHelper.Unzip(bytes);
            Console.WriteLine(unzip.Length);
        }

        static void DemoCapture()
        {
            try
            {
                var saveTo = AppDomain.CurrentDomain.Combine("test.png");
                var mainScreenArea = CaptureHelper.CopyScreenArea(0, 0, 200, 200, "123");
                CaptureHelper.SavePng(mainScreenArea, saveTo);
                //on success callback
                var base64String = mainScreenArea.ToBase64String();
                Console.WriteLine(base64String);

                //var fromFile = Image.FromFile(saveTo);
                //var imageCodecInfo2 = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == fromFile.RawFormat.Guid);
                //Console.WriteLine(imageCodecInfo2);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void DemoBase64()
        {
            var image = Image.FromFile("_files\\user.png");
            var base64String = image.ToBase64String();

            var image2 = base64String.Base64StringToImage();
            var base64String2 = image2.ToBase64String();
            Console.WriteLine("1 and 2 same? => {0}", base64String == base64String2);
            image2.SaveTo("_files\\user2.png");

            var image3 = Image.FromFile("_files\\user2.png");
            var base64String3 = image3.ToBase64String();
            Console.WriteLine("2 and 3 same? => {0}", base64String2 == base64String3);

            //why??? => GQAAAABJRU5ErkJggg==
            ShowDiff(base64String, base64String2);
        }

        static void ShowDiff(string value, string value2)
        {
            var diff = new Dictionary<int, string>();
            var maxInt = value.Length;
            if (value2.Length > maxInt)
            {
                maxInt = value2.Length;
            }

            for (int i = 0; i < maxInt; i++)
            {
                char c1 = 'X';
                if (value.Length <= i)
                {
                    c1 = value[i];
                }
                char c2 = 'X';
                if (value2.Length <= i)
                {
                    c2 = value[i];
                }

                if (c1 != c2)
                {
                    diff.Add(i, $"{c1}:{c2}");
                }
            }

            foreach (var item in diff)
            {
                Console.WriteLine(item.Key + " => " + item.Value);
            }
        }
    }
}
