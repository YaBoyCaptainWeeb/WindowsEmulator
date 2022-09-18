using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для Desktop.xaml
    /// </summary>
    public partial class Desktop : Window
    {
        User _currentUser;
        public ObservableCollection<User> _users;
        DispatcherTimer dispatcher;
        public Desktop(User CurrentUser, ObservableCollection<User> users)
        {
            InitializeComponent();
            InitializeFolders();
            this.DataContext = this;
            dispatcher = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            dispatcher.Tick += SystemTimer;
            dispatcher.Start();

            _currentUser = CurrentUser;
            _users = users;
            CurrentUserTitle.Text = "Пользователь: " + _currentUser._username;
        }
        private void SystemTimer(object sender, EventArgs e)
        {
            SysTime.Text = DateTime.Now.ToString("T") + "\n" + DateTime.Now.ToString("d");
        }
        public void InitializeFolders()
        {
            // Журнал безопасности
            DisplayTile journal = new DisplayTile();
            journal.Text = "Журнал безопасности";
            journal.Image = new BitmapImage(new Uri(@"Assets/Journal.png", UriKind.Relative));
            journal.MouseDoubleClick += new MouseButtonEventHandler(OpenJournal);
            FolderArea.Children.Add(journal);
            // Админ панель
            DisplayTile journalAdmin = new DisplayTile();
            journalAdmin.Text = "Панель администратора";
            journalAdmin.Image = new BitmapImage(new Uri(@"Assets/AdminPanel.png", UriKind.Relative));
            journalAdmin.MouseDoubleClick += new MouseButtonEventHandler(OpenAdminPanel);
            FolderArea.Children.Add(journalAdmin);
            Canvas.SetLeft(journalAdmin, 125);      
        }

        private void OpenAdminPanel(object sender, RoutedEventArgs e)
        {
            if (_currentUser._AccountsAdministrating == true)
            {
                var task = new AdminPanel(_currentUser, _users);
                if (task.IsActive == false)
                {
                    task.Show();
                    File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Вход в панель администратора: " + _currentUser._username + "\n");
                }
            } else
            {
                MessageBox.Show("У вас нет доступа к панели Администратора.", "Ошибка", MessageBoxButton.OK);
                File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Попытка доступа к панели администратора: " + _currentUser._username + "\n");
            }
        }

        private void OpenJournal(object sender, RoutedEventArgs e)
        {
                if (_currentUser._Journal == true) // Проверка на наличие доступа у пользователя, должно быть true
                {
                   var task = new Journal();
                if (task.IsActive == false)
                {
                    task.Show();
                    File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Вход в журнал безопасности: " + _currentUser._username + "\n");
                }
                }
                else
                {
                    MessageBox.Show("У вас нет доступа к журналу.", "Ошибка", MessageBoxButton.OK);
                    File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Попытка доступа к журналу безопасности: " + _currentUser._username + "\n");  
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
            Canvas.SetLeft(obj, DropPos.X - 45);
            Canvas.SetTop(obj, DropPos.Y - 60);
        }

        private void FolderArea_DragOver(object sender, DragEventArgs e)
        {
            var obj = e.Data.GetData(typeof(DisplayTile)) as DisplayTile;
            Point DropPos = e.GetPosition(FolderArea);
            Canvas.SetLeft(obj, DropPos.X - 45);
            Canvas.SetTop(obj, DropPos.Y - 60);
        }
    }
}
