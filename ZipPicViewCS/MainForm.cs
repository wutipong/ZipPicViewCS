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
            }
        }

        private AbstractMediaProvider mediaProvider;
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

            this.MediaProvider = new ArchiveMediaProvider(openFileDialog.FileName);
            this.Text = openFileDialog.FileName;
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = String.Format("ZipPicView : {0}", value);
            }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            this.MediaProvider = new FileSystemMediaProvider(folderBrowserDialog.SelectedPath);
            this.Text = folderBrowserDialog.SelectedPath;
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
            var fileList = this.MediaProvider.GetChildEntries(folderBox.SelectedItem.ToString());

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

        private void thumbnailBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            var entry = e.Argument as string;
            try
            {
                var fileList = this.MediaProvider.GetChildEntries(entry);

                for (int i = 0; i<fileList.Length; i++)
                {
                    var file = fileList[i];
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    
                    var stream = MediaProvider.OpenEntry(file);

                    var image = CreateThumbnail(Image.FromStream(stream), 200, 200);
                    
                    stream.Close();
                    imageList.Add(image);
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
    }
}
