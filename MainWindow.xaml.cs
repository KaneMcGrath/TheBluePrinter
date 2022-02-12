using System;
using System.Drawing;
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
using Microsoft.Win32;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

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
        }

        private void OnClickImageSourcePath(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                ImageSourcePathTextBox.Text = openFileDialog.FileName;
        }
        
        private void OnClickIconSourcePath(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                IconImageSourcePathTextBox.Text = openFileDialog.FileName;
        }

        private void TestButtonAddUC(object sender, RoutedEventArgs e)
        {            ItemSelectionWidget isw = new ItemSelectionWidget();
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

        private void ApplyColorsOnClick(object sender, RoutedEventArgs e)
        {
            Settings.PrimaryColor = TestingHelper.HextoColor(PrimaryColorBox.Text);
            Settings.SecondaryColor = TestingHelper.HextoColor(SecondaryColorBox.Text);
            WM.UpdateColors();
        }

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
            ResourceLoader.LoadItems();
            ResourceLoader.LoadFactorioIcons();
            ItemSelector.LoadItems();
            ItemSelector.ReloadIcons();
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
            Bitmap image = new Bitmap(GeneratePrinter.ImageSourcePath);
            ResultTextBox.Text = BlueprintBuilder.BuildBlueprint(BlueprintBuilder.BuildImageAssembler(ImageAnalyzer.CreateItemImage(image)));
        }
    }
}
