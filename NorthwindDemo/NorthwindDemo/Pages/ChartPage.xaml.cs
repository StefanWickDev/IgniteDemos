using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NorthwindDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChartPage : Page
    {
        public bool IsBusy { get; set; }
        public ChartPage()
        {
            this.InitializeComponent();
            var begin = DateTime.Today.AddMonths(-1);
            BeginDate.Date = DateTimeOffset.Parse(begin.ToShortDateString());
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GetData();
        }

        private void DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            GetData();
        }

        private void GetData()
        {
            IsBusy = true;
            List<CategorySales> salesList = DataHelper.GetSalesByCategory(
                (App.Current as App).ConnectionString,
                BeginDate.Date.DateTime, EndDate.Date.DateTime);
            myChart.Series[0].ItemsSource = salesList;
            IsBusy = false;
        }
    }
}
