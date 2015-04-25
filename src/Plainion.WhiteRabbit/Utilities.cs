using System;

namespace Plainion.WhiteRabbit
{
    public static class StringExtensions
    {
        /// <summary>
        /// Checks whether the string is null or empty (after
        /// white spaces have been trimmed).
        /// </summary>
        public static bool IsNullOrTrimmedEmpty(this string s)
        {
            return (s == null || string.IsNullOrEmpty(s.Trim()));
        }
    }

    public static class ObjectExtensions
    {
        public static bool IsEmpty(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return true;
            }

            string s = value as string;
            if (s != null)
            {
                return s.IsNullOrTrimmedEmpty();
            }

            return false;
        }
    }
}