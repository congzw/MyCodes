﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Drawing.Brushes;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace MyApp.Captures
{
    public class CaptureHelper
    {
        /// <summary>
        /// 从控件截取，不包含其他层
        /// </summary>
        /// <param name="element"></param>
        /// <param name="filePath"></param>
        public static void CopyControlTo(FrameworkElement element, string filePath)
        {
            var rtb = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(element);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            AutoFixDirectory(filePath);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// 从屏幕截取，包含所有层(使用现有控件和相对控件定位)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="relativeTo"></param>
        /// <param name="filePath"></param>
        /// <param name="appendText"></param>
        public static void CopyScreenAreaTo(FrameworkElement element, UIElement relativeTo, string filePath, string appendText)
        {
            var rect = GetWpfControlScreenRect(element, relativeTo);
            CopyScreenAreaTo(rect, filePath, appendText);
        }

        /// <summary>
        /// 从屏幕截取，包含所有层
        /// </summary>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="filePath"></param>
        /// <param name="appendText"></param>
        public static void CopyScreenAreaTo(int sourceX, int sourceY, int width, int height, string filePath, string appendText)
        {
            var mainScreenArea = CopyScreenArea(sourceX, sourceY, width, height, appendText);
            SavePng(mainScreenArea, filePath);
        }
        
        public static Bitmap CopyScreenArea(int sourceX, int sourceY, int width, int height, string appendText)
        {
            var screenBmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var bmpGraphics = Graphics.FromImage(screenBmp))
            {
                bmpGraphics.CopyFromScreen(sourceX, sourceY, 0, 0, new Size(width, height));
                if (!string.IsNullOrWhiteSpace(appendText))
                {
                    bmpGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                    bmpGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    bmpGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //todo position customize
                    bmpGraphics.DrawString(appendText, new Font(System.Drawing.SystemFonts.DefaultFont.Name, 24, System.Drawing.FontStyle.Bold),
                        Brushes.Red, 0, 0);
                }
                return screenBmp;
            }
        }

        public static void SavePng(Bitmap bitmap, string filePath)
        {
            AutoFixDirectory(filePath);
            using (Stream fs = File.OpenWrite(filePath))
            {
                bitmap.Save(fs, ImageFormat.Png);
                fs.Flush();
                fs.Close();
            }
        }

        #region helpers

        /// <summary>
        /// 获取控件相对屏幕的位置
        /// </summary>
        /// <param name="control"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        internal static Rect GetWpfControlScreenRect(FrameworkElement control, UIElement window)
        {
            var locationFromWindow = control.TranslatePoint(new Point(0, 0), window);
            var locationFromScreen = control.PointToScreen(locationFromWindow);
            return new Rect(locationFromScreen, control.RenderSize);
        }

        /// <summary>
        /// 抓取屏幕的图像
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="filePath"></param>
        /// <param name="appendText"></param>
        internal static void CopyScreenAreaTo(Rect rect, string filePath, string appendText)
        {
            var mainScreenArea = CopyScreenArea((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, appendText);
            SavePng(mainScreenArea, filePath);
        }

        internal static void AutoFixDirectory(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                if (string.IsNullOrWhiteSpace(dirPath))
                {
                    throw new ArgumentException("非法的路径:" + filePath);
                }
                Directory.CreateDirectory(dirPath);
            }
        }

        #endregion
    }
}