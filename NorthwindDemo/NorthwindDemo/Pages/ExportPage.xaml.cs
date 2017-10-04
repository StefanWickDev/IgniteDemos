using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NorthwindDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExportPage : Page
    {
        public ExportPage()
        {
            this.InitializeComponent();
            var begin = DateTime.Today.AddMonths(-1);
            BeginDate.Date = DateTimeOffset.Parse(begin.ToShortDateString());
        }

        private async void ExportButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            List<OrderData> orders = DataHelper.GetOrderDataByDate(
                (App.Current as App).ConnectionString,
                BeginDate.Date.DateTime, EndDate.Date.DateTime);

            if (orders != null)
            {
                var picker = new FileSavePicker();
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });
                picker.SuggestedFileName = "NorthwindOrders";
                var file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    using (Stream stream = await file.OpenStreamForWriteAsync())
                    {
                        using (StreamWriter sw = new StreamWriter(stream))
                        {
                            using (CsvHelper.CsvWriter writer = new CsvHelper.CsvWriter(sw, false))
                            {
                                writer.WriteRecords(orders);
                            }
                        }
                    }
                }
                // If Excel is installed it will launch as default handler for CSV files
                await Windows.System.Launcher.LaunchFileAsync(file);
            }
            else
            {
                await new MessageDialog("No data found for dates specified.").ShowAsync();
            }

        }
    }
}
