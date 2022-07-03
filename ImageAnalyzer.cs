using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace TheBluePrinter
{
    /// <summary>
    /// Handles almoast all image analysis and manipulation
    /// </summary>
    class ImageAnalyzer
    {
        public static List<string> lastUsedItems = new List<string>();

        public static Bitmap LastPreviewImage;
        /// <summary>
        /// Determins what Items should be placed where and saves their index in the allItems list into a 2d array
        /// This is like a chain of hacks that eventually works, should clean this up later but I probably wont.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int[,] CreateItemImage(Bitmap input)
        {
            if (Item.ItemColorLookup.Keys.Count == 0)
            {
                Log.New("Failed to create item map: Color averages are not loaded!", CC.red);
                return null;
            }
            Log.New("Creating Item Map", CC.yellow);
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

        public static Bitmap FillAlphaChannel(Bitmap image, Color color)
        {
            Bitmap result = (Bitmap)image.Clone();
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(color);
                g.DrawImage(image, 0, 0);
            }
            return result;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// thanks mpen. https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
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
            if (mipmapLevel == 1)
            {
                Bitmap result = new Bitmap(32, 32);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(icon, -64, 0);
                }
                return result;
            }
            if (mipmapLevel == 2)
            {
                Bitmap result = new Bitmap(16, 16);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(icon, new Point(-96, 0));
                }
                return result;
            }
            if (mipmapLevel == 3)
            {
                Bitmap result = new Bitmap(8, 8);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(icon, new Point(-112, 0));
                }
                return result;
            }
            return null;
        }

        public static Bitmap CreatePreviewImage(Bitmap input, bool debugPixels = false, int mipmapLevel = 0)
        {
            int iconRes = 64;
            
            if (mipmapLevel == 0)           iconRes = 64;
            else if (mipmapLevel == 1)      iconRes = 32;
            else if (mipmapLevel == 2)      iconRes = 16;
            else if (mipmapLevel == 3)      iconRes = 8;
            int iconHalf = (iconRes / 2);
            
            int[,] itemMap = CreateItemImage(input);
            if (itemMap == null)
            {
                return null;
            }

            if (File.Exists(GeneratePrinter.ValidFactorioPath + "\\data\\base\\graphics\\entity\\express-transport-belt\\"))

            Log.New("Building Preview", CC.yellow);
            Bitmap result = new Bitmap((input.Width * iconRes) + iconHalf, input.Height * iconRes);
            using (Graphics g = Graphics.FromImage(result))
            {
                for (int y = 0; y < itemMap.GetLength(0); y++)
                {
                    for (int x = 0; x < itemMap.GetLength(1); x++)
                    {
                        Color pixel = input.GetPixel(x, y);
                        Color nearest = NearestColor(pixel);
                        Item mappedItem = Item.FindByID(itemMap[y, x]);
                        Bitmap icon = FormatFactorioIconImage(mappedItem.Icon, mipmapLevel);

                        if (!lastUsedItems.Contains(mappedItem.Name))
                        {
                            lastUsedItems.Add(mappedItem.Name);
                        }
                        g.DrawImage(icon, new Point(x * iconRes, y * iconRes));
                        g.DrawImage(icon, new Point(x * iconRes + iconHalf, y * iconRes));
                        if (debugPixels)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                for (int k = 0; k < 10; k++)
                                {
                                    result.SetPixel(x * iconRes + k, y * iconRes + i, pixel);
                                    result.SetPixel(x * iconRes + 10 + k, y * iconRes + i, nearest);
                                }
                            }
                        }
                    }
                }
            }
            LastPreviewImage = result;
            Log.New("Created preview image");
            return result;
        }
    }
}
