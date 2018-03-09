using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    internal class FileSystemMediaProvider : AbstractMediaProvider
    {
        private StorageFolder folder;

        public FileSystemMediaProvider(StorageFolder folder)
        {
            this.folder = folder;
            FileFilter = new PhysicalFileFilter();
        }

        public override async Task<(Stream, Exception error)> OpenEntryAsync(string entry)
        {
            return (await folder.OpenStreamForReadAsync(entry), null);
        }

        public override async Task<(string[], Exception error)> GetChildEntries(string entry)
        {
            try
            {
                var subFolder = entry == Root ? folder : await folder.GetFolderAsync(entry);

                var files = await subFolder.GetFilesAsync();

                var output = new List<string>(files.Count);

                var startIndex = subFolder.Path.Length;

                foreach (var path in
                    from f in files
                    where FilterImageFileType(f.Name)
                    select f.Path)
                {
                    output.Add(path.Substring(folder.Path.Length + 1));
                }
                return (output.ToArray(), null);
            }
            catch (Exception e)
            {
                return (null, e);
            }
        }

        public override async Task<(string[], Exception error)> GetFolderEntries()
        {
            try
            {
                var options = new QueryOptions(CommonFolderQuery.DefaultQuery)
                {
                    FolderDepth = FolderDepth.Deep
                };

                var subFolders = await folder.CreateFolderQueryWithOptions(options).GetFoldersAsync();

                var output = new List<string>(subFolders.Count) { Root };

                var startIndex = folder.Path.Length + 1;
                foreach (var folder in subFolders)
                {
                    output.Add(folder.Path.Substring(startIndex));
                }

                return (output.ToArray(), null);
            }
            catch (Exception e)
            {
                return (null, e);
            }
        }

        public override async Task<(IRandomAccessStream, Exception error)> OpenEntryAsRandomAccessStreamAsync(string entry)
        {
            var (results, error) = await OpenEntryAsync(entry);
            if (error != null)
            {
                return (null, error);
            }
            return (results.AsRandomAccessStream(), null);
        }
    }
}