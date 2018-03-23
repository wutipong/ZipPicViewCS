using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    internal class PathNameComparer
    {
        public static Comparison<string> FileNameComparer = (string s1, string s2) =>
        {
            string s1WithoutExtension = s1.Contains('.') ? s1.Substring(0, s1.LastIndexOf(".")) : s1;
            string s2WithoutExtension = s2.Contains('.') ? s2.Substring(0, s2.LastIndexOf(".")) : s2;

            if (Int32.TryParse(s1WithoutExtension, out int i1) && Int32.TryParse(s2WithoutExtension, out int i2))
            {
                return i1.CompareTo(i2);
            }
            return s1.CompareTo(s2);
        };

        public static Comparison<string> FolderNameComparer = (string s1, string s2) =>
        {
            if (s1 == "\\") return -1;
            else if (s2 == "\\") return 1;
            else return FileNameComparer.Invoke(s1, s2);
        };
    }
}