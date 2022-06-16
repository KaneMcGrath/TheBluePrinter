﻿using System;
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
        public static string IconSourcePath = "";
        public static bool TF_ImageSource_IconSource = true;
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
        /// Called every time text is entered into the icon source path
        /// </summary>
        public static void UpdateIconSourcePath()
        {
            if (WM.MainWindow.IconImageSourcePathTextBox.Text != IconSourcePath)
            {
                IconSourcePath = WM.MainWindow.IconImageSourcePathTextBox.Text;

                if (File.Exists(IconSourcePath))
                {
                    string extention = IconSourcePath.Split('.')[1].Trim().ToUpper();
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
        /// After the ImageSourcePath or IconSourcePath is determined to be a valid Image
        /// try loading the image and showing the preview
        /// there appears to be memory issues with the bitmap cache 
        /// but unless you load like 1000 images consecutivly it shouldent be too much of an issue
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
