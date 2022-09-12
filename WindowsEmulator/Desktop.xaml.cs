using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для Desktop.xaml
    /// </summary>
    public partial class Desktop : Window
    {
        User _currentUser;
        ObservableCollection<User> _users;
        public Desktop(User CurrentUser, ObservableCollection<User> users)
        {
            InitializeComponent();
            InitializeFolders();
            this.DataContext = this;
            _currentUser = CurrentUser;
            _users = users;
            CurrentUserTitle.Text = "Пользователь: " + _currentUser._username;
        }
        public void InitializeFolders()
        {
            DisplayTile journal = new DisplayTile();
            journal.Text = "Журнал безопасности";
            journal.Image = new BitmapImage(new Uri(@"Assets/Journal.png", UriKind.Relative));
            FolderArea.Children.Add(journal);
        }

        private void OpenJournal(object sender, RoutedEventArgs e)
        {
            if (_currentUser._Journal == true) // Проверка на наличие доступа у пользователя, должно быть true
            {
                var task = new Journal();
                task.Show();
            }
            else
            {
                MessageBox.Show("У вас нет доступа к журналу.", "Ошибка", MessageBoxButton.OK);
            }
        }

        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
        private void ShutDown(object sender, RoutedEventArgs e)
        {
            File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Завершение работы системы" + "\n");
            Application.Current.Shutdown();
        }

        private void FolderArea_Drop(object sender, DragEventArgs e)
        {
            var obj = e.Data.GetData(typeof(DisplayTile)) as DisplayTile;
            Point DropPos = e.GetPosition(FolderArea);
            // ((Canvas)(obj.Parent)).SetLeft(obj, DropPos.X);
        }
    }
}
