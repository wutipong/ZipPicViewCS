using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    class PhysicalFileFilter : FileFilter
    {
        public override bool IsImageFile(string filename)
        {
            int indexOfDot = filename.LastIndexOf(".");
            if (indexOfDot == -1) return false;

            string extension = filename.Substring(indexOfDot + 1).ToLower(); ;

            string[] formats = { "jpg", "png", "jpeg" };
            foreach (var format in formats)
            {
                if (format == extension) return true;
            }

            return false;
        }
    }
}
