using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.Entity;

namespace WpfApp1.Pages
{
    public partial class AddShipmentPage : Page
    {
        private Shipment _shipment;

        public AddShipmentPage(Shipment selectedShipment)
        {
            InitializeComponent();

            ComboBoxType.ItemsSource = Core.DB.CargoType.ToList();
            ComboBoxUser.ItemsSource = Core.DB.Users.ToList();
            DatePickerShip.DisplayDateEnd = DateTime.Today.AddYears(1);

            if (selectedShipment == null)
            {
                _shipment = new Shipment();
                Title = "Добавление перевозки";
                DatePickerShip.SelectedDate = DateTime.Today;
            }
            else
            {
                _shipment = selectedShipment;
                Title = "Редактирование перевозки";
                TextBoxName.Text = _shipment.Name;
                ComboBoxType.SelectedValue = _shipment.CargoTypeID;
                ComboBoxUser.SelectedValue = _shipment.UserID;
                DatePickerShip.SelectedDate = _shipment.ShipDate;
                TextBoxWeight.Text = _shipment.Weight.ToString();
                TextBoxPrice.Text = _shipment.Price.ToString();
            }
        }

        private void Digits_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Helper.IsDigits(e.Text);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            string name = TextBoxName.Text.Trim();
            int weight;
            int price;

            if (name.Length < 2)
                errors.AppendLine("Укажите название груза (минимум 2 символа)!");
            if (ComboBoxType.SelectedValue == null)
                errors.AppendLine("Выберите тип груза!");
            if (ComboBoxUser.SelectedValue == null)
                errors.AppendLine("Выберите клиента!");
            if (DatePickerShip.SelectedDate == null)
                errors.AppendLine("Укажите дату перевозки!");
            if (!int.TryParse(TextBoxWeight.Text, out weight) || weight < 1 || weight > 40000)
                errors.AppendLine("Вес должен быть числом от 1 до 40000 кг!");
            if (!int.TryParse(TextBoxPrice.Text, out price) || price < 100 || price > 10000000)
                errors.AppendLine("Стоимость должна быть числом от 100 до 10 000 000 руб.!");

            if (errors.Length > 0)
            {
                Helper.Error(errors.ToString());
                return;
            }

            _shipment.Name = name;
            _shipment.CargoTypeID = (int)ComboBoxType.SelectedValue;
            _shipment.UserID = (int)ComboBoxUser.SelectedValue;
            _shipment.ShipDate = DatePickerShip.SelectedDate.Value;
            _shipment.Weight = weight;
            _shipment.Price = price;

            if (_shipment.ID == 0)
                Core.DB.Shipment.Add(_shipment);

            if (Helper.Save())
            {
                Helper.Info("Данные успешно сохранены!");
                NavigationService.GoBack();
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TextBoxName.Text = "";
            ComboBoxType.SelectedIndex = -1;
            ComboBoxUser.SelectedIndex = -1;
            DatePickerShip.SelectedDate = null;
            TextBoxWeight.Text = "";
            TextBoxPrice.Text = "";
        }
    }
}
