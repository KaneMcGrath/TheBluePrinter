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
using System.Windows.Shapes;

namespace TheBluePrinter
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        public ConsoleWindow()
        {
            InitializeComponent();
        }

        public void AddNewMessage(LogMessage message)
        {
            bool flag = false;

            if (ConsoleLogScrollViewer.ContentVerticalOffset == ConsoleLogScrollViewer.ScrollableHeight)
                flag = true;
            TextBlock block = new TextBlock();
            block.Text = message.Content;
            block.TextWrapping = TextWrapping.Wrap;
            block.Margin = new Thickness(3.0);
            block.Foreground = new SolidColorBrush(message.MessageColor);

            Line line = new Line();
            line.X1 = 1.0;
            line.Stroke = new SolidColorBrush(Color.FromRgb(0,0,0));
            line.StrokeThickness = 1.0;
            line.Stretch = Stretch.Fill;

            ConsoleLogStackPanel.Children.Add(block);
            ConsoleLogStackPanel.Children.Add(line);

            LogTextLatestConsole.Content = message.Content;

            if (flag) ConsoleLogScrollViewer.ScrollToBottom();
           
        }

        private void ConsoleWindowClosed(object sender, EventArgs e)
        {
            
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!WM.Closing)
                e.Cancel = true;
            Hide();
        }

        private void Testt(object sender, MouseEventArgs e)
        {
            ConsoleLogScrollViewer.ToolTip = new ToolTip().Content = ConsoleLogScrollViewer.ContentVerticalOffset.ToString() + "," + ConsoleLogScrollViewer.ScrollableHeight.ToString();
        }
    }
}
