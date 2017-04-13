using SharpCompress.Archives;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZipPicViewUWP
{
    internal class SevenZipMediaProvider : ArchiveMediaProvider
    {
        public SevenZipMediaProvider(Stream stream, IArchive archive) : base(stream, archive)
        {
            Separator = '/';
        }

        protected override string[] CreateFileList()
        {
            var output = new LinkedList<string>();

            if (Archive != null)
            {
                lock (Archive)
                {
                    foreach (var entry in Archive.Entries)
                    {
                        output.AddLast(entry.Key);
                    }
                }
            }

            return output.ToArray();
        }

        protected override string[] CreateFolderList()
        {
            var folders = new HashSet<string>();
            folders.Add(Root);

            foreach (var entry in FileList)
            {
                var parts = entry.Split(Separator);

                if (parts.Length == 1) continue;

                string path = parts[0];
                folders.Add(path);

                for (int i = 1; i < parts.Length - 1; i++)
                {
                    path += Separator + parts[i];
                    folders.Add(path);
                }
            }

            return folders.ToArray();
        }
    }
}