using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace BulkThumbnailCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string folderPath = string.Empty;

        public void GenerateThumbnail(string path)
        {
            var files = Directory.GetFiles(path, "*.*").Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase));
            var fileNames = RenameFiles(path);
            int fn = 0;
            foreach (string currentFile in files)
            {
                
                string name = fileNames[fn];
                System.Drawing.Image currentImg = System.Drawing.Image.FromFile(currentFile);
                Bitmap resizedImg = ResizeImage(currentImg, 1280, 720);

                using(MemoryStream memory = new MemoryStream())
                {
                    using(FileStream fs = new FileStream(path + "\\" + name, FileMode.Create, FileAccess.ReadWrite))
                    {
                        resizedImg.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                    }
                }
                resizedImg.Dispose();
                fn += 1;
            }
        }
        

        private List<string> RenameFiles(string path)
        {
            List<string> fileNames = new List<string>();
            var originalNames = Directory.GetFiles(path, "*.*").Select(System.IO.Path.GetFileName);
            foreach(string file in originalNames)
            {
                fileNames.Add(file);
            }
            
            for (int i=0; i<fileNames.Count - 1; i++)
            {
                fileNames[i] = "Thumbnail_" + fileNames[i];
            };

            return fileNames;
        }

        public Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            //maintains DPI regardless of physical size -- may increase quality when reducing image dimensions or when printing
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                //graphics.CompositingMode determines whether pixels from a source image overwrite or are combined with background pixels. SourceCopy specifies that when a color is rendered, it overwrites the background color.
                graphics.CompositingMode = CompositingMode.SourceCopy;
                //graphics.CompositingQuality determines the rendering quality level of layered images.
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                //graphics.InterpolationMode determines how intermediate values between two endpoints are calculated
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //graphics.SmoothingMode specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing) -- probably only works on vectors
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                //graphics.PixelOffsetMode affects rendering quality when drawing the new image
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using(var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        private void selectFolder_Click(object sender, RoutedEventArgs e)
        {
            

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = folderBrowserDialog.SelectedPath;
                pathName.Text = folderPath;
            }
        }

        private void GenerateThumbnails_Click(object sender, RoutedEventArgs e)
        {

            //resultsBox.Text += "Generating.\n";
            GenerateThumbnail(pathName.Text);
            //resultsBox.Text += "Complete.\n";
        }
    }
}
