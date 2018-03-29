using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace ZipPicViewUWP
{
    class PhysicalFileFilter : FileFilter
    {
        private readonly string[] coverKeywords = new string[]{ "cover", "top" };
        private static string[] ImageFileExtensions
        {
            get
            {
                if(formats == null)
                {
                    List<string> exts = new List<string>();
                    foreach(var decoderInfo in BitmapDecoder.GetDecoderInformationEnumerator())
                    {
                        exts.AddRange(decoderInfo.FileExtensions);
                    }

                    formats = exts.ToArray();
                }
                return formats;
            }
        }

        private static string[] formats = null; 

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

            foreach (var format in ImageFileExtensions)
            {
                if (filename.ToLower().EndsWith(format.ToLower())) return true;
            }

            return false;
        }
    }
}
