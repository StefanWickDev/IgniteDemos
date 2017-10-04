using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Reflection;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NorthwindDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationView currentView;
        public MainPage()
        {
            this.InitializeComponent();

            // Set up reference to this window so secondary windows can find it
            (App.Current as App).MainPageInstance = this;

            // This is needed to ensure secondary windows are all closed when this one is
            currentView = ApplicationView.GetForCurrentView();
            currentView.Consolidated += CurrentView_Consolidated;

            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            currentView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            currentView.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        private void CurrentView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            // Clean up code to shut down secondary windows as this one closes
            Application.Current.Exit();
        }

        private void MoreInfoButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var version = typeof(App).GetTypeInfo().Assembly.GetName().Version;
            NavView.Header = "   About Northwind Demo version " + version.ToString();
            ContentFrame.Navigate(typeof(AboutPage));
        }

        private async void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args != null && args.IsSettingsInvoked)
            {
                NavView.Header = "   Settings";
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else 
            {
                object item = "Products";
                if(args != null)
                {
                    item = args.InvokedItem;
                }
                switch (item)
                {
                    case "Products":
                        NavView.Header = "   Northwind Traders Products";
                        ContentFrame.Navigate(typeof(ProductsPage));
                        break;

                    case "Orders":
                        NavView.Header = "   View Orders";
                        ContentFrame.Navigate(typeof(OrdersPage));
                        break;

                    case "Chart":
                        NavView.Header = "   Sales by Category";
                        ContentFrame.Navigate(typeof(ChartPage));
                        break;

                    case "New order":
                        object o = ContentFrame.Content;
                        if (o is ProductsPage)
                        {
                            await (o as ProductsPage).GetNewOrder();
                        }
                        else
                        {
                            NavView.Header = "   Northwind Traders Products";
                            ContentFrame.Navigate(typeof(ProductsPage), "new");
                        }
                        break;

                    case "Export to CSV":
                        NavView.Header = "   Export to CSV";
                        ContentFrame.Navigate(typeof(ExportPage));
                        break;
                }
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Products", Icon = new SymbolIcon(Symbol.Shop), Tag = "products", AccessKey = "P" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "New order", Icon = new SymbolIcon(Symbol.Add), Tag = "order", AccessKey = "N" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Orders", Icon = new SymbolIcon(Symbol.ViewAll), Tag = "orders", AccessKey = "O" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Chart", Icon = new SymbolIcon(Symbol.ThreeBars), Tag = "chart", AccessKey = "C" });
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "Export to CSV", Icon = new SymbolIcon(Symbol.Download), Tag = "export", AccessKey = "X" });

            NavView_ItemInvoked(NavView, null);
        }

    }
}
