using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace MyApp.Extensions
{
    public static class ImageExtensions
    {
        public static string ToBase64String(this Image img)
        {
            var format = TryGetSaveRawFormat(img, ImageFormat.Png);
            return ToBase64String(img, format);
        }

        public static string ToBase64String(this Image img, ImageFormat imageFormat)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, imageFormat);
                ms.Position = 0;
                byte[] byteBuffer = ms.ToArray();
                ms.Close();
                var base64String = Convert.ToBase64String(byteBuffer);
                return base64String;
            }
        }

        public static Image Base64StringToImage(this string base64String)
        {
            //互转些许有差异的问题，todo
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            using (var memoryStream = new MemoryStream(byteBuffer))
            {
                memoryStream.Position = 0;
                var img = Image.FromStream(memoryStream);
                memoryStream.Close();
                return img;
            }
        }

        public static string ToBase64ImageTag(this Image img, ImageFormat imageFormat)
        {
            var base64String = img.ToBase64String(imageFormat).AppendBase64ImageFormat(imageFormat);
            var imgTag = "<img src=\"data:image/" + imageFormat + ";base64," + base64String + "\" ";
            imgTag += "width=\"" + img.Width + "\" ";
            imgTag += "height=\"" + img.Height + "\" />";
            return imgTag;
        }

        public static string AppendBase64ImageFormat(this string base64String, ImageFormat imageFormat)
        {
            return "data:image/" + imageFormat + ";base64," + base64String;
        }

        public static void SaveTo(this Image img, string path)
        {
            using (var stream = File.Create(path))
            {
                var format = TryGetSaveRawFormat(img, ImageFormat.Png);
                img.Save(stream, format);
                stream.Close();
            }
        }

        private static ImageFormat TryGetSaveRawFormat(Image image, ImageFormat defaultImageFormat)
        {
            //如果从磁盘加载图像，则可以使用image.RawFormat其原始格式保存该图像。
            //但是没有与内存位图关联的编码器，因此您必须自己指定图像格式（例如ImageFormat.Png等）。
            var imageCodecInfo = ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(codec => codec.FormatID == image.RawFormat.Guid);
            return imageCodecInfo != null ? image.RawFormat : defaultImageFormat;
        }
    }
}
