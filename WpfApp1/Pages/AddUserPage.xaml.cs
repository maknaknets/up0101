using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.Pages
{
    public partial class AddUserPage : Page
    {
        private Users _user;

        public AddUserPage(Users selectedUser)
        {
            InitializeComponent();
            if (selectedUser == null)
            {
                _user = new Users();
                Title = "Добавление пользователя";
            }
            else
            {
                _user = selectedUser;
                Title = "Редактирование пользователя";
                TextBoxLogin.Text = _user.Login;
                TextBoxFio.Text = _user.FIO;
                TextBoxPhoto.Text = _user.Photo;
                ComboBoxRole.SelectedIndex = _user.Role == "Admin" ? 1 : 0;
                TextBlockPassword.Text = "Новый пароль";
                PasswordBoxUser.ToolTip = "Оставьте пустым, чтобы не менять пароль";
            }
        }

        private void Fio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Helper.IsFioText(e.Text);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text.Trim();
            string fio = TextBoxFio.Text.Trim();
            int id = _user.ID;
            bool isNew = id == 0;
            StringBuilder errors = new StringBuilder();

            if (!Helper.IsLogin(login))
                errors.AppendLine("Логин: 3-30 символов, только английские буквы и цифры!");
            else if (Core.DB.Users.Any(u => u.Login == login && u.ID != id))
                errors.AppendLine("Пользователь с таким логином уже существует!");

            if (!Helper.IsFio(fio))
                errors.AppendLine("ФИО должно содержать только буквы, пробелы и дефис!");

            if (isNew || PasswordBoxUser.Password.Length > 0)
            {
                string error = Helper.CheckPassword(PasswordBoxUser.Password);
                if (error != null)
                    errors.AppendLine(error);
            }

            if (errors.Length > 0)
            {
                Helper.Error(errors.ToString());
                return;
            }

            _user.Login = login;
            _user.FIO = fio;
            _user.Role = ((ComboBoxItem)ComboBoxRole.SelectedItem).Content.ToString();
            _user.Photo = string.IsNullOrWhiteSpace(TextBoxPhoto.Text) ? null : TextBoxPhoto.Text.Trim();
            if (PasswordBoxUser.Password.Length > 0)
                _user.Password = Helper.GetHash(PasswordBoxUser.Password);

            if (isNew)
                Core.DB.Users.Add(_user);

            if (Helper.Save())
            {
                Helper.Info("Данные успешно сохранены!");
                NavigationService.GoBack();
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TextBoxLogin.Text = "";
            PasswordBoxUser.Clear();
            ComboBoxRole.SelectedIndex = 0;
            TextBoxFio.Text = "";
            TextBoxPhoto.Text = "";
        }
    }
}
