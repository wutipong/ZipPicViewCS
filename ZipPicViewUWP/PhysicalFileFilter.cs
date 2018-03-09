using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    class PhysicalFileFilter : FileFilter
    {
        private readonly string[] coverKeywords = new string[]{ "cover", "top" };

        public override string FindCoverPage(string[] filenames)
        {
            if (filenames.Length <= 0) return null;
            foreach(var keyword in coverKeywords){
                var name = filenames.FirstOrDefault((s) => s.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                if (name != null) return name;
            }

            return filenames[0];
            
        }

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
