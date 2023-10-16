using System;
using System.Configuration;
using System.Web;

namespace Scorecraft.Utilities
{
    public static class ConfigHelper
    {
        static ConfigHelper()
        { }

        public static string AppRootPath => (HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/") : Environment.CurrentDirectory);

        public static string GetConnStr(string key) => ConfigurationManager.ConnectionStrings[key]?.ConnectionString;

        public static string GetSetting(string key) => ConfigurationManager.AppSettings[key];
    }
}