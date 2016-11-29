using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public override async Task<Stream> OpenEntryAsync(string subfolder, string entry)
        {
            return await folder.OpenStreamForReadAsync(subfolder + @"\" + entry);
        }

        public override async Task<string[]> GetChildEntries(string entry)
        {
            var subFolder = entry == @"\" ? folder : await folder.GetFolderAsync(entry);

            var files = await subFolder.GetFilesAsync();

            var output = new List<string>(files.Count);

            var startIndex = subFolder.Path.Length;
            var path = subFolder.Path + @"\";
            foreach (var f in files)
            {
                Debug.WriteLine(f.Name);
            }
            foreach (var file in
                from f in files
                where f.Name.EndsWith(".jpg") || f.Name.EndsWith(".jpeg") || f.Name.EndsWith(".png")
                select f.Name)
            {
                output.Add(file);
            }
            return output.ToArray();
        }

        public override async Task<string[]> GetFolderEntries()
        {
            QueryOptions options = new QueryOptions(CommonFolderQuery.DefaultQuery);
            options.FolderDepth = FolderDepth.Deep;
            var subFolders = await folder.CreateFolderQueryWithOptions(options).GetFoldersAsync();

            var output = new List<string>(subFolders.Count);

            output.Add(@"\");
            var startIndex = folder.Path.Length + 1;
            foreach (var folder in subFolders)
            {
                output.Add(folder.Path.Substring(startIndex));
            }
            output.Sort();

            return output.ToArray();
        }

        public override async Task<IRandomAccessStream> OpenEntryAsRandomAccessStreamAsync(string subfolder, string entry)
        {
            return (await OpenEntryAsync(subfolder, entry)).AsRandomAccessStream();
        }
    }
}
