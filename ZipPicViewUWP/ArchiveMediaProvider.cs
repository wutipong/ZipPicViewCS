using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    class ArchiveMediaProvider : AbstractMediaProvider
    {
        IArchive archive;
        Stream stream;
       
        public ArchiveMediaProvider(Stream stream)
        {
            this.stream = stream;
            archive = ArchiveFactory.Open(stream);
        }
        public override async Task<string[]> GetFolderEntries()
        {
            return await Task.Run<string[]>(() =>
            {
                var folderEntries = from entry in archive.Entries
                                    where entry.IsDirectory
                                    orderby entry.Key
                                    select entry.Key;

                var output = new List<string>();
                output.Add(@"\");
                output.AddRange(folderEntries);

                return output.ToArray();
            });
            
            
        }

        public override Task<string[]> GetChildEntries(string entry)
        {
            return Task.Run<string[]>(() =>
            {
                var entryLength = entry.Length;

                var imageEntries = from e in archive.Entries
                                   where (!e.IsDirectory) && (entry == @"\" || e.Key.StartsWith(entry)) && !e.Key.Substring(entryLength + 1).Contains(@"\") &&
                                   (e.Key.EndsWith(".jpg") || e.Key.EndsWith(".jpeg") || e.Key.EndsWith(".png"))
                                   orderby e.Key
                                   select e.Key;

                return imageEntries.ToArray<string>();
            });
        }

        public override Task<Stream> OpenEntryAsync(string entry)
        {
            return Task.Run<Stream>(() => { return archive.Entries.First(e => e.Key == entry).OpenEntryStream(); });
        }

        public override void Dispose()
        {
            base.Dispose();
            archive.Dispose();
            stream.Dispose();
        }
    }
}
