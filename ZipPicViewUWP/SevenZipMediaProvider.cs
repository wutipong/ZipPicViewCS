using SharpCompress.Archives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZipPicViewUWP
{
    internal class SevenZipMediaProvider : ArchiveMediaProvider
    {
        private readonly List<string> keys = new List<string>();
        private readonly HashSet<string> folders = new HashSet<string>();

        private const string pathSeparator = "/";
        private const string root = pathSeparator;

        public SevenZipMediaProvider(Stream stream, IArchive archive) : base(stream, archive)
        {
            foreach (var entry in archive.Entries)
            {
                keys.Add(entry.Key);
                folders.Add(entry.Key.Substring(0, entry.Key.LastIndexOf(pathSeparator)));
                folders.Add(root);
            }
        }

        public override async Task<string[]> GetFolderEntries()
        {
            return await Task.Run(() => folders.ToArray());
        }

        public override Task<string[]> GetChildEntries(string entry)
        {
            return Task.Run(() =>
            {
                LinkedList<string> output = new LinkedList<string>();

                if (Archive == null) return output.ToArray();
                foreach (var key in keys)
                {
                    if(FilterKey(key, entry))
                        output.AddLast(key);
                }

                return output.ToArray();
            });
        }

        protected bool FilterKey(string key, string folder)
        {
            folder = folder == root ? "" : folder;

            if (!key.StartsWith(folder)) return false;

            var innerKey = key.Substring(folder.Length + 1);

            if (innerKey.Contains(pathSeparator)) return false;

            var lower = innerKey.ToLower();

            if (!lower.EndsWith(".jpg") && !lower.EndsWith(".png") && !lower.EndsWith(".jpeg")) return false;

            return true;
        }
    }
}