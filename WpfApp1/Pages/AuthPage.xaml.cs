using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class AuthPage : Page
    {
        private static int failedAttempts = 0;

        public AuthPage()
        {
            InitializeComponent();
            if (failedAttempts >= 3)
                ShowCaptcha();
        }

        private void ShowCaptcha()
        {
            CaptchaPanel.Visibility = Visibility.Visible;
            TextBlockCaptcha.Text = Helper.GenerateCaptcha();
            TextBoxCaptcha.Clear();
        }

        private void Field_Changed(object sender, RoutedEventArgs e)
        {
            Helper.UpdateHint(sender);
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLogin.Text) || PasswordBoxLogin.Password.Length == 0)
            {
                Helper.Error("Введите логин и пароль!");
                return;
            }

            if (CaptchaPanel.Visibility == Visibility.Visible &&
                !Helper.CheckCaptcha(TextBlockCaptcha.Text, TextBoxCaptcha.Text))
            {
                Helper.Error("Неверно введена капча!");
                ShowCaptcha();
                return;
            }

            Users user = Helper.Authenticate(TextBoxLogin.Text.Trim(), PasswordBoxLogin.Password);
            if (user == null)
            {
                failedAttempts++;
                Helper.Error("Неверный логин или пароль!");
                if (failedAttempts >= 3)
                    ShowCaptcha();
                return;
            }

            failedAttempts = 0;
            Core.CurrentUser = user;
            Helper.Info("Добро пожаловать, " + user.FIO + "!");

            if (user.Role == "Admin")
                NavigationService.Navigate(new AdminPage());
            else
                NavigationService.Navigate(new UserPage());
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage());
        }
    }
}
