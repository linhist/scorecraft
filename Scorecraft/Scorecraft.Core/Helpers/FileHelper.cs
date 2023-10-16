using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Scorecraft.Utilities
{
    public static class FileHelper
    {
        static FileHelper()
        { }

        public static string MapPath(string path)
        {
            string file = (path ?? "").Trim();
            if (file.Contains("\\")) return file;
            if (file == "" || file == "~" || file == "/" || file == ".") return MapPath("~/");

            try
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            catch { }

            return "";
        }

        public static string WebPath(string path)
        {
            string url = (path ?? "").Trim();
            if (url == "" || url == "~" || url == "/" || url == ".") return "/";
            if (url.StartsWith("~/")) return url.Substring(1);
            if (url.Contains("/")) return url;

            try
            {
                string webRoot = MapPath("~/");
                if (url.StartsWith(webRoot)) return url.Substring(webRoot.Length - 1).Replace("\\", "/");
            }
            catch { }

            return "";
        }

        public static string BuildPath(string path)
        {
            string dir = MapPath(path);
            if (dir == "") return "";

            try
            {
                int idx = dir.LastIndexOf("\\");
                if (dir.LastIndexOf(".") > idx)
                {
                    Directory.CreateDirectory(dir.Substring(0, idx));
                }
                else
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
            catch { }

            return "";
        }

        public static bool Exists(string path, bool isFile = true)
        {
            try
            {
                string file = MapPath(path);
                return isFile ? File.Exists(file) : Directory.Exists(file);
            }
            catch { }

            return false;
        }

        public static bool Delete(string path, bool isFile = true)
        {
            try
            {
                string file = MapPath(path);
                if (isFile)
                {
                    File.Delete(file);
                }
                else
                {
                    Directory.Exists(file);
                }

                return true;
            }
            catch { }

            return false;
        }

        public static string[] GetFiles(string path, bool topOnly = false, bool fullPath = false, params string[] fileExts)
        {
            try
            {
                string folder = MapPath(path);
                if (Directory.Exists(folder))
                {
                    string[] files = GetInners(folder, true, topOnly, fullPath).Where(f => fileExts.Any(e => f.EndsWith($".{e}"))).ToArray();
                }
            }
            catch { }

            return Array.Empty<string>();
        }

        public static string[] GetInners(string path, bool fileOnly = false, bool topOnly = false, bool fullPath = false)
        {
            try
            {
                string folder = MapPath(path);
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder, "", topOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
                    if (!fileOnly)
                    {
                        files.Concat(Directory.GetDirectories(folder, "", topOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories));
                    }

                    if (!fullPath) return files.Select(f => WebPath(f)).ToArray();
                    return files;
                }
            }
            catch { }

            return Array.Empty<string>();
        }

        public static bool ReadFile(string path, out string contents)
        {
            contents = "";
            try
            {
                string file = MapPath(path);
                if (File.Exists(file))
                {
                    contents = File.ReadAllText(file);
                    return true;
                }
            }
            catch { }

            return false;
        }

        public static bool ReadUrl(string path, out string contents)
        {
            contents = "";

            try
            {
                string url = WebPath(path);
                using (WebClient client = new WebClient())
                {
                    contents = client.DownloadString(url);
                }
                return true;
            }
            catch { }

            return false;
        }

        public static bool WriteFile(string path, string contents, bool overWrite = true)
        {
            try
            {
                string file = BuildPath(path);
                if (!overWrite && File.Exists(file)) return false;

                if (overWrite && File.Exists(file))
                {
                    File.Delete(file);
                }

                File.WriteAllText(file, contents);
                return true;
            }
            catch { }

            return false;
        }

        public static bool WriteStream(string path, Stream stream, bool overWrite = false)
        {
            try
            {
                string file = BuildPath(path);
                if (File.Exists(file))
                {
                    if (!overWrite) return false;
                    File.Delete(file);
                }

                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fs);
                    fs.Flush();
                }
                return true;
            }
            catch { }

            return false;
        }

        public static bool WriteUrl(string path, string file)
        {
            try
            {
                string url = WebPath(path);
                string save = BuildPath(file);
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, save);
                }
                return true;
            }
            catch { }

            return false;
        }

        private static ImageFormat GetImageType(string file)
        {
            file = file?.Trim() ?? "";
            int idx = (file).LastIndexOf(".");

            if (idx > 0)
            {
                switch (file.Substring(idx + 1).ToLower())
                {
                    case "bmp": return ImageFormat.Bmp;
                    case "gif": return ImageFormat.Gif;
                    case "png": return ImageFormat.Png;
                }
            }

            return ImageFormat.Jpeg;
        }

        private static Rectangle GetResizeRect(int wid, int hei, int owid, int ohei, bool keepRatio)
        {
            if (wid == 0)
            {
                wid = owid;
            }
            if (hei == 0)
            {
                hei = ohei;
            }

            if (keepRatio)
            {
                if (owid > ohei)
                {
                    hei = ohei * wid / owid;
                }
                else
                {
                    wid = owid * hei / ohei;
                }

                return new Rectangle(0, 0, wid, hei);
            }
            
            
            if (owid > ohei)
            {
                ohei = ohei * wid / owid;
                owid = wid;
            }
            else
            {
                owid = owid * hei / ohei;
                ohei = hei;
            }

            return new Rectangle((wid - owid) / 2, (hei - ohei) / 2, wid, hei);
        }

        public static bool SaveImage(string path, Stream stream, int wid = 0, int hei = 0, bool overWrite = false, bool keepRatio = false)
        {
            try
            {
                string file = MapPath(path);
                if (File.Exists(file))
                {
                    if (!overWrite) return false;
                    File.Delete(file);
                }

                using (Bitmap origin = new Bitmap(stream))
                {
                    Rectangle rect = GetResizeRect(wid, hei, origin.Width, origin.Height, keepRatio);
                    using (Bitmap image = new Bitmap(rect.Width, rect.Height))
                    {
                        image.SetResolution(origin.HorizontalResolution, origin.VerticalResolution);
                        using (Graphics graphics = Graphics.FromImage(image))
                        {
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            using (var wrapMode = new ImageAttributes())
                            {
                                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                                graphics.DrawImage(image, rect, 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel, wrapMode);
                            }
                        }

                        image.Save(path, GetImageType(file));
                    }
                }

                return true;
            }
            catch { }

            return false;
        }
    }
}
