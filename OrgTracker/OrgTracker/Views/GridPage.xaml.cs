using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OrgTracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GridPage : Page
    {
        private ObservableCollection<Employee> items = null;
        public ObservableCollection<Employee> GridItems
        {
            get { return items; }
        }
        public GridPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            items = Employee.GetAllEmployees(App.Organization, "../Assets/");
            this.DataContext = items;
        }

        private void grid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            if (employeeView.Visibility != Visibility.Visible)
            {
                employeeView.Visibility = Visibility.Visible;
            }
            employeeView.DataContext = grid.SelectedItem;
        }

        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            employeeView.Visibility = Visibility.Collapsed;
        }

        private async void SymbolIcon_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Employee employee = employeeView.DataContext as Employee;
            ApplicationView newView = null;
            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(EmployeePage), employee);
                Window.Current.Content = frame;
                Window.Current.Activate();
                newView = ApplicationView.GetForCurrentView();
                newView.SetPreferredMinSize(new Size(200, 200));
                newView.Title = employee.Name;
                newView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                newView.TitleBar.BackgroundColor = Windows.UI.Colors.Transparent;
            });

            var compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Size(300, 200);
            compactOptions.ViewSizePreference = ViewSizePreference.Custom;
            await ApplicationViewSwitcher.TryShowAsViewModeAsync(newView.Id, ApplicationViewMode.CompactOverlay, compactOptions);
            employeeView.Visibility = Visibility.Collapsed;
        }
    }
}
