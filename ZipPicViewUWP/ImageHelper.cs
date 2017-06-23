using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ZipPicViewUWP
{
    class ImageHelper
    {
        private enum ImageOrientation { Portrait, Landscape };

        public static async Task<SoftwareBitmap> CreateResizedBitmap(IRandomAccessStream stream, uint expectedWidth, uint expectedHeight)
        {
            var expectedOrientation = expectedWidth > expectedHeight ? ImageOrientation.Landscape : ImageOrientation.Portrait;

            var decoder = await BitmapDecoder.CreateAsync(stream);

            var width = decoder.PixelWidth;
            var height = decoder.PixelHeight;
            var imageOrientation = width > height ? ImageOrientation.Landscape : ImageOrientation.Portrait;

            if (expectedOrientation != imageOrientation)
            {
                if (imageOrientation == ImageOrientation.Landscape)
                {
                    height = (expectedWidth * height) / width;
                    width = expectedWidth;
                }
                else
                {
                    width = (expectedHeight * width) / height;
                    height = expectedHeight;
                }
            }
            else
            {
                if (imageOrientation == ImageOrientation.Landscape)
                {
                    width = (expectedHeight * width) / height;
                    height = expectedHeight;
                }
                else
                {
                    height = (expectedWidth * height) / width;
                    width = expectedWidth;
                }
            }

            var transform = new BitmapTransform();
            transform.InterpolationMode = BitmapInterpolationMode.Fant;
            transform.ScaledWidth = width;
            transform.ScaledHeight = height;

            return await decoder.GetSoftwareBitmapAsync(
                      BitmapPixelFormat.Bgra8,
                      BitmapAlphaMode.Premultiplied,
                      transform,
                      ExifOrientationMode.RespectExifOrientation,
                      ColorManagementMode.ColorManageToSRgb);
        }
    }
}
