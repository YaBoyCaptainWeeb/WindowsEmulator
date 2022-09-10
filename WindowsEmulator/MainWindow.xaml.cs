using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] UserList;
        string[] UserData = new string[3];
        ObservableCollection<User> Users;
        Regex rgx = new Regex(@"\b\S+\b");
        
        public MainWindow()
        {
            InitializeComponent();
            Users = new ObservableCollection<User>();
            try
            {
                UserList = File.ReadAllLines(@"C:\\Users\\User\\source\\repos\\WindowsEmulator\\WindowsEmulator\\UserList.txt");
            }
            catch (Exception)
            {
                MessageBox.Show("Список пользователей не обнаружен.");
                Process.GetCurrentProcess().Kill();
            }
            if (UserList.Length != 0 && UserList[0][0] != ' ')
            {
                foreach (string user in UserList)
                {
                    int i = 0;
                    foreach (Match match in rgx.Matches(user))
                    {
                        UserData[i] = match.Value;
                        i++;
                    }
                    Users.Add(new User(UserData[0], UserData[1], UserData[2])); // Создание объектов класса User на основе записей в списке
                }
                List<string> UserNames = new List<string>();
                foreach(User user in Users)
                {
                    UserNames.Add(user._username); // Добавление имен в выпадающий список в окне авторизации
                }
                UsersList.ItemsSource = UserNames;
            }
            else
            {
                MessageBox.Show("Список пользователей пуст");
                Process.GetCurrentProcess().Kill();
            }
        }

        private void LogIn(object sender, RoutedEventArgs e) // Авторизация
        {
            int i = 0;
            //MessageBox.Show(Convert.ToString(Users.Count));
            while (i != Users.Count)
            {
                if (Convert.ToString(UsersList.SelectedItem) == Users[i]._username && Convert.ToString(User_PassWord.Password) == Users[i]._password)
                {
                        MessageBox.Show("Вы успешно вошли как: " + Users[i]._username); // Отладка, потом удалить
                        return;

                } else
                {
                    i++;
                }
            }
            MessageBox.Show("Введен неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void TurnOff(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
