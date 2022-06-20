using System;
using System.Drawing;
using System.Windows.Media;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBluePrinter
{
    /// <summary>
    /// Contains functions purely for testing purposes
    /// like assigning random names and colors
    /// </summary>
    class TestingHelper
    {
        public static Random rand = new Random();

        public static string[] allnames;
        public static void Init()
        {
            allnames = File.ReadAllLines("Data/allnames.txt");
        }

        public static string RandomItemName()
        {
            if(allnames == null)
            {
                Init();
            }
            
            return allnames[rand.Next(0,allnames.Length - 1)];
        }

        public static System.Windows.Media.Color RandomColor()
        {
            
            return System.Windows.Media.Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));
        }

        /// <summary>
        /// will generate a random number out of 100, if it is less than the specified percent
        /// it will return true
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static bool Chance(float percent)
        {
            
            if (rand.NextDouble() < (percent / 100f))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public static System.Drawing.Color AverageAverageColor()
        {
            int r = 0, g = 0, b = 0;
            foreach (Item i in Item.AllItems)
            {
                r += i.AverageColor.R;
                g += i.AverageColor.G;
                b += i.AverageColor.B;
            }
            return System.Drawing.Color.FromArgb(r / Item.AllItems.Count, g / Item.AllItems.Count, b / Item.AllItems.Count);
        }

        /// <summary>
        /// parses 3 comma seperated bytes and converts them to a Windows.Media Color
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color CSVtoColor(string input)
        {
            string[] split = input.Split(',');
            return System.Windows.Media.Color.FromRgb(Convert.ToByte(split[0]), Convert.ToByte(split[1]), Convert.ToByte(split[2]));
        }

        /// <summary>
        /// Converts Hexidecimal color codes to a Windows.Media Color
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
        
        /// <summary>
        /// Reads Factorios item.lua file and gets all items name, icon path, stack size and flags.
        /// will be used for asssembling a precompiled list of usable items
        /// </summary>
        public static void ParseItems(string path)
        {
            string[] lines = File.ReadAllLines(path);
            bool begin = false;
            bool readingItem = false;
            List<string> flags = new List<string>();

            string readItemName = "";
            string readIconPath = "";
            string[] readFlags = new string[0];
            int readStackSize = 0;
            int r, g, b = 0;
            int bracketOffset = 0;


            List<string> nameCollisionCheck = new List<string>();
            List<LuaItem> itemList = new List<LuaItem>();

            
            for (int i = 0; i < lines.Length; i++)
            {
                
                string line = lines[i].Trim();
                
                if (begin)
                {
                    if (readingItem)
                    {
                        //Log.New("Line :" + i + "     BracketedOffset :" + bracketOffset);
                        if (line.StartsWith("name = "))
                        {
                            readItemName = line.Split('"')[1];
                            //Log.New("readItemName: " + readItemName);
                        }
                        if (line.StartsWith("icon = "))
                        {
                            readIconPath = line.Split('"')[1];
                            //Log.New("readIconPath :" + readIconPath);
                        }
                        if (line.StartsWith("flags = {"))
                        {
                            string goods = line.Substring(line.IndexOf('{')+1).Split('}')[0];
                            readFlags = goods.Split(',');
                            //Log.New("readFlags :" + readFlags.ToString());
                        }
                        if (line.StartsWith("stack_size"))
                        {
                            string s = line.Split('=')[1].TrimEnd(new char[] { ' ', ',' });
                            readStackSize = int.Parse(s);
                            //Log.New("readStackSize :" + readStackSize);
                        }
                        if (line.Contains("{"))
                        {
                            foreach (char c in line)
                            {
                                if (c == '{')
                                {
                                    bracketOffset++;
                                }
                            }
                        }
                        if (line.Contains("}"))
                        {
                            foreach (char c in line)
                            {
                                if (c == '}')
                                {
                                    if (bracketOffset > 0)
                                        bracketOffset--;
                                    else
                                    {
                                        
                                        string icopath = readIconPath.Replace("__base__", Settings.FactorioPath + "/data/base");
                                        Log.New("ICOPath: " + icopath);
                                        if (!nameCollisionCheck.Contains(readItemName))
                                        {
                                            nameCollisionCheck.Add(readItemName);
                                            if (File.Exists(icopath))
                                            {
                                                Bitmap Icon = new Bitmap(icopath);

                                                System.Drawing.Color av = ImageAnalyzer.AverageColor(Icon);

                                                r = av.R;
                                                g = av.G;
                                                b = av.B;
                                                Log.New("Processed Icon with Average Color of [" + r.ToString() + "," + g.ToString() + "," + b.ToString() + "]", System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b));

                                                itemList.Add(new LuaItem(readItemName, readIconPath, readFlags, readStackSize, r, g, b));
                                            }
                                            else
                                            {
                                                itemList.Add(new LuaItem(readItemName, readIconPath, readFlags, readStackSize));
                                            }
                                        }
                                        string resultflags = "";
                                        foreach (string f in readFlags)
                                        {
                                            resultflags = resultflags + f + ",";
                                        }
                                        if (resultflags.Length > 0)
                                        {
                                            resultflags = resultflags.Remove(resultflags.Length - 1);
                                        }
                                        Log.New("Parsed Item \"" + readItemName + "\"  with stacksize :" + readStackSize.ToString() + "  with flags [" + resultflags + "]");

                                        readItemName = "";
                                        readIconPath = "";
                                        readFlags = new string[0];
                                        readStackSize = 0;
                                        readingItem = false;
                                        
                                        
                                    }
                                }
                            }


                            //Log.New("Line end.  BracketOffset :" + bracketOffset);
                            
                        }
                    }
                    else if (line == "{")
                    {
                        readingItem = true;
                    }
                    
                }
                else if (line.Contains("data:extend("))
                {
                    begin = true;
                    i++;
                }
            }
            Log.New("Parsed " + itemList.Count.ToString() + " Items");

            List<string> OutputLines = new List<string>();
            foreach (LuaItem I in itemList)
            {
                OutputLines.Add("ItemName:" + I.itemName);
                OutputLines.Add("IconPath:" + I.iconPath);
                OutputLines.Add("StackSize:" + I.stackSize.ToString());
                OutputLines.Add("AverageColor:" + I.r.ToString() + "," + I.g.ToString() + "," + I.b.ToString());
                OutputLines.Add("Flags{");
                foreach (string s in I.flags)
                {
                    OutputLines.Add(s.Replace('"',' ').Trim());
                }
                OutputLines.Add("}");
                
                
            }
            File.WriteAllLines("Data\\Items.txt", OutputLines);
        }

        /// <summary>
        /// Gets all flags that were attached to items
        /// </summary>
        public static void parseAllFlags()
        {
            List<string> flaglist = new List<string>();

            string[] lines = File.ReadAllLines("Data/Items.txt");
            bool isFlag = false;

            Log.New("");
            Log.New("All Flags:");

            foreach (string l in lines)
            {
                string s = l.Trim();
                if (isFlag)
                {
                    if (s.Contains("}"))
                    {
                        isFlag = false;
                    }
                    else if(!flaglist.Contains(s))
                    {
                        flaglist.Add(s);
                    }

                }
                else if (s.Contains("Flags{"))
                {
                    isFlag = true;
                }
            }
            
            foreach (string f in flaglist)
            {
                Log.New(f);
            }

            
        }
        
        private class LuaItem
        {
            
            public string itemName;
            public string iconPath;
            public string[] flags;
            public int stackSize;
            public int r, g, b;

            public LuaItem(string name, string icon_path, string[] flags, int stacksize)
            {
                itemName = name;
                iconPath = icon_path;
                this.flags = flags;
                stackSize = stacksize;
            }
            public LuaItem(string name, string icon_path, string[] flags, int stacksize, int r, int g, int b)
            {
                itemName = name;
                iconPath = icon_path;
                this.flags = flags;
                stackSize = stacksize;
                this.r = r;
                this.g = g;
                this.b = b;
            }


        }
    }
}
