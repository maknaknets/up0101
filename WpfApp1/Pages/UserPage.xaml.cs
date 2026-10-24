using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            Title = "Мои перевозки - " + Core.CurrentUser.FIO;

            List<CargoType> types = Core.DB.CargoType.ToList();
            types.Insert(0, new CargoType { ID = 0, Name = "Все типы грузов" });
            ComboBoxType.ItemsSource = types;
            ComboBoxType.DisplayMemberPath = "Name";
            ComboBoxType.SelectedValuePath = "ID";
            ComboBoxType.SelectedIndex = 0;

            IsVisibleChanged += Page_IsVisibleChanged;
            UpdateGrid();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                UpdateGrid();
        }

        private void UpdateGrid()
        {
            if (!IsInitialized)
                return;
            int typeId = ComboBoxType.SelectedValue is int ? (int)ComboBoxType.SelectedValue : 0;
            DataGridShipments.ItemsSource = Helper.GetShipments(Core.CurrentUser.ID, TextBoxSearch.Text, ComboBoxSort.SelectedIndex, typeId);
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized)
                return;
            if (sender == TextBoxSearch)
                Helper.UpdateHint(sender);
            UpdateGrid();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSearch.Text = "";
            ComboBoxSort.SelectedIndex = 0;
            ComboBoxType.SelectedIndex = 0;
        }
    }
}
