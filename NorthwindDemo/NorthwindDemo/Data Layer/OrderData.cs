using System;

namespace NorthwindDemo
{
    public class OrderData
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public int EmployeeID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public decimal OrderTotal { get; set; }

    }
}
