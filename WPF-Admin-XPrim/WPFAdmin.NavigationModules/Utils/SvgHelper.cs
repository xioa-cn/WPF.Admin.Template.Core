using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using SkiaSharp;
using Svg.Skia;

namespace WPFAdmin.NavigationModules.Utils
{
    public static class SvgHelper
    {
        public static Bitmap ConvertSvgToBitmap(string svgContent, int width, int height)
        {
            // 1. 解析 SVG 内容为 Skia 的 SKPicture
            using var svg = new SKSvg();
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent));
            var picture = svg.Load(stream); // 加载 SVG 流，返回矢量图形

            if (picture == null)
                throw new Exception("SVG 解析失败");

            // 2. 创建 Skia 位图，设置尺寸和格式
            using var skBitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(skBitmap);

            // 3. 清除画布（可选：设置背景色）
            canvas.Clear(SKColors.Transparent); // 透明背景

            // 4. 计算 SVG 缩放比例（适配目标尺寸，保持宽高比）
            var svgBounds = picture.CullRect; // SVG 原始边界（宽高）
            float scaleX = width / svgBounds.Width;
            float scaleY = height / svgBounds.Height;
            float scale = Math.Min(scaleX, scaleY); // 按最小比例缩放，避免裁剪

            // 5. 应用缩放和平移（居中显示）
            canvas.Translate(width / 2f, height / 2f); // 原点移至中心
            canvas.Scale(scale); // 缩放
            canvas.Translate(-svgBounds.MidX, -svgBounds.MidY); // 平移使 SVG 居中

            // 6. 绘制 SVG 矢量图形到画布
            canvas.DrawPicture(picture);
            canvas.Flush(); // 刷新画布

            // 7. 将 SKBitmap 转换为 System.Drawing.Bitmap
            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100); // 编码为 PNG
            using var memoryStream = new MemoryStream();
            data.SaveTo(memoryStream);
            memoryStream.Position = 0;

            return new Bitmap(memoryStream);
        }

        // 重载：从 SVG 文件转换
        public static Bitmap ConvertSvgFileToBitmap(string svgFilePath, int width, int height)
        {
            string svgContent = File.ReadAllText(svgFilePath);
            return ConvertSvgToBitmap(svgContent, width, height);
        }
        
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            using (var stream = new MemoryStream())
            {
                // 将 Bitmap 保存到内存流（格式可选，如 PNG）
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                // 从流创建 BitmapSource
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 加载后关闭流
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // 冻结以支持跨线程访问（可选）

                return bitmapImage;
            }
        }
    }
}