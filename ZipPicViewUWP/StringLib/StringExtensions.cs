using System;

namespace ZipPicViewUWP.StringLib
{
    public static class StringExtensions
    {
        public static string ExtractFilename(this string path)
        {
            int index = path.LastIndexOfAny(new char[] { '\\', '/' });
            return index >= 0 ? path.Substring(index + 1) : path;
        }

        public static string Ellipses(this string input, int length)
        {
            if (input.Length > length)
            {
                return String.Format("{0} … {1}",
                    input.Substring(0, length / 2 - 3), input.Substring(input.Length - length / 2));
            }
            else
            {
                return input;
            }
        }
    }
}