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
        public static string ValidFactorioPath = "";
        public static Bitmap sourceImage;
        private static BitmapImage ImagePreview;
        public static string[] SupportedImageTypes = { "BMP", "GIF", "EXIF", "JPG", "PNG", "TIFF" };
        public static Color AlphaFillColor = Color.Black;
        public static int resizeX = 0;
        public static int resizeY = 0;

        public static string lastFactorioPath = "";
        /// <summary>
        /// Updates the factorio path and loads all of the Icons
        /// </summary>
        public static void UpdateFactorioPath()
        {//0:success 1:someNotLoaded 2:AllNotLoaded 3:itemsNotLoaded 4:iconsAlreadyLoaded
            string path = WM.MainWindow.FactorioPathTextBox.Text.TrimEnd(new char[] { '/', '\\', ' ' }).Trim();
            if (path == lastFactorioPath) return;
            lastFactorioPath = path;
            if (Directory.Exists(path + "\\data\\base"))
            {
                
                Settings.FactorioPath = WM.MainWindow.FactorioPathTextBox.Text;
                Log.New("Factorio game folder found " + path);
                Log.New("Loading Icons", CC.yellow);
                int succsessState = 0;
                try
                {
                    succsessState = ResourceLoader.LoadFactorioIcons();
                }
                catch (Exception ex)
                {
                    Log.New("Failed to load icons!.  " + ex.Message);
                }
                if (succsessState == 0)
                    Log.New("Icons loaded!", CC.green);
                if (succsessState == 1)
                    Log.New("Some Icons were not loaded properly!", CC.red);
                if (succsessState == 2)
                    Log.New("Icons could not be loaded!", CC.red);
                if (succsessState == 3)
                    Log.New("Items are not loaded!", CC.red);
                ItemSelector.ReloadIcons();
                ValidFactorioPath = path;
            }

        }

        /// <summary>
        /// Called every time text is entered into the Image source path text box
        /// checks if the file exists and if it is an image file, it then attempts to load it
        /// </summary>
        public static void UpdateImageSourcePath(bool reload = false)
        {
            if ((WM.MainWindow.ImageSourcePathTextBox.Text != ImageSourcePath) || reload)
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
                        Bitmap LoadedImage = new Bitmap(ImageSourcePath);
                        sourceImage = (Bitmap)LoadedImage.Clone();
                        LoadedImage.Dispose();
                        LoadImagePreview();
                    }
                    else
                    {
                        Log.New("This image type is not supported.  Supported types are [BMP, GIF, EXIF, JPG, PNG, TIFF]", CC.red);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all of the filters applied and creates an image withh all applied
        /// </summary>
        /// <returns></returns>
        public static Bitmap CreateFilteredImage(Bitmap Source)
        {
            Bitmap result = Source;
            result = ImageAnalyzer.FillAlphaChannel(result, AlphaFillColor);
            if (WM.MainWindow.FormatFactorioIconCheckbox.IsChecked == true)
            {
                int mipmapLevel = 0;
                if (WM.MainWindow.IconSourceResolutionSlider.Value == 64) mipmapLevel = 0;
                if (WM.MainWindow.IconSourceResolutionSlider.Value == 32) mipmapLevel = 1;
                if (WM.MainWindow.IconSourceResolutionSlider.Value == 16) mipmapLevel = 2;
                if (WM.MainWindow.IconSourceResolutionSlider.Value == 8) mipmapLevel = 3;
                result = ImageAnalyzer.FormatFactorioIconImage(result, mipmapLevel);
            }
            if (WM.MainWindow.ResizeImageCheckbox.IsChecked == true)
            {
                int width = 0;
                int height = 0;
                if (int.TryParse(WM.MainWindow.RICGTextBoxX.Text, out width) && int.TryParse(WM.MainWindow.RICGTextBoxY.Text, out height))
                {
                    result = ImageAnalyzer.ResizeImage(result, width, height);
                }
                else
                {
                    Log.New("Could not resize image: unable to parse width or height", CC.red);
                    return null;
                }
                
            }
            return result;
        }

        /// <summary>
        /// After the ImageSourcePath is determined to be a valid Image or the mipmap value is changed
        /// try loading the image and showing the preview
        /// This is not the one made of items, just the image you want to convert
        /// </summary>
        public static void LoadImagePreview()
        {
            if(sourceImage == null) return;

            try
            {
                Bitmap sImage = sourceImage;
                sImage = CreateFilteredImage(sourceImage);

                MemoryStream ms = new MemoryStream();
                sImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();

                sImage.Dispose();
                //BitmapImage image = new BitmapImage();
                //image.BeginInit();
                //image.UriSource = new Uri(ImageSourcePath);
                //image.CacheOption = BitmapCacheOption.OnLoad;
                //image.EndInit();


                ImagePreview = image.Clone();
                ImagePreview.Freeze();
                
                WM.MainWindow.UserImagePreview.Source = ImagePreview;
                WM.MainWindow.SourceImageReminderLabel.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception e)
            {
                Log.New(e.Message, CC.red);
            }
        }

        public static bool IsFactorioPathValid()
        {
            if (Directory.Exists(ValidFactorioPath))
            {
                return true;
            }
            return false;
        }
    }
}
