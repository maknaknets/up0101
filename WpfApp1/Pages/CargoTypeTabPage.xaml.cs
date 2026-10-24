using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace WpfApp1.Pages
{
    public partial class CargoTypeTabPage : Page
    {
        public CargoTypeTabPage()
        {
            InitializeComponent();
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
            DataGridTypes.ItemsSource = Core.DB.CargoType.ToList();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCargoTypePage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            CargoType selected = DataGridTypes.SelectedItem as CargoType;
            if (selected == null)
            {
                Helper.Error("Выберите тип груза для редактирования!");
                return;
            }
            NavigationService.Navigate(new AddCargoTypePage(selected));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            List<CargoType> selected = DataGridTypes.SelectedItems.Cast<CargoType>().ToList();
            if (selected.Count == 0)
            {
                Helper.Error("Выберите типы грузов для удаления!");
                return;
            }
            if (!Helper.Confirm("Вы точно хотите удалить выбранные записи (" + selected.Count + ")? Все перевозки этих типов также будут удалены."))
                return;
            foreach (var item in selected)
                Core.DB.CargoType.Remove(item);
            if (Helper.Save())
            {
                Helper.Info("Данные успешно удалены!");
                UpdateGrid();
            }
        }
    }
}
