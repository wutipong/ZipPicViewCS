using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    public class AbstractMediaProvider : IDisposable
    {
        public virtual async Task<string[]> GetFolderEntries() { return await Task.Run(() => { return new string[0]; }); }
        public virtual async Task<string[]> GetChildEntries(string entry) { return await Task.Run(() => { return new string[0]; }); }
        public virtual async Task<Stream> OpenEntryAsync(string entry) { return await Task.Run(() => { return (Stream)null; }); }

        public virtual void Dispose() { }
    }
}
