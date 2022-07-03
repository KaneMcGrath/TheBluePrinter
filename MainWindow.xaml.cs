using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TheBluePrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            ResourceLoader.LoadDefaultItems();
            ItemSelector.LoadItems();
            IconSourceResolutionSlider.Visibility = Visibility.Collapsed;
            IconSourceResolutionSliderTickGrid.Visibility = Visibility.Collapsed;
            SettingsColumnWidth.Width = new GridLength(0);
            SettingsMenuStackPanel.Visibility = Visibility.Hidden;
            ResizeImageControlGrid.Visibility = Visibility.Collapsed;
            RICGApplyGrid.Visibility = Visibility.Collapsed;
            WM.SettingsMenuOpen = false;
            
            Settings.PrimaryColor = TestingHelper.HextoColor("433b45");
            Settings.SecondaryColor = TestingHelper.HextoColor("f79a76");
            WM.UpdateColors();
            

            //Dev Init Here
            FactorioPathTextBox.Text = "C:\\Users\\Kane\\Desktop\\games\\Factorio_Latest";
            ImageSourcePathTextBox.Text = "C:\\Users\\Kane\\Desktop\\games\\Factorio_Latest\\data\\base\\graphics\\icons\\fish.png";
            FormatFactorioIconCheckbox.IsChecked = true;
            IconSourceResolutionSlider.Value = 32;
            IconSourceResolutionSlider.Visibility = Visibility.Visible;
            IconSourceResolutionSliderTickGrid.Visibility = Visibility.Visible;
        }

        private void OnClickImageSourcePath(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                ImageSourcePathTextBox.Text = openFileDialog.FileName;
        }


        
        private void TestButtonAddUC(object sender, RoutedEventArgs e)
        {            
            ItemSelectionWidget isw = new ItemSelectionWidget();
            isw.Height = 60.0;
            isw.Margin = new Thickness(0.0, 0.0, 0.0, 4.0);
            isw.Randomize();
            AllItemsStackPanel.Children.Add(isw);
            Log.New("Added Item Selection Widget [" + isw.itemName + "] With Average Color RGB{" + isw.averageColor.ToString() + "}", isw.averageColor);
        }

        private void MainWindowInit(object sender, EventArgs e)
        {
            WM.MainWindow = this;
        }
        
        private void OnClickOpenConsoleButton(object sender, RoutedEventArgs e)
        {
            WM.OpenConsoleWindow();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WM.CloseMainWindow();
        }

        //private void ApplyColorsOnClick(object sender, RoutedEventArgs e)
        //{
        //    Settings.PrimaryColor = TestingHelper.HextoColor(PrimaryColorBox.Text);
        //    Settings.SecondaryColor = TestingHelper.HextoColor(SecondaryColorBox.Text);
        //    WM.UpdateColors();
        //}

        private void ApplyScheme1Click(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = TestingHelper.HextoColor("566258");
            Settings.SecondaryColor = TestingHelper.HextoColor("cfccbd");
            WM.UpdateColors();
        }

        private void ApplyScheme2Click(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = TestingHelper.HextoColor("313131");
            Settings.SecondaryColor = TestingHelper.HextoColor("ffad5a");
            WM.UpdateColors();
        }

        private void ApplyScheme3Click(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = TestingHelper.HextoColor("3d659b");
            Settings.SecondaryColor = TestingHelper.HextoColor("d9d3bf");
            WM.UpdateColors();
        }

        private void ApplyScheme4Click(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = TestingHelper.HextoColor("313131");
            Settings.SecondaryColor = TestingHelper.HextoColor("c6c6c6");
            WM.UpdateColors();
        }

        private void ApplyScheme5Click(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = TestingHelper.HextoColor("433b45");
            Settings.SecondaryColor = TestingHelper.HextoColor("f79a76");
            WM.UpdateColors();
        }

        private void ParseItemsOnClick(object sender, RoutedEventArgs e)
        {
            TestingHelper.ParseItems("C:\\Users\\Kane\\Desktop\\games\\Factorio_Latest\\data\\base\\prototypes\\item.lua");
            //ResourceLoader.LoadItems();
        }

        private void LogAllFlagsOnClick(object sender, RoutedEventArgs e)
        {
            TestingHelper.parseAllFlags();
        }

        private void OnClickConvertToJSON(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.ConvertBlueprintToJSON();
        }

        private void OnClickConvertToBlueprint(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.ConvertJSONToBlueprint();
        }

        private void ImageSourcePathTextChanged(object sender, TextChangedEventArgs e)
        {
            GeneratePrinter.UpdateImageSourcePath();
        }

        private void FactorioPathTextChanged(object sender, TextChangedEventArgs e)
        {
            GeneratePrinter.UpdateFactorioPath();
        }

        private void OnClickFactorioSourcePath(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FactorioPathTextBox.Text = dialog.FileName;
                GeneratePrinter.UpdateFactorioPath();
            }
        }

        private void CVBlueprintCopyOnClick(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.Copy(true);
        }

        private void CVBlueprintPasteOnClick(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.Paste(true);
        }

        private void CVJSONCopyOnClick(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.Copy(false);
        }

        private void CVJSONPasteOnClick(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.Paste(false);
        }

        private void CVBlueprintRevertOnClick(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.Revert(true);
        }

        private void CVJSONRevertOnClick(object sender, RoutedEventArgs e)
        {
            BlueprintConverter.Revert(false);
        }
        
        private void OnClickLoadAllItems(object sender, RoutedEventArgs e)
        {
            Log.StartTimer(0);
            ResourceLoader.LoadItems();
            
            Log.New("ResourceLoader finished in " + Log.GetTimer(0));
            Log.StartTimer(1);
            ItemSelector.LoadItems();
            Log.New("Item Selection Widgets Generated in " + Log.GetTimer(1));
        }

        private void OnClickLoadAllIcons(object sender, RoutedEventArgs e)
        {
            Log.StartTimer(2);
            ResourceLoader.LoadFactorioIcons();
            Log.New("Icons Loaded in " + Log.GetTimer(2));

            Log.StartTimer(3);
            ItemSelector.ReloadIcons();
            Log.New("Item Selection Widgets Reloaded in " + Log.GetTimer(3));
        }

        private void AverageColorButtonOnClick(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = Tools.ColorConverter(TestingHelper.AverageAverageColor());
            WM.UpdateColors();
        }

        private void ISClickToMoveChecked(object sender, RoutedEventArgs e)
        {
            ItemSelector.ClickToMoveCheckBoxChecked(true);
        }

        private void ISClickToMoveUnchecked(object sender, RoutedEventArgs e)
        {
            ItemSelector.ClickToMoveCheckBoxChecked(false);

        }

        private void ISMoveButtonOnClick(object sender, RoutedEventArgs e)
        {
            ItemSelector.MoveSelected();
        }

        private void ISAllItemsSearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            ItemSelector.BuildAllList(ISAllItemsSearchBox.Text, WM.MainWindow.ISShowHidden.IsChecked.Value, WM.MainWindow.ISShowUnstackable.IsChecked.Value);
        }

        private void ISAllowedItemsSearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            ItemSelector.BuildAllowedList(ISAllowedItemsSearchBox.Text, WM.MainWindow.ISShowHidden.IsChecked.Value, WM.MainWindow.ISShowUnstackable.IsChecked.Value);
        }

        private void ISShowHiddenChecked(object sender, RoutedEventArgs e)
        {
            ItemSelector.BuildLists();
        }

        private void ISShowHiddenUnChecked(object sender, RoutedEventArgs e)
        {
            ItemSelector.BuildLists();
        }

        private void ISShowUnstackableChecked(object sender, RoutedEventArgs e)
        {
            ItemSelector.BuildLists();
        }

        private void ISShowUnstackableUnChecked(object sender, RoutedEventArgs e)
        {
            ItemSelector.BuildLists();
        }

        private void ISSelectAllDisallowedOnClick(object sender, RoutedEventArgs e)
        {
            ItemSelector.SelectMultiDisallowed(true);
        }

        private void ISSelectNoneDisallowedOnClick(object sender, RoutedEventArgs e)
        {
            ItemSelector.SelectMultiDisallowed(false);
        }

        private void ISSelectAllAllowedOnClick(object sender, RoutedEventArgs e)
        {
            ItemSelector.SelectMultiAllowed(true);
        }

        private void ISSelectNoneAllowedOnCllick(object sender, RoutedEventArgs e)
        {
            ItemSelector.SelectMultiAllowed(false);
        }

        private void Key_Down(object sender, KeyEventArgs e)
        {
            IM.KeyDown(e.Key);
        }

        private void Key_Up(object sender, KeyEventArgs e)
        {
            IM.KeyUp(e.Key);
        }

        private void GeneratePrinterOnClick(object sender, RoutedEventArgs e)
        {
            string source = ImageSourcePathTextBox.Text;
            bool flag = false;
            if (File.Exists(source))
            {
                string extention = source.Split('.')[1].Trim().ToUpper();

                foreach (string ext in GeneratePrinter.SupportedImageTypes)
                {
                    if (ext == extention) flag = true;
                }
                if (!flag)
                {
                    Log.New("This image type is not supported.  Supported types are [BMP, GIF, EXIF, JPG, PNG, TIFF]", CC.red);
                    return;
                }
            }
            else
            {
                Log.New("Please choose an image under \"Image Source Path\"", CC.red);
                return;
            }

            int mipmapLevel = 0;
            if (IconSourceResolutionSlider.Value == 64) mipmapLevel = 0;
            if (IconSourceResolutionSlider.Value == 32) mipmapLevel = 1;
            if (IconSourceResolutionSlider.Value == 16) mipmapLevel = 2;
            if (IconSourceResolutionSlider.Value == 8) mipmapLevel = 3;

            Bitmap image = GeneratePrinter.CreateFilteredImage(GeneratePrinter.sourceImage);
            
            ResultTextBox.Text = BlueprintConverter.ConvertToBlueprint(BlueprintBuilder.BuildBlueprint(BlueprintBuilder.BuildImageAssembler(ImageAnalyzer.CreateItemImage(image)), image.Width));
            Log.New("Generated Printer", CC.green);
            image.Dispose();
        }

        private void GeneratePreviewOnClick(object sender, RoutedEventArgs e)
        {
            if (!GeneratePrinter.IsFactorioPathValid())
            {
                Log.New("You need to select a factorio path in order to generate a preview", CC.red);
                return;
            }
            string source = ImageSourcePathTextBox.Text;
            bool flag = false;
            if (File.Exists(source))
            {
                string extention = source.Split('.')[1].Trim().ToUpper();
                
                foreach (string ext in GeneratePrinter.SupportedImageTypes)
                {
                    if (ext == extention) flag = true;
                }
                if (!flag)
                {
                    Log.New("This image type is not supported.  Supported types are [BMP, GIF, EXIF, JPG, PNG, TIFF]", CC.red);
                    return;
                }
            }
            else
            {
                Log.New("Please choose an image under \"Image Source Path\"", CC.red);
                return;
            }
           // int mipmapLevel = 0;
           // if (IconSourceResolutionSlider.Value == 64) mipmapLevel = 0;
           // if (IconSourceResolutionSlider.Value == 32) mipmapLevel = 1;
           // if (IconSourceResolutionSlider.Value == 16) mipmapLevel = 2;
           // if (IconSourceResolutionSlider.Value == 8) mipmapLevel = 3;
            int previewMipmapLevel = 0;
            if (IconResolutionSlider.Value == 64) previewMipmapLevel = 0;
            if (IconResolutionSlider.Value == 32) previewMipmapLevel = 1;
            if (IconResolutionSlider.Value == 16) previewMipmapLevel = 2;
            if (IconResolutionSlider.Value == 8) previewMipmapLevel = 3;
            try
            {
                //Bitmap sourceImage;
                //if (FormatFactorioIconCheckbox.IsChecked == true)
                //{
                //    sourceImage = ImageAnalyzer.FillAlphaChannel(ImageAnalyzer.FormatFactorioIconImage(new Bitmap(GeneratePrinter.ImageSourcePath), mipmapLevel), GeneratePrinter.AlphaFillColor);
                //}
                //else
                //{
                //    sourceImage = ImageAnalyzer.FillAlphaChannel(new Bitmap(GeneratePrinter.ImageSourcePath), GeneratePrinter.AlphaFillColor);
                //}

                Bitmap bitImage = ImageAnalyzer.CreatePreviewImage(GeneratePrinter.CreateFilteredImage(GeneratePrinter.sourceImage), false, previewMipmapLevel);
                if (bitImage == null) return;
                MemoryStream ms = new MemoryStream();
                bitImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();
                PreviewGeneratedImage.Source = image;
                PreviewImageReminderLabel.Visibility = Visibility.Hidden;
                bitImage.Dispose();
            }
            catch (Exception ex)
            {
                Log.New(ex.Message, CC.red);
            }
            
        }
        private void Pick_Primary_Color_Button_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Pick_Secondary_Color_Button_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        // "BMP", "GIF", "EXIF", "JPG", "PNG", "TIFF"
        private void SavePreviewImageButtonOnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image (*.png)|*.png|JPG Image (*.jpg)|*.jpg|BMP Image (*.bmp)|*.bmp|GIF Image (*.gif)|*.gif|TIFF Image (*.tiff)|*.tiff|EXIF Image (*.exif)|*.exif";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (ImageAnalyzer.LastPreviewImage != null)
                {
                    try
                    {
                        ImageAnalyzer.LastPreviewImage.Save(saveFileDialog.FileName);
                    }
                    catch(Exception exc)
                    {
                        Log.New(exc.Message, CC.red);
                    }
                }
                else
                {
                    Log.New("There is no preview image", CC.red);
                }
            }
        }


        private void FormatFactorioIconCheckboxOnClick(object sender, RoutedEventArgs e)
        {
            if (FormatFactorioIconCheckbox.IsChecked == true)
            {
                IconSourceResolutionSlider.Visibility = Visibility.Visible;
                IconSourceResolutionSliderTickGrid.Visibility = Visibility.Visible;
            }
            else
            {
                IconSourceResolutionSlider.Visibility = Visibility.Collapsed;
                IconSourceResolutionSliderTickGrid.Visibility = Visibility.Collapsed;
            }
            if (File.Exists(GeneratePrinter.ImageSourcePath))
            {
                GeneratePrinter.LoadImagePreview();
            }
        }

        private void GenerateItemCodeOnClick(object sender, RoutedEventArgs e)
        {
            Tools.ReallyStupidDefaultItemsCodeGenerator();
        }

        private void IconSourceResolutionSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (File.Exists(GeneratePrinter.ImageSourcePath))
            {
                GeneratePrinter.LoadImagePreview();
            }
        }

        private void SettingsMenuButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (WM.SettingsMenuOpen)
            {
                SettingsColumnWidth.Width = new GridLength(0);
                SettingsMenuStackPanel.Visibility = Visibility.Hidden;

                WM.SettingsMenuOpen = false;
            }
            else
            {
                SettingsColumnWidth.Width = new GridLength(230);
                SettingsMenuStackPanel.Visibility = Visibility.Visible;
                WM.SettingsMenuOpen = true;
            }
        }

        private void SettingsParseItemsButtonOnClick(object sender, RoutedEventArgs e)
        {
            Log.New("Reading Factorio Data", CC.yellow);
            ResourceLoader.itemsLoaded = false;
            ResourceLoader.iconsLoaded = false;
            Item.ClearItemsData();
            ItemSelector.ClearLists();
            ResourceLoader.ParseItemsLua();
            ResourceLoader.LoadFactorioIcons();
            ItemSelector.LoadItems();
        }

        private void ClearItemListsOnClick(object sender, RoutedEventArgs e)
        {
            Item.ClearItemsData();
            ItemSelector.ClearLists();

        }

        private void SetttingsGenerateInfinityButtonOnClick(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = BlueprintConverter.ConvertToBlueprint(BlueprintBuilder.BuildBlueprint(BlueprintBuilder.BuildAllInfinityChests()));
            Log.New("Generated Infinity Chest Blueprint", CC.green);
        }

        private void SettingsSetPrimaryColorOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.PrimaryColor = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                WM.UpdateColors();
            }
        }

        private void SettingsSetSecondaryColorOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.SecondaryColor = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                WM.UpdateColors();
            }
        }

        private void SettinsRestoreDefaultsOnClick(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = TestingHelper.HextoColor("433b45");
            Settings.SecondaryColor = TestingHelper.HextoColor("f79a76");
            WM.UpdateColors();
        }

        private void SettingsApplicationExitOnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OnClickReloadImage(object sender, RoutedEventArgs e)
        {

            GeneratePrinter.UpdateImageSourcePath(true);
        }

        private void OnClickReloadFactorioSource(object sender, RoutedEventArgs e)
        {
            GeneratePrinter.lastFactorioPath = "";
            GeneratePrinter.UpdateFactorioPath();
        }

        private void CopyResultBlueprintOnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ResultTextBox.Text);
        }

        private void SetBackgroundColorOnClick(object sender, RoutedEventArgs e)
        {
            

            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.CustomColors = new int[] { 7431760, 3561115, 8816262, 5729973, 2369580, 7501173 };
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GeneratePrinter.AlphaFillColor = colorDialog.Color;
                BackGroundColorPreviewRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
            GeneratePrinter.UpdateImageSourcePath(true);
        }

        private void ApplyImageSizeOnClick(object sender, RoutedEventArgs e)
        {
            GeneratePrinter.LoadImagePreview();
        }

        private void ResizeImageCheckboxOnClick(object sender, RoutedEventArgs e)
        {
            if (ResizeImageCheckbox.IsChecked == true)
            {
                ResizeImageControlGrid.Visibility = Visibility.Visible;
                RICGApplyGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ResizeImageControlGrid.Visibility=Visibility.Collapsed;
                RICGApplyGrid.Visibility=Visibility.Collapsed;
            }
            GeneratePrinter.LoadImagePreview();
        }
    }
}
