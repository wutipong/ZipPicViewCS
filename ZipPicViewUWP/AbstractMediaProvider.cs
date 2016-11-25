using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    public class AbstractMediaProvider : IDisposable
    {
        public virtual async Task<string[]> GetFolderEntries() { return await Task.Run(() => { return new string[0]; }); }
        public virtual async Task<string[]> GetChildEntries(string entry) { return await Task.Run(() => { return new string[0]; }); }
        public virtual async Task<Stream> OpenEntryAsync(string entry) { return await Task.Run(() => { return (Stream)null; }); }

        public virtual void Dispose() { }

        public async Task<IRandomAccessStream> OpenEntryAsRandomAccessStreamAsync(string entry)
        {
            var stream = await OpenEntryAsync(entry);

            if (stream.CanSeek)
                return stream.AsRandomAccessStream();

            else
            {
                var memoryStream = new InMemoryRandomAccessStream();
                var buffersize = 1024 * 64;
                byte[] buffer = new byte[buffersize];

                var writer = new DataWriter(memoryStream);

                while (true)
                {
                    var read = stream.Read(buffer, 0, buffersize);

                    if (read == 0) break;
                    byte[] readBuffer = new byte[read];
                    Array.Copy(buffer, readBuffer, read);
                    writer.WriteBytes(readBuffer);
                    await writer.StoreAsync();
                    await Task.Delay(13);
                }
                await writer.FlushAsync();
                writer.DetachStream();
                memoryStream.Seek(0);

                stream.Dispose();
                return memoryStream;
            }
        }
    }
}
