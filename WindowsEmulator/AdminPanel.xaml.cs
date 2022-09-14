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

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        public User currentuser;
        public ObservableCollection<User> users = new ObservableCollection<User>();
        IEnumerable<User> usersList { get; set; }
        ViewModel viewModel;
        class ViewModel
        {
            public IEnumerable<User> DataGridItems { get; set; }
            public ObservableCollection<string> source = new ObservableCollection<string>
            {
                "True","False"
            };
        }
        public AdminPanel(User _currentUser, ObservableCollection<User> Users)
        {
            InitializeComponent();
            currentuser = _currentUser;
            viewModel = new ViewModel();
            for (int i = 0; i!= Users.Count; i++)
            {
                if (currentuser._username != Users[i]._username)
                {
                    users.Add(Users[i]);
                }
            }
            viewModel.DataGridItems = users;

            DataContext = this.viewModel;
        }
    }
}
