using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void Field_Changed(object sender, RoutedEventArgs e)
        {
            Helper.UpdateHint(sender);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLogin.Text) || PasswordBoxOld.Password == "" ||
                PasswordBoxNew.Password == "" || PasswordBoxConfirm.Password == "")
            {
                Helper.Error("Все поля обязательны к заполнению!");
                return;
            }

            Users user = Helper.Authenticate(TextBoxLogin.Text.Trim(), PasswordBoxOld.Password);
            if (user == null)
            {
                Helper.Error("Логин или текущий пароль неверный!");
                return;
            }

            string error = Helper.CheckPassword(PasswordBoxNew.Password);
            if (error != null)
            {
                Helper.Error(error);
                return;
            }
            if (PasswordBoxNew.Password != PasswordBoxConfirm.Password)
            {
                Helper.Error("Новые пароли не совпадают!");
                return;
            }
            if (PasswordBoxNew.Password == PasswordBoxOld.Password)
            {
                Helper.Error("Новый пароль не должен совпадать со старым!");
                return;
            }

            user.Password = Helper.GetHash(PasswordBoxNew.Password);
            if (Helper.Save())
            {
                Helper.Info("Пароль успешно изменён!");
                NavigationService.GoBack();
            }
        }
    }
}
