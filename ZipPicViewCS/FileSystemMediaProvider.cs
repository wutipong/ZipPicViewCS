using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipPicViewCS
{
    class FileSystemMediaProvider : AbstractMediaProvider
    {
        DirectoryInfo directory;
        public FileSystemMediaProvider(string path)
        {
            directory = new DirectoryInfo(path);
        }

        public override Stream OpenEntry(string entry)
        {
            var file = new FileInfo(directory.FullName + entry);
            return file.Open(FileMode.Open, FileAccess.Read);
        }

        public override string[] GetChildEntries(string entry)
        {
            string fullpath = directory.FullName;
            if (entry != @"\")
                fullpath += entry;

            var subDirectory = new DirectoryInfo(fullpath);

            var names = from f in subDirectory.GetFiles()
                        where f.Name.EndsWith(".jpg") || f.Name.EndsWith(".jpeg") || f.Name.EndsWith(".png")
                        orderby f.FullName
                        select f.FullName;
            var output = new List<string>(names.Count());

            var startIndex = directory.FullName.Length;

            foreach (var name in names)
            {
                output.Add(name.Substring(startIndex));
            }

            return output.ToArray();
        }

        public override string[] FolderEntries
        {
            get
            {
                var names = from d in directory.GetDirectories("*", SearchOption.AllDirectories)
                              orderby d.FullName
                              select d.FullName;
                var output = new List<string>(names.Count());
                output.Add(@"\");
                var startIndex = directory.FullName.Length;

                foreach (var name in names)
                {
                    output.Add(name.Substring(startIndex));
                }

                return output.ToArray();
            }
        }

    }
}
