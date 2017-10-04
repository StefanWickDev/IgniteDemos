using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NorthwindDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductsPage : Page
    {
        public ProductList Products { get; set; } // products currently displayed on this page
        public List<Category> Categories { get; set; }
        public ProductsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadProductsAndCategories();
            if(e.Parameter is string)
            {
                await GetNewOrder();
            }
        }
        private async Task LoadProductsAndCategories()
        {
            Products =  DataHelper.GetProducts((App.Current as App).ConnectionString);
            if (Products is ProductList)
            {
                // Store list of all products available at App level
                (App.Current as App).Products = Products;
                InventoryList.ItemsSource = Products;
                Categories = DataHelper.GetCategories((App.Current as App).ConnectionString);
                Categories.Insert(0, new Category(0, "<Show all categories>"));
            }
            else
            {
                await new MessageDialog("Unable to connect to SQL Server! Check connection string in Settings.").ShowAsync();
            }
        }

        public async Task GetNewOrder()
        {
            // Create New Orders window on new thread
            var newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(OrderPage), null);
                Window.Current.Content = frame;
                Window.Current.Activate();

                // Save Id of Orders view
                newViewId = ApplicationView.GetForCurrentView().Id;
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

        #region Drag and drop handling code       
        private void InventoryList_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private void InventoryList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (InventoryList.SelectedItem != null)
            {
                var product = (Product)InventoryList.SelectedItem;
                e.Data.SetText(product.ProductID.ToString());
                e.Data.RequestedOperation = DataPackageOperation.Copy;
            }
        }

        #endregion

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FilterBox.SelectedItem is Category)
            {
                if((FilterBox.SelectedItem as Category).CategoryId == 0)
                {
                    Products = (App.Current as App).Products;
                }
                else
                {
                    Products = (App.Current as App).Products.GetProductsByCategoryId(
                        (FilterBox.SelectedItem as Category).CategoryId);
                }
                InventoryList.ItemsSource = Products;
            }
        }
    }
}
