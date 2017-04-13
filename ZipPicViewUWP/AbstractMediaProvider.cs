using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    public class AbstractMediaProvider : IDisposable
    {
        public virtual async Task<string[]> GetFolderEntries()
        {
            return await Task.Run(() => { return new string[0]; });
        }

        public virtual async Task<string[]> GetChildEntries(string entry)
        {
            return await Task.Run(() => { return new string[0]; });
        }

        public virtual async Task<Stream> OpenEntryAsync(string entry)
        {
            return await Task.Run(() => { return (Stream)null; });
        }

        public virtual void Dispose()
        {
        }

        public virtual async Task<IRandomAccessStream> OpenEntryAsRandomAccessStreamAsync(string entry)
        {
            return await Task.Run(() => { return (IRandomAccessStream)null; });
        }

        public bool FilterImageFileType(string entryName)
        {
            int indexOfDot = entryName.LastIndexOf(".");
            if (indexOfDot == -1) return false;

            string extension = entryName.Substring(indexOfDot + 1).ToLower(); ;

            string[] formats = { "jpg", "png", "jpeg" };
            foreach (var format in formats)
            {
                if (format == extension) return true;
            }

            return false;
        }

        protected string Root
        {
            get { return @"\"; }
        }

        protected char Separator
        {
            get; set;
        }
    }
}