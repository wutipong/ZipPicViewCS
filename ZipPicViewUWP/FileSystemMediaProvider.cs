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
        }

        public override async Task<Stream> OpenEntryAsync(string entry)
        {
            return await folder.OpenStreamForReadAsync(entry);
        }

        public override async Task<string[]> GetChildEntries(string entry)
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
            return output.ToArray();
        }

        public override async Task<string[]> GetFolderEntries()
        {
            var options = new QueryOptions(CommonFolderQuery.DefaultQuery);
            options.FolderDepth = FolderDepth.Deep;

            var subFolders = await folder.CreateFolderQueryWithOptions(options).GetFoldersAsync();

            var output = new List<string>(subFolders.Count);

            output.Add(Root);

            var startIndex = folder.Path.Length + 1;
            foreach (var folder in subFolders)
            {
                output.Add(folder.Path.Substring(startIndex));
            }

            return output.ToArray();
        }

        public override async Task<IRandomAccessStream> OpenEntryAsRandomAccessStreamAsync(string entry)
        {
            return (await OpenEntryAsync(entry)).AsRandomAccessStream();
        }
    }
}