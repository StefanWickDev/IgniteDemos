using System;
using System.ComponentModel;

namespace NorthwindDemo
{
    public class OrderDetail : INotifyPropertyChanged
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity == value) return;
                quantity = value;
                NotifyPropertyChanged("Quantity");
            }
        }
        public decimal UnitPrice { get; set; }
        public string UnitPriceString { get { return UnitPrice.ToString("######.00"); } }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get { return netAmount; } }
        private decimal netAmount;
        public string NetAmountString {  get { return NetAmount.ToString("######.00"); } }
        public float Discount { get; set; }

        public OrderDetail()
        {
            Quantity = 1;
            TaxAmount = 0;
            Discount = 0;
        }
        public void UpdateTotal()
        {
            var preTax = Math.Round(Quantity * UnitPrice, 2);
            netAmount = preTax + TaxAmount;
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
