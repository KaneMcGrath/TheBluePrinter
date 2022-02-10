using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TheBluePrinter
{

    /// <summary>
    /// Handles Most functions of the item selection tab and stores information about each item.
    /// </summary>
    class ItemSelector
    {


        
        /// <summary>
        /// Contains a copy of everything in AllItemsStackPanel, 
        /// so search can dynamically update this list and update the stack panel to reflect it
        /// </summary>
        public static Dictionary<string, ItemSelectionWidget> AllItems = new Dictionary<string, ItemSelectionWidget>();
        public static Dictionary<string, ItemSelectionWidget> AllowedItems = new Dictionary<string, ItemSelectionWidget>();

        public static ItemSelectionWidget lastSelectedAllItemsWidget = null;
        public static ItemSelectionWidget lastSelectedAllowedItemsWidget = null;


        /// <summary>
        /// Returns a list of all allowed items specified by the user in this tab
        /// </summary>
        /// <returns></returns>
        public static List<Item> GetAllowedItems()
        {
            List<Item> result = new List<Item>();
            foreach(ItemSelectionWidget isw in AllowedItems.Values)
            {
                result.Add(isw.myItem);
            }
            return result;
        }



        /// <summary>
        /// updates the value of the ISClickToMove when the checkbox is interacted with
        /// </summary>
        /// <param name="value"></param>
        public static void ClickToMoveCheckBoxChecked(bool value)
        {
            Settings.ISClickToMove = value;
        }

        /// <summary>
        /// Clears then populates the AllowedItemsStackPanel with the values from AllowedItems ignores items not allowed by the filters so they are not shown
        /// </summary>
        /// <param name="filter"></param>
        public static void BuildAllowedList(string filter = "", bool showHidden = true, bool showUnstackable = true)
        {
            WM.MainWindow.AllowedItemsStackPanel.Children.Clear();
            if (filter == "")
            {
                foreach (ItemSelectionWidget isw in AllowedItems.Values)
                {
                    if ((!isw.isHidden || showHidden) && (!isw.isUnstackable || showUnstackable))
                    {
                        WM.MainWindow.AllowedItemsStackPanel.Children.Add(isw);
                        
                    }
                }
            }
            else
            {
                foreach (ItemSelectionWidget isw in AllowedItems.Values)
                {
                    if (isw.itemName.Contains(filter) && (!isw.isHidden || showHidden) && (!isw.isUnstackable || showUnstackable))
                    {
                        WM.MainWindow.AllowedItemsStackPanel.Children.Add(isw);
                        
                    }
                }
            }
        }

        
        /// <summary>
        /// Populates the AllItemsStackPanel with items from allItems, based on the search filters provided
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="showHidden"></param>
        /// <param name="showUnstackable"></param>
        public static void BuildAllList(string filter = "", bool showHidden = true, bool showUnstackable = true)
        {
            WM.MainWindow.AllItemsStackPanel.Children.Clear();
            if (filter == "")
            { 
                foreach (ItemSelectionWidget isw in AllItems.Values)
                {
                    if ((!isw.isHidden || showHidden) && (!isw.isUnstackable || showUnstackable))
                    {
                        WM.MainWindow.AllItemsStackPanel.Children.Add(isw);
                    }
                }
            }
            else
            {
                foreach (ItemSelectionWidget isw in AllItems.Values)
                {
                    if (isw.itemName.Contains(filter) && (!isw.isHidden || showHidden) && (!isw.isUnstackable || showUnstackable))
                    {
                        WM.MainWindow.AllItemsStackPanel.Children.Add(isw);
                    }
                }
            }
        }

        public static void UpdateTotalCount()
        {
            WM.MainWindow.AllowedItemsTotalCountRectangle.Content = "Total: " + AllowedItems.Count;
            WM.MainWindow.AllItemsTotalCountRectangle.Content = "Total: " + AllItems.Count;
        }

        public static void UpdateSelectedCount()
        {
            ItemSelectionWidget[] all = new ItemSelectionWidget[AllItems.Count];
            AllItems.Values.CopyTo(all, 0);
            ItemSelectionWidget[] allowed = new ItemSelectionWidget[AllowedItems.Count];
            AllowedItems.Values.CopyTo(allowed, 0);
            int allCount = 0;
            int allowedCount = 0;

            foreach (ItemSelectionWidget isw in all)
            {
                if (isw.isSelected)
                {
                    allCount += 1;
                }
            }
            foreach (ItemSelectionWidget isw in allowed)
            {
                if (isw.isSelected)
                {
                    allowedCount += 1;
                }
            }
            WM.MainWindow.AllItemsSelectedCountRectangle.Content = "Selected: " + allCount.ToString();
            WM.MainWindow.AllowedItemsSelectedCountRectangle.Content = "Selected: " + allowedCount.ToString();
        }

        public static void SelectMultiDisallowed(bool TFAllNone)
        {
            foreach(ItemSelectionWidget isw in WM.MainWindow.AllItemsStackPanel.Children)
            {
                isw.Select(TFAllNone);
            }
            UpdateSelectedCount();
        }
        public static void SelectMultiAllowed(bool TFAllNone)
        {
            foreach (ItemSelectionWidget isw in WM.MainWindow.AllowedItemsStackPanel.Children)
            {
                isw.Select(TFAllNone);
            }
            UpdateSelectedCount();
        }

        /// <summary>
        /// Moves a ItemSelectionWidget from one panel to another
        /// </summary>
        /// <param name="isw"></param>
        public static void MoveWidget(ItemSelectionWidget isw)
        {
            isw.Select(false);
            string name = isw.itemName;
            if (AllowedItems.ContainsKey(name))
            {
                WM.MainWindow.AllowedItemsStackPanel.Children.Remove(isw);
                WM.MainWindow.AllItemsStackPanel.Children.Add(isw);
                AllowedItems.Remove(name);
                AllItems.Add(name, isw);

            }
            else if (AllItems.ContainsKey(name))
            {
                WM.MainWindow.AllItemsStackPanel.Children.Remove(isw);
                WM.MainWindow.AllowedItemsStackPanel.Children.Add(isw);
                AllItems.Remove(name);
                AllowedItems.Add(name, isw);
            }

        }

        /// <summary>
        /// finds all selected widgets and moves them to the other side
        /// </summary>
        public static void MoveSelected()
        {
            ItemSelectionWidget[] all = new ItemSelectionWidget[AllItems.Count];
            AllItems.Values.CopyTo(all, 0);
            ItemSelectionWidget[] allowed = new ItemSelectionWidget[AllowedItems.Count];
            AllowedItems.Values.CopyTo(allowed, 0);
            

            
            foreach (ItemSelectionWidget isw in all)
            {
                if (isw.isSelected)
                {
                    MoveWidget(isw);
                }
            }
            foreach (ItemSelectionWidget isw in allowed)
            {
                if (isw.isSelected)
                {
                    MoveWidget(isw);
                }
            }
            ItemSelector.UpdateSelectedCount();
            UpdateTotalCount();
        }

        public static void AddAllowedItem(ItemSelectionWidget isw)
        {
            if (!AllowedItems.ContainsKey(isw.itemName))
            {
                AllowedItems.Add(isw.itemName, isw);
             
            }
        }

        public static void AddAllItem(ItemSelectionWidget isw)
        {
            if (!AllItems.ContainsKey(isw.itemName))
            {
                AllItems.Add(isw.itemName, isw);
                
            }
        }

        public static void ReloadIcons()
        {
            foreach (ItemSelectionWidget isw in ItemSelectionWidget.AllWidgets)
            {
                isw.ReloadIcon();
            }
        }
        public static void LoadItems()
        {
            foreach (Item i in Item.AllItems)
            {
                ItemSelectionWidget isw = new ItemSelectionWidget(i);
                if (i.StackSize <= 1 || i.Flags.Contains("hidden"))
                {
                    AddAllItem(isw);
                }
                else
                {
                    AddAllowedItem(isw);
                }
                BuildLists();
                UpdateTotalCount();
            }
        }

        public static void BuildLists()
        {
            BuildAllowedList("", WM.MainWindow.ISShowHidden.IsChecked.Value, WM.MainWindow.ISShowUnstackable.IsChecked.Value);
            BuildAllList("", WM.MainWindow.ISShowHidden.IsChecked.Value, WM.MainWindow.ISShowUnstackable.IsChecked.Value);
        }
    }
}
