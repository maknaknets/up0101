using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace WpfApp1.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
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
            DataGridUsers.ItemsSource = Core.DB.Users.ToList();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUserPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Users selected = DataGridUsers.SelectedItem as Users;
            if (selected == null)
            {
                Helper.Error("Выберите пользователя для редактирования!");
                return;
            }
            NavigationService.Navigate(new AddUserPage(selected));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Users> selected = DataGridUsers.SelectedItems.Cast<Users>().ToList();
            if (selected.Count == 0)
            {
                Helper.Error("Выберите пользователей для удаления!");
                return;
            }
            if (selected.Any(u => u.ID == Core.CurrentUser.ID))
            {
                Helper.Error("Нельзя удалить текущего пользователя!");
                return;
            }
            if (!Helper.Confirm("Вы точно хотите удалить выбранные записи (" + selected.Count + ")? Все их перевозки также будут удалены."))
                return;
            foreach (var item in selected)
                Core.DB.Users.Remove(item); ;
            if (Helper.Save())
            {
                Helper.Info("Данные успешно удалены!");
                UpdateGrid();
            }
        }
    }
}
