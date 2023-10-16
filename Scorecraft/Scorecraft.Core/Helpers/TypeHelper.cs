using System;
using System.Globalization;
using System.Linq;

namespace Scorecraft.Utilities
{
    public static class TypeHelper
    {
        private static DateTime Date1970 = new DateTime(1970, 1, 1);
        private static Type[] NumTypes = new Type[] { typeof(int), typeof(long), typeof(short), typeof(sbyte) };
        private static Type[] UNumTypes = new Type[] { typeof(uint), typeof(ulong), typeof(ushort), typeof(byte) };
        private static Type[] DecTypes = new Type[] { typeof(double), typeof(float), typeof(decimal) };
        private static string[] DateFormats = new string[] {
            "dd/MM", "dd/MM/yy", "dd/MM/yyyy",
            "ddMMyyyy", "yyyyMMdd", "yyyyMMddHHmmss",
            "HH:mm dd/MM/yy", "HH:mm dd/MM/yyyy",
            "HH:mm:ss dd/MM/yy", "HH:mm:ss dd/MM/yyyy",
            "hh:mm dd/MM/yy t", "hh:mm dd/MM/yyyy t",
            "hh:mm:ss dd/MM/yy t", "hh:mm:ss dd/MM/yyyy t",
            "hh:mm dd/MM/yy tt", "hh:mm dd/MM/yyyy tt",
            "hh:mm:ss dd/MM/yy tt", "hh:mm:ss dd/MM/yyyy tt"
        };

        static TypeHelper()
        { }

        public static bool IsEmpty(string value)
        {
            if (value == null) return true;
            return (value.Trim().Length == 0);
        }

        public static DateTime? FromJsDateNum(int? millisecs)
        {
            return millisecs == null ? default : Date1970.AddMilliseconds(millisecs.Value);
        }

        public static T To<T>(object value)
        {
            try
            {
                Type type = typeof(T);
                if (type == value.GetType()) return (T)value;

                if (type == typeof(bool)) value = ToBool(value); 
                if (type == typeof(DateTime)) value = ToDate(value, DateFormats);
                if (DecTypes.Contains(type) || NumTypes.Contains(type) || UNumTypes.Contains(type)) value = ToNumber(value);
                if (type.IsEnum) value = ToEnum(value, type);

                return (T) Convert.ChangeType(value, typeof(T));
            }
            catch { }

            return default;
        }

        public static bool? ToBool(object value)
        {
            if (value == null) return null;

            Type type = value.GetType();
            if (type == typeof(string))
            {
                string val = value.ToString().ToLower();
                if (val == "true" || val == "1") return true;
                if (val == "false" || val == "0") return false;
                return null;
            }

            if (NumTypes.Contains(value.GetType()) || UNumTypes.Contains(type))
            {
                decimal? val = ToNumber(value);
                if (val == null) return null;
                return val.Value != 0;
            }

            return null;
        }

        public static object ToEnum(object value, Type enumType)
        {
            if (value == null) return null;

            Type type = value.GetType();
            var enums = Enum.GetValues(enumType);

            if (type == typeof(string))
            {
                foreach (var e in enums)
                {
                    if (e.ToString().Equals(value)) return e;
                }
                return null;
            }

            if (NumTypes.Contains(type) || UNumTypes.Contains(type))
            {
                decimal? val = ToNumber(value);
                if (val == null) return null;

                foreach (var e in enums)
                {
                    if ((int)e == (int)val.Value) return e;
                }
            }

            return null;
        }

        public static DateTime? ToDate(object value, params string[] formats)
        {
            if (value == null) return null;

            Type type = value.GetType();
            if (type == typeof(string))
            {
                DateTime d;
                if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                {
                    return d;
                }
                if (DateTime.TryParseExact(value.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                {
                    return d;
                }
                else
                {
                    decimal? val = ToNumber(value, "#", "X");
                    if (val != null) new DateTime((long) val.Value);
                }
                return null;
            }

            if (NumTypes.Contains(type) || UNumTypes.Contains(type))
            {
                long val = (long) value;
                return new DateTime(val);
            }

            return null;
        }

        public static decimal? ToNumber(object value, params string[] formats)
        {
            if (value == null) return null;

            Type type = value.GetType();
            if (type == typeof(string))
            {
                decimal d;
                if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                {
                    return d;
                }
                return null;
            }

            return 0;
        }
    }
}
