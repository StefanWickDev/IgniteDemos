using System.ComponentModel;

namespace NorthwindDemo
{
    public class OrderHeader : INotifyPropertyChanged
    {
        public int OrderID { get; set; }
        public string OrderDate { get; set; }
        public int EmployeeID { get; set; }
        public string CustomerID { get; set; }
        public string OrderTotal { get; set; }

        public OrderHeader()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
