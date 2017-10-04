using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace OrgTracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ValueSet table;
        public MainPage()
        {
            this.InitializeComponent();
            App.AppServiceConnected += MainPage_AppServiceConnected;

            // extend client area under title bar and make the title transparent
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StartupTask startupTask = await StartupTask.GetAsync("MyStartupId");
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    // Task is disabled but can be enabled.
                    StartupTaskState newState = await startupTask.RequestEnableAsync();
                    break;
                case StartupTaskState.DisabledByUser:
                    // Task is disabled and user must enable it manually.
                    MessageDialog dialog = new MessageDialog(
                        "I know you don't want this app to run " +
                        "as soon as you sign in, but if you change your mind, " +
                        "you can enable this in the Startup tab in Task Manager.",
                        "TestStartup");
                    await dialog.ShowAsync();
                    break;
                case StartupTaskState.DisabledByPolicy:
                    break;
                case StartupTaskState.Enabled:
                    break;
            }
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Org Grid", Icon = new SymbolIcon(Symbol.People), Tag = "grid", AccessKey = "G" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Charts", Icon = new SymbolIcon(Symbol.ThreeBars), Tag = "charts", AccessKey = "C" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Payroll", Icon = new SymbolIcon(Symbol.Calculator), Tag = "payroll", AccessKey = "R" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Permission", Icon = new SymbolIcon(Symbol.Permissions), Tag = "permissions", AccessKey = "P" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Calendar", Icon = new SymbolIcon(Symbol.Calendar), Tag = "calendar", AccessKey = "D" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "International", Icon = new SymbolIcon(Symbol.Globe), Tag = "int", AccessKey = "I" });

            NavView.Header = new PageHeader(string.Format("   {0} Organization Grid", App.Organization), true, false);
            ContentFrame.Navigate(typeof(GridPage));
            NavView.IsPaneOpen = false;
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args != null && args.IsSettingsInvoked)
            {
                NavView.Header = new PageHeader("   Settings", false, false);
                ContentFrame.Navigate(typeof(NotImplPage));
            }
            else
            {
                object item = "Org Grid";
                if (args != null)
                {
                    item = args.InvokedItem;
                }
                switch (item)
                {
                    case "Org Grid":
                        NavView.Header = new PageHeader(string.Format("   {0} Organization Grid", App.Organization), true, false);
                        ContentFrame.Navigate(typeof(GridPage));
                        break;
                    case "Charts":
                        NavView.Header = new PageHeader(string.Format("   {0} Organization Charts", App.Organization), false, false);
                        ContentFrame.Navigate(typeof(ChartPage));
                        break;
                    case "Payroll":
                        NavView.Header = new PageHeader(string.Format("   {0} Payroll", App.Organization), false, false);
                        ContentFrame.Navigate(typeof(NotImplPage));
                        break;
                    case "Permission":
                        NavView.Header = new PageHeader(string.Format("   {0} Permission", App.Organization), false, false);
                        ContentFrame.Navigate(typeof(NotImplPage));
                        break;
                    case "Calendar":
                        NavView.Header = new PageHeader(string.Format("   {0} Calendar", App.Organization), false, false);
                        ContentFrame.Navigate(typeof(NotImplPage));
                        break;
                    case "International":
                        NavView.Header = new PageHeader(string.Format("   {0} International", App.Organization), false, false);
                        ContentFrame.Navigate(typeof(NotImplPage));
                        break;
                }
            }
        }


        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            // create a ValueSet from the datacontext
            table = new ValueSet();
            if (ContentFrame.Content is GridPage)
            {
                ObservableCollection<Employee> items = (ContentFrame.Content as GridPage).GridItems;
                table.Add("REQUEST", "CreateSpreadsheet");
                for (int i = 0; i < items.Count; i++)
                {
                    table.Add("Name" + i.ToString(), items[i].Name);
                    table.Add("Title" + i.ToString(), items[i].Title);
                    table.Add("Department" + i.ToString(), items[i].Department);
                    table.Add("Manager" + i.ToString(), items[i].Manager);
                    table.Add("Phone" + i.ToString(), items[i].OfficePhone);
                    table.Add("Email" + i.ToString(), items[i].Mail);
                }

                // launch the fulltrust process and for it to connect to the app service            
                if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
                {
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("This feature is only available on Windows 10 Desktop SKU");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void MainPage_AppServiceConnected(object sender, EventArgs e)
        {
            // send the ValueSet to the fulltrust process
            AppServiceResponse response = await App.Connection.SendMessageAsync(table);

            // check the result
            object result;
            response.Message.TryGetValue("RESPONSE", out result);
            if (result.ToString() != "SUCCESS")
            {
                MessageDialog dialog = new MessageDialog(result.ToString());
                await dialog.ShowAsync();
            }
            // no longer need the AppService connection
            App.AppServiceDeferral.Complete();
        }

        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Not yet implemented");
            await dialog.ShowAsync();
        }
    }

    public class PageHeader
    {
        public string Title { get; set; }
        public bool CanExport { get; set; }
        public bool CanPrint { get; set; }

        public PageHeader(string title, bool canExport, bool canPrint)
        {
            Title = title;
            CanExport = canExport;
            CanPrint = canPrint;
        }
    }
}
