using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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
                var output = new List<string>();
                lock (archive)
                {
                    var folderEntries = from entry in archive.Entries
                                        where entry.IsDirectory
                                        orderby entry.Key
                                        select entry.Key;

                    
                    output.Add(@"\");
                    output.AddRange(folderEntries);
                }

                return output.ToArray();
            });
            
            
        }

        public override Task<string[]> GetChildEntries(string entry)
        {
            return Task.Run<string[]>(() =>
            {
                var entryLength = entry.Length;
                lock (archive)
                {
                    var imageEntries = from e in archive.Entries
                                       where (!e.IsDirectory) && (entry == @"\" || e.Key.StartsWith(entry)) && !e.Key.Substring(entryLength + 1).Contains(@"\") &&
                                       (e.Key.EndsWith(".jpg") || e.Key.EndsWith(".jpeg") || e.Key.EndsWith(".png"))
                                       orderby e.Key
                                       select e.Key.Substring(e.Key.LastIndexOf(@"\") + 1);
                    return imageEntries.ToArray<string>();
                }
            });
        }

        public override Task<Stream> OpenEntryAsync(string subfolder, string entry)
        {
            return Task.Run<Stream>(() => {
                lock (archive)
                {
                    return archive.Entries.First(e => e.Key == (subfolder + @"\" + entry)).OpenEntryStream();
                }
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            archive.Dispose();
            stream.Dispose();
        }

        public override async Task<IRandomAccessStream> OpenEntryAsRandomAccessStreamAsync(string subfolder, string entry)
        {
            using (var stream = await OpenEntryAsync(subfolder, entry))
            {
                var memoryStream = new InMemoryRandomAccessStream();
                var buffersize = 1024 * 64;
                byte[] buffer = new byte[buffersize];

                var writer = new DataWriter(memoryStream);
                lock (stream)
                {
                    while (true)
                    {
                        var read = stream.Read(buffer, 0, buffersize);

                        if (read == 0) break;
                        byte[] readBuffer = new byte[read];
                        Array.Copy(buffer, readBuffer, read);
                        writer.WriteBytes(readBuffer);
                    }
                }
                await writer.StoreAsync();
                await writer.FlushAsync();
                writer.DetachStream();
                memoryStream.Seek(0);

                return memoryStream;   
            }
        }
    }
}
