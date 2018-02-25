using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    public abstract class FileFilter
    {
        public abstract bool IsImageFile(string filename);
    }
}
