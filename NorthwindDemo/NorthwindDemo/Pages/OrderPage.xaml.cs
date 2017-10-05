using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Payments;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Text;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NorthwindDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderPage : Page
    {
        DataPackageView packageView;
        bool isCompactOverlay = false;
        TextBlock footerTextBlock;
        PrintHelper printHelper;
        CoreDispatcher dispatcher;

        public OrderDetailsList OrderDetails { get; set; }
        public List<Customer> Customers { get; set; }
        public string OrderTotal { get { return OrderDetails.GetNetTotal().ToString("######0.00"); } }

        private int orderId = 0;
        private Order currentOrder;
        public OrderPage()
        {
            this.InitializeComponent();

            // Footer textblock is created programmatically here
            footerTextBlock = new TextBlock();
            footerTextBlock.Margin = new Thickness(16);
            OrderDetailsListView.Footer = footerTextBlock;

            // Save current dispatcher - we'll need this later after printing
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is int)
            {
                orderId = (int)e.Parameter;
                SetUpPageForOrder(orderId);
            }
            else
            {
                Customers = DataHelper.GetCustomers((App.Current as App).ConnectionString);
                OrderDetails = new OrderDetailsList();
                OrderDetailsListView.ItemsSource = OrderDetails;
                footerTextBlock.Text = "Order total:  " + OrderTotal;

                // This window can be launched multiple times
                ApplicationView.GetForCurrentView().Title = "New Purchase Order";
                Toggle_Click(this, null); // switch to compact overlay size
            }
        }
        #region Set up page for existing order
        private void SetUpPageForOrder(int orderId)
        {
            var order = DataHelper.GetOrderByOrderId((App.Current as App).ConnectionString, orderId);
            if (order is Order)
            {
                currentOrder = order;
                SetUpCustomer(order);

                OrderDetails = new OrderDetailsList();
                OrderDetailsListView.ItemsSource = OrderDetails;
                foreach (OrderDetail detail in order.OrderDetails)
                {
                    OrderDetails.Add(detail);
                }
                footerTextBlock.Text = "Order total:  " + OrderTotal;

                ApplicationView.GetForCurrentView().Title = "View Order " + orderId.ToString();
                OrderPageHeading.Text = "View Order " + orderId.ToString();
                Instructions.Text = "Order date: " + order.OrderDate.ToLongDateString();

                // Disable buttons that are only use for new orders
                Pay.Visibility = Visibility.Collapsed;
                Save.Visibility = Visibility.Collapsed;
                Reprint.Visibility = Visibility.Visible;

                Toggle_Click(this, null); // switch to compact overlay size
            }
        }

        private void SetUpCustomer(Order order)
        {
            Customer customer = new Customer();
            customer.CustomerID = order.CustomerID;
            customer.CompanyName = order.CompanyName;
            Customers = new List<Customer>();
            Customers.Add(customer);
            CustomerComboBox.SelectedItem = customer;
        }
#endregion
        #region Drop handling code
        private void OrderText_DragOver(object sender, DragEventArgs e)
        {
            if (orderId == 0)
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
                packageView = e.DataView;
            }
        }

        private async void OrderText_Drop(object sender, DragEventArgs e)
        {
            // If valid ProductID is passed in, add a line item to collection
            if (orderId == 0 && e.DataView.Contains(StandardDataFormats.Text))
            {
                var inventoryId = await e.DataView.GetTextAsync();
                int productId = 0;
                int.TryParse(inventoryId, out productId);
                if (productId > 0)
                {
                    var products = (App.Current as App).Products;
                    var product = products.GetProductById(productId);
                    if (product != null && !OrderDetails.ProductIdExists(productId))
                    {
                        var lineItem = new OrderDetail();
                        lineItem.ProductID = productId;
                        lineItem.ProductName = product.ProductName;
                        lineItem.Quantity = 1;
                        lineItem.UnitPrice = product.UnitPrice;

                        OrderDetails.Add(lineItem);
                    }
                }
            }
        }
        #endregion

        #region Toggle CompactOverlay (picture in picture) mode
        private async void Toggle_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (isCompactOverlay)
            {
                await view.TryEnterViewModeAsync(ApplicationViewMode.Default);
            }
            else
            {
                // Setting the initial size isn't required but it's handy
                var compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                compactOptions.ViewSizePreference = ViewSizePreference.Custom;
                compactOptions.CustomSize = new Windows.Foundation.Size(800, 600);
                await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);
            }
            isCompactOverlay = !isCompactOverlay;

        }
        #endregion

        #region Microsoft Payments code
        private async void Pay_Click(object sender, RoutedEventArgs e)
        {
            // Package line items and total to submit to Microsoft Payments
            var paymentItems = new List<PaymentItem>();
            foreach (OrderDetail detail in OrderDetails)
            {
                paymentItems.Add(new PaymentItem(detail.ProductName,
                    new PaymentCurrencyAmount(detail.NetAmountString, "USD")));
            }

            var paymentDetails = new PaymentDetails();
            paymentDetails.Total = new PaymentItem("Total",
                new PaymentCurrencyAmount(OrderTotal, "USD"));
            paymentDetails.DisplayItems = paymentItems;
            paymentDetails.ShippingOptions = CreateShippingOptions();
            var paymentOptions = new PaymentOptions()
            {
                RequestShipping = true,
                ShippingType = PaymentShippingType.Delivery,
                RequestPayerEmail = PaymentOptionPresence.Optional,
                RequestPayerName = PaymentOptionPresence.Required,
                RequestPayerPhoneNumber = PaymentOptionPresence.None
            };
            var merchantInfo = new PaymentMerchantInfo(new Uri("http://www.contoso.com"));
            var paymentMethods = new List<PaymentMethodData>();
            paymentMethods.Add(new PaymentMethodData(new List<string>()
                { "basic-card" }));

            // Send payment request to Microsoft for cardholder authentication
            // Microsoft prompts user to enter CVV
            var paymentRequest = new PaymentRequest(paymentDetails,
                paymentMethods, merchantInfo, paymentOptions);
            var mediator = new PaymentMediator();
            var result = await mediator.SubmitPaymentRequestAsync(paymentRequest);

            await UpdateUI(result);

        }
        private async Task UpdateUI(PaymentRequestSubmitResult result)
        {
            if (result.Status == PaymentRequestStatus.Succeeded)
            {
                // If payment request is successful, Microsoft returns a json token
                //   containing Microsoft Wallet data to submit to merchant's payment processor
                Debug.WriteLine("Method ID: " + result.Response.PaymentToken.PaymentMethodId);
                Debug.WriteLine("Payment JSON Token: " + (result.Response.PaymentToken.JsonDetails ?? ""));

                // Simulate payment processing by merchant's payment processor
                // Note: actual time will vary quite a bit by payment processor
                await Task.Delay(TimeSpan.FromSeconds(2));

                // Report that we successfully charged their card using the payment token we were given
                await result.Response.CompleteAsync(PaymentRequestCompletionStatus.Succeeded);

                await new MessageDialog("Payment succeeded!").ShowAsync();
                Save_Click(this, null);
            }
            else
            {
                await new MessageDialog("Payment failed, status: " + result.Status).ShowAsync();
            }
        }

        private IReadOnlyList<PaymentShippingOption> CreateShippingOptions()
        {
            List<PaymentShippingOption> paymentShippingOptions = new List<PaymentShippingOption>();

            PaymentShippingOption shippingOption = new PaymentShippingOption("Standard", new PaymentCurrencyAmount("2.00", "USD"), false, "STANDARD");
            paymentShippingOptions.Add(shippingOption);

            shippingOption = new PaymentShippingOption("Two Day", new PaymentCurrencyAmount("5.99", "USD"), false, "TWODAY");
            paymentShippingOptions.Add(shippingOption);

            return paymentShippingOptions;
        }
        #endregion

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            string customerId = "ALFKI"; // default customer if none specified
            string customerName = "Alfreds Futterkiste";
            if (CustomerComboBox.SelectedItem is Customer)
            {
                customerId = (CustomerComboBox.SelectedItem as Customer).CustomerID;
                customerName = (CustomerComboBox.SelectedItem as Customer).CompanyName;
            }
            var order = new Order(customerId, 1);
            order.OrderDetails = OrderDetails;
            var savedOrderId = DataHelper.SaveOrderAndDetails(order, (App.Current as App).ConnectionString);
            if (!string.IsNullOrEmpty(savedOrderId))
            {
                int.TryParse(savedOrderId, out orderId);
                Pay.IsEnabled = false;
                Save.IsEnabled = false;
                ShowToastNotification("Order saved: " + savedOrderId);
                BuildReceipt(savedOrderId, customerName, order);
                await PrintReceipt();
            }
            else
            {
                await new MessageDialog("ERROR! There was a problem saving this order.").ShowAsync();
            }
        }

        private void ShowToastNotification(string message)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            if (notifier == null)
                return;
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode("Northwind Traders"));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(message));
            var toastNode = toastXml.SelectSingleNode("/toast");
            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");
            ToastNotification toast = new ToastNotification(toastXml);
            notifier.Show(toast);
        }

        private void QuantityTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int quantity = 0;
            int.TryParse((sender as TextBox).Text, out quantity);
            var dc = (sender as TextBox).DataContext;
            var orderDetail = (dc as OrderDetail);
            orderDetail.Quantity = quantity;
            orderDetail.UpdateTotal();

            // Display extended price for line item
            var parent = VisualTreeHelper.GetParent(sender as DependencyObject);
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            var totalTextbox = (TextBox)VisualTreeHelper.GetChild(parent, childCount - 1);
            totalTextbox.Text = orderDetail.NetAmountString;

            // Display updated order total
            footerTextBlock.Text = "Order total:  " + OrderTotal;
        }

        #region Recipt printing code
        private async Task PrintReceipt()
        {
            if (((App.Current as App).UsePrinter) )
            {
                try
                {
                    // Use UWP Community Toolkit PrintHelper to print directly from page
                    printHelper = new PrintHelper(DirectPrintContainer);
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
        }

        private void BuildReceipt(string orderId, string customerName, Order order)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n\r\n");
            sb.Append("       " + "Northwind Traders Company\r\n");
            sb.Append("       " + "One Microsoft Way\r\n");
            sb.Append("       " + "Redmond WA 98052-8300\r\n\r\n");
            sb.Append("       " + "  (425) 882-8080\r\n");
            sb.Append("\r\n\r\n");
            if (currentOrder != null)
            {
                sb.Append("       " + "*** DUPLICATE RECEIPT ***\r\n\r\n");
            }
            sb.Append(" Order ID   " + orderId + "\r\n");
            sb.Append(" Order date " + order.OrderDate.ToShortDateString() + "\r\n");
            sb.Append(" Ordered by " + customerName.PadRight(29).Substring(0, 29) + "\r\n\r\n");
            sb.Append(" Qty         Description        Amount\r\n\r\n");
            foreach (OrderDetail item in OrderDetails)
            {
                sb.Append(item.Quantity.ToString("###.00").PadLeft(6) + " " +
                    item.ProductName.PadRight(20).Substring(0, 20) + " " +
                    item.NetAmount.ToString("######.00").PadLeft(10) + "\r\n");
            }
            sb.Append("\r\n\r\n");
            sb.Append("       " + "TOTAL:               " + OrderTotal.PadLeft(10) + "\r\n");
            sb.Append("\r\n\r\n");

            PrintText.Text = sb.ToString();
        }
#endregion
        private async void Reprint_Click(object sender, RoutedEventArgs e)
        {
            Reprint.IsEnabled = false;
            BuildReceipt(orderId.ToString(), (CustomerComboBox.SelectedItem as Customer).CompanyName,
                currentOrder);
            await PrintReceipt();
        }

        #region EventHandlers for UWP Community Toolkit PrintHelper
        private async Task ReleasePrintHelper()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    PrintText.Text = "";
                    printHelper.Dispose();
                });
        }
        private async void PrintHelper_OnPrintSucceeded()
        {
            await ReleasePrintHelper();
        }
        private async void PrintHelper_OnPrintFailed()
        {
            await ReleasePrintHelper();
            var dialog = new MessageDialog("Printing failed.");
            await dialog.ShowAsync();
        }
        private async void PrintHelper_OnPrintCanceled()
        {
            await ReleasePrintHelper();
        }
        #endregion

    }
}
