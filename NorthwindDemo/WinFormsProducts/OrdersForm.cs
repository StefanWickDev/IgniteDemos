using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NorthwindDemo;


namespace WinFormsProducts
{
    public partial class OrdersForm : Form
    {
        private string connectionString =
          @"Data Source=Surfacebook\SQLDeveloper;Initial Catalog=NORTHWIND;" +
          "Integrated Security=SSPI";
        List<OrderHeader> Orders { get; set; }
        public OrdersForm()
        {
            InitializeComponent();
            this.dateTimePicker1.Value = DateTime.Today.AddMonths(-1);
            this.dateTimePicker2.Value = DateTime.Today;
        }
        private void OrdersForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1_ValueChanged(this, null);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Orders = DataHelper.GetOrderHeaders(connectionString, 
                this.dateTimePicker1.Value, this.dateTimePicker2.Value);
            this.dataGridView1.DataSource = Orders;
        }
    }
}
