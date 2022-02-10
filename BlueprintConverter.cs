using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ZLibNet;
using System.Text.Json;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;

namespace TheBluePrinter
{
    /// <summary>
    /// Responsible for Converting between a factorio blueprint string and JSON text and also the other way around
    /// Also controls most functions of the "Convert Blueprint" tab
    /// </summary>
    class BlueprintConverter
    {

        private static Stack<FlowDocument> BlueprintRevertStack = new Stack<FlowDocument>();
        private static Stack<FlowDocument> JSONRevertStack = new Stack<FlowDocument>();

        /// <summary>
        /// Main Entrypoint for Main Window buttons to covert between the two text boxes
        /// </summary>
        public static void ConvertJSONToBlueprint()
        {
            FlowDocument doc = new FlowDocument();
            TextRange range = new TextRange(WM.MainWindow.CVJSONTextBox.Document.ContentStart, WM.MainWindow.CVJSONTextBox.Document.ContentEnd);
            doc.Blocks.Add(new Paragraph(new Run(BlueprintConverter.ConvertToBlueprint(range.Text))));
            if (BlueprintRevertStack.Count > 0)
            {
                TextRange range1 = new TextRange(WM.MainWindow.CVBlueprintTextBox.Document.ContentStart, WM.MainWindow.CVBlueprintTextBox.Document.ContentEnd);
                TextRange range2 = new TextRange(BlueprintRevertStack.Peek().ContentStart, BlueprintRevertStack.Peek().ContentEnd);
                if (range1.Text == range2.Text)
                BlueprintRevertStack.Push(WM.MainWindow.CVBlueprintTextBox.Document);
            }
            else
            {
                BlueprintRevertStack.Push(WM.MainWindow.CVBlueprintTextBox.Document);
            }
            WM.MainWindow.CVBlueprintTextBox.Document = doc;

        }

        
        /// <summary>
        /// Main Entrypoint for Main Window buttons to covert between the two text boxes
        /// </summary>
        public static void ConvertBlueprintToJSON()
        {
            FlowDocument doc = new FlowDocument();
            TextRange range = new TextRange(WM.MainWindow.CVBlueprintTextBox.Document.ContentStart, WM.MainWindow.CVBlueprintTextBox.Document.ContentEnd);
            doc.Blocks.Add(new Paragraph(new Run(BlueprintConverter.ConvertToJSON(range.Text))));
            if (JSONRevertStack.Count > 0)
            {
                TextRange range1 = new TextRange(WM.MainWindow.CVJSONTextBox.Document.ContentStart, WM.MainWindow.CVJSONTextBox.Document.ContentEnd);
                TextRange range2 = new TextRange(JSONRevertStack.Peek().ContentStart, JSONRevertStack.Peek().ContentEnd);
                if (range1.Text == range2.Text)
                    JSONRevertStack.Push(WM.MainWindow.CVJSONTextBox.Document);
            }
            else
            {
                JSONRevertStack.Push(WM.MainWindow.CVJSONTextBox.Document);
            }
            WM.MainWindow.CVJSONTextBox.Document = doc;
        }

        public static void Copy(bool TFLeftRight)
        {
            if (TFLeftRight)
            {
                TextRange range = new TextRange(WM.MainWindow.CVBlueprintTextBox.Document.ContentStart, WM.MainWindow.CVBlueprintTextBox.Document.ContentEnd);

                Clipboard.SetText(range.Text);
            }
            else
            {
                TextRange range = new TextRange(WM.MainWindow.CVJSONTextBox.Document.ContentStart, WM.MainWindow.CVJSONTextBox.Document.ContentEnd);

                Clipboard.SetText(range.Text);
            }
        }

        public static void Paste(bool TFLeftRight)
        {
            if (TFLeftRight)
            {
                FlowDocument doc = new FlowDocument();
                doc.Blocks.Add(new Paragraph(new Run(Clipboard.GetText())));
                WM.MainWindow.CVBlueprintTextBox.Document = doc;
            }
            else
            {
                FlowDocument doc = new FlowDocument();
                doc.Blocks.Add(new Paragraph(new Run(Clipboard.GetText())));
                WM.MainWindow.CVJSONTextBox.Document = doc;
            }
        }

        public static void Revert(bool TFLeftRight)
        {
            if (TFLeftRight)
            {
                if (BlueprintRevertStack.Count > 0)
                    WM.MainWindow.CVBlueprintTextBox.Document = BlueprintRevertStack.Pop();
                else
                    Log.New("There is nothing left to revert");
            }
            else
            {
                if (JSONRevertStack.Count > 0)
                    WM.MainWindow.CVJSONTextBox.Document = JSONRevertStack.Pop();
                else
                    Log.New("There is nothing left to revert");
            }
        }

        /// <summary>
        /// Converts a factorio blueprint string into formatted JSON text
        /// </summary>
        /// <param name="blueprint"></param>
        /// <returns></returns>
        public static string ConvertToJSON(string blueprint)
        {
            return decompress(blueprint.Substring(1));
        }

        /// <summary>
        /// Converts JSON text to a blueprint string
        /// </summary>
        /// <param name="JSON"></param>
        /// <returns></returns>
        public static string ConvertToBlueprint(string JSON)
        {
            return "0" + compress(JSON);
        }


        /// <summary>
        /// Formats json text to be readable
        /// Thanks Andrew Shepherd
        /// https://stackoverflow.com/questions/2661063/how-do-i-get-formatted-json-in-net-using-c
        /// </summary>
        static string FormatJsonText(string jsonString)
        {
            try
            {
                var doc = JsonDocument.Parse(
                    jsonString,
                    new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true
                    }
                );
                MemoryStream memoryStream = new MemoryStream();
                using (
                    var utf8JsonWriter = new Utf8JsonWriter(
                        memoryStream,
                        new JsonWriterOptions
                        {
                            Indented = true
                        }
                    )
                )
                {
                    doc.WriteTo(utf8JsonWriter);
                }
                return new System.Text.UTF8Encoding()
                    .GetString(memoryStream.ToArray());
            }
            catch (Exception e)
            {
                Log.New(e.Message, CC.red);
                return jsonString;
            }
        }
       

        /// <summary>
        /// This took an hour to get working
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string decompress(string input)
        {
            try
            {
                byte[] data = Convert.FromBase64String(input);


                MemoryStream stream = new MemoryStream(data);
                MemoryStream outStream = new MemoryStream();


                ZLibStream stream1 = new ZLibStream(stream, CompressionMode.Decompress, CompressionLevel.Level9, true);

                string text = new StreamReader(stream1).ReadToEnd();

                string result = FormatJsonText(text);
                return result;
            }
            catch (Exception e)
            {
                Log.New(e.Message,CC.red);
                return e.Message;
            }
            
        }

        
        /// <summary>
        /// Why did this take 2 days to get working?
        /// Streams suck fat dick
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string compress(string input)
        {
            try
            {
                
                byte[] data = Encoding.UTF8.GetBytes(input);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZLibStream dStream = new ZLibStream(ms, CompressionMode.Compress, CompressionLevel.Level9, true))
                    {
                        dStream.Write(data, 0, input.Length);
                    }
                    byte[] result = ms.ToArray();
                    string text = Convert.ToBase64String(result);
                    return text;
                }
            }
            catch (Exception e)
            {
                Log.New(e.Message, CC.red);
                return e.Message;
            }
        }

    }
}
