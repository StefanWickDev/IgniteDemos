using System.Collections.ObjectModel;
using Windows.Data.Json;

namespace NorthwindDemo
{
    public class OrderDetailsList : ObservableCollection<OrderDetail>
    {
        public decimal GetNetTotal()
        {
            decimal total = 0;
            foreach(OrderDetail detail in Items)
            {
                total += detail.NetAmount;
            }
            return total;
        }

        public bool ProductIdExists(int id)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ProductID == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
