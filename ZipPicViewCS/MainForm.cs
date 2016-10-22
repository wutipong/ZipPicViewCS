using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpCompress.Archives;
using System.Collections.Generic;

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

                mediaProvider = value;

                folderBox.Items.Clear();
                folderBox.Items.AddRange(this.MediaProvider.FolderEntries);
                
                folderBox.SelectedIndex = 0;
            }
        }

        private AbstractMediaProvider mediaProvider;

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
                    button.Image = CreateThumbnail(Image.FromStream(stream), 200, 200);
                    button.Size = button.Image.Size;
                    button.TextAlign = ContentAlignment.BottomCenter;

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
            }
        }
    }
}
