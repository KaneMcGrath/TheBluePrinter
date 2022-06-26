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
        public static bool iconsLoaded = false;
        public static bool itemsLoaded = false;

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
        public static int LoadFactorioIcons()
        {
            int successState = 2; //0:success 1:someNotLoaded 2:AllNotLoaded 3:itemsNotLoaded 4:iconsAlreadyLoaded
            if (!itemsLoaded) 
            { 
                Log.New("Cannot load icons untill items are loaded!", CC.red);
                return 3;
            }
            if (!iconsLoaded)
            {
                bool someLoaded = false;
                bool allLoaded = true;
                foreach (Item item in Item.AllItems)
                {
                    if (item.IconPath != "") 
                    { 
                        string icopath = item.IconPath.Replace("__base__", Settings.FactorioPath + "\\data\\base");
                        
                        if (File.Exists(icopath))
                        {
                            Bitmap Icon = new Bitmap(icopath);
                            item.Icon = Icon;
                            someLoaded = true;
                        }
                        else
                        {
                            allLoaded = false;
                            Log.New("Could not find icon for item " + item.Name + " at [" + icopath + "]",CC.red);
                        }
                        if (allLoaded)
                        {
                            successState = 0;
                        }
                        else
                        {
                            if (someLoaded) successState = 1;
                            else successState = 2;
                        }
                    }
                }
                return successState;
            }
            return 4;
        }
        
        public static void LoadDefaultItems()
        {
            foreach (Item I in Defaults.DefaultItemList)
            {
                I.Init();
            }
            itemsLoaded = true;
        }
        
        /// <summary>
        /// Loads all items from Items.txt
        /// </summary>
        public static void LoadItems()
        {
            if (!itemsLoaded)
            {
                string[] lines = File.ReadAllLines("Data\\items.txt");
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
                            //Log.New(item.ToString());
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
                itemsLoaded = true;
                return;
            }
            Log.New("Items are already loaded!", CC.red);
        }

        /// <summary>
        /// Reads the items from factorios item.lua file
        /// this shitty custom parser will probably fail when the game updates too far
        /// but it might work for a while
        /// </summary>
        /// <param name="path"></param>
        public static void ParseItemsLua()
        {
            if (GeneratePrinter.IsFactorioPathValid())
            {
                string[] lines = File.ReadAllLines(GeneratePrinter.ValidFactorioPath + "\\data\\base\\prototypes\\item.lua");
                bool begin = false;
                bool readingItem = false;
                List<string> flags = new List<string>();

                string readItemName = "";
                string readIconPath = "";
                string[] readFlags = new string[0];
                int readStackSize = 0;
                int r, g, b;
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
                                string goods = line.Substring(line.IndexOf('{') + 1).Split('}')[0];
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

                                            string icopath = readIconPath.Replace("__base__", Settings.FactorioPath + "\\data\\base");
                                            Log.New("ICOPath: " + icopath);
                                            if (!nameCollisionCheck.Contains(readItemName))
                                            {
                                                nameCollisionCheck.Add(readItemName);
                                                if (File.Exists(icopath))
                                                {
                                                    Bitmap Icon = new Bitmap(icopath);

                                                    Color av = ImageAnalyzer.AverageColor(Icon);

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

                foreach (LuaItem I in itemList)
                {
                    Item newItem = new Item(I.itemName, I.iconPath, I.flags, I.stackSize, Color.FromArgb(I.r, I.g, I.b));
                    newItem.Init();
                }
                itemsLoaded = true;
            }
            else
            {
                Log.New("Please Enter a valid factorio path", CC.red);
            }
        }

        /// <summary>
        /// I could have just read directly into the Item class 
        /// but i dont want to rewrite the parse function
        /// </summary>
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
        public static Item[] DefaultItemList = new Item[]
        {
            new Item("stone-brick", "__base__/graphics/icons/stone-brick.png", new string[0], 100, Color.FromArgb(111, 112, 107)),
            new Item("wood", "__base__/graphics/icons/wood.png", new string[0], 100, Color.FromArgb(126, 80, 33)),
            new Item("coal", "__base__/graphics/icons/coal.png", new string[0], 50, Color.FromArgb(44, 40, 36)),
            new Item("stone", "__base__/graphics/icons/stone.png", new string[0], 50, Color.FromArgb(121, 102, 68)),
            new Item("iron-ore", "__base__/graphics/icons/iron-ore.png", new string[0], 50, Color.FromArgb(80, 102, 113)),
            new Item("copper-ore", "__base__/graphics/icons/copper-ore.png", new string[0], 50, Color.FromArgb(155, 86, 54)),
            new Item("iron-plate", "__base__/graphics/icons/iron-plate.png", new string[0], 100, Color.FromArgb(134, 134, 134)),
            new Item("copper-plate", "__base__/graphics/icons/copper-plate.png", new string[0], 100, Color.FromArgb(181, 110, 87)),
            new Item("copper-cable", "__base__/graphics/icons/copper-cable.png", new string[0], 200, Color.FromArgb(181, 114, 88)),
            new Item("iron-stick", "__base__/graphics/icons/iron-stick.png", new string[0], 100, Color.FromArgb(130, 131, 131)),
            new Item("iron-gear-wheel", "__base__/graphics/icons/iron-gear-wheel.png", new string[0], 100, Color.FromArgb(92, 95, 90)),
            new Item("electronic-circuit", "__base__/graphics/icons/electronic-circuit.png", new string[0], 200, Color.FromArgb(97, 137, 40)),
            new Item("wooden-chest", "__base__/graphics/icons/wooden-chest.png", new string[0], 50, Color.FromArgb(130, 98, 46)),
            new Item("stone-furnace", "__base__/graphics/icons/stone-furnace.png", new string[0], 50, Color.FromArgb(108, 93, 58)),
            new Item("burner-mining-drill", "__base__/graphics/icons/burner-mining-drill.png", new string[0], 50, Color.FromArgb(92, 83, 81)),
            new Item("electric-mining-drill", "__base__/graphics/icons/electric-mining-drill.png", new string[0], 50, Color.FromArgb(91, 90, 82)),
            new Item("burner-inserter", "__base__/graphics/icons/burner-inserter.png", new string[0], 50, Color.FromArgb(92, 88, 86)),
            new Item("inserter", "__base__/graphics/icons/inserter.png", new string[0], 50, Color.FromArgb(145, 114, 61)),
            new Item("fast-inserter", "__base__/graphics/icons/fast-inserter.png", new string[0], 50, Color.FromArgb(66, 121, 140)),
            new Item("filter-inserter", "__base__/graphics/icons/filter-inserter.png", new string[0], 50, Color.FromArgb(114, 85, 138)),
            new Item("long-handed-inserter", "__base__/graphics/icons/long-handed-inserter.png", new string[0], 50, Color.FromArgb(141, 70, 63)),
            new Item("offshore-pump", "__base__/graphics/icons/offshore-pump.png", new string[0], 20, Color.FromArgb(91, 77, 59)),
            new Item("pipe", "__base__/graphics/icons/pipe.png", new string[0], 100, Color.FromArgb(95, 85, 63)),
            new Item("boiler", "__base__/graphics/icons/boiler.png", new string[0], 50, Color.FromArgb(92, 88, 72)),
            new Item("steam-engine", "__base__/graphics/icons/steam-engine.png", new string[0], 10, Color.FromArgb(99, 89, 74)),
            new Item("small-electric-pole", "__base__/graphics/icons/small-electric-pole.png", new string[0], 50, Color.FromArgb(124, 108, 85)),
            new Item("radar", "__base__/graphics/icons/radar.png", new string[0], 50, Color.FromArgb(116, 110, 89)),
            new Item("small-lamp", "__base__/graphics/icons/small-lamp.png", new string[0], 50, Color.FromArgb(160, 157, 155)),
            new Item("pipe-to-ground", "__base__/graphics/icons/pipe-to-ground.png", new string[0], 50, Color.FromArgb(114, 100, 76)),
            new Item("assembling-machine-1", "__base__/graphics/icons/assembling-machine-1.png", new string[0], 50, Color.FromArgb(100, 83, 73)),
            new Item("assembling-machine-2", "__base__/graphics/icons/assembling-machine-2.png", new string[0], 50, Color.FromArgb(76, 74, 85)),
            new Item("red-wire", "__base__/graphics/icons/red-wire.png", new string[0], 200, Color.FromArgb(212, 75, 48)),
            new Item("green-wire", "__base__/graphics/icons/green-wire.png", new string[0], 200, Color.FromArgb(42, 180, 52)),
            new Item("raw-fish", "__base__/graphics/icons/fish.png", new string[0], 100, Color.FromArgb(139, 140, 94)),
            new Item("repair-pack", "__base__/graphics/icons/repair-pack.png", new string[0], 100, Color.FromArgb(160, 149, 129)),
            new Item("stone-wall", "__base__/graphics/icons/wall.png", new string[0], 100, Color.FromArgb(88, 78, 57)),
            new Item("lab", "__base__/graphics/icons/lab.png", new string[0], 10, Color.FromArgb(84, 107, 140)),
            new Item("copy-paste-tool", "__base__/graphics/icons/copy-paste-tool.png", new string[] {"only-in-cursor","hidden","not-stackable"}, 1, Color.FromArgb(35, 35, 35)),
            new Item("cut-paste-tool", "__base__/graphics/icons/cut-paste-tool.png", new string[] {"only-in-cursor","hidden","not-stackable"}, 1, Color.FromArgb(26, 26, 26)),
            new Item("blueprint", "__base__/graphics/icons/blueprint.png", new string[] {"not-stackable","spawnable"}, 1, Color.FromArgb(54, 130, 192)),
            new Item("automation-science-pack", "__base__/graphics/icons/automation-science-pack.png", new string[0], 200, Color.FromArgb(167, 96, 96)),
            new Item("logistic-science-pack", "__base__/graphics/icons/logistic-science-pack.png", new string[0], 200, Color.FromArgb(102, 169, 103)),
            new Item("steel-plate", "__base__/graphics/icons/steel-plate.png", new string[0], 100, Color.FromArgb(117, 117, 114)),
            new Item("car", "__base__/graphics/icons/car.png", new string[0], 1, Color.FromArgb(103, 78, 49)),
            new Item("engine-unit", "__base__/graphics/icons/engine-unit.png", new string[0], 50, Color.FromArgb(92, 83, 65)),
            new Item("electric-furnace", "__base__/graphics/icons/electric-furnace.png", new string[0], 50, Color.FromArgb(87, 84, 76)),
            new Item("solid-fuel", "__base__/graphics/icons/solid-fuel.png", new string[0], 50, Color.FromArgb(75, 75, 75)),
            new Item("rocket-fuel", "__base__/graphics/icons/rocket-fuel.png", new string[0], 10, Color.FromArgb(93, 84, 63)),
            new Item("iron-chest", "__base__/graphics/icons/iron-chest.png", new string[0], 50, Color.FromArgb(109, 93, 78)),
            new Item("big-electric-pole", "__base__/graphics/icons/big-electric-pole.png", new string[0], 50, Color.FromArgb(111, 120, 125)),
            new Item("medium-electric-pole", "__base__/graphics/icons/medium-electric-pole.png", new string[0], 50, Color.FromArgb(146, 116, 90)),
            new Item("grenade", "__base__/graphics/icons/grenade.png", new string[0], 100, Color.FromArgb(83, 89, 71)),
            new Item("steel-furnace", "__base__/graphics/icons/steel-furnace.png", new string[0], 50, Color.FromArgb(96, 87, 67)),
            new Item("gate", "__base__/graphics/icons/gate.png", new string[0], 50, Color.FromArgb(90, 81, 48)),
            new Item("steel-chest", "__base__/graphics/icons/steel-chest.png", new string[0], 50, Color.FromArgb(118, 102, 100)),
            new Item("solar-panel", "__base__/graphics/icons/solar-panel.png", new string[0], 50, Color.FromArgb(77, 123, 135)),
            new Item("locomotive", "__base__/graphics/icons/locomotive.png", new string[0], 5, Color.FromArgb(100, 63, 59)),
            new Item("cargo-wagon", "__base__/graphics/icons/cargo-wagon.png", new string[0], 5, Color.FromArgb(88, 85, 84)),
            new Item("rail", "__base__/graphics/icons/rail.png", new string[0], 100, Color.FromArgb(105, 96, 73)),
            new Item("train-stop", "__base__/graphics/icons/train-stop.png", new string[0], 10, Color.FromArgb(131, 82, 67)),
            new Item("rail-signal", "__base__/graphics/icons/rail-signal.png", new string[0], 50, Color.FromArgb(85, 91, 59)),
            new Item("rail-chain-signal", "__base__/graphics/icons/rail-chain-signal.png", new string[0], 50, Color.FromArgb(74, 101, 108)),
            new Item("concrete", "__base__/graphics/icons/concrete.png", new string[0], 100, Color.FromArgb(118, 121, 123)),
            new Item("refined-concrete", "__base__/graphics/icons/refined-concrete.png", new string[0], 100, Color.FromArgb(113, 115, 116)),
            new Item("hazard-concrete", "__base__/graphics/icons/hazard-concrete.png", new string[0], 100, Color.FromArgb(111, 109, 88)),
            new Item("refined-hazard-concrete", "__base__/graphics/icons/refined-hazard-concrete.png", new string[0], 100, Color.FromArgb(115, 108, 86)),
            new Item("landfill", "__base__/graphics/icons/landfill.png", new string[0], 100, Color.FromArgb(131, 122, 56)),
            new Item("accumulator", "__base__/graphics/icons/accumulator.png", new string[0], 50, Color.FromArgb(115, 135, 136)),
            new Item("uranium-ore", "__base__/graphics/icons/uranium-ore.png", new string[0], 50, Color.FromArgb(85, 145, 27)),
            new Item("defender-capsule", "__base__/graphics/icons/defender.png", new string[0], 100, Color.FromArgb(93, 117, 130)),
            new Item("transport-belt", "__base__/graphics/icons/transport-belt.png", new string[0], 100, Color.FromArgb(153, 123, 101)),
            new Item("fast-transport-belt", "__base__/graphics/icons/fast-transport-belt.png", new string[0], 100, Color.FromArgb(152, 106, 98)),
            new Item("express-transport-belt", "__base__/graphics/icons/express-transport-belt.png", new string[0], 100, Color.FromArgb(131, 122, 116)),
            new Item("stack-inserter", "__base__/graphics/icons/stack-inserter.png", new string[0], 50, Color.FromArgb(110, 135, 68)),
            new Item("stack-filter-inserter", "__base__/graphics/icons/stack-filter-inserter.png", new string[0], 50, Color.FromArgb(133, 132, 132)),
            new Item("assembling-machine-3", "__base__/graphics/icons/assembling-machine-3.png", new string[0], 50, Color.FromArgb(87, 92, 62)),
            new Item("fluid-wagon", "__base__/graphics/icons/fluid-wagon.png", new string[0], 5, Color.FromArgb(104, 100, 98)),
            new Item("artillery-wagon", "__base__/graphics/icons/artillery-wagon.png", new string[0], 5, Color.FromArgb(97, 92, 70)),
            new Item("player-port", "__base__/graphics/icons/player-port.png", new string[] {"hidden"}, 50, Color.FromArgb(223, 215, 212)),
            new Item("tank", "__base__/graphics/icons/tank.png", new string[0], 1, Color.FromArgb(94, 73, 46)),
            new Item("chemical-science-pack", "__base__/graphics/icons/chemical-science-pack.png", new string[0], 200, Color.FromArgb(99, 157, 175)),
            new Item("military-science-pack", "__base__/graphics/icons/military-science-pack.png", new string[0], 200, Color.FromArgb(115, 116, 124)),
            new Item("production-science-pack", "__base__/graphics/icons/production-science-pack.png", new string[0], 200, Color.FromArgb(146, 91, 174)),
            new Item("utility-science-pack", "__base__/graphics/icons/utility-science-pack.png", new string[0], 200, Color.FromArgb(178, 157, 104)),
            new Item("space-science-pack", "__base__/graphics/icons/space-science-pack.png", new string[0], 2000, Color.FromArgb(181, 180, 180)),
            new Item("underground-belt", "__base__/graphics/icons/underground-belt.png", new string[0], 50, Color.FromArgb(107, 91, 65)),
            new Item("fast-underground-belt", "__base__/graphics/icons/fast-underground-belt.png", new string[0], 50, Color.FromArgb(105, 65, 61)),
            new Item("express-underground-belt", "__base__/graphics/icons/express-underground-belt.png", new string[0], 50, Color.FromArgb(79, 90, 96)),
            new Item("splitter", "__base__/graphics/icons/splitter.png", new string[0], 50, Color.FromArgb(101, 91, 71)),
            new Item("fast-splitter", "__base__/graphics/icons/fast-splitter.png", new string[0], 50, Color.FromArgb(111, 84, 81)),
            new Item("express-splitter", "__base__/graphics/icons/express-splitter.png", new string[0], 50, Color.FromArgb(94, 104, 109)),
            new Item("loader", "__base__/graphics/icons/loader.png", new string[] {"hidden"}, 50, Color.FromArgb(127, 121, 116)),
            new Item("fast-loader", "__base__/graphics/icons/fast-loader.png", new string[] {"hidden"}, 50, Color.FromArgb(127, 119, 115)),
            new Item("express-loader", "__base__/graphics/icons/express-loader.png", new string[] {"hidden"}, 50, Color.FromArgb(123, 119, 118)),
            new Item("advanced-circuit", "__base__/graphics/icons/advanced-circuit.png", new string[0], 200, Color.FromArgb(171, 69, 42)),
            new Item("processing-unit", "__base__/graphics/icons/processing-unit.png", new string[0], 100, Color.FromArgb(88, 106, 164)),
            new Item("logistic-robot", "__base__/graphics/icons/logistic-robot.png", new string[0], 50, Color.FromArgb(112, 95, 81)),
            new Item("construction-robot", "__base__/graphics/icons/construction-robot.png", new string[0], 50, Color.FromArgb(118, 106, 88)),
            new Item("logistic-chest-passive-provider", "__base__/graphics/icons/logistic-chest-passive-provider.png", new string[0], 50, Color.FromArgb(112, 69, 57)),
            new Item("logistic-chest-active-provider", "__base__/graphics/icons/logistic-chest-active-provider.png", new string[0], 50, Color.FromArgb(92, 67, 93)),
            new Item("logistic-chest-storage", "__base__/graphics/icons/logistic-chest-storage.png", new string[0], 50, Color.FromArgb(108, 96, 52)),
            new Item("logistic-chest-buffer", "__base__/graphics/icons/logistic-chest-buffer.png", new string[0], 50, Color.FromArgb(71, 104, 68)),
            new Item("logistic-chest-requester", "__base__/graphics/icons/logistic-chest-requester.png", new string[0], 50, Color.FromArgb(75, 95, 96)),
            new Item("rocket-silo", "__base__/graphics/icons/rocket-silo.png", new string[0], 1, Color.FromArgb(110, 102, 85)),
            new Item("roboport", "__base__/graphics/icons/roboport.png", new string[0], 10, Color.FromArgb(103, 97, 90)),
            new Item("coin", "__base__/graphics/icons/coin.png", new string[] {"hidden"}, 100000, Color.FromArgb(210, 154, 64)),
            new Item("substation", "__base__/graphics/icons/substation.png", new string[0], 50, Color.FromArgb(79, 87, 95)),
            new Item("beacon", "__base__/graphics/icons/beacon.png", new string[0], 10, Color.FromArgb(91, 82, 75)),
            new Item("storage-tank", "__base__/graphics/icons/storage-tank.png", new string[0], 50, Color.FromArgb(112, 100, 78)),
            new Item("pump", "__base__/graphics/icons/pump.png", new string[0], 50, Color.FromArgb(88, 74, 62)),
            new Item("upgrade-planner", "__base__/graphics/icons/upgrade-planner.png", new string[] {"spawnable"}, 1, Color.FromArgb(19, 149, 19)),
            new Item("deconstruction-planner", "__base__/graphics/icons/deconstruction-planner.png", new string[] {"spawnable"}, 1, Color.FromArgb(197, 58, 58)),
            new Item("blueprint-book", "__base__/graphics/icons/blueprint-book.png", new string[] {"spawnable"}, 1, Color.FromArgb(67, 121, 163)),
            new Item("pumpjack", "__base__/graphics/icons/pumpjack.png", new string[0], 20, Color.FromArgb(90, 112, 62)),
            new Item("oil-refinery", "__base__/graphics/icons/oil-refinery.png", new string[0], 10, Color.FromArgb(113, 84, 55)),
            new Item("chemical-plant", "__base__/graphics/icons/chemical-plant.png", new string[0], 10, Color.FromArgb(99, 91, 74)),
            new Item("sulfur", "__base__/graphics/icons/sulfur.png", new string[0], 50, Color.FromArgb(167, 153, 30)),
            new Item("empty-barrel", "__base__/graphics/icons/fluid/barreling/empty-barrel.png", new string[0], 10, Color.FromArgb(108, 108, 107)),
            new Item("plastic-bar", "__base__/graphics/icons/plastic-bar.png", new string[0], 100, Color.FromArgb(199, 199, 199)),
            new Item("electric-engine-unit", "__base__/graphics/icons/electric-engine-unit.png", new string[0], 50, Color.FromArgb(112, 90, 87)),
            new Item("explosives", "__base__/graphics/icons/explosives.png", new string[0], 50, Color.FromArgb(133, 73, 64)),
            new Item("battery", "__base__/graphics/icons/battery.png", new string[0], 200, Color.FromArgb(80, 77, 74)),
            new Item("flying-robot-frame", "__base__/graphics/icons/flying-robot-frame.png", new string[0], 50, Color.FromArgb(94, 98, 95)),
            new Item("low-density-structure", "__base__/graphics/icons/low-density-structure.png", new string[0], 10, Color.FromArgb(133, 114, 84)),
            new Item("nuclear-fuel", "__base__/graphics/icons/nuclear-fuel.png", new string[] {"light"}, 1, Color.FromArgb(78, 111, 64)),
            new Item("rocket-control-unit", "__base__/graphics/icons/rocket-control-unit.png", new string[0], 10, Color.FromArgb(125, 143, 115)),
            new Item("rocket-part", "__base__/graphics/icons/rocket-part.png", new string[] {"hidden"}, 5, Color.FromArgb(105, 107, 102)),
            new Item("satellite", "__base__/graphics/icons/satellite.png", new string[0], 1, Color.FromArgb(128, 130, 136)),
            new Item("spidertron", "__base__/graphics/icons/spidertron.png", new string[0], 1, Color.FromArgb(111, 75, 42)),
            new Item("spidertron-remote", "__base__/graphics/icons/spidertron-remote.png", new string[0], 1, Color.FromArgb(93, 79, 75)),
            new Item("selection-tool", "__base__/graphics/icons/blueprint.png", new string[] {"hidden","not-stackable","spawnable"}, 1, Color.FromArgb(54, 130, 192)),
            new Item("electric-energy-interface", "", new string[] {"hidden"}, 50, Color.FromArgb(0, 0, 0)),
            new Item("heat-interface", "__base__/graphics/icons/heat-interface.png", new string[] {"hidden"}, 20, Color.FromArgb(152, 149, 145)),
            new Item("nuclear-reactor", "__base__/graphics/icons/nuclear-reactor.png", new string[0], 10, Color.FromArgb(113, 115, 83)),
            new Item("uranium-235", "__base__/graphics/icons/uranium-235.png", new string[0], 100, Color.FromArgb(83, 155, 51)),
            new Item("uranium-238", "__base__/graphics/icons/uranium-238.png", new string[0], 100, Color.FromArgb(49, 90, 48)),
            new Item("centrifuge", "__base__/graphics/icons/centrifuge.png", new string[0], 50, Color.FromArgb(101, 118, 91)),
            new Item("uranium-fuel-cell", "__base__/graphics/icons/uranium-fuel-cell.png", new string[] {"light"}, 50, Color.FromArgb(70, 128, 63)),
            new Item("used-up-uranium-fuel-cell", "__base__/graphics/icons/used-up-uranium-fuel-cell.png", new string[0], 50, Color.FromArgb(54, 67, 54)),
            new Item("heat-exchanger", "__base__/graphics/icons/heat-boiler.png", new string[0], 50, Color.FromArgb(117, 63, 48)),
            new Item("steam-turbine", "__base__/graphics/icons/steam-turbine.png", new string[0], 10, Color.FromArgb(79, 80, 75)),
            new Item("heat-pipe", "__base__/graphics/icons/heat-pipe.png", new string[0], 50, Color.FromArgb(184, 98, 78)),
            new Item("simple-entity-with-force", "__base__/graphics/icons/steel-chest.png", new string[] {"hidden"}, 50, Color.FromArgb(118, 102, 100)),
            new Item("simple-entity-with-owner", "__base__/graphics/icons/wooden-chest.png", new string[] {"hidden"}, 50, Color.FromArgb(130, 98, 46)),
            new Item("item-with-tags", "__base__/graphics/icons/wooden-chest.png", new string[] {"hidden"}, 1, Color.FromArgb(130, 98, 46)),
            new Item("item-with-label", "__base__/graphics/icons/wooden-chest.png", new string[] {"hidden"}, 1, Color.FromArgb(130, 98, 46)),
            new Item("item-with-inventory", "__base__/graphics/icons/wooden-chest.png", new string[] {"hidden"}, 1, Color.FromArgb(130, 98, 46)),
            new Item("infinity-chest", "__base__/graphics/icons/infinity-chest.png", new string[] {"hidden"}, 10, Color.FromArgb(82, 73, 72)),
            new Item("infinity-pipe", "__base__/graphics/icons/pipe.png", new string[] {"hidden"}, 10, Color.FromArgb(95, 85, 63)),
            new Item("burner-generator", "__base__/graphics/icons/steam-engine.png", new string[] {"hidden"}, 10, Color.FromArgb(99, 89, 74)),
            new Item("linked-chest", "__base__/graphics/icons/linked-chest-icon.png", new string[] {"hidden"}, 10, Color.FromArgb(119, 121, 115)),
            new Item("linked-belt", "__base__/graphics/icons/linked-belt.png", new string[] {"hidden"}, 10, Color.FromArgb(99, 95, 94)),
            new Item("speed-module", "__base__/graphics/icons/speed-module.png", new string[0], 50, Color.FromArgb(71, 112, 127)),
            new Item("speed-module-2", "__base__/graphics/icons/speed-module-2.png", new string[0], 50, Color.FromArgb(71, 117, 133)),
            new Item("speed-module-3", "__base__/graphics/icons/speed-module-3.png", new string[0], 50, Color.FromArgb(70, 124, 140)),
            new Item("effectivity-module", "__base__/graphics/icons/effectivity-module.png", new string[0], 50, Color.FromArgb(86, 121, 67)),
            new Item("effectivity-module-2", "__base__/graphics/icons/effectivity-module-2.png", new string[0], 50, Color.FromArgb(96, 131, 73)),
            new Item("effectivity-module-3", "__base__/graphics/icons/effectivity-module-3.png", new string[0], 50, Color.FromArgb(103, 140, 75)),
            new Item("productivity-module", "__base__/graphics/icons/productivity-module.png", new string[0], 50, Color.FromArgb(136, 89, 60)),
            new Item("productivity-module-2", "__base__/graphics/icons/productivity-module-2.png", new string[0], 50, Color.FromArgb(142, 98, 66)),
            new Item("productivity-module-3", "__base__/graphics/icons/productivity-module-3.png", new string[0], 50, Color.FromArgb(148, 105, 64)),
            new Item("uranium-rounds-magazine", "__base__/graphics/icons/uranium-rounds-magazine.png", new string[] {"light"}, 200, Color.FromArgb(46, 108, 46)),
            new Item("flamethrower-ammo", "__base__/graphics/icons/flamethrower-ammo.png", new string[0], 100, Color.FromArgb(126, 107, 102)),
            new Item("rocket", "__base__/graphics/icons/rocket.png", new string[0], 200, Color.FromArgb(119, 114, 90)),
            new Item("explosive-rocket", "__base__/graphics/icons/explosive-rocket.png", new string[0], 200, Color.FromArgb(120, 97, 95)),
            new Item("atomic-bomb", "__base__/graphics/icons/atomic-bomb.png", new string[] {"light"}, 10, Color.FromArgb(97, 128, 90)),
            new Item("piercing-shotgun-shell", "__base__/graphics/icons/piercing-shotgun-shell.png", new string[0], 200, Color.FromArgb(91, 121, 92)),
            new Item("cannon-shell", "__base__/graphics/icons/cannon-shell.png", new string[0], 200, Color.FromArgb(97, 80, 56)),
            new Item("explosive-cannon-shell", "__base__/graphics/icons/explosive-cannon-shell.png", new string[0], 200, Color.FromArgb(125, 80, 56)),
            new Item("uranium-cannon-shell", "__base__/graphics/icons/uranium-cannon-shell.png", new string[] {"light"}, 200, Color.FromArgb(78, 100, 45)),
            new Item("explosive-uranium-cannon-shell", "__base__/graphics/icons/explosive-uranium-cannon-shell.png", new string[] {"light"}, 200, Color.FromArgb(106, 100, 45)),
            new Item("artillery-shell", "__base__/graphics/icons/artillery-shell.png", new string[0], 1, Color.FromArgb(139, 109, 83)),
            new Item("flamethrower", "__base__/graphics/icons/flamethrower.png", new string[0], 5, Color.FromArgb(95, 85, 79)),
            new Item("shell-particle", "__base__/graphics/icons/submachine-gun.png", new string[] {"hidden"}, 1, Color.FromArgb(86, 81, 78)),
            new Item("tank-flamethrower", "__base__/graphics/icons/flamethrower.png", new string[] {"hidden"}, 1, Color.FromArgb(95, 85, 79)),
            new Item("land-mine", "__base__/graphics/icons/land-mine.png", new string[0], 100, Color.FromArgb(106, 91, 72)),
            new Item("rocket-launcher", "__base__/graphics/icons/rocket-launcher.png", new string[0], 5, Color.FromArgb(101, 101, 89)),
            new Item("combat-shotgun", "__base__/graphics/icons/combat-shotgun.png", new string[0], 5, Color.FromArgb(126, 96, 82)),
            new Item("tank-cannon", "__base__/graphics/icons/tank-cannon.png", new string[] {"hidden"}, 5, Color.FromArgb(116, 91, 53)),
            new Item("artillery-shell-particle", "__base__/graphics/icons/tank-cannon.png", new string[] {"hidden"}, 1, Color.FromArgb(116, 91, 53)),
            new Item("spidertron-rocket-launcher-1", "__base__/graphics/icons/rocket-launcher.png", new string[] {"hidden"}, 1, Color.FromArgb(101, 101, 89)),
            new Item("spidertron-rocket-launcher-2", "__base__/graphics/icons/rocket-launcher.png", new string[] {"hidden"}, 1, Color.FromArgb(101, 101, 89)),
            new Item("spidertron-rocket-launcher-3", "__base__/graphics/icons/rocket-launcher.png", new string[] {"hidden"}, 1, Color.FromArgb(101, 101, 89)),
            new Item("spidertron-rocket-launcher-4", "__base__/graphics/icons/rocket-launcher.png", new string[] {"hidden"}, 1, Color.FromArgb(101, 101, 89)),
            new Item("modular-armor", "__base__/graphics/icons/modular-armor.png", new string[0], 1, Color.FromArgb(112, 84, 52)),
            new Item("power-armor", "__base__/graphics/icons/power-armor.png", new string[0], 1, Color.FromArgb(100, 62, 25)),
            new Item("power-armor-mk2", "__base__/graphics/icons/power-armor-mk2.png", new string[0], 1, Color.FromArgb(103, 66, 30)),
            new Item("cluster-grenade", "__base__/graphics/icons/cluster-grenade.png", new string[0], 100, Color.FromArgb(101, 68, 58)),
            new Item("poison-capsule", "__base__/graphics/icons/poison-capsule.png", new string[0], 100, Color.FromArgb(85, 120, 126)),
            new Item("slowdown-capsule", "__base__/graphics/icons/slowdown-capsule.png", new string[0], 100, Color.FromArgb(133, 112, 93)),
            new Item("distractor-capsule", "__base__/graphics/icons/distractor.png", new string[0], 100, Color.FromArgb(99, 95, 44)),
            new Item("destroyer-capsule", "__base__/graphics/icons/destroyer.png", new string[0], 100, Color.FromArgb(128, 64, 44)),
            new Item("cliff-explosives", "__base__/graphics/icons/cliff-explosives.png", new string[] {"hide-from-bonus-gui"}, 20, Color.FromArgb(65, 108, 133)),
            new Item("firearm-magazine", "__base__/graphics/icons/firearm-magazine.png", new string[0], 200, Color.FromArgb(86, 82, 53)),
            new Item("piercing-rounds-magazine", "__base__/graphics/icons/piercing-rounds-magazine.png", new string[0], 200, Color.FromArgb(90, 53, 53)),
            new Item("shotgun-shell", "__base__/graphics/icons/shotgun-shell.png", new string[0], 200, Color.FromArgb(154, 71, 48)),
            new Item("light-armor", "__base__/graphics/icons/light-armor.png", new string[0], 1, Color.FromArgb(99, 77, 49)),
            new Item("heavy-armor", "__base__/graphics/icons/heavy-armor.png", new string[0], 1, Color.FromArgb(115, 83, 48)),
            new Item("shotgun", "__base__/graphics/icons/shotgun.png", new string[0], 5, Color.FromArgb(125, 86, 71)),
            new Item("solar-panel-equipment", "__base__/graphics/icons/solar-panel-equipment.png", new string[0], 20, Color.FromArgb(74, 113, 131)),
            new Item("fusion-reactor-equipment", "__base__/graphics/icons/fusion-reactor-equipment.png", new string[0], 20, Color.FromArgb(107, 104, 100)),
            new Item("battery-equipment", "__base__/graphics/icons/battery-equipment.png", new string[0], 20, Color.FromArgb(102, 102, 86)),
            new Item("battery-mk2-equipment", "__base__/graphics/icons/battery-mk2-equipment.png", new string[0], 20, Color.FromArgb(95, 93, 79)),
            new Item("belt-immunity-equipment", "__base__/graphics/icons/belt-immunity-equipment.png", new string[0], 20, Color.FromArgb(97, 98, 100)),
            new Item("exoskeleton-equipment", "__base__/graphics/icons/exoskeleton-equipment.png", new string[0], 20, Color.FromArgb(120, 109, 107)),
            new Item("personal-roboport-equipment", "__base__/graphics/icons/personal-roboport-equipment.png", new string[0], 20, Color.FromArgb(103, 95, 75)),
            new Item("personal-roboport-mk2-equipment", "__base__/graphics/icons/personal-roboport-mk2-equipment.png", new string[0], 20, Color.FromArgb(87, 100, 103)),
            new Item("night-vision-equipment", "__base__/graphics/icons/night-vision-equipment.png", new string[0], 20, Color.FromArgb(85, 99, 77)),
            new Item("energy-shield-equipment", "__base__/graphics/icons/energy-shield-equipment.png", new string[0], 20, Color.FromArgb(91, 117, 116)),
            new Item("energy-shield-mk2-equipment", "__base__/graphics/icons/energy-shield-mk2-equipment.png", new string[0], 20, Color.FromArgb(154, 103, 68)),
            new Item("personal-laser-defense-equipment", "__base__/graphics/icons/personal-laser-defense-equipment.png", new string[0], 20, Color.FromArgb(97, 78, 50)),
            new Item("discharge-defense-equipment", "__base__/graphics/icons/discharge-defense-equipment.png", new string[0], 20, Color.FromArgb(108, 134, 119)),
            new Item("discharge-defense-remote", "__base__/graphics/icons/discharge-defense-equipment-controller.png", new string[0], 1, Color.FromArgb(121, 88, 74)),
            new Item("gun-turret", "__base__/graphics/icons/gun-turret.png", new string[0], 50, Color.FromArgb(113, 87, 48)),
            new Item("laser-turret", "__base__/graphics/icons/laser-turret.png", new string[0], 50, Color.FromArgb(114, 93, 40)),
            new Item("flamethrower-turret", "__base__/graphics/icons/flamethrower-turret.png", new string[0], 50, Color.FromArgb(99, 81, 64)),
            new Item("artillery-turret", "__base__/graphics/icons/artillery-turret.png", new string[0], 10, Color.FromArgb(108, 100, 78)),
            new Item("artillery-targeting-remote", "__base__/graphics/icons/artillery-targeting-remote.png", new string[0], 1, Color.FromArgb(197, 103, 79)),
            new Item("arithmetic-combinator", "__base__/graphics/icons/arithmetic-combinator.png", new string[0], 50, Color.FromArgb(83, 93, 77)),
            new Item("decider-combinator", "__base__/graphics/icons/decider-combinator.png", new string[0], 50, Color.FromArgb(107, 95, 57)),
            new Item("constant-combinator", "__base__/graphics/icons/constant-combinator.png", new string[0], 50, Color.FromArgb(109, 69, 65)),
            new Item("power-switch", "__base__/graphics/icons/power-switch.png", new string[0], 50, Color.FromArgb(90, 85, 85)),
            new Item("programmable-speaker", "__base__/graphics/icons/programmable-speaker.png", new string[0], 50, Color.FromArgb(100, 97, 88)),
            new Item("dummy-steel-axe", "__base__/graphics/icons/steel-axe.png", new string[] {"hidden"}, 1, Color.FromArgb(144, 129, 115))
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

        public static void ReallyStupidDefaultItemsCodeGenerator()
        {
            string[] lines = new string[Item.AllItems.Count];
            for (int i = 0; i < Item.AllItems.Count; i++)
            {
                Item I = Item.AllItems[i];

                string flags = "";
                if (I.Flags.Length == 0)
                {
                    flags = "new string[0]";
                }
                else
                {
                    flags = "new string[] {";
                    for (int ff = 0; ff < I.Flags.Length; ff++)
                    {
                        flags += "\"" + I.Flags[ff] + "\"";
                        if (ff != I.Flags.Length - 1)
                        {
                            flags += ",";
                        }
                    }
                    flags += "}";
                }

                lines[i] = "new Item(\"" + I.Name + "\", \"" + I.IconPath + "\", " + flags + ", " + I.StackSize.ToString() + ", Color.FromArgb(" + I.AverageColor.R.ToString() + ", " + I.AverageColor.G.ToString() + ", " + I.AverageColor.B.ToString() + ")),";
            }
            File.WriteAllLines("GeneratedCode.txt", lines);
        }
    }

    /// <summary>
    /// Holds all needed information for an item including its Icon bitmap and stackSize
    /// </summary>
    public class Item
    {
        /// <summary>
        /// used to lookup items without iterating through the list
        /// </summary>
        private static Dictionary<string, Item> ItemLibrary = new Dictionary<string, Item>();

        /// <summary>
        /// Allows me to iterate through all the average colors and find their item name
        /// </summary>
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
        /// Clears all loaded item data so a different set can be loaded
        /// </summary>
        public static void ClearItemsData()
        {
            ItemLibrary.Clear();
            ItemColorLookup.Clear();
            AllItems.Clear();
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

        }

        public Item(string Name, string IconPath, string[] Flags, int StackSize)
        {
            this.Name = Name;
            this.IconPath = IconPath;
            this.Flags = Flags;
            this.StackSize = StackSize;

        }

        public Item(string Name, string IconPath, string[] Flags, int StackSize, System.Drawing.Color AverageColor)
        {
            this.Name = Name;
            this.IconPath = IconPath;
            this.Flags = Flags;
            this.StackSize = StackSize;
            this.AverageColor = AverageColor;
            //Log.New("Adding " + Name + " To Library");
            
        }
        
        public Item(string Name, string IconPath, string[] Flags, int StackSize, System.Drawing.Color AverageColor, Bitmap Icon)
        {
            this.Name = Name;
            this.IconPath = IconPath;
            this.Flags = Flags;
            this.StackSize = StackSize;
            this.AverageColor = AverageColor;
            this.Icon = Icon;
            //Log.New("Adding " + Name + " To Library");
            
        }

        public void Init()
        {
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
