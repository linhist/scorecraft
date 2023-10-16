using System;
using Scorecraft.Utilities;

namespace Scorecraft.Sofa
{
    public class SofaLogger : ISofaLogger
    {
        private string LocalPath { get; }

        public bool AllowSave => !string.IsNullOrEmpty(LocalPath);

        public string Errors { get; private set; }

        public SofaLogger(string locKey = null)
        {
            LocalPath = ConfigHelper.GetSetting(locKey ?? "DIR.SOFAPATH");
            if (!string.IsNullOrEmpty(LocalPath) && !LocalPath.EndsWith("\\"))
            {
                LocalPath += "\\";
            }
        }

        public void ResetError()
        {
            Errors = "";
        }

        public void AddError(string message, string source)
        {
            Errors += $"{DateTime.Now:HH:mm:ss dd/MM/yyyy} [{source}]::{Environment.NewLine}{message}{Environment.NewLine}{Environment.NewLine}";
        }

        public bool SaveContent(string content, string file)
        {
            if (!AllowSave || string.IsNullOrEmpty(file) || string.IsNullOrEmpty(content)) return false;
            return FileHelper.WriteFile(LocalPath + (file.StartsWith("\\") ? file.Substring(1) : file), content);
        }

        public bool SaveResource(string url, string file)
        {
            if (!AllowSave || string.IsNullOrEmpty(file) || string.IsNullOrEmpty(url)) return false;
            return FileHelper.WriteUrl(url, LocalPath + (file.StartsWith("\\") ? file.Substring(1) : file));
        }
    }
}
