using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo
{
    public class CategorySales
    {
        public string CategoryName { get; set; }
        public decimal Sales { get; set; }
        public int CategoryId { get; set; }
    }
}
