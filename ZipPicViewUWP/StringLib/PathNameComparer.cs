using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP.StringLib
{
    public class PathNameComparer
    {
        public static Comparison<string> FileNameComparer = (string s1, string s2) =>
        {
            var iter1 = s1.GetEnumerator();
            var iter2 = s2.GetEnumerator();

            while(iter1.MoveNext() && iter2.MoveNext())
            {
                if (iter1.Current > iter2.Current) return 1;
                if (iter1.Current > iter2.Current) return -1;
            }

            if (s1.Length > s2.Length) return 1;
            else if (s1.Length < s2.Length) return -1;
            else return 0;
        };

        public static Comparison<string> FolderNameComparer = (string s1, string s2) =>
        {
            if (s1 == "\\") return -1;
            else if (s2 == "\\") return 1;
            else return FileNameComparer.Invoke(s1, s2);
        };

        public static (int results, bool hasNext) ExtractNumber(CharEnumerator iter)
        {
            var output = 0;

            bool hasNext = true;
            for(int i = 0; char.IsDigit(iter.Current); i++)
            {
                if(i!=0)
                {
                    output *= 10;
                }
                output += int.Parse("" + iter.Current);

                if (iter.MoveNext() == false)
                {
                    hasNext = false;
                    break;
                }
            } 

            return (output, hasNext);
        }
    }
}