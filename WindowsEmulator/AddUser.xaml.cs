using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void AddNewUser(object sender, RoutedEventArgs e)
        {
            ObservableCollection<string> UsersNames = new ObservableCollection<string>();
            string[] UserData = new string[6];
            Regex rgx = new Regex(@"\b\S+\b");
            string UserName = User.Text;
            string password = User_PassWord.Text;
            string repeatPassword = User_RepeatPassWord.Text;
            try
            {
                string[] UserList = File.ReadAllLines(@"UserList.txt");
                if (UserName.Length != 0 && UserList[0][0] != ' ' && password == repeatPassword)
                {
                    foreach (string user in UserList) // Подгружаем имена из списка пользователей, для дальнейшей проверки на уникальность нового имени
                    {
                        int i = 0;
                        foreach (Match match in rgx.Matches(user))
                        {
                            UserData[i] = match.Value;
                            i++;
                        }
                        UsersNames.Add(UserData[0]);
                    }
                    foreach (string name in UsersNames)
                    {
                        if (name == UserName)
                        {
                            MessageBox.Show("Данное имя пользователя уже занято в системе, укажите другое.", "Ошибка");
                            return;
                        }
                    }
                    File.AppendAllText(@"UserList.txt", UserName + " " + password + " " + "False True False False" + "\n");
                    File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Создан новый пользователь: " + UserName + "\n");
                    this.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Список пользователей не обнаружен.");
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
