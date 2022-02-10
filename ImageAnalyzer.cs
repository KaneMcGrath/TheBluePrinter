using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TheBluePrinter
{


    /// <summary>
    /// Handles almoast all image analysis and manipulation
    /// </summary>
    class ImageAnalyzer
    {
        public static List<string> lastUsedItems = new List<string>();

        public static int[,] CreateItemImage(Bitmap input)
        {

            lastUsedItems.Clear();
            int[,] result = new int[input.Height, input.Width];
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Color pixel = input.GetPixel(x, y);
                    Color nearest = NearestAllowedColor(pixel);
                    string item = Item.ItemColorLookup[nearest];
                    if (!lastUsedItems.Contains(item))
                    {
                        lastUsedItems.Add(item);
                    }
                    int id = Item.findID(item);
                    result[y, x] = id;
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the average color of an image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Color AverageColor(Bitmap image)
        {
            //for each pixel rgb values are averaged seperatly
            int red = 0;
            int blue = 0;
            int green = 0;

            // counts transparent pixels and ignores them in average
            // argb values can have color in completly transparent pixels
            // though factorio icons are quite clean
            int alpha = 0;
            int alphaThreshold = 200;


            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color c = image.GetPixel(x, y);
                    if (c.A < alphaThreshold)
                    {
                        alpha++;
                    }
                    else
                    {
                        red += c.R;
                        blue += c.B;
                        green += c.G;
                    }
                }
            }

            int total = (image.Width * image.Height) - alpha;

            int finalRed = red / total;
            int finalBlue = blue / total;
            int finalGreen = green / total;



            return Color.FromArgb(finalRed, finalGreen, finalBlue);

        }




        public static float lastMin = 0f;
        public static Color NearestColor(Color c)
        {
            Color result = Color.Black;
            
            float currentMin = float.MaxValue;
            foreach (Color color in Item.ItemColorLookup.Keys)
            {
                float distance = ColorDistanceRGB(c, color);

                if (distance < currentMin)
                {
                    currentMin = distance;
                    result = color;
                }
            }
            lastMin = currentMin;
            return result;
        }

        public static Color NearestAllowedColor(Color c)
        {
            Color result = Color.Black;
            List<Item> allowedItems = ItemSelector.GetAllowedItems();

            float currentMin = float.MaxValue;
            foreach (Color color in Item.ItemColorLookup.Keys)
            {
                float distance = ColorDistanceRGB(c, color);

                if ((distance < currentMin) && allowedItems.Contains(Item.Find(Item.ItemColorLookup[color])))
                {
                    currentMin = distance;
                    result = color;
                }
            }
            lastMin = currentMin;
            return result;
        }

        /// <summary>
        /// Determines distance from one color to another 
        /// </summary>
        /// <param name="myColor"></param>
        /// <param name="targetColor"></param>
        /// <returns></returns>
        public static float ColorDistanceRGB(Color myColor, Color targetColor)
        {


            float r = Math.Abs(myColor.R - targetColor.R);
            float g = Math.Abs(myColor.G - targetColor.G);
            float b = Math.Abs(myColor.B - targetColor.B);
            if (myColor.A < 50)
            {
                return targetColor.R + targetColor.G + targetColor.B;
            }
            return r + g + b;
        }

        /// <summary>
        /// crops a factorio icon into the selected mipmap
        /// </summary>
        /// <param name="icon">input factorio source icon</param>
        /// <param name="mipmapLevel"> which level of mipmap starting from 0 = highest</param>
        /// <returns></returns>
        public static Bitmap FormatFactorioIconImage(Bitmap icon, int mipmapLevel)
        {

            if (mipmapLevel == 0)
            {
                Bitmap result = new Bitmap(64, 64);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(icon, Point.Empty);
                }
                return result;
            }
            return null;
        }
    }
}
