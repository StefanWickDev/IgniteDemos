using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NorthwindDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        Microsoft.Toolkit.Uwp.Helpers.PrintHelper printHelper;
        public SettingsPage()
        {
            this.InitializeComponent();
            SqlTextbox.Text = (App.Current as App).ConnectionString;
            UseToolkit.IsChecked = (App.Current as App).UsePrinter;
        }
        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use UWP Community Toolkit PrintHelper to print sample page with printer UI
                printHelper = new Microsoft.Toolkit.Uwp.Helpers.PrintHelper(DirectPrintContainer);
                printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
                printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
                printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;
                await printHelper.ShowPrintUIAsync("UWP Community Toolkit PrintHelper", true);
            }
            catch (Exception ePrinter)
            {
                await new MessageDialog("Exception: " + ePrinter.Message).ShowAsync();
            }
        }

        #region EventHandlers for UWP Community Toolkit PrintHelper
        private void ReleasePrintHelper()
        {
            printHelper.Dispose();
        }
        private async void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();
            var dialog = new MessageDialog("Printing done.");
            await dialog.ShowAsync();
        }
        private async void PrintHelper_OnPrintFailed()
        {
            ReleasePrintHelper();
            var dialog = new MessageDialog("Printing failed.");
            await dialog.ShowAsync();
        }
        private void PrintHelper_OnPrintCanceled()
        {
            ReleasePrintHelper();
        }
#endregion
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).UsePrinter = (UseToolkit.IsChecked == true);
            (App.Current as App).ConnectionString = SqlTextbox.Text;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["useprinter"] = (UseToolkit.IsChecked == true);
            localSettings.Values["sqlconnection"] = SqlTextbox.Text;
        }
    }
}
