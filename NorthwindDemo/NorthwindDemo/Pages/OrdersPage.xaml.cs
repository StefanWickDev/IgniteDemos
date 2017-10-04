using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NorthwindDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            this.InitializeComponent();
            var begin = DateTime.Today.AddMonths(-1);
            BeginDate.Date = DateTimeOffset.Parse(begin.ToShortDateString());
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
             RefreshData();
        }
        private void RefreshData()
        {
            List<OrderHeader> orders = DataHelper.GetOrderHeaders(
                (App.Current as App).ConnectionString,
                BeginDate.Date.DateTime, EndDate.Date.DateTime);
            DataGrid.ItemsSource = orders;
        }

        private void DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            RefreshData();
        }

        private async void DataGrid_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var o = DataGrid.SelectedItem;
            if(o is OrderHeader)
            {
                await ShowOrder((o as OrderHeader).OrderID);
            }
        }
        public async Task ShowOrder(int orderId)
        {
            // Create New Orders window on new thread
            var newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(OrderPage), orderId);
                Window.Current.Content = frame;
                Window.Current.Activate();

                // Save Id of Orders view
                newViewId = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Id;
            });

            // Show the Orders window as standalone window
            if (newViewId != 0)
            {
                bool isShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
                if (isShown)
                {
                    await ApplicationViewSwitcher.SwitchAsync(newViewId);
                }
            }
        }

    }
}
