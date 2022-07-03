using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Threading;

namespace TheBluePrinter
{

    /// <summary>
    /// Window Manager
    /// Contains references to all open windows as well as managing some related functions
    /// </summary>
    public static class WM
    {
        public static MainWindow MainWindow;

        public static ConsoleWindow ConsoleWindow = new ConsoleWindow();

        public static bool Closing = false;

        public static bool SettingsMenuOpen = false;

        /// <summary>
        /// Updates the text at the bottom
        /// </summary>
        /// <param name="message"></param>
        public static void UpdateLatestLogMessage(LogMessage message)
        {
            WM.MainWindow.LogTextLatestMainWindow.Content = message.Content;
            if (message.MessageColor != LogMessage.DefaultColor)
            {
                WM.MainWindow.LogTextLatestMainWindow.Foreground = new SolidColorBrush(message.MessageColor);
            }
            else
            {
                WM.MainWindow.LogTextLatestMainWindow.Foreground = new SolidColorBrush(CC.white);
            }
        }

        public static void OpenConsoleWindow()
        {
            if (ConsoleWindow == null)
            {
                ConsoleWindow = new ConsoleWindow();
            }
            if(ConsoleWindow.WindowState == System.Windows.WindowState.Minimized)
            {
                ConsoleWindow.WindowState = System.Windows.WindowState.Normal;
            }
            else
            {
                ConsoleWindow.Show();
            }
        }

        /// <summary>
        /// Updates the primary and secondary colors throughout the UI with the colors from Settings.PrimaryColor and Settings.SecondaryColor
        /// </summary>
        public static void UpdateColors()
        {
            SolidColorBrush Primary = new SolidColorBrush(Settings.PrimaryColor);
            SolidColorBrush Secondary = new SolidColorBrush(Settings.SecondaryColor);
            MainWindow.MainWindowBGP.Background = Primary;
            MainWindow.MainWindow1BGS.Background = Secondary;
            MainWindow.MainWindow2FGP.Foreground = Primary;
            MainWindow.MainWindow3BGP.Background = Primary;
            MainWindow.MainWindow4FGP.Foreground = Primary;
            MainWindow.SourceImageReminderLabel.Foreground = Primary;
            MainWindow.MainWindow6FGP.Foreground = Primary;
            MainWindow.PreviewImageReminderLabel.Foreground = Primary;
            MainWindow.MainWindow8FGP.Foreground = Primary;
            MainWindow.MainWindow9FGP.Foreground = Primary;
            MainWindow.MainWindow10FGS.Foreground = Secondary;
            MainWindow.MainWindow11FGS.Foreground = Secondary;
            MainWindow.MainWindow12FGP.Foreground = Primary;
            MainWindow.MainWindow13FGP.Foreground = Primary;
            //MainWindow.ImageSourceTabControl.Background = Primary;
            MainWindow.MainWindow15FGP.Foreground = Primary;
            MainWindow.MainWindow16FGP.Foreground = Primary;
            //MainWindow.MainWindow17FGS.Foreground = Secondary;
            //MainWindow.MainWindow18FGP.Foreground = Primary;
            //MainWindow.MainWindow19FGP.Foreground = Primary;
            MainWindow.FormatFactorioIconCheckbox.Foreground = Primary;
            MainWindow.MainWindow20FGP.Foreground = Primary;
            MainWindow.MainWindow21FGP.Foreground = Primary;
            //MainWindow.MainWindow22FGP.Foreground = Primary;
            MainWindow.MainWindow23FGP.Foreground = Primary;
            MainWindow.MainWindow24FGP.Foreground = Primary;
            MainWindow.MainWindow25FGP.Foreground = Primary;
            MainWindow.MainWindow26FGPBGS.Foreground = Primary;
            MainWindow.MainWindow26FGPBGS.Background = Secondary;
            MainWindow.MainWindow27FGPBGS.Foreground = Primary;
            MainWindow.MainWindow27FGPBGS.Background = Secondary;
            MainWindow.MainWindow28FGP.Foreground = Primary;
            MainWindow.CVConvertToJSON.Foreground = Primary;
            MainWindow.CVConvertToBlueprint.Foreground = Primary;
            MainWindow.CVBlueprintRevert.Foreground = Primary;
            MainWindow.CVJSONRevert.Foreground = Primary;
            MainWindow.CVBlueprintCopy.Foreground = Primary;
            MainWindow.CVBlueprintPaste.Foreground = Primary;
            MainWindow.CVJSONCopy.Foreground = Primary;
            MainWindow.CVJSONPaste.Foreground = Primary;
            MainWindow.MainWindow38FGP.Foreground = Primary;
            MainWindow.MainWindow39FGP.Foreground = Primary;
            //MainWindow.ISFGTitleBar.Fill = Primary;
            MainWindow.ISMoveButton.Foreground = Primary;
            MainWindow.ISClickToMove.Foreground = Primary;
            MainWindow.ISSelectAllowedLabelFGP.Foreground = Primary;
            MainWindow.ISSelectDisallowedLabelFGP.Foreground = Primary;
            MainWindow.ISSelectAllAllowed.Foreground = Primary;
            MainWindow.ISSelectAllDisallowed.Foreground = Primary;
            MainWindow.ISSelectNoneAllowed.Foreground = Primary;
            MainWindow.ISSelectNoneDisallowed.Foreground = Primary;
            MainWindow.ISShowHidden.Foreground = Primary;
            MainWindow.ISShowUnstackable.Foreground = Primary;
            MainWindow.AllItemsSelectedCountRectangle.Foreground = Primary;
            MainWindow.AllowedItemsSelectedCountRectangle.Foreground = Primary;
            MainWindow.AllItemsTotalCountRectangle.Foreground = Primary;
            MainWindow.AllowedItemsTotalCountRectangle.Foreground = Primary;
            MainWindow.SettingsParseItemsButton.Foreground = Primary;
            MainWindow.SetttingsGenerateInfinityButton.Foreground = Primary;
            MainWindow.SettingsSetPrimaryColor.Foreground = Primary;
            MainWindow.SettingsSetSecondaryColor.Foreground = Primary;
            MainWindow.SettingsApplicationExit.Foreground = Primary;
            MainWindow.SettinsRestoreDefaults.Foreground = Primary;
            //MainWindow.MainWindow40FGP.Fill = Primary;
            //MainWindow.MainWindow41FGP.Fill = Primary;
            MainWindow.Reload.Foreground = Primary;
            MainWindow.Reload1.Foreground = Primary;
            MainWindow.ICOTicks1.Foreground = Primary;
            MainWindow.ICOTicks2.Foreground = Primary;
            MainWindow.ICOTicks3.Foreground = Primary;
            MainWindow.ICOTicks4.Foreground = Primary;
            MainWindow.BackgroundColor.Foreground = Primary;
            MainWindow.PreviewTicks1.Foreground = Secondary;
            MainWindow.PreviewTicks2.Foreground = Secondary;
            MainWindow.PreviewTicks3.Foreground = Secondary;
            MainWindow.PreviewTicks4.Foreground = Secondary;
            MainWindow.ResizeImageCheckbox.Foreground = Primary;
            MainWindow.RICGLabelX.Foreground = Primary;
            MainWindow.RICGLabelY.Foreground = Primary;
            MainWindow.RICGApply.Foreground = Primary;



            Log.New("Updated Primary Color to :" + Primary.ToString(), Settings.PrimaryColor);
            Log.New("Updated Secondary Color to :" + Secondary.ToString(), Settings.SecondaryColor);

            //Update all user contrls currently instantiated
            foreach(ItemSelectionWidget widget in ItemSelectionWidget.AllWidgets)
            {
                widget.NameplateBackgroundRectangle.Fill = Primary;
                widget.ItemNameLabel.Foreground = Secondary;
            }
            Log.New("Updated " + ItemSelectionWidget.AllWidgets.Count + " widgets");
            Log.New("New Color Scheme Applied");
        }

        /// <summary>
        /// Forces the console window to close as well and ensures the application exits properly
        /// </summary>
        public static void CloseMainWindow()
        {
            Closing = true;
            ConsoleWindow.Close();
        }


       
    }

    

    
}
