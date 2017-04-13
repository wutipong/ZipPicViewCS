using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    class ArchiveMediaProvider : AbstractMediaProvider
    {
        IArchive archive;
        protected IArchive Archive
        {
            get { return archive; }
        }
        Stream stream;

        public static bool IsArchiveEncrypted(Stream stream)
        {
            var archive = ArchiveFactory.Open(stream);
            var entry = archive.Entries.First(e => !e.IsDirectory);

            if (entry != null && entry.IsEncrypted)
                return true;
            
            return false;
        }

        public static ArchiveMediaProvider Create(Stream stream, string password)
        {            
            var options = new ReaderOptions()
            {
                Password = password
            };
            var archive = ArchiveFactory.Open(stream, options);

            if (archive.Type == SharpCompress.Common.ArchiveType.SevenZip)
                return new SevenZipMediaProvider(stream, archive);
            return new ArchiveMediaProvider(stream, archive);
        }

        public ArchiveMediaProvider(Stream stream, IArchive archive)
        {
            this.archive = archive;
            this.stream = stream;
        }

        public override async Task<string[]> GetFolderEntries()
        {
            return await Task.Run<string[]>(() =>
            {
                var output = new List<string>();
                lock (archive)
                {
                    if (archive == null) return output.ToArray();
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
                LinkedList<string> output = new LinkedList<string>();
                var folder = entry == "\\" ? "" : entry;
                lock (archive)
                {
                    if (archive == null) return output.ToArray();
                    foreach (var e in archive.Entries)
                    {
                        if (e.IsDirectory) continue;
                        if (!e.Key.StartsWith(folder)) continue;

                        var innerKey = e.Key.Substring(folder.Length + 1);

                        if (innerKey.Contains("/") || innerKey.Contains('\\')) continue;

                        var lower = innerKey.ToLower();

                        if (!lower.EndsWith(".jpg") && !lower.EndsWith(".png") && !lower.EndsWith(".jpeg")) continue;
                        output.AddLast(e.Key);
                    }
                }

                return output.ToArray();
            });
        }

        public override Task<Stream> OpenEntryAsync(string entry)
        {
            return Task.Run<Stream>(() => {
                var outputStream = new MemoryStream();
                if (archive == null) return outputStream;
                lock (archive)
                {
                    using (var entryStream = archive.Entries.First(e => e.Key == entry).OpenEntryStream())
                    {
                        entryStream.CopyTo(outputStream);
                        outputStream.Seek(0, SeekOrigin.Begin);
                    }
                    return outputStream;
                }
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            lock (archive)
            {
                archive.Dispose();
                archive = null;
            }
            lock (stream)
            {
                stream.Dispose();
                stream = null;
            }
        }

        public override async Task<IRandomAccessStream> OpenEntryAsRandomAccessStreamAsync(string entry)
        {
            var stream = await OpenEntryAsync(entry);
            return stream.AsRandomAccessStream();

        }
    }
}
