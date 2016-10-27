using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ZipPicViewCS
{
    public partial class MainForm : Form
    {
        private string[] fileList = null;
        private int selectedIndex = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        public AbstractMediaProvider MediaProvider
        {
            get { return mediaProvider; }
            set {
                if (mediaProvider != null)
                    mediaProvider.Dispose();
                if(thumbnailBackgroundWorker.IsBusy)
                    thumbnailBackgroundWorker.CancelAsync();

                mediaProvider = value;

                folderBox.Items.Clear();
                folderBox.Items.AddRange(this.MediaProvider.FolderEntries);
                
                folderBox.SelectedIndex = 0;

                tabControl1.SelectedIndex = 0;

                viewerPictureBox.Image = null;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }

            set
            {
                selectedIndex = value;
                lock (MediaProvider)
                {
                    var stream = MediaProvider.OpenEntry(fileList[selectedIndex]);
                    var image = Image.FromStream(stream);

                    viewerPictureBox.Image = image;
                }
            }
        }

        private AbstractMediaProvider mediaProvider = new FileSystemMediaProvider(".");
        private List<Image> imageList = new List<Image>();
     

        private Image CreateThumbnail(Image image, int maxWidth, int maxHeight, bool disposeOriginal = true)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            if (disposeOriginal)
                image.Dispose();

            return newImage;
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            LoadArchive(sender, e);
        }

        private void archiveMenuItem_Click(object sender, EventArgs e)
        {
            LoadArchive(sender, e);
        }

        private void LoadArchive(object sender, EventArgs e)
        {
            var results = openFileDialog.ShowDialog();
            if (results != DialogResult.OK) return;

            lock (MediaProvider)
            {
                this.MediaProvider = new ArchiveMediaProvider(openFileDialog.FileName);
            }
            pathLabel.Text = openFileDialog.FileName;
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;
            lock (MediaProvider)
            {
                this.MediaProvider = new FileSystemMediaProvider(dialog.FileName);
            }
            pathLabel.Text = dialog.FileName;
        }

        private void folderBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (thumbnailBackgroundWorker.IsBusy)
                thumbnailBackgroundWorker.CancelAsync();
            else
                RecreateThumbnail();

        }

        private void RecreateThumbnail()
        {
            lock (MediaProvider)
            {
                fileList = this.MediaProvider.GetChildEntries(folderBox.SelectedItem.ToString());

                var buttonList = new List<Button>();
                try
                {
                    foreach (var file in fileList)
                    {
                        var stream = MediaProvider.OpenEntry(file);

                        var button = new Button();

                        button.Text = file.Substring(file.LastIndexOf('\\') + 1);
                        button.BackColor = SystemColors.ControlDark;

                        button.Size = new Size(200, 200);
                        button.TextAlign = ContentAlignment.BottomCenter;
                        button.Name = file;
                        button.Click += ImageButton_Click;

                        stream.Close();

                        buttonList.Add(button);
                    }
                }
                catch (NotSupportedException err)
                {
                    MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonList.Clear();
                }
                finally
                {
                    thmbnailPanel.Controls.Clear();
                    thmbnailPanel.Controls.AddRange(buttonList.ToArray());
                    imageList.Clear();

                    thumbnailBackgroundWorker.RunWorkerAsync(folderBox.SelectedItem.ToString());
                }
            }
        }

        private void ImageButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            SelectedIndex = Array.IndexOf(fileList, button.Name);
            tabControl1.SelectedIndex = 1;
        }

        private void thumbnailBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            var entry = e.Argument as string;
            try
            {
                for (int i = 0; i<fileList.Length; i++)
                {
                    var file = fileList[i];
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }

                    lock (MediaProvider)
                    {
                        var stream = MediaProvider.OpenEntry(file);

                        var image = CreateThumbnail(Image.FromStream(stream), 200, 200);

                        stream.Close();
                        imageList.Add(image);
                    }
                    worker.ReportProgress((i * 100) / fileList.Length);   
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
            
        }

        private void thumbnailBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100)
            {
                int index = imageList.Count - 1;
                var button = thmbnailPanel.Controls[index] as Button;
                button.Image = imageList[index];

            }
            thumbnailProgressBar.Value = e.ProgressPercentage;
        }

        private void thumbnailBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled) thumbnailProgressBar.Value = 100;
            else RecreateThumbnail();
        }

        private void zoomFitButton_Click(object sender, EventArgs e)
        {
            viewerPictureBox.SizeMode = zoomFitButton.Checked ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.AutoSize;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (SelectedIndex == 0) SelectedIndex = fileList.Length - 1;
            else SelectedIndex--;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (SelectedIndex == fileList.Length -1) SelectedIndex = 0;
            else SelectedIndex++;
        }
    }
}
