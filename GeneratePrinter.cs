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
        /// After the ImageSourcePath is determined to be a valid Image
        /// try loading the image and showing the preview
        /// there appears to be memory issues with the bitmap cache 
        /// but unless you load like 100 images consecutivly it shouldent be too much of an issue
        /// </summary>
        public static void LoadImagePreview()
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(ImageSourcePath);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                ImagePreview = image.Clone();
                ImagePreview.Freeze();
                WM.MainWindow.UserImagePreview.Source = ImagePreview;
                Log.New("Loaded Image " + ImageSourcePath);
            }
            catch (Exception e)
            {
                Log.New(e.Message, CC.red);
            }
        }


    }
}
