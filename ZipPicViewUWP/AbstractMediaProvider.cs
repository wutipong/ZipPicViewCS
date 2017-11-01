using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    public abstract class AbstractMediaProvider : IDisposable
    {
        public abstract Task<(string[], Exception error)> GetFolderEntries();

        public abstract Task<(string[], Exception error)> GetChildEntries(string entry);

        public abstract Task<(Stream, Exception error)> OpenEntryAsync(string entry);

        public virtual void Dispose()
        {
        }

        public abstract Task<(IRandomAccessStream, Exception error)> OpenEntryAsRandomAccessStreamAsync(string entry);

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