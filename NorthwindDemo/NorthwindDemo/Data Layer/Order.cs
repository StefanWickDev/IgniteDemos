using System;
using System.ComponentModel;

namespace NorthwindDemo
{
    public class Order : INotifyPropertyChanged
    {
        public string CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderID { get; set; }
        public decimal OrderTotal { get; set; }

        public string CompanyName { get; set; }
        
        public OrderDetailsList OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new OrderDetailsList();
        }

        public Order(string customerId, int employeeId)
        {
            OrderDetails = new OrderDetailsList();

            // Set up default customer and employee IDs for demo
            CustomerID = customerId;
            EmployeeID = employeeId;
            OrderDate = DateTime.Today;
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
