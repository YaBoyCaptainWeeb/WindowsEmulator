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
        bool flag = true;
        class ViewModel
        {
            public IEnumerable<User> DataGridItems { get; set; }
        }
        public AdminPanel(User _currentUser, ObservableCollection<User> Users)
        {
            InitializeComponent();
            currentuser = _currentUser;
            viewModel = new ViewModel();
            users = Users;
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
            viewModel.DataGridItems = users1;
            DataContext = this.viewModel;
        }
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            users = (ObservableCollection<User>)AccessGrid.ItemsSource;
            List<string> line = new List<string>();
            flag = true;
            SaveBtn.IsEnabled = false;
            File.Delete(@"UserList.txt");
            File.AppendAllText(@"UserList.txt",currentuser._username + " " + currentuser._password + " " +
                currentuser._OpenFolders + " " + currentuser._OpenPersonalFolder + " "+ currentuser._Journal+ " " + currentuser._AccountsAdministrating + "\n");
            foreach (User user in users)
            {
                File.AppendAllText(@"UserList.txt",user._username + " " + user._password + " " + user._OpenFolders + " " + user._OpenPersonalFolder + " " + user._Journal +
                    " " + user._AccountsAdministrating + "\n");
            }          
            Load(users);
            SaveBtn.IsEnabled = false;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                Load(users); 
        }
    }
}