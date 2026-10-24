using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private void Field_Changed(object sender, RoutedEventArgs e)
        {
            Helper.UpdateHint(sender);
        }

        private void Fio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Helper.IsFioText(e.Text);
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text.Trim();
            string fio = TextBoxFio.Text.Trim();

            if (login == "" || fio == "" || PasswordBox1.Password == "" || PasswordBox2.Password == "")
            {
                Helper.Error("Заполните все поля!");
                return;
            }
            if (!Helper.IsLogin(login))
            {
                Helper.Error("Логин: 3-30 символов, только английские буквы и цифры!");
                return;
            }
            if (!Helper.IsFio(fio))
            {
                Helper.Error("ФИО должно содержать только буквы, пробелы и дефис!");
                return;
            }
            if (Core.DB.Users.Any(u => u.Login == login))
            {
                Helper.Error("Пользователь с таким логином уже существует!");
                return;
            }
            string error = Helper.CheckPassword(PasswordBox1.Password);
            if (error != null)
            {
                Helper.Error(error);
                return;
            }
            if (PasswordBox1.Password != PasswordBox2.Password)
            {
                Helper.Error("Пароли не совпадают!");
                return;
            }

            Users user = new Users();
            user.Login = login;
            user.Password = Helper.GetHash(PasswordBox1.Password);
            user.Role = ((ComboBoxItem)ComboBoxRole.SelectedItem).Content.ToString();
            user.FIO = fio;
            Core.DB.Users.Add(user);

            if (Helper.Save())
            {
                Helper.Info("Пользователь успешно зарегистрирован!");
                NavigationService.GoBack();
            }
        }
    }
}
