using System;
using System.IO;
//using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace TheBluePrinter
{
    /// <summary>
    /// Handles loading most resources into the application and checking if they are valid
    /// Resources are stored in the Resources class
    /// </summary>
    static class ResourceLoader
    {

        /// <summary>
        /// Loads the settings file Data/Settings.txt and applies the settings to the running application
        /// </summary>
        public static void LoadSettings()
        {
            
        }
        
        /// <summary>
        /// Saves all current settings to Data/Settings.txt
        /// </summary>
        public static void SaveSettingsFile()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public static void LoadInternalColerAverages()
        {

        }

        /// <summary>
        /// Loads all icons for each item
        /// its kept seperate from item loading so the factorio path can be chosen at any time
        /// </summary>
        public static void LoadFactorioIcons()
        {
            foreach (Item item in Item.AllItems)
            {
                string icopath = item.IconPath.Replace("__base__", Settings.FactorioPath + "/data/base");
                if (File.Exists(icopath))
                {
                    Bitmap Icon = new Bitmap(icopath);
                    item.Icon = Icon;
                }
            }
        }
        
        /// <summary>
        /// Loads all items from Items.txt
        /// </summary>
        public static void LoadItems()
        {
            string[] lines = File.ReadAllLines("Data/items.txt");
            string name = "";
            string icon = "";
            List<string> Flags = new List<string>();
            int stacksize = 0;
            int r = 0, g = 0, b = 0;

            bool readFlags = false;
            foreach (string l in lines)
            {
                if (readFlags)
                {
                    if (l.StartsWith("}"))
                    {
                        readFlags = false;
                        Item item;
                        item = new Item(name, icon, Flags.ToArray(), stacksize, System.Drawing.Color.FromArgb((byte)r, (byte)g, (byte)b));
                        Log.New(item.ToString());
                        Flags.Clear();
                    }
                    else
                    {
                        Flags.Add(l);
                    }
                }
                else
                {
                    if (l.StartsWith("ItemName"))
                    {
                        name = l.Split(':')[1];
                    }
                    if (l.StartsWith("IconPath"))
                    {
                        icon = l.Split(':')[1];
                    }
                    if (l.StartsWith("StackSize"))
                    {
                        stacksize = int.Parse(l.Split(':')[1]);
                    }
                    if (l.StartsWith("Flags"))
                    {
                        readFlags = true;
                    }
                    if (l.StartsWith("AverageColor"))
                    {
                        string content = l.Split(':')[1];
                        string[] rgb = content.Split(',');
                        r = int.Parse(rgb[0]);
                        g = int.Parse(rgb[1]);
                        b = int.Parse(rgb[2]);
                    }
                }
            }
        }        
    }

    /// <summary>
    /// Holds all Settings and options for the application so they can be loaded later.
    /// </summary>
    static class Settings
    {
        public static System.Windows.Media.Color PrimaryColor = System.Windows.Media.Color.FromRgb(61, 101, 155);
        public static System.Windows.Media.Color SecondaryColor = System.Windows.Media.Color.FromRgb(217,211,191);

        public static string FactorioPath = "";

        public static bool ISClickToMove = false;
        

    }

    
    /// <summary>
    /// stores more complicated data about the default state of the application
    /// </summary>
    static class Defaults
    {
        public static string[] ProcessItems = new string[]
        {
            "stone-brick",
            "coal",
            "stone",
            "iron-ore",
            "copper-ore",
            "iron-plate",
            "copper-plate",
            "copper-cable",
            "iron-stick",
            "iron-gear-wheel",
            "electronic-circuit",
            "inserter",
            "pipe",
            "repair-pack",
            "automation-science-pack",
            "logistic-science-pack",
            "steel-plate",
            "solid-fuel",
            "grenade",
            "rail",
            "uranium-ore",
            "transport-belt",
            "fast-transport-belt",
            "express-transport-belt",
            "chemical-science-pack",
            "military-science-pack",
            "production-science-pack",
            "utility-science-pack",
            "space-science-pack",
            "underground-belt",
            "fast-underground-belt",
            "express-underground-belt",
            "advanced-circuit",
            "processing-unit",
            "sulfur",
            "plastic-bar",
            "explosives",
            "firearm-magazine",
            "piercing-rounds-magazine"
        };
    }

    /// <summary>
    /// Stores most of the resources used for the application
    /// not including settings
    /// probably depracated now but it was fun while it lasted
    /// </summary>
    static class Resources
    {
        



        

    }

    /// <summary>
    /// Converters and other small things that have no specific place
    /// </summary>
    public static class Tools
    {
        public static System.Windows.Media.Color ColorConverter(System.Drawing.Color c)
        {
            return System.Windows.Media.Color.FromRgb(c.R, c.G, c.B);
        }
        public static System.Drawing.Color ColorConverter(System.Windows.Media.Color c)
        {
            return System.Drawing.Color.FromArgb(c.R, c.G, c.B);
        }

        /// <summary>
        /// Thanks Aurelio Lopez Ovando & dreamcrash
        ///     [insert stack overflow url]
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource BitmapConverter(Bitmap bitmap)
        {
            
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        //20th 21st
        /// <summary>
        /// parses 3 comma seperated bytes and converts them to a Color
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color CSVtoColor(string input)
        {
            string[] split = input.Split(',');
            return System.Windows.Media.Color.FromRgb(Convert.ToByte(split[0]), Convert.ToByte(split[1]), Convert.ToByte(split[2]));
        }

        /// <summary>
        /// Converts Hexidecimal color codes to a Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color HextoColor(string hex)
        {
            string s = hex;
            if (hex.Length == 7) s = hex.Substring(1);
            if (hex.Length == 6)
            {
                int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                return System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b);
            }
            return System.Windows.Media.Color.FromRgb(0, 0, 0);
        }
    }

    /// <summary>
    /// Holds all needed information for an item including its Icon bitmap and 
    /// </summary>
    public class Item
    {
        /// <summary>
        /// used to lookup items without iterating through the list
        /// </summary>
        private static Dictionary<string, Item> ItemLibrary = new Dictionary<string, Item>();
        public static Dictionary<Color, string> ItemColorLookup = new Dictionary<Color, string>();
                /// <summary>
        /// List of every Item in the Game
        /// </summary>
        public static List<Item> AllItems = new List<Item>();

        /// <summary>
        /// Finds an items position on the item list
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int findID(string name)
        {
            for (int i = 0; i < AllItems.Count; i++)
            {
                if (AllItems[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public static Item FindByID(int id)
        {
            if (id < AllItems.Count)
            return AllItems[id];
            else return null;
        }

        /// <summary>
        /// Finds an Item based on its item name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Item Find(string name)
        {
            if (ItemLibrary.ContainsKey(name))
            {
                return ItemLibrary[name];
            }
            Log.New("Could not find Item " + name, CC.red);
            return null;
        }

        public string Name;
        public string IconPath;
        public string[] Flags;
        public int StackSize;
        public System.Drawing.Color AverageColor;
        public Bitmap Icon;
        
        public Item(string Name, string IconPath, int StackSize)
        {
            this.Name = Name;
            this.IconPath = IconPath;
            this.StackSize = StackSize;
            ItemLibrary.Add(Name, this);
            Item.AllItems.Add(this);
        }

        public Item(string Name, string IconPath, string[] Flags, int StackSize)
        {
            this.Name = Name;
            this.IconPath = IconPath;
            this.Flags = Flags;
            this.StackSize = StackSize;
            ItemLibrary.Add(Name, this);
            Item.AllItems.Add(this);
        }

        public Item(string Name, string IconPath, string[] Flags, int StackSize, System.Drawing.Color AverageColor)
        {
            this.Name = Name;
            this.IconPath = IconPath;
            this.Flags = Flags;
            this.StackSize = StackSize;
            this.AverageColor = AverageColor;
            Log.New("Adding " + Name + " To Library");
            if (ItemLibrary.ContainsKey(Name))
            {
                Item.AllItems.Remove(ItemLibrary[Name]);
                ItemLibrary[Name] = this;
                Item.AllItems.Add(this);

            }
            else
            {

                ItemLibrary.Add(Name, this);
                Item.AllItems.Add(this);
            }
            if (!ItemColorLookup.ContainsKey(AverageColor))
            {
                ItemColorLookup.Add(AverageColor, Name);
            }
        }
        
        public Item(string Name, string IconPath, string[] Flags, int StackSize, System.Drawing.Color AverageColor, Bitmap Icon)
        {
            this.Name = Name;
            this.IconPath = IconPath;
            this.Flags = Flags;
            this.StackSize = StackSize;
            this.AverageColor = AverageColor;
            this.Icon = Icon;
            Log.New("Adding " + Name + " To Library");
            ItemLibrary.Add(Name, this);
            Item.AllItems.Add(this);
            if (!ItemColorLookup.ContainsKey(AverageColor))
            {
                ItemColorLookup.Add(AverageColor, Name);
            }
        }

        public override string ToString()
        {
            string f = "";
            foreach (string h in Flags)
            {
                f = f + h + ",";
            }
            if (f.Length > 0) f = f.Remove(f.Length - 1);
            return Name + ":  IconPath: " + IconPath + "  Flags: {" + f + "}   StackSize: " + StackSize.ToString() + " [" + AverageColor.ToString() + "]";
        }
    }

    
}
