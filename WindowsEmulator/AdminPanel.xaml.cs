using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        public User currentuser;
        public ObservableCollection<User> users = new ObservableCollection<User>();
        ViewModel viewModel;
        Regex rgx = new Regex(@"\b\S+\b");
        class ViewModel
        {
            public IEnumerable<User> DataGridItems { get; set; }
        }
        public AdminPanel(User _currentUser)
        {
            InitializeComponent();
            currentuser = _currentUser;
            viewModel = new ViewModel();
            string[] lines = File.ReadAllLines(@"UserList.txt");
            string[] UserData = new string[6];
            foreach (string line in lines)
            {
                int i = 0;
                foreach (Match match in rgx.Matches(line))
                {
                    UserData[i] = match.Value;
                    i++;
                }
                users.Add(new User(UserData[0], UserData[1],
                    Convert.ToBoolean(UserData[2]), Convert.ToBoolean(UserData[3]),
                    Convert.ToBoolean(UserData[4]), Convert.ToBoolean(UserData[5]))); // Создание объектов класса User на основе записей в списке
            }
            Load(users);
        }
        private void Load(ObservableCollection<User> users )
        {
            ObservableCollection<User> users1 = new ObservableCollection<User>();
            int count = users.Count;
            for (int i = 0; i != count; i++)
            {
                if (users[i]._username != "Admin")
                {
                    users1.Add(users[i]);
                }
            }
            if (currentuser._username != "Admin")
            {
                AccessGrid.Columns[5].IsReadOnly = true;
            }
            AccessGrid.Columns[3].IsReadOnly = true;
            viewModel.DataGridItems = users1;
            DataContext = this.viewModel;
        }
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            users = (ObservableCollection<User>)AccessGrid.ItemsSource;
            File.Delete(@"UserList.txt");
            File.AppendAllText(@"UserList.txt","Admin 11 True True True True" + "\n");
            foreach (User user in users)
            {
                File.AppendAllText(@"UserList.txt",user._username + " " + user._password + " " + user._OpenFolders + " " + true + " " + user._Journal +
                    " " + user._AccountsAdministrating + "\n");
            }          
            Load(users);
            File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Внесенны изменения в учетные записи: " + currentuser._username + "\n");
        }
    }
}