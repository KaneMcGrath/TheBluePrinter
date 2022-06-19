using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TheBluePrinter
{
    /// <summary>
    /// Covers most functions of the "Generate Printer" tab
    /// counterintuitively has no part in generating the printer which is
    /// in the Image analyzer and blueprint builder
    /// </summary>
    class GeneratePrinter
    {
        public static string ImageSourcePath = "";
        private static BitmapImage ImagePreview;
        private static string[] SupportedImageTypes = { "BMP", "GIF", "EXIF", "JPG", "PNG", "TIFF" };

        /// <summary>
        /// Updates the factorio path and loads all of the Icons
        /// </summary>
        public static void UpdateFactorioPath()
        {
            string path = WM.MainWindow.FactorioPathTextBox.Text.TrimEnd(new char[] { '/', '\\', ' '}).Trim();
            if (Directory.Exists(path + "\\data\\base"))
            {
                Settings.FactorioPath = WM.MainWindow.FactorioPathTextBox.Text;
                Log.New("Factorio game folder found " + path);
                Log.New("Loading Icons", CC.yellow);
                ResourceLoader.LoadFactorioIcons();

                ItemSelector.ReloadIcons();
            }

        }

        /// <summary>
        /// Called every time text is entered into the Image source path text box
        /// checks if the file exists and if it is an image file then attempts to load it
        /// </summary>
        public static void UpdateImageSourcePath()
        {
            if (WM.MainWindow.ImageSourcePathTextBox.Text != ImageSourcePath)
            {
                ImageSourcePath = WM.MainWindow.ImageSourcePathTextBox.Text;

                if (File.Exists(ImageSourcePath))
                {
                    string extention = ImageSourcePath.Split('.')[1].Trim().ToUpper();
                    bool flag = false;
                    foreach (string ext in SupportedImageTypes)
                    {
                        if (ext == extention) flag = true;
                    }
                    if (flag)
                    {
                        Log.New("Loading Image...");
                        LoadImagePreview();
                        
                    }
                }
            }
        }
        
        /// <summary>
        /// After the ImageSourcePath is determined to be a valid Image or the mipmap value is changed
        /// try loading the image and showing the preview
        /// This is not the one made of items, just the image you want to convert
        /// </summary>
        public static void LoadImagePreview()
        {
            try
            {
                

                Bitmap sourceImage;
                if (WM.MainWindow.FormatFactorioIconCheckbox.IsChecked == true)
                {
                    int mipmapLevel = 0;
                    if (WM.MainWindow.IconSourceResolutionSlider.Value == 64) mipmapLevel = 0;
                    if (WM.MainWindow.IconSourceResolutionSlider.Value == 32) mipmapLevel = 1;
                    if (WM.MainWindow.IconSourceResolutionSlider.Value == 16) mipmapLevel = 2;
                    if (WM.MainWindow.IconSourceResolutionSlider.Value == 8) mipmapLevel = 3;
                    sourceImage = ImageAnalyzer.FormatFactorioIconImage(new Bitmap(GeneratePrinter.ImageSourcePath), mipmapLevel);
                }
                else
                {
                    sourceImage = new Bitmap(GeneratePrinter.ImageSourcePath);
                }

                
                MemoryStream ms = new MemoryStream();
                sourceImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();

                
                //BitmapImage image = new BitmapImage();
                //image.BeginInit();
                //image.UriSource = new Uri(ImageSourcePath);
                //image.CacheOption = BitmapCacheOption.OnLoad;
                //image.EndInit();
                

                ImagePreview = image.Clone();
                ImagePreview.Freeze();
                WM.MainWindow.UserImagePreview.Source = ImagePreview;
                Log.New("Loaded Image " + ImageSourcePath);
                WM.MainWindow.SourceImageReminderLabel.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception e)
            {
                Log.New(e.Message, CC.red);
            }
        }


    }
}
