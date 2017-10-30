using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.Helpers;

namespace ZipPicViewUWP
{
    public sealed partial class MainPage : Page
    {
        private AbstractMediaProvider provider;
        private CancellationTokenSource cancellationTokenSource;
        private string[] fileList;
        private string[] folderList;
        private int currentFileIndex = 0;
        private Task thumbnailTask = null;
        private MediaElement clickSound;
        private string filename;
        private PrintHelper printHelper;

        private string FileName
        {
            get { return filename; }
            set
            {
                filename = value;
                filenameTextBlock.Text = filename.Ellipses(100);
            }
        }

        private string selectedFolder;

        private string SelectedFolder
        {
            get { return selectedFolder; }
            set
            {
                selectedFolder = value;
                selectFolderTextBlock.Text = selectedFolder.Ellipses(50);
            }
        }

        private string currentImageFile;

        public async void SetMediaProvider(AbstractMediaProvider provider)
        {
            if (this.provider != null) this.provider.Dispose();
            this.provider = provider;
            subFolderListCtrl.Items.Clear();
            folderList = await provider.GetFolderEntries();

            await RebuildSubFolderList();

            subFolderListCtrl.SelectedIndex = 0;
            HideImageControl();
            this.IsEnabled = true;
        }

        private async Task RebuildSubFolderList()
        {
            Array.Sort(folderList, (string s1, string s2) =>
            {
                if (s1 == "\\") return -1;
                else if (s2 == "\\") return 1;
                else return s1.CompareTo(s2);
            });

            foreach (var f in folderList)
            {
                var folder = f;
                if (folder == "/") folder = "\\";
                if (folder != "\\")
                {
                    char separator = folder.Contains("\\") ? '\\' : '/';
                    int count = folder.Count(c => c == separator);

                    if (folder.EndsWith("" + separator))
                        folder = folder.Substring(0, folder.Length - 1);

                    folder = folder.Substring(folder.LastIndexOf(separator) + 1);

                    var prefix = "";
                    for (int i = 0; i < count; i++) prefix += "  ";
                    folder = prefix + folder;
                }

                var children = await provider.GetChildEntries(f);

                SoftwareBitmapSource source = null;
                if (children.Length > 0)
                {
                    var bitmap = await ImageHelper.CreateResizedBitmap(await provider.OpenEntryAsRandomAccessStreamAsync(children[0]), 40, 50);
                    source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(bitmap);
                }

                var item = new FolderListItem { Text = folder, Value = f, ImageSource = source };
                subFolderListCtrl.Items.Add(item);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
        }

        private async void page_Loaded(object sender, RoutedEventArgs e)
        {
            clickSound = await LoadSound("beep.wav");
            imageControl.OnPreCount += ImageControl_OnPreCount;
            printHelper = new PrintHelper(printPanel)
            {
                ApplicationContentMarginLeft = 0,
                ApplicationContentMarginTop = 0
            };
        }

        private void ImageControl_OnPreCount(object sender)
        {
            clickSound.Play();
        }

        private async Task<MediaElement> LoadSound(string filename)
        {
            var sound = new MediaElement();
            var soundFile = await Package.Current.InstalledLocation.GetFileAsync(String.Format(@"Assets\{0}", filename));
            sound.AutoPlay = false;
            sound.SetSource(await soundFile.OpenReadAsync(), "");
            sound.Stop();

            return sound;
        }

        private async void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");
            picker.FileTypeFilter.Add(".rar");
            picker.FileTypeFilter.Add(".7z");
            picker.FileTypeFilter.Add("*");

            var selected = await picker.PickSingleFileAsync();
            if (selected == null)
            {
                IsEnabled = true;
                return;
            }

            Stream stream = null;
            bool isEncrypted;
            try
            {
                stream = await selected.OpenStreamForReadAsync();

                var archive = ArchiveMediaProvider.TryOpenArchive(stream, null, out isEncrypted);
                if (isEncrypted)
                {
                    var dialog = new PasswordDialog();
                    var result = await dialog.ShowAsync();
                    if (result != ContentDialogResult.Primary)
                        return;
                    var password = dialog.Password;
                    archive = ArchiveMediaProvider.TryOpenArchive(stream, password, out isEncrypted);
                }

                var provider = ArchiveMediaProvider.Create(stream, archive);

                FileName = selected.Name;
                SetMediaProvider(provider);
            }
            catch (Exception err)
            {
                var dialog = new MessageDialog(String.Format("Cannot open file: {0} : {1}.", selected.Name, err.Message), "Error");
                await dialog.ShowAsync();
                stream?.Dispose();
                IsEnabled = true;
                return;
            }
        }

        private async void openFolderButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            var selected = await picker.PickSingleFolderAsync();
            if (selected == null)
            {
                IsEnabled = true;
                return;
            }
            FileName = selected.Name;

            SetMediaProvider(new FileSystemMediaProvider(selected));
        }

        private void subFolderButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = true;
        }

        private async void subFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var selected = ((FolderListItem)e.AddedItems.First()).Value;
            var provider = this.provider;

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            if (thumbnailTask != null)
                await thumbnailTask;

            thumbnailTask = CreateThumbnails(selected, provider);

            var pathToken = selected.Split(new char[] { '/', '\\' });
        }

        private async Task CreateThumbnails(string selected, AbstractMediaProvider provider)
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            thumbnailGrid.Items.Clear();
            fileList = await provider.GetChildEntries(selected);
            Array.Sort(fileList, (string s1, string s2) =>
            {
                int i1, i2;
                string s1WithoutExtension = s1.Substring(0, s1.LastIndexOf("."));
                string s2WithoutExtension = s2.Substring(0, s2.LastIndexOf("."));

                if (Int32.TryParse(s1WithoutExtension, out i1) && Int32.TryParse(s2WithoutExtension, out i2))
                {
                    return i1.CompareTo(i2);
                }
                return s1.CompareTo(s2);
            });

            try
            {
                thumbProgress.IsActive = true;
                for (int i = 0; i < fileList.Length; i++)
                {
                    var file = fileList[i];
                    var thumbnail = new Thumbnail();
                    thumbnail.Click += Thumbnail_Click;
                    thumbnail.Label.Text = file.ExtractFilename().Ellipses(25);
                    thumbnail.UserData = file;
                    thumbnailGrid.Items.Add(thumbnail);

                    var stream = await provider.OpenEntryAsRandomAccessStreamAsync(file);

                    SoftwareBitmap bitmap = await ImageHelper.CreateResizedBitmap(stream, 200, 200);
                    var source = new SoftwareBitmapSource();
                    var setSourceTask = source.SetBitmapAsync(bitmap);

                    token.ThrowIfCancellationRequested();
                    thumbnail.Image.Source = source;
                    thumbnail.ProgressRing.Visibility = Visibility.Collapsed;

                    await setSourceTask;
                    await Task.Delay(1);
                }

                thumbProgress.IsActive = false;
            }
            catch { }
            finally
            {
                thumbProgress.IsActive = false;
                cancellationTokenSource = null;
            }
        }

        private async void Thumbnail_Click(object sender, RoutedEventArgs e)
        {
            BlurBehavior.Value = 10;
            BlurBehavior.StartAnimation();
            imageControl.Visibility = Visibility.Visible;

            var file = ((Thumbnail)sender).UserData;
            currentFileIndex = Array.FindIndex(fileList, (string value) => value == file);

            await SetCurrentFile(file, false);
            thumbnailGrid.IsEnabled = false;
            splitView.IsEnabled = false;
        }

        private async Task SetCurrentFile(string file, bool withDelay = true)
        {
            currentImageFile = file;
            var delayTask = Task.Delay(withDelay ? 250 : 0);

            uint width = (uint)canvas.RenderSize.Width;
            uint height = (uint)canvas.RenderSize.Height;

            var createBitmapTask = Task.Run(async () =>
            {
                var stream = await provider.OpenEntryAsRandomAccessStreamAsync(file);
                var bitmap = await ImageHelper.CreateResizedBitmap(stream, width, height);

                return bitmap;
            });

            await delayTask;
            HideImage();
            imageControl.Filename = file.ExtractFilename();

            var source = new SoftwareBitmapSource();
            await source.SetBitmapAsync(await createBitmapTask);
            image.Source = source;

            ShowImage();
        }

        private void ShowImage()
        {
            loadingBorder.Visibility = Visibility.Collapsed;
            imageBorder.Visibility = Visibility.Visible;
            ImageTransitionBehavior.Value = 0;
            ImageTransitionBehavior.StartAnimation();
        }

        private void HideImage()
        {
            loadingBorder.Visibility = Visibility.Visible;
            ImageTransitionBehavior.Value = 10;
            ImageTransitionBehavior.StartAnimation();
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
            HideImageControl();
        }

        private void HideImageControl()
        {
            BlurBehavior.Value = 0;
            BlurBehavior.StartAnimation();
            imageBorder.Visibility = Visibility.Collapsed;
            imageControl.Visibility = Visibility.Collapsed;

            thumbnailGrid.IsEnabled = true;
            imageControl.AutoEnabled = false;
            splitView.IsEnabled = true;
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

            var picker = new FileSavePicker();

            picker.SuggestedFileName = filename.ExtractFilename();
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
            while (currentFileIndex < 0 || currentFileIndex >= fileList.Length)
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

            if (deltaX > 5)
            {
                await AdvanceImage(-1);
            }
            else if (deltaX < -5)
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

        private void page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            fullscreenButton.IsChecked = ApplicationView.GetForCurrentView().IsFullScreenMode;
        }

        private void image_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            imageControl.Visibility = imageControl.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            page.TopAppBar.Visibility = page.TopAppBar.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void imageControl_PrintButtonClick(object sender, RoutedEventArgs e)
        {
            var stream = await provider.OpenEntryAsRandomAccessStreamAsync(currentImageFile);
            var output = new BitmapImage();
            output.SetSource(stream);

            printHelper.ClearListOfPrintableFrameworkElements();
            var image = new Image()
            {
                Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0),
            };

            image.Source = output;
            printHelper.AddFrameworkElementToPrint(image);

            await printHelper.ShowPrintUIAsync("ZipPicView - " + currentImageFile.ExtractFilename());
        }
    }
}