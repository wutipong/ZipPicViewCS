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

        public abstract Task<(Stream stream, string suggestedFileName, Exception error)> OpenEntryAsync(string entry);

        public FileFilter FileFilter { get; protected set; }

        public virtual void Dispose()
        {
        }

        public abstract Task<(IRandomAccessStream, Exception error)> OpenEntryAsRandomAccessStreamAsync(string entry);

        public bool FilterImageFileType(string entryName)
        {
            if (FileFilter == null) return true;
            return FileFilter.IsImageFile(entryName);
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