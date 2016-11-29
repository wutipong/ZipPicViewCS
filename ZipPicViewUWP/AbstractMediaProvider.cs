using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    public class AbstractMediaProvider : IDisposable
    {
        public virtual async Task<string[]> GetFolderEntries() { return await Task.Run(() => { return new string[0]; }); }
        public virtual async Task<string[]> GetChildEntries(string entry) { return await Task.Run(() => { return new string[0]; }); }
        public virtual async Task<Stream> OpenEntryAsync(string folder, string entry) { return await Task.Run(() => { return (Stream)null; }); }

        public virtual void Dispose() { }

        public virtual async Task<IRandomAccessStream> OpenEntryAsRandomAccessStreamAsync(string folder, string entry) 
        {
            return await Task.Run(() => { return (IRandomAccessStream)null; });
        }
    }
}
