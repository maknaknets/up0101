using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Helper.LoadTheme();
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTimeNow.Text = DateTime.Now.ToString();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, t) => { DateTimeNow.Text = DateTime.Now.ToString(); };
            timer.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!Helper.Confirm("Вы действительно хотите выйти из приложения?"))
                e.Cancel = true;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Page page = e.Content as Page;
            if (page == null)
                return;

            if (page is AuthPage || page is UserPage || page is AdminPage)
            {
                while (MainFrame.CanGoBack)
                    MainFrame.RemoveBackEntry();
            }

            Title = "Грузоперевозки - " + page.Title;
            TextBlockTitle.Text = page.Title;
            TextBlockTitle.ToolTip = page.Title;
            ButtonBack.IsEnabled = MainFrame.CanGoBack;
            ButtonLogout.IsEnabled = Core.CurrentUser != null;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            if (Helper.Confirm("Вы действительно хотите выйти из аккаунта?"))
            {
                Core.CurrentUser = null;
                MainFrame.Navigate(new AuthPage());
            }
        }

        private void ButtonTheme_Click(object sender, RoutedEventArgs e)
        {
            Helper.SetTheme(Helper.CurrentTheme == "Dark" ? "Light" : "Dark");
        }
    }
}
