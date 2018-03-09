using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    class PdfFileFIlter : FileFilter
    {
        public override bool IsImageFile(string filename)
        {
            return true;
        }
    }
}
