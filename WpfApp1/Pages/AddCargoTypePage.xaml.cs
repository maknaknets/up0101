using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace WpfApp1.Pages
{
    public partial class AddCargoTypePage : Page
    {
        private CargoType _type;

        public AddCargoTypePage(CargoType selectedType)
        {
            InitializeComponent();
            if (selectedType == null)
            {
                _type = new CargoType();
                Title = "Добавление типа груза";
            }
            else
            {
                _type = selectedType;
                Title = "Редактирование типа груза";
                TextBoxName.Text = _type.Name;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBoxName.Text.Trim();
            int id = _type.ID;

            if (name.Length < 2)
            {
                Helper.Error("Укажите название типа груза (минимум 2 символа)!");
                return;
            }
            if (Core.DB.CargoType.Any(c => c.Name == name && c.ID != id))
            {
                Helper.Error("Такой тип груза уже существует!");
                return;
            }

            _type.Name = name;
            if (id == 0)
                Core.DB.CargoType.Add(_type);

            if (Helper.Save())
            {
                Helper.Info("Данные успешно сохранены!");
                NavigationService.GoBack();
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TextBoxName.Text = "";
        }
    }
}
