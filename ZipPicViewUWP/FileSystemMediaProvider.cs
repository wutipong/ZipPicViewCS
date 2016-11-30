using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    class FileSystemMediaProvider : AbstractMediaProvider
    {
        StorageFolder folder;

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
            var subFolder = entry == @"\" ? folder : await folder.GetFolderAsync(entry);

            var files = await folder.GetFilesAsync(CommonFileQuery.OrderByName);

            var output = new List<string>(files.Count);

            var startIndex = subFolder.Path.Length;
            var path = subFolder.Path + @"\";
            foreach (var file in
                from f in files
                where f.Name.EndsWith(".jpg") || f.Name.EndsWith(".jpeg") || f.Name.EndsWith(".png")
                select f.Name)
            {
                output.Add(path.Substring(startIndex + 1) + file);
            }
            return output.ToArray();
        }

        public override async Task<string[]> GetFolderEntries()
        {
            var subFolders = await folder.GetFoldersAsync();

            var output = new List<string>(subFolders.Count);

            output.Add(@"\");
            var startIndex = folder.Path.Length;
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
