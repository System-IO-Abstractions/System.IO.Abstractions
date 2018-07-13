﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;

namespace System.IO.Abstractions.TestingHelpers
{
    public static class StringExtensions
    {
        [Pure]
        public static string[] SplitLines(this string input)
        {
            var list = new List<string>();
            using (var reader = new StringReader(input))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    list.Add(str);
                }
            }

            return list.ToArray();
        }

        [Pure]
        public static string Replace(this string source, string oldValue, string newValue, StringComparison comparisonType)
        {
            // from http://stackoverflow.com/a/22565605 with some adaptions
            if (string.IsNullOrEmpty(oldValue))
            {
                throw new ArgumentNullException(nameof(oldValue));
            }

            if (source.Length == 0)
            {
                return source;
            }

            if (newValue == null)
            {
                newValue = string.Empty;
            }

            var result = new StringBuilder();
            int startingPos = 0;
            int nextMatch;
            while ((nextMatch = source.IndexOf(oldValue, startingPos, comparisonType)) > -1)
            {
                result.Append(source, startingPos, nextMatch - startingPos);
                result.Append(newValue);
                startingPos = nextMatch + oldValue.Length;
            }

            result.Append(source, startingPos, source.Length - startingPos);

            return result.ToString();
        }

        [Pure]
        public static string TrimSlashes(this string path)
        {
            if (path == null)
            {
                return null;
            }

            if (MockUnixSupport.IsUnixPlatform() && path == "/")
            {
                return path;
            }

            var trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (!MockUnixSupport.IsUnixPlatform()
                && trimmed.Length == 2
                && Char.IsLetter(trimmed[0])
                && trimmed[1] == ':')
            {
                return trimmed + Path.DirectorySeparatorChar;
            }

            return trimmed;
        }
    }
}