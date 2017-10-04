using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NorthwindDemo;

namespace WinFormsProducts
{
    public partial class Form1 : Form
    {
        //TODO: replace string with your own SQL server connection string, then reinstall the app
        private string connectionString =
            @"Data Source=tcp:sausing-desktop.redmond.corp.microsoft.com;Initial Catalog=Northwind;Integrated Security=True";


        public ProductList Products { get; set; } // products currently displayed on this page
        public Form1()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            Products = DataHelper.GetProducts(connectionString);
            dataGridView1.DataSource = Products;
        }
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                var o = this.dataGridView1.SelectedRows[0].DataBoundItem;
                if (o is Product)
                {
                    var productId = (o as Product).ProductID;
                    dataGridView1.DoDragDrop(productId.ToString(), DragDropEffects.Copy);
                }
            }
        }
        private void OrdersMenuItem_Click(object sender, EventArgs e)
        {
            var form = new OrdersForm();
            form.ShowDialog();
        }


    }
}
