using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace WpfApp1
{
    public static class Helper
    {
        private static Random random = new Random();
        private static string themeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme.txt");

        public static string CurrentTheme = "Light";

        public static string GetHash(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return string.Concat(bytes.Select(b => b.ToString("X2")));
            }
        }

        public static Users Authenticate(string login, string password)
        {
            string hash = GetHash(password);
            return Core.DB.Users.FirstOrDefault(u => u.Login == login && u.Password == hash);
        }

        public static string GenerateCaptcha()
        {
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
            string result = "";
            for (int i = 0; i < 5; i++)
            {
                result += chars[random.Next(chars.Length)];
            }
            return result;
        }

        public static bool CheckCaptcha(string shown, string entered)
        {
            return shown == entered;
        }

        public static void Info(string text)
        {
            MessageBox.Show(text, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Error(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool Confirm(string text)
        {
            return MessageBox.Show(text, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static string CheckPassword(string password)
        {
            if (password.Length < 6)
                return "Пароль слишком короткий, минимум 6 символов!";
            if (!password.All(c => c > 32 && c < 127))
                return "Используйте только английскую раскладку!";
            if (!password.Any(c => c >= '0' && c <= '9'))
                return "Добавьте хотя бы одну цифру!";
            return null;
        }

        public static bool IsLogin(string login)
        {
            return Regex.IsMatch(login, @"^[A-Za-z0-9]{3,30}$");
        }

        public static bool IsFio(string fio)
        {
            return fio.Length <= 100 && Regex.IsMatch(fio, @"^[A-Za-zА-Яа-яЁё]+([ \-][A-Za-zА-Яа-яЁё]+)*$");
        }

        public static bool IsFioText(string text)
        {
            return Regex.IsMatch(text, @"^[A-Za-zА-Яа-яЁё \-]+$");
        }

        public static bool IsDigits(string text)
        {
            return text.All(c => c >= '0' && c <= '9');
        }

        public static void UpdateHint(object sender)
        {
            Control control = sender as Control;
            UIElement hint = control == null ? null : control.Tag as UIElement;
            if (hint == null)
                return;
            bool empty;
            if (sender is PasswordBox)
                empty = ((PasswordBox)sender).Password.Length == 0;
            else
                empty = ((TextBox)sender).Text.Length == 0;
            hint.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;
        }

        public static bool Save()
        {
            try
            {
                Core.DB.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                Error("Не удалось сохранить данные: " + inner.Message);
                foreach (var entry in Core.DB.ChangeTracker.Entries().ToList())
                {
                    if (entry.State == System.Data.EntityState.Added)
                        entry.State = System.Data.EntityState.Detached;
                    else if (entry.State != System.Data.EntityState.Unchanged)
                        entry.Reload();
                }
                return false;
            }
        }

        public static List<Shipment> GetShipments(int userId, string search, int sortIndex, int typeId)
        {
            IEnumerable<Shipment> list = Core.DB.Shipment.ToList();
            if (userId != 0)
                list = list.Where(s => s.UserID == userId);
            if (!string.IsNullOrWhiteSpace(search))
                list = list.Where(s => s.Name.ToLower().Contains(search.Trim().ToLower()));
            if (typeId != 0)
                list = list.Where(s => s.CargoTypeID == typeId);
            switch (sortIndex)
            {
                case 1:
                    list = list.OrderBy(s => s.ShipDate);
                    break;
                case 2:
                    list = list.OrderBy(s => s.Price);
                    break;
                case 3:
                    list = list.OrderByDescending(s => s.Price);
                    break;
                default:
                    list = list.OrderByDescending(s => s.ShipDate);
                    break;
            }
            return list.ToList();
        }

        public static void SetTheme(string theme)
        {
            CurrentTheme = theme;
            string file = theme == "Dark" ? "DictionaryDark.xaml" : "Dictionary.xaml";
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(file, UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dictionary);
            try
            {
                File.WriteAllText(themeFile, theme);
            }
            catch (Exception)
            {
            }
        }

        public static void LoadTheme()
        {
            string theme = "Light";
            try
            {
                if (File.Exists(themeFile))
                    theme = File.ReadAllText(themeFile).Trim();
            }
            catch (Exception)
            {
            }
            SetTheme(theme == "Dark" ? "Dark" : "Light");
        }
    }
}
