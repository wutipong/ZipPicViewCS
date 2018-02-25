using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Data.Pdf;

namespace ZipPicViewUWP
{
    internal class PdfMediaProvider : AbstractMediaProvider
    {
        private PdfDocument pdfDocument;
        private StorageFile file;
        private uint pageCount = 0;
        public PdfMediaProvider(StorageFile file)
        {
            this.file = file;
            FileFilter = new PdfFileFIlter();
        }

        public async Task Load()
        {
            pdfDocument = await PdfDocument.LoadFromFileAsync(file);
            pageCount = pdfDocument.PageCount;
        }

        public override async Task<(string[], Exception error)> GetChildEntries(string entry)
        {
            return await Task.Run<(string[], Exception)>(() =>
            {
                var output = new string[pageCount];
                for (uint i = 0; i < pageCount; i++)
                {
                    output[i] = i.ToString(); 
                }

                return (output, null);
            });
        }

        public override async Task<(string[], Exception error)> GetFolderEntries()
        {
            return await Task.Run<(string[], Exception)>(() =>
            {
                return (new string[] { "/" }, null);
            });
        }

        public override Task<(Stream, Exception error)> OpenEntryAsync(string entry)
        {
            throw new NotImplementedException("not supported yet");
        }

        public override async Task<(IRandomAccessStream, Exception error)> OpenEntryAsRandomAccessStreamAsync(string entry)
        {

            var pageindex = uint.Parse(entry);

            var page = pdfDocument.GetPage(pageindex);

            var stream = new InMemoryRandomAccessStream();

            await page.RenderToStreamAsync(stream);

            return (stream, null);
        }
    }
}
