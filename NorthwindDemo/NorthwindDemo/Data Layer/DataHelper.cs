using System;
using System.Diagnostics;
using System.Data.SqlClient;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;

namespace NorthwindDemo
{
    public class DataHelper
    {
        public static ProductList GetProducts(string connectionString)
        {
            const string GetProductsQuery = "select ProductID, ProductName, QuantityPerUnit," +
               " UnitPrice, UnitsInStock, Products.CategoryID " +
               " from Products inner join Categories on Products.CategoryID = Categories.CategoryID " +
               " where Discontinued = 0";

            var products = new ProductList();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetProductsQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var product = new Product();
                                    product.ProductID = reader.GetInt32(0);
                                    product.ProductName = reader.GetString(1);
                                    product.QuantityPerUnit = reader.GetString(2);
                                    product.UnitPrice = reader.GetDecimal(3);
                                    product.UnitsInStock = reader.GetInt16(4);
                                    product.CategoryId = reader.GetInt32(5);
                                    products.Add(product);
                                }
                            }
                        }
                    }
                }
                return products;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        public static List<Category> GetCategories(string connectionString)
        {
            const string query = "select CategoryId, CategoryName from Categories";

            var categories = new List<Category>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = query;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var category = new Category();
                                    category.CategoryId = reader.GetInt32(0);
                                    category.CategoryName = reader.GetString(1);
                                    categories.Add(category);
                                }
                            }
                        }
                    }
                }
                return categories;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        public static Order GetOrderByOrderId(string connectionString, int Id)
        {
            const string query = "select top(1) Orders.CustomerID, EmployeeID, OrderDate, CompanyName "+
                " from Orders inner join Customers on Orders.CustomerID = Customers.CustomerID " +
                " where OrderID = @OrderId";

            const string detailsQuery = "select [Order Details].ProductID, ProductName, " +
                " [Order Details].UnitPrice, Quantity, Discount " +
                " from[Order Details] " +
                " inner join Products on[Order Details].ProductID = Products.ProductID " +
                " where OrderID = @OrderId";

            var order = new Order();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = query;
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@OrderId", Id);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    order.CustomerID = reader.GetString(0);
                                    order.EmployeeID = reader.GetInt32(1);
                                    order.OrderDate = reader.GetDateTime(2);
                                    order.CompanyName = reader.GetString(3);
                                    reader.Close();

                                    using (SqlCommand cmd2 = conn.CreateCommand())
                                    {
                                        cmd2.CommandText = detailsQuery;
                                        cmd2.Parameters.Clear();
                                        cmd2.Parameters.AddWithValue("@OrderId", Id);
                                        using (SqlDataReader reader2 = cmd2.ExecuteReader())
                                        {
                                            while (reader2.Read())
                                            {
                                                var detail = new OrderDetail();
                                                detail.OrderID = Id;
                                                detail.ProductID = reader2.GetInt32(0);
                                                detail.ProductName = reader2.GetString(1);
                                                detail.UnitPrice = reader2.GetDecimal(2);
                                                detail.Quantity = reader2.GetInt16(3);
                                                detail.Discount = reader2.GetFloat(4);
                                                order.OrderDetails.Add(detail);
                                            }
                                        }
                                    }
                                }
                            } 
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return null;
            }
            return order;
        }
        public static List<OrderData> GetOrderDataByDate(string connectionString, DateTime beginDate, DateTime endDate)
        {
            const string GetOrdersQuery = " select Orders.OrderID, Orders.OrderDate, Orders.CustomerID, Orders.EmployeeID, CompanyName, " +
                " sum(Details.Quantity * Details.UnitPrice) as OrderTotal " +
                " from Orders " +
                " inner join Customers on Orders.CustomerID = Customers.CustomerID " +
                " left join [Order Details] Details on Details.OrderID = Orders.OrderID " +
                " where Orders.OrderDate >= @MinDate and Orders.OrderDate <= @MaxDate " +
                " group by Orders.OrderID, Orders.OrderDate, Orders.CustomerID, Orders.EmployeeID, CompanyName " +
                " order by Orders.OrderDate";

            var orders = new List<OrderData>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetOrdersQuery;
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@MinDate", beginDate);
                            cmd.Parameters.AddWithValue("@MaxDate", endDate);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var order = new OrderData();
                                    order.OrderID = reader.GetInt32(0);
                                    order.OrderDate = reader.GetDateTime(1);
                                    order.CustomerID = reader.GetString(2);
                                    order.EmployeeID = reader.GetInt32(3);
                                    order.CompanyName = reader.GetString(4);
                                    order.OrderTotal = reader.GetDecimal(5);
                                    orders.Add(order);
                                }
                            }
                        }
                    }
                }
                return orders;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        public static List<OrderHeader> GetOrderHeaders(string connectionString,  DateTime beginDate, DateTime endDate)
        {
            const string GetOrdersQuery = "select Orders.OrderID, " +
                " Orders.OrderDate, Orders.CustomerID, Orders.EmployeeID, " +
                " sum(Details.Quantity * Details.UnitPrice) as OrderTotal " +
                " from Orders " +
                " left join [Order Details] Details on Details.OrderID = Orders.OrderID " +
                " WHERE (((Orders.OrderDate) Between @MinDate And @MaxDate))" +
                " group by Orders.OrderID, Orders.CustomerID, Orders.OrderDate, Orders.EmployeeID " +
                " order by Orders.OrderID desc";
            var orders = new List<OrderHeader>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetOrdersQuery;
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@MinDate", beginDate);
                            cmd.Parameters.AddWithValue("@MaxDate", endDate);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var order = new OrderHeader();
                                    order.OrderID = reader.GetInt32(0);
                                    order.OrderDate = reader.GetDateTime(1).ToShortDateString();
                                    order.CustomerID = reader.GetString(2);
                                    order.EmployeeID = reader.GetInt32(3);
                                    order.OrderTotal = reader.GetDecimal(4).ToString("######.00").PadLeft(10);
                                    orders.Add(order);
                                }
                            }
                        }
                    }
                }
                return orders;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        public static List<Customer> GetCustomers(string connectionString)
        {
            const string GetCustomersQuery = "select CustomerID, CompanyName from Customers";

            var customers = new List<Customer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetCustomersQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var customer = new Customer();
                                    customer.CustomerID = reader.GetString(0);
                                    customer.CompanyName = reader.GetString(1);
                                    customers.Add(customer);
                                }
                            }
                        }
                    }
                }
                return customers;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        public static string SaveOrderAndDetails(Order order, string connectionString)
        {
            const string InsertOrderQuery = "insert into Orders (CustomerID, EmployeeID, OrderDate) " +
                "values (@CustomerID, @EmployeeID, @OrderDate) select SCOPE_IDENTITY()";

            const string InsertOrderDetailQuery = "insert into [Order Details] " +
                " (OrderID, ProductID, UnitPrice, Quantity, Discount) " +
                " values (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount) ";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        Debug.WriteLine("Creating SQL command...");
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = InsertOrderQuery;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("CustomerID", order.CustomerID));
                            cmd.Parameters.Add(new SqlParameter("EmployeeID", order.EmployeeID));
                            cmd.Parameters.Add(new SqlParameter("OrderDate", order.OrderDate));
                            var orderID = cmd.ExecuteScalar();
                            Debug.WriteLine("Inserted order {0}", orderID.ToString());

                            foreach (OrderDetail detail in order.OrderDetails)
                            {
                                if (detail.Quantity != 0)
                                {
                                    cmd.CommandText = InsertOrderDetailQuery;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add(new SqlParameter("OrderID", orderID));
                                    cmd.Parameters.Add(new SqlParameter("ProductID", detail.ProductID));
                                    cmd.Parameters.Add(new SqlParameter("UnitPrice", detail.UnitPrice));
                                    cmd.Parameters.Add(new SqlParameter("Quantity", detail.Quantity));
                                    cmd.Parameters.Add(new SqlParameter("Discount", detail.Discount));
                                    cmd.ExecuteNonQuery();
                                    Debug.WriteLine("Inserted line item for product ID {0}", detail.ProductID.ToString());
                                }
                            }
                            return orderID.ToString();
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception " + eSql.Message);
            }
            return "";
        }
        public static List<CategorySales> GetSalesByCategory(string connectionString, DateTime beginDate, DateTime endDate)
        {
            const string query = "SELECT Categories.CategoryName,  Categories.CategoryID, " +
                " Sum(CONVERT(decimal, ([Order Details].UnitPrice * Quantity * (1 - Discount) / 100)) * 100) AS ProductSales " +
                " FROM (Categories INNER JOIN Products ON Categories.CategoryID = Products.CategoryID) " +
                " JOIN(Orders INNER JOIN [Order Details] ON Orders.OrderID = [Order Details].OrderID) " +
                " ON Products.ProductID = [Order Details].ProductID " +
                " WHERE (((Orders.OrderDate) Between @MinDate And @MaxDate))" +
                " GROUP BY Categories.CategoryName, Categories.CategoryID ";

            var list = new List<CategorySales>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = query;
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@MinDate", beginDate);
                            cmd.Parameters.AddWithValue("@MaxDate", endDate);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var sales = new CategorySales();
                                    sales.CategoryName = reader.GetString(0);
                                    sales.CategoryId = reader.GetInt32(1);
                                    sales.Sales = reader.GetDecimal(2);
                                    list.Add(sales);
                                }
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
    }
}

