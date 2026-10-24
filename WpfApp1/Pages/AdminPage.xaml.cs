using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            TextBlockWelcome.Text = "Администратор: " + Core.CurrentUser.FIO;
        }

        private void ButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UsersTabPage());
        }

        private void ButtonTypes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CargoTypeTabPage());
        }

        private void ButtonShipments_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ShipmentTabPage());
        }

        private void ButtonDiagram_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DiagramPage());
        }
    }
}
