using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Casting;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.System.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using ZipPicViewUWP.StringLib;
using NaturalSort.Extension;

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
        private DisplayRequest displayRequest;
        private CastingConnection castingConnection = null;

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

        public async Task<Exception> SetMediaProvider(AbstractMediaProvider provider)
        {
            FolderReadingDialog dialog = new FolderReadingDialog();
            var t = dialog.ShowAsync(ContentDialogPlacement.Popup);
            var waitTask = Task.Delay(1000);

            if (this.provider != null) this.provider.Dispose();
            this.provider = provider;

            subFolderListCtrl.Items.Clear();
            var (list, error) = await provider.GetFolderEntries();

            if (error != null)
            {
                dialog.Hide();
                return error;
            }

            folderList = list;

            await RebuildSubFolderList();

            subFolderListCtrl.SelectedIndex = 0;
            HideImageControl();
            IsEnabled = true;

            await waitTask;

            dialog.Hide();

            return null;
        }

        private async Task RebuildSubFolderList()
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase.WithNaturalSort();
            Array.Sort(folderList, (s1,s2)=> {

                if (s1 == "\\") return -1;
                else if (s2 == "\\") return 1;
                else return comparer.Compare(s1, s2);
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

                var (children, error) = await provider.GetChildEntries(f);
                if (error != null)
                {
                    throw error;
                }

                var item = new FolderListItem { Text = folder, Value = f };
                subFolderListCtrl.Items.Add(item);

                if (children.Length > 0)
                {
                    var cover = provider.FileFilter.FindCoverPage(children);
                    if (cover != null)
                    {
                        var t = SetFolderThumbnail(cover, item);
                    }
                }
            }
        }

        private async Task SetFolderThumbnail(string entry, FolderListItem item)
        {
            SoftwareBitmapSource source = null;
            var output = await provider.OpenEntryAsRandomAccessStreamAsync(entry);

            if (output.error != null)
                throw output.error;

            var bitmap = await ImageHelper.CreateResizedBitmap(output.Item1, 40, 50);
            source = new SoftwareBitmapSource();
            await source.SetBitmapAsync(bitmap);

            item.SetImageSourceAsync(source);
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

        private async void Picker_CastingDeviceSelected(CastingDevicePicker sender, CastingDeviceSelectedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                castingConnection = args.SelectedCastingDevice.CreateCastingConnection();
                
                castingConnection.ErrorOccurred += Connection_ErrorOccurred;
                castingConnection.StateChanged += Connection_StateChanged;

                castingConnection.Source = image.GetAsCastingSource();
            });
        }

        private async void Connection_StateChanged(CastingConnection sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                inAppNotification.Show("Casting Connection State Changed: " + sender.State, 2000);
            });
        }

        private async void Connection_ErrorOccurred(CastingConnection sender, CastingConnectionErrorOccurredEventArgs args)
        {
            castingConnection = null;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                inAppNotification.Show("Casting Error Occured: " + args.Message, 2000);
            });
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
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add("*");

            var selected = await picker.PickSingleFileAsync();
            if (selected == null)
            {
                IsEnabled = true;
                return;
            }

            if (selected.FileType == ".pdf")
            {
                var provider = new PdfMediaProvider(selected);
                await provider.Load();

                await SetMediaProvider(provider);
                FileName = selected.Name;
            }
            else
            {
                Stream stream = null;
                try
                {
                    stream = await selected.OpenStreamForReadAsync();

                    var archive = ArchiveMediaProvider.TryOpenArchive(stream, null, out bool isEncrypted);
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

                    var error = await SetMediaProvider(provider);
                    if (error != null) throw error;
                    FileName = selected.Name;
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

            await SetMediaProvider(new FileSystemMediaProvider(selected));
            FileName = selected.Name;
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
            var results = await provider.GetChildEntries(selected);

            fileList = results.Item1;

            Array.Sort(fileList, StringComparer.InvariantCultureIgnoreCase.WithNaturalSort());

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
                    if (stream.error != null)
                    {
                        throw stream.error;
                    }

                    SoftwareBitmap bitmap = await ImageHelper.CreateResizedBitmap(stream.Item1, 200, 200);
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
            viewerPanel.Visibility = Visibility.Visible;

            var file = ((Thumbnail)sender).UserData;
            currentFileIndex = Array.FindIndex(fileList, (string value) => value == file);

            await SetCurrentFile(file, false);
            if (viewerPanel.Visibility == Visibility.Visible)
            {
                thumbnailGrid.IsEnabled = false;
                splitView.IsEnabled = false;
            }
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
                var bitmap = await ImageHelper.CreateResizedBitmap(stream.Item1, width, height);

                return bitmap;
            });

            await delayTask;
            HideImage();
            imageControl.Filename = file.ExtractFilename();

            var source = new SoftwareBitmapSource();
            await source.SetBitmapAsync(await createBitmapTask);
            image.Source = source;

            ShowImage();

            if (castingConnection != null)
            {
                await castingConnection.DisconnectAsync();
                await castingConnection.RequestStartCastingAsync(image.GetAsCastingSource());
            }

            if (viewerPanel.Visibility == Visibility.Collapsed)
                imageBorder.Visibility = Visibility.Collapsed;

            displayRequest = new DisplayRequest();
            displayRequest.RequestActive();

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
                if (child is FrameworkElement fe)
                {
                    fe.Width = e.NewSize.Width;
                    fe.Height = e.NewSize.Height;
                }
            }

            foreach (var child in viewerPanel.Children)
            {
                if (child is FrameworkElement fe)
                {
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
            viewerPanel.Visibility = Visibility.Collapsed;
            thumbnailGrid.IsEnabled = true;
            imageControl.AutoEnabled = false;
            splitView.IsEnabled = true;

            if (displayRequest != null)
            {
                displayRequest.RequestRelease();
                displayRequest = null;
            }
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
            var (stream, suggestedFileName, error) = await provider.OpenEntryAsync(filename);

            var picker = new FileSavePicker
            {
                SuggestedFileName = suggestedFileName
            };

            picker.FileTypeChoices.Add("All", new List<string>() { "." });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var output = await file.OpenStreamForWriteAsync();

                if (error != null)
                    throw error;

                stream.CopyTo(output);
                output.Dispose();
            }
            stream.Dispose();
            
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
            output.SetSource(stream.Item1);

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

        private void castButton_Click(object sender, RoutedEventArgs e)
        {
            var transform = castButton.TransformToVisual(Window.Current.Content as UIElement);
            var pt = transform.TransformPoint(new Point(0, 0));


            var picker = new CastingDevicePicker();
            picker.Filter.SupportsPictures = true;
            picker.CastingDeviceSelected += Picker_CastingDeviceSelected;

            picker.Show(new Windows.Foundation.Rect(pt.X, pt.Y, 100, 100), Placement.Below);
        }

        private async void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog dialog = new AboutDialog();
            await dialog.ShowAsync();
        }
    }
}