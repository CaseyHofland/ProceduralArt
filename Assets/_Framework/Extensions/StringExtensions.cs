using System;
using System.Text.RegularExpressions;

namespace Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Capitalizes the first character of a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Capitalize(this string str)
        {
            if (string.IsNullOrEmpty(str) || Char.IsUpper(str[0]))
                return str;

            return Char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Converts the first character of a string to lowercase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeCapitalize(this string str)
        {
            if (string.IsNullOrEmpty(str) || Char.IsLower(str[0]))
                return str;

            return Char.ToLower(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Capitalizes each word and removes whitespace.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            // whitespace -> anything
            string pattern = @"(\s)(.)";

            return Regex.Replace(str, pattern, c => Char.ToUpper(c.ToString()[1]).ToString());
        }

        /// <summary>
		/// Takes a string in camel case, split it into separate words, and 
		/// capitalizes the first word.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string SplitCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            // lowercase -> not lowercase & not whitespace
            string pattern = @"(\p{Ll})((?!\s)\P{Ll})";

            return Regex.Replace(str, pattern, "$1 $2");
        }
    }
}

