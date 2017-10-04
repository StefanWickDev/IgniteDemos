using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using OrgTracker;

namespace Contoso_Mobile
{
	public partial class MainPage : TabbedPage
	{
        public ObservableCollection<Employee> Reports { get; set; }
        public ObservableCollection<Employee> Managers { get; set; }
        public Employee Employee { get; set; }
        public ObservableCollection<Employee> Org { get; set; }
        private ObservableCollection<Employee> All = null;
        public MainPage(string employeeName)
        {
            InitializeComponent();

            Employee.EmployeeMap.Clear();
            All = Employee.GetAllEmployees("", "");
            SetBindingContext(employeeName);
        }

        private void SetBindingContext(string employeeName)
        { 
            Org = new ObservableCollection<Employee>();
            Reports = new ObservableCollection<Employee>();
            Managers = new ObservableCollection<Employee>();

            foreach (Employee employee in All)
            {
                if (employee.Manager == employeeName)
                {
                    Reports.Add(employee);
                    Org.Add(employee);
                }
                else if (employee.Name == employeeName)
                {
                    Employee = employee;
                    if (employee.Manager != "")
                    {
                        Employee manager = Employee.EmployeeMap[employee.Manager];
                        Managers.Add(manager);
                        while (manager.Manager != "")
                        {
                            manager = Employee.EmployeeMap[manager.Manager];
                            Managers.Add(manager);
                        }
                    }
                }
                else if (employee.Manager != "")
                {
                    Employee manager = Employee.EmployeeMap[employee.Manager];
                    while (manager.Manager != "")
                    {
                        if (manager.Manager == employeeName)
                        {
                            Org.Add(employee);
                            break;
                        }
                        manager = Employee.EmployeeMap[manager.Manager];
                    }
                }
            }

            this.BindingContext = null;
            this.BindingContext = this;
            this.CurrentPage = this.Children[0];
            Title = Employee?.Name ?? string.Empty + " - Contoso Headtrax";
        }

        //protected override void OnCurrentPageChanged()
        //{
        //    base.OnCurrentPageChanged();
        //    Title = Employee?.Name ?? string.Empty + " - " + CurrentPage?.Title ?? string.Empty;
        //}

        private void DirectsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SetBindingContext((e.SelectedItem as Employee).Name);
        }

        private void OrgListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SetBindingContext((e.SelectedItem as Employee).Name);
        }
    }
}
