using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace WpfApp1.Pages
{
    public partial class ShipmentTabPage : Page
    {
        public ShipmentTabPage()
        {
            InitializeComponent();

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
            DataGridShipments.ItemsSource = Helper.GetShipments(0, TextBoxSearch.Text, ComboBoxSort.SelectedIndex, typeId);
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

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddShipmentPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Shipment selected = DataGridShipments.SelectedItem as Shipment;
            if (selected == null)
            {
                Helper.Error("Выберите перевозку для редактирования!");
                return;
            }
            NavigationService.Navigate(new AddShipmentPage(selected));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Shipment> selected = DataGridShipments.SelectedItems.Cast<Shipment>().ToList();
            if (selected.Count == 0)
            {
                Helper.Error("Выберите перевозки для удаления!");
                return;
            }
            if (!Helper.Confirm("Вы точно хотите удалить выбранные записи (" + selected.Count + ")?"))
                return;
            foreach(var item in selected)
    Core.DB.Shipment.Remove(item);
            if (Helper.Save())
            {
                Helper.Info("Данные успешно удалены!");
                UpdateGrid();
            }
        }
    }
}
