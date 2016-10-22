using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewCS
{
    public abstract class AbstractMediaProvider : IDisposable
    {
        public abstract string[] FolderEntries
        {
            get;
        }

        public virtual void Dispose()
        {

        }
        public abstract string[] GetChildEntries(string entry);
        public abstract Stream OpenEntry(string entry);
    }
}
