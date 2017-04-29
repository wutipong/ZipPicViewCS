using SharpCompress.Archives;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    internal class ArchiveMediaProvider : AbstractMediaProvider
    {
        protected IArchive Archive { get; set; }

        protected string[] FileList
        {
            get
            {
                if (fileList == null) fileList = CreateFileList();
                return fileList;
            }
        }

        protected string[] FolderList
        {
            get
            {
                if (folderList == null) folderList = CreateFolderList();
                return folderList;
            }
        }

        private string[] fileList;
        private string[] folderList;

        private Stream stream;

        public static bool IsArchiveEncrypted(Stream stream)
        {
            var archive = ArchiveFactory.Open(stream);
            var entry = archive.Entries.First(e => !e.IsDirectory);

            if (entry != null && entry.IsEncrypted)
                return true;

            return false;
        }

        public static void TestPassword(Stream stream, string password)
        {
            var options = new ReaderOptions
            {
                Password = password
            };

            var archive = ArchiveFactory.Open(stream, options);
            var entry = archive.Entries.First(e => !e.IsDirectory);

            if (entry != null)
            {
                using (var entryStream = entry.OpenEntryStream())
                {

                }
                
            }
            stream.Seek(0, SeekOrigin.Begin);
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
            this.Archive = archive;
            this.stream = stream;

            if (archive.Type == SharpCompress.Common.ArchiveType.Rar)
                Separator = '\\';
            else
                Separator = '/';
        }

        public override async Task<string[]> GetFolderEntries()
        {
            return await Task.Run(() => FolderList);
        }

        protected virtual string[] CreateFolderList()
        {
            var output = new List<string>();
            lock (Archive)
            {
                if (Archive != null)
                {
                    var folderEntries = from entry in Archive.Entries
                                        where entry.IsDirectory
                                        orderby entry.Key
                                        select entry.Key;

                    output.Add(Root);
                    output.AddRange(folderEntries);
                }
            }

            return output.ToArray();
        }

        public override Task<string[]> GetChildEntries(string entry)
        {
            return Task.Run(() =>
            {
                var entryLength = entry.Length;
                LinkedList<string> output = new LinkedList<string>();
                var folder = entry == Root ? "" : entry;

                foreach (var file in FileList)
                {
                    if (!file.StartsWith(folder)) continue;

                    var innerKey = file.Substring(folder.Length + 1);
                    if (innerKey.Contains(Separator)) continue;

                    if (FilterImageFileType(innerKey)) output.AddLast(file);
                }

                return output.ToArray();
            });
        }

        protected virtual string[] CreateFileList()
        {
            List<string> files = new List<string>();
            lock (Archive)
            {
                foreach (var e in Archive.Entries)
                {
                    if (e.IsDirectory) continue;
                    files.Add(e.Key);
                }
            }
            return files.ToArray();
        }

        public override Task<Stream> OpenEntryAsync(string entry)
        {
            return Task.Run<Stream>(() =>
            {
                var outputStream = new MemoryStream();
                if (Archive == null) return outputStream;
                lock (Archive)
                {
                    using (var entryStream = Archive.Entries.First(e => e.Key == entry).OpenEntryStream())
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
            lock (Archive)
            {
                Archive.Dispose();
                Archive = null;
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