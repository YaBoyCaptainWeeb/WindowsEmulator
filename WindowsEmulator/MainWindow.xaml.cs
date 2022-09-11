using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        DateTime _date = DateTime.Now;
        string[] UserList;
        string[] UserData = new string[7];
        ObservableCollection<User> Users;
        Regex rgx = new Regex(@"\b\S+\b");
        
        public MainWindow()
        {
            InitializeComponent();
            Users = new ObservableCollection<User>();
            try
            {
                UserList = File.ReadAllLines(@"UserList.txt");
                File.AppendAllText(@"Journal.txt",_date.ToString(@"g") + " Выполнен запуск системы" + "\n");
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
                    Users.Add(new User(UserData[0], UserData[1], 
                        Convert.ToBoolean(UserData[2]), Convert.ToBoolean(UserData[3]), 
                        Convert.ToBoolean(UserData[4]), Convert.ToBoolean(UserData[5]), 
                        Convert.ToBoolean(UserData[6]))); // Создание объектов класса User на основе записей в списке
                }
                /* MessageBox.Show(Users[1]._username + Users[1]._password + Convert.ToString(Users[1]._OpenFolders) +
                    Convert.ToString(Users[1]._OpenPersonalFolder) + Convert.ToString(Users[1]._Journal) +
                    Convert.ToString(Users[1]._Settings) + Convert.ToString(Users[1]._AccountsAdministrating)); */
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
                if (Convert.ToString(UsersList.SelectedItem) == Users[i]._username)
                {
                    if (Convert.ToString(User_PassWord.Password) == Users[i]._password)
                    {
                        // MessageBox.Show("Вы успешно вошли как: " + Users[i]._username); // Отладка, потом удалить
                        File.AppendAllText(@"Journal.txt", _date.ToString(@"g") + " Вход в систему: " + Users[i]._username + "\n");
                        // User CurrentUser = Users[i];
                        new Desktop(Users[i], Users).Show();
                        this.Close();
                        return;
                    } else
                    {
                        File.AppendAllText(@"Journal.txt",_date.ToString(@"g") + " Неудачная попытка входа: " + Users[i]._username + "\n");
                        MessageBox.Show("Введен неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                } else
                {
                    i++;
                }
            }
        }

        private void TurnOff(object sender, RoutedEventArgs e)
        {
            File.AppendAllText(@"Journal.txt",_date.ToString(@"g") + " Завершение работы системы" + "\n");
            Application.Current.Shutdown();
        }
    }
}
