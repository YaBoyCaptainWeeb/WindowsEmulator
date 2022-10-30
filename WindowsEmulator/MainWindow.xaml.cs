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
        Regex rgx = new Regex(@"\b\S+\b");
        ObservableCollection<User> Users = new ObservableCollection<User>();

        public MainWindow()
        {
            InitializeComponent();
            File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Выполнен запуск системы" + "\n");
            LoadList();
        }
        private void LoadList()
        {
            Users = new ObservableCollection<User>();
            string[] UserData = new string[6];
            try
            {
                string[] UserList = File.ReadAllLines(@"UserList.txt");
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
                            Convert.ToBoolean(UserData[4]), Convert.ToBoolean(UserData[5]))); // Создание объектов класса User на основе записей в списке
                    }
                    List<string> UserNames = new List<string>();
                    foreach (User user in Users)
                    {
                        UserNames.Add(user._username); // Добавление имен в выпадающий список в окне авторизации
                    }
                    UsersList.ItemsSource = UserNames;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Список пользователей не обнаружен.");
                Process.GetCurrentProcess().Kill();
            }
            
        } // Загрузка списка пользователей из файла UserList.txt

        private void LogIn(object sender, RoutedEventArgs e) // Авторизация
        {
            int i = 0;
            while (i != Users.Count)
            {
                if (Convert.ToString(UsersList.SelectedItem) == Users[i]._username)
                {
                    if (Convert.ToString(User_PassWord.Password) == Users[i]._password)
                    {
                        File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Вход в систему: " + Users[i]._username + "\n");
                        File.WriteAllText(@"CurrentUser.txt", Users[i]._username + " " + Users[i]._password + " "
                            + Users[i]._OpenFolders + " " + Users[i]._OpenPersonalFolder + " "
                            + Users[i]._Journal + " " + Users[i]._AccountsAdministrating);
                        new Desktop(Users[i], Users).Show();
                        this.Close();
                        return;
                    } else
                    {
                        File.AppendAllText(@"Journal.txt",DateTime.Now.ToString(@"g") + " Неудачная попытка входа: " + Users[i]._username + "\n");
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
            File.AppendAllText(@"Journal.txt",DateTime.Now.ToString(@"g") + " Завершение работы системы" + "\n");
            Application.Current.Shutdown();
        }

        private void AddUser(object sender, RoutedEventArgs e) // Вызов окна регистрации
        {
            new AddUser().ShowDialog();
            LoadList();
        }
    }
}
