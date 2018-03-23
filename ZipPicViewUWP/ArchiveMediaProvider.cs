using SharpCompress.Archives;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using ZipPicViewUWP.StringLib;

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

        public static IArchive TryOpenArchive(Stream stream, string password, out bool isEncrypted)
        {
            isEncrypted = false;
            var options = new ReaderOptions
            {
                Password = password
            };

            var archive = ArchiveFactory.Open(stream, options);

            var entry = archive.Entries.First(e => !e.IsDirectory);

            if (entry != null)
            {
                isEncrypted = entry.IsEncrypted;
                if (isEncrypted && password == null) return null;

                // Try open a stream to see if it can be opened
                using (var entryStream = entry.OpenEntryStream())
                {
                }
            }

            return archive;
        }

        public static ArchiveMediaProvider Create(Stream stream, IArchive archive)
        {
            if (archive.Type == SharpCompress.Common.ArchiveType.SevenZip)
                return new SevenZipMediaProvider(stream, archive);
            return new ArchiveMediaProvider(stream, archive);
        }

        public ArchiveMediaProvider(Stream stream, IArchive archive)
        {
            this.Archive = archive;
            this.stream = stream;

            Separator = DetermineSeparator();
            FileFilter = new PhysicalFileFilter();
        }

        private char DetermineSeparator()
        {
            foreach(var entry in Archive.Entries)
            {
                if (entry.Key.Contains('\\')) {
                    return '\\';
                }
            }

            return '/';
        }

        public override async Task<(string[], Exception error)> GetFolderEntries()
        {
            return await Task.Run<(string[], Exception)>(() =>
            {
                try
                {
                    return (FolderList, null);
                }
                catch (Exception e)
                {
                    return (null, e);
                }
            });
        }

        protected virtual string[] CreateFolderList()
        {
            var output = new List<string>();
            Exception exception = null;
            lock (Archive)
            {
                if (Archive != null)
                {
                    try
                    {
                        var folderEntries = from entry in Archive.Entries
                                            where entry.IsDirectory
                                            orderby entry.Key
                                            select entry.Key;

                        output.Add(Root);
                        output.AddRange(folderEntries);

                        foreach(var entry in Archive.Entries)
                        {
                            var key = entry.Key;
                            var separatorIndex = key.LastIndexOf(Separator);
                            if (separatorIndex < 0) continue;

                            var parent = key.Substring(0, separatorIndex +1);

                            if (!output.Contains(parent))
                                output.Add(parent);
                        }
                    }
                    catch (Exception err)
                    {
                        exception = err;
                    }
                }
            }

            if (exception != null)
                throw exception;
            return output.ToArray();
        }

        public override Task<(string[], Exception error)> GetChildEntries(string entry)
        {
            return Task.Run<(string[], Exception)>(() =>
            {
                try
                {
                    var entryLength = entry.Length;
                    var output = new LinkedList<string>();
                    var folder = entry == Root ? "" : entry;

                    foreach (var file in FileList)
                    {
                        if (!file.StartsWith(folder)) continue;

                        var innerKey = file.Substring(folder.Length + 1);
                        if (innerKey.Contains(Separator)) continue;

                        if (FilterImageFileType(innerKey)) output.AddLast(file);
                    }

                    return (output.ToArray(), null);
                }
                catch (Exception e)
                {
                    return (null, e);
                }
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

        public override Task<(Stream stream, string suggestedFileName, Exception error)> OpenEntryAsync(string entry)
        {
            return Task.Run<(Stream, string, Exception)>(() =>
            {
                var outputStream = new MemoryStream();
                if (Archive == null) return (null, null, new Exception("Cannot Read Archive"));
                try
                {
                    lock (Archive)
                    {
                        using (var entryStream = Archive.Entries.First(e => e.Key == entry).OpenEntryStream())
                        {
                            entryStream.CopyTo(outputStream);
                            outputStream.Seek(0, SeekOrigin.Begin);
                        }
                        return (outputStream, entry.ExtractFilename(), null);
                    }
                }
                catch (Exception e)
                {
                    return (null, null, e);
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

        public override async Task<(IRandomAccessStream, Exception error)> OpenEntryAsRandomAccessStreamAsync(string entry)
        {
            try
            {
                var (stream, name, error) = await OpenEntryAsync(entry);
                return (stream.AsRandomAccessStream(), error);
            }
            catch (Exception e)
            {
                return (null, e);
            }
        }
    }
}