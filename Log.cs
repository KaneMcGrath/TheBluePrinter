﻿using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TheBluePrinter
{
    /// <summary>
    /// Main class for logging messages
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Main List that stores all LogMessages
        /// </summary>
        public static List<LogMessage> MessageList = new List<LogMessage>();
        
        /// <summary>
        /// Converts Hexidecimal color codes to a Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HextoColor(string hex)
        {
            string s = hex;
            if (hex.Length == 7) s = hex.Substring(1);
            if (hex.Length == 6)
            {
                int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                return Color.FromRgb((byte)r, (byte)g, (byte)b);
            }
            return Color.FromRgb(0, 0, 0);
        }

        public static void New(string Message)
        {
            Console.WriteLine(Message);
            LogMessage logMessage = new LogMessage(Message);
            MessageList.Add(logMessage);
            UpdateLatestMessage(logMessage);
        }



        public static void New(string Message, Color MessageColor)
        {
            Console.WriteLine(Message);
            LogMessage logMessage = new LogMessage(Message, MessageColor);
            MessageList.Add(logMessage);
            UpdateLatestMessage(logMessage);
        }


        public static void UpdateLatestMessage(LogMessage logMessage)
        {
            WM.UpdateLatestLogMessage(logMessage);
            WM.ConsoleWindow.AddNewMessage(logMessage);
        }


    }

    public static class CC
    {
        public static Color red = Log.HextoColor("ff0000");
        public static Color white = Log.HextoColor("ffffff");
    }
    /// <summary>
    /// Holds a individual message and all of its properties
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Default text color
        /// Latest message will check for this and invert if it remains default.
        /// </summary>
        public static Color DefaultColor = Color.FromRgb(0, 0, 0);

        /// <summary>
        /// The text that will be displayed for the message
        /// </summary>
        public string Content;

        /// <summary>
        /// Text color of the message
        /// </summary>
        public Color MessageColor;

        public LogMessage(string Message)
        {
            Content = Message;
            this.MessageColor = DefaultColor;
        }

        public LogMessage(string Message, Color MessageColor)
        {
            Content = Message;
            this.MessageColor = MessageColor;
        }
    }
}