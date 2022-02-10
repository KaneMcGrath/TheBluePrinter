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

namespace TheBluePrinter
{
    /// <summary>
    /// Interaction logic for ItemSelectionWidget.xaml
    /// </summary>
    public partial class ItemSelectionWidget : UserControl
    {

        public static List<ItemSelectionWidget> AllWidgets = new List<ItemSelectionWidget>();
        /// <summary>
        /// The ingame name of an item
        /// used for display and for finding the Item
        /// </summary>
        public string itemName = "Default Name";
        
        //Initialized for the color perview to display on the right
        public Color averageColor = Color.FromRgb(255,255,0);

        public Item myItem;

        public bool isSelected = false;

        public bool isHidden = false;
        public bool isUnstackable = false;

        public ItemSelectionWidget()
        {
            InitializeComponent();
            AllWidgets.Add(this);
            NameplateBackgroundRectangle.Fill = new SolidColorBrush(Settings.PrimaryColor);
            ItemNameLabel.Foreground = new SolidColorBrush(Settings.SecondaryColor);
            this.Height = 60.0;
            this.Margin = new Thickness(0.0, 0.0, 0.0, 4.0);
        }
        public ItemSelectionWidget(Item item)
        {
            InitializeComponent();
            InitItem(item);
            AllWidgets.Add(this);
            NameplateBackgroundRectangle.Fill = new SolidColorBrush(Settings.PrimaryColor);
            ItemNameLabel.Foreground = new SolidColorBrush(Settings.SecondaryColor);
            this.Height = 60.0;
            this.Margin = new Thickness(0.0, 0.0, 0.0, 4.0);
        }


        
        public void ReloadIcon()
        {
            Item item = Item.Find(itemName);
            if (item.Icon != null)
            {
                IconImage.Source = Tools.BitmapConverter(ImageAnalyzer.FormatFactorioIconImage(item.Icon, 0));
            }
        }

        public void InitItem(Item item)
        {
            
            averageColor = Tools.ColorConverter(item.AverageColor);
            AverageColorDisplayRectangle.Fill = new SolidColorBrush(averageColor);
            myItem = item;
            itemName = item.Name;
            ItemNameLabel.Content = itemName;
            ItemNameLabel.ToolTip = itemName;
            ISWSelectionRectangle.ToolTip = new ToolTip().Content = item.Name;
            if (item.Flags.Contains("hidden"))
            {
                isHidden = true;
                ShowErrorRectangle("This item is flagged as \"hidden\" and is likely not useable in game");
            }
            else if(item.Flags.Contains("only-in-cursor"))
            {
                ShowErrorRectangle("This item is flagged as \"only-in-cursor\" and is likely not useable in game");
            }
            else if (item.StackSize == 1)
            {
                isUnstackable = true;
                ShowErrorRectangle();
            }
            
            if (item.Icon != null)
            {
                IconImage.Source = Tools.BitmapConverter(ImageAnalyzer.FormatFactorioIconImage(item.Icon, 0));

            }
            Log.New("Init Item " + item.Name + " [" + item.AverageColor.ToString() + "]   {hidden = " + isHidden + "}");
        }

        public void ShowErrorRectangle(string reason = "This item has a stack size of 1 and should not be used in the printer")
        {
            ItemErrorRectangle.Visibility = Visibility.Visible;
            ItemErrorLabel.Visibility = Visibility.Visible;
            ItemErrorRectangle.ToolTip = new ToolTip().Content = reason;
            ItemErrorLabel.ToolTip = new ToolTip().Content = reason;
            ISWSelectionRectangle.ToolTip = new ToolTip().Content = reason;
        }

        public void Randomize()
        {
            averageColor = TestingHelper.RandomColor();
            AverageColorDisplayRectangle.Fill = new SolidColorBrush(averageColor);
            itemName = TestingHelper.RandomItemName();
            ItemNameLabel.Content = itemName;
            ItemNameLabel.ToolTip = itemName;
            if (TestingHelper.Chance(16f))
            {
                ShowErrorRectangle();
            }
        }

        public void Select(bool TFSelectDeselect)
        {
            if (TFSelectDeselect)
            {
                
                isSelected = true;
                ISWSelectionRectangle.StrokeThickness = 5;
                
            }
            else
            {
                isSelected = false;
                ISWSelectionRectangle.StrokeThickness = 0;
                
            }
        }

        private void ISWMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.ISClickToMove)
            {
                ItemSelector.MoveWidget(this);
                ItemSelector.UpdateSelectedCount();
            }
            else
            {
                if (isSelected)
                {
                    Select(false);
                    ItemSelector.UpdateSelectedCount();
                }
                else
                {
                    Select(true);
                    ItemSelector.UpdateSelectedCount();
                }
            }
        }
    }
}
