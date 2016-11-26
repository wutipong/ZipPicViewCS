using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ZipPicViewUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AbstractMediaProvider provider;
        private CancellationTokenSource cancellationTokenSource;

        public async void SetMediaProvider(AbstractMediaProvider provider)
        {
            if (this.provider != null) this.provider.Dispose();
            this.provider = provider;
            subFolderList.Items.Clear();
            var folders = await provider.GetFolderEntries();

            foreach (var folder in folders)
            {
                subFolderList.Items.Add(folder);
            }

            subFolderList.SelectedIndex = 0;
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");
            picker.FileTypeFilter.Add(".rar");
            picker.FileTypeFilter.Add(".7z");
            picker.FileTypeFilter.Add("*");
            var selected = await picker.PickSingleFileAsync();
            if (selected == null) return;
            filenameTextBlock.Text = selected.Name;
            var stream = await selected.OpenStreamForReadAsync();
            SetMediaProvider(new ArchiveMediaProvider(stream));
        }

        private async void openFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            var selected = await picker.PickSingleFolderAsync();
            if (selected == null) return;
            filenameTextBlock.Text = selected.Name;

            SetMediaProvider(new FileSystemMediaProvider(selected));
        }

        private void subFolderButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = true;
        }

        private async void subFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var selected = (String)e.AddedItems.First();
            var fileList = await provider.GetChildEntries(selected);

            if (cancellationTokenSource != null) cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            thumbnailGrid.Items.Clear();
            try
            {
                foreach (var file in fileList)
                {
                    token.ThrowIfCancellationRequested();

                    var stream = await provider.OpenEntryAsRandomAccessStreamAsync(file);

                    SoftwareBitmap bitmap = await CreateResizedBitmap(stream, 200, 200);
                    var source = new SoftwareBitmapSource();
                    var setSourceTask = source.SetBitmapAsync(bitmap);

                    var thumbnail = new Thumbnail();
                    thumbnail.Image.Source = source;
                    thumbnail.Click += Thumbnail_Click;
                    thumbnail.Label.Text = file;

                    thumbnailGrid.Items.Add(thumbnail);

                    await setSourceTask;
                    await Task.Delay(1);
                }
            }
            catch (OperationCanceledException) { }
            finally { cancellationTokenSource = null; }
        }

        private enum ImageOrientation { Portrait, Landscape};

        private static async Task<SoftwareBitmap> CreateResizedBitmap(IRandomAccessStream stream, uint expectedWidth, uint expectedHeight)
        {
            var expectedOrientation = expectedWidth > expectedHeight? ImageOrientation.Landscape: ImageOrientation.Portrait;
            
            var decoder = await BitmapDecoder.CreateAsync(stream);
            
            var width = decoder.PixelWidth;
            var height = decoder.PixelHeight;
            var imageOrientation = width > height? ImageOrientation.Landscape : ImageOrientation.Portrait;

            if(expectedOrientation != imageOrientation)
            {
                if(imageOrientation == ImageOrientation.Landscape)
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

        private async void Thumbnail_Click(object sender, RoutedEventArgs e)
        {
            loadingBorder.Visibility = Visibility.Visible;
            imageControl.Visibility = Visibility.Visible;

            var file = ((Thumbnail)((Button)e.OriginalSource).Parent).Label.Text;
            imageControl.Filename = file;

            var streamTask = provider.OpenEntryAsRandomAccessStreamAsync(file);
            var stream = await streamTask;

            SoftwareBitmap bitmap = await CreateResizedBitmap(stream, (uint)canvas.RenderSize.Width, (uint)canvas.RenderSize.Height);

            var source = new SoftwareBitmapSource();
            var setSourceTask = source.SetBitmapAsync(bitmap);
            
            image.Source = source;
            await setSourceTask;
            loadingBorder.Visibility = Visibility.Collapsed;
            imageBorder.Visibility = Visibility.Visible;
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in canvas.Children)
            {
                if (child is FrameworkElement)
                {
                    var fe = (FrameworkElement)child;
                    fe.Width = e.NewSize.Width;
                    fe.Height = e.NewSize.Height;
                }
            }
        }

        private void imageControl_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            imageBorder.Visibility = Visibility.Collapsed;
            imageControl.Visibility = Visibility.Collapsed;
        }
    }
}
