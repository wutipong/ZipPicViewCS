using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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
        private string[] fileList;
        private string[] folderList;
        private int currentFileIndex = 0;
        private Task thumbnailTask = null;
        private MediaElement clickSound;

        public async void SetMediaProvider(AbstractMediaProvider provider)
        {
            if (this.provider != null) this.provider.Dispose();
            this.provider = provider;
            subFolderListCtrl.Items.Clear();
            folderList = await provider.GetFolderEntries();

            RebuildSubFolderList();

            subFolderListCtrl.SelectedIndex = 0;
            imageControl.Visibility = Visibility.Collapsed;
            imageControl.AutoEnabled = false;
            imageBorder.Visibility = Visibility.Collapsed;
            loadingBorder.Visibility = Visibility.Collapsed;
            thumbnailGrid.IsEnabled = true;
            this.IsEnabled = true;
        }

        struct FolderListItem
        {
            private readonly string display;
            private readonly string value;

            public string Value { get { return value; } }

            public FolderListItem(string display, string value)
            {
                this.display = display;
                this.value = value;
            }

            public override string ToString()
            {
                return display;
            }
        }

        private void RebuildSubFolderList()
        {
            Array.Sort(folderList);

            foreach (var f in folderList)
            {
                var folder = f;
                if(folder != "\\")
                {
                    char separator = folder.Contains("\\") ? '\\' : '/';
                    int count = folder.Count(c => c == separator);

                    if(folder.EndsWith("" + separator))
                        folder = folder.Substring(0, folder.Length - 1);

                    folder = folder.Substring(folder.LastIndexOf(separator) + 1);

                    var prefix = "  ";
                    for (int i = 0; i < count; i++) prefix += "  ";
                    folder = prefix + "\u25CF " + folder;
                }
                var item = new FolderListItem(folder, f);
                subFolderListCtrl.Items.Add(item);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void page_Loaded(object sender, RoutedEventArgs e)
        {
            clickSound = await LoadSound("beep.wav");
            imageControl.OnPreCount += ImageControl_OnPreCount;
        }

        private void ImageControl_OnPreCount(object sender)
        {
            clickSound.Play();
        }

        private async Task<MediaElement> LoadSound(string filename)
        {
            var sound = new MediaElement();
            var soundFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(String.Format(@"Assets\{0}", filename));
            sound.AutoPlay = false;
            sound.SetSource(await soundFile.OpenReadAsync(), "");
            sound.Stop();
            
            return sound;
        }

        private async void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");
            picker.FileTypeFilter.Add(".rar");
            picker.FileTypeFilter.Add(".7z");
            picker.FileTypeFilter.Add("*");
            var selected = await picker.PickSingleFileAsync();
            if (selected == null)
            {
                this.IsEnabled = true;
                return;
            }
            
            var stream = await selected.OpenStreamForReadAsync();
            AbstractMediaProvider provider = null;
            try
            {
                provider = new ArchiveMediaProvider(stream);
            }
            catch
            {
                var dialog = new MessageDialog(String.Format("Cannot open file: {0}.", selected.Name), "Error");
                await dialog.ShowAsync();
                stream.Dispose();
                this.IsEnabled = true;
                return;
            }
            filenameTextBlock.Text = selected.Name;
            SetMediaProvider(provider);
        }

        private async void openFolderButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            var selected = await picker.PickSingleFolderAsync();
            if (selected == null)
            {
                this.IsEnabled = true;
                return;
            }
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
            var selected = ((FolderListItem) e.AddedItems.First()).Value;
            var provider = this.provider;

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();
            if (thumbnailTask != null)
                await thumbnailTask;

            thumbnailTask = CreateThumbnails(selected, provider);
        }

        private async Task CreateThumbnails(string selected, AbstractMediaProvider provider)
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            thumbnailGrid.Items.Clear();
            fileList = await provider.GetChildEntries(selected);
            Array.Sort(fileList, (string s1, string s2)=>
            {
                int i1, i2;
                string s1WithoutExtension = s1.Substring(0, s1.LastIndexOf("."));
                string s2WithoutExtension = s2.Substring(0, s2.LastIndexOf("."));

                if(Int32.TryParse(s1WithoutExtension, out i1) && Int32.TryParse(s2WithoutExtension, out i2))
                {
                    return i1.CompareTo(i2);
                }
                return s1.CompareTo(s2);
            });

            try
            {
                foreach (var file in fileList)
                {
                    var stream = await provider.OpenEntryAsRandomAccessStreamAsync(file);

                    SoftwareBitmap bitmap = await CreateResizedBitmap(stream, 200, 200);
                    var source = new SoftwareBitmapSource();
                    var setSourceTask = source.SetBitmapAsync(bitmap);

                    var thumbnail = new Thumbnail();
                    thumbnail.Image.Source = source;
                    thumbnail.Click += Thumbnail_Click;
                    if (file.Contains('\\'))
                        thumbnail.Label.Text = file.Substring(file.LastIndexOf('\\') + 1);
                    else
                        thumbnail.Label.Text = file.Substring(file.LastIndexOf('/') + 1);

                    thumbnail.UserData = file;

                    token.ThrowIfCancellationRequested();
                    thumbnailGrid.Items.Add(thumbnail);

                    await setSourceTask;
                    await Task.Delay(1);
                }
            }
            catch { }
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
            imageControl.Visibility = Visibility.Visible;

            var file = ((Thumbnail)sender).UserData;
            currentFileIndex = Array.FindIndex(fileList, (string value) => value == file);

            await SetCurrentFile(file);
            thumbnailGrid.IsEnabled = false;
        }

        private async Task SetCurrentFile(string file)
        {
            loadingBorder.Visibility = Visibility.Visible;

            var streamTask = provider.OpenEntryAsRandomAccessStreamAsync(file);
            var stream = await streamTask;

            SoftwareBitmap bitmap = await CreateResizedBitmap(stream, (uint)canvas.RenderSize.Width, (uint)canvas.RenderSize.Height);

            var source = new SoftwareBitmapSource();
            var setSourceTask = source.SetBitmapAsync(bitmap);

            image.Source = source;
            imageControl.Filename = file;

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
            thumbnailGrid.IsEnabled = true;
            imageControl.AutoEnabled = false;
        }

        private async void imageControl_NextButtonClick(object sender, RoutedEventArgs e)
        {
            await AdvanceImage(1);
        }

        private async void imageControl_PrevButtonClick(object sender, RoutedEventArgs e)
        {
            await AdvanceImage(-1);
        }

        private async void imageControl_SaveButtonClick(object sender, RoutedEventArgs e)
        {
            var filename = fileList[currentFileIndex];
            var filenameWithoutPath = filename;
            if (filenameWithoutPath.Contains("\\")) filenameWithoutPath = filenameWithoutPath.Substring(filename.LastIndexOf("\\")+1);
            else if(filenameWithoutPath.Contains("/")) filenameWithoutPath = filenameWithoutPath.Substring(filename.LastIndexOf("/")+1);

            var picker = new FileSavePicker();
            
            picker.SuggestedFileName = filename;
            picker.FileTypeChoices.Add("All", new List<string>() { "." });
            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            var output = await file.OpenStreamForWriteAsync();
            var input = await provider.OpenEntryAsync(filename);
            input.CopyTo(output);

            input.Dispose();
            output.Dispose();
        }

        private async Task AdvanceImage(int step)
        {
            currentFileIndex += step;
            while(currentFileIndex < 0 || currentFileIndex >= fileList.Length)
            {
                if (currentFileIndex < 0) currentFileIndex += fileList.Length;
                else if (currentFileIndex >= fileList.Length) currentFileIndex -= fileList.Length;
            }
            
            imageControl.ResetCounter();

            await SetCurrentFile(fileList[currentFileIndex]);
        }

        private async void image_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            var deltaX = e.Cumulative.Translation.X;

            if (deltaX > 0)
            {
                await AdvanceImage(-1);
            }
            else if (deltaX < 0)
            {
                await AdvanceImage(1);
            }
        }

        private async void page_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (imageBorder.Visibility == Visibility.Collapsed) return;
            var key = e.Key;
            if (key == VirtualKey.Left ||
                key == VirtualKey.PageUp)
            {
                await AdvanceImage(-1);
            }
            else if (key == VirtualKey.Right ||
                key == VirtualKey.PageDown ||
                key == VirtualKey.Space)
            {
                await AdvanceImage(1);
            }
            e.Handled = true;
        }

        private void fullscreenButton_Checked(object sender, RoutedEventArgs e)
        {
            fullscreenButton.Icon = new SymbolIcon(Symbol.BackToWindow);
            fullscreenButton.Label = "Exit Fullscreen";
            var view = ApplicationView.GetForCurrentView();
            if (view.TryEnterFullScreenMode())
            {
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            }
        }

        private void fullscreenButton_Unchecked(object sender, RoutedEventArgs e)
        {
            fullscreenButton.Icon = new SymbolIcon(Symbol.FullScreen);
            fullscreenButton.Label = "Fullscreen";
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
        }
    }
}
